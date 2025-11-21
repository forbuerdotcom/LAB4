using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLab4
{
    public static class LibLab4
    {
        #region Truth Table Generation

        public static List<TruthTableRow> GenerateTruthTableByNumber(int numVariables, int functionNumber, List<string> variableNames)
        {
            if (numVariables < 1 || numVariables > 20)
                throw new ArgumentOutOfRangeException(nameof(numVariables), "Количество переменных должно быть от 1 до 20");

            int numRows = (int)Math.Pow(2, numVariables);
            long maxFunctionNumber = (long)Math.Pow(2, numRows) - 1;
            if (functionNumber < 0 || functionNumber > maxFunctionNumber)
                throw new ArgumentOutOfRangeException(nameof(functionNumber), $"Номер функции должен быть от 0 до {maxFunctionNumber}");

            var tableData = new List<TruthTableRow>();
            string binaryFunction = Convert.ToString(functionNumber, 2).PadLeft(numRows, '0');

            for (int i = 0; i < numRows; i++)
            {
                var row = new TruthTableRow();
                for (int j = 0; j < numVariables; j++)
                {
                    int bitValue = (i >> (numVariables - 1 - j)) & 1;
                    row.SetValue(variableNames[j], bitValue);
                }
                row.Result = int.Parse(binaryFunction[i].ToString());
                tableData.Add(row);
            }

            return tableData;
        }

        public static List<TruthTableRow> GenerateTruthTableByFormula(string formula, List<string> variables)
        {
            int numVariables = variables.Count;
            if (numVariables == 0) throw new Exception("В формуле нет переменных");

            var tableData = new List<TruthTableRow>();
            int numRows = (int)Math.Pow(2, numVariables);
            var rpn = ConvertToRPN(formula);

            for (int i = 0; i < numRows; i++)
            {
                var row = new TruthTableRow();
                var variableValues = new Dictionary<string, int>();

                for (int j = 0; j < numVariables; j++)
                {
                    int bitValue = (i >> (numVariables - 1 - j)) & 1;
                    string varName = variables[j];
                    variableValues[varName] = bitValue;
                    row.SetValue(varName, bitValue);
                }

                row.Result = EvaluateRPN(rpn, variableValues);
                tableData.Add(row);
            }

            return tableData;
        }

        #endregion

        #region DNF and KNF Generation

        public static (string DNF, string KNF) GenerateDNFAndKNF(List<TruthTableRow> tableData, List<string> variables)
        {
            var dnfTerms = new List<string>();
            var knfTerms = new List<string>();

            foreach (var row in tableData)
            {
                if (row.Result == 1)
                {
                    var term = new List<string>();
                    foreach (var varName in variables)
                    {
                        int value = row.GetValue(varName);
                        term.Add(value == 0 ? $"!{varName}" : varName);
                    }
                    dnfTerms.Add($"({string.Join(" & ", term)})");
                }

                if (row.Result == 0)
                {
                    var term = new List<string>();
                    foreach (var varName in variables)
                    {
                        int value = row.GetValue(varName);
                        term.Add(value == 1 ? $"!{varName}" : varName);
                    }
                    knfTerms.Add($"({string.Join(" | ", term)})");
                }
            }

            string dnf = dnfTerms.Count > 0 ? string.Join(" | ", dnfTerms) : "0";
            string knf = knfTerms.Count > 0 ? string.Join(" & ", knfTerms) : "1";

            return (dnf, knf);
        }

        #endregion

        #region Formula Parsing and Evaluation

        public static List<string> GetVariablesFromFormula(string formula)
        {
            var variables = new HashSet<string>();
            for (int i = 0; i < formula.Length; i++)
            {
                if (formula[i] == 'x' && i + 1 < formula.Length && char.IsDigit(formula[i + 1]))
                {
                    string varName = "x";
                    i++;
                    while (i < formula.Length && char.IsDigit(formula[i]))
                    {
                        varName += formula[i];
                        i++;
                    }
                    i--;
                    variables.Add(varName);
                }
            }
            return variables.OrderBy(v => int.Parse(v.Substring(1))).ToList();
        }

        public static List<string> ConvertToRPN(string formula)
        {
            var tokens = Tokenize(formula);
            var output = new List<string>();
            var operators = new Stack<string>();

            foreach (var token in tokens)
            {
                if (IsVariable(token))
                {
                    output.Add(token);
                }
                else if (token == "(")
                {
                    operators.Push(token);
                }
                else if (token == ")")
                {
                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        output.Add(operators.Pop());
                    }
                    if (operators.Count == 0) throw new Exception("Несогласованные скобки в формуле");
                    operators.Pop(); // Выкидываем открывающую скобку
                }
                else // Если токен - это оператор
                {
                    // Унарный оператор "!" имеет правую ассоциативность
                    bool isRightAssociative = (token == "!");

                    while (operators.Count > 0 && operators.Peek() != "(" &&
                           (isRightAssociative ? GetOperatorPriority(operators.Peek()) > GetOperatorPriority(token)
                                               : GetOperatorPriority(operators.Peek()) >= GetOperatorPriority(token)))
                    {
                        output.Add(operators.Pop());
                    }
                    operators.Push(token);
                }
            }

            while (operators.Count > 0)
            {
                string op = operators.Pop();
                if (op == "(" || op == ")") throw new Exception("Несогласованные скобки в формуле");
                output.Add(op);
            }
            return output;
        }

        private static List<string> Tokenize(string formula)
        {
            var tokens = new List<string>();
            int i = 0;
            while (i < formula.Length)
            {
                char c = formula[i];
                if (char.IsWhiteSpace(c)) { i++; continue; }
                if (c == '(' || c == ')' || c == '!' || c == '&' || c == '|' || c == '^' || c == '=')
                {
                    tokens.Add(c.ToString());
                    i++;
                }
                else if (c == '-' && i + 1 < formula.Length && formula[i + 1] == '>')
                {
                    tokens.Add("->");
                    i += 2;
                }
                else if (c == 'x')
                {
                    string varName = "x";
                    i++;
                    while (i < formula.Length && char.IsDigit(formula[i]))
                    {
                        varName += formula[i];
                        i++;
                    }
                    tokens.Add(varName);
                }
                else
                {
                    throw new Exception($"Неожиданный символ '{c}' в позиции {i}");
                }
            }
            return tokens;
        }

        private static bool IsVariable(string token) => token.StartsWith("x") && token.Length > 1 && token.Substring(1).All(char.IsDigit);

        private static int GetOperatorPriority(string op)
        {
            switch (op) { case "!": return 4; case "&": return 3; case "|": return 2; case "^": return 2; case "->": return 1; case "=": return 1; default: return 0; }
        }

        private static int EvaluateRPN(List<string> rpn, Dictionary<string, int> variableValues)
        {
            var stack = new Stack<int>();
            foreach (var token in rpn)
            {
                if (IsVariable(token))
                {
                    stack.Push(variableValues[token]);
                }
                else
                {
                    switch (token)
                    {
                        case "!":
                            stack.Push(NOT(stack.Pop()));
                            break;
                        case "&":
                            int op2_and = stack.Pop();
                            int op1_and = stack.Pop();
                            stack.Push(AND(op1_and, op2_and));
                            break;
                        case "|":
                            int op2_or = stack.Pop();
                            int op1_or = stack.Pop();
                            stack.Push(OR(op1_or, op2_or));
                            break;
                        case "^":
                            int op2_xor = stack.Pop();
                            int op1_xor = stack.Pop();
                            stack.Push(XOR(op1_xor, op2_xor));
                            break;
                        case "->":
                            int op2_impl = stack.Pop();
                            int op1_impl = stack.Pop();
                            stack.Push(IMPLICATION(op1_impl, op2_impl));
                            break;
                        case "=":
                            int op2_eq = stack.Pop();
                            int op1_eq = stack.Pop();
                            stack.Push(EQUIVALENCE(op1_eq, op2_eq));
                            break;
                        default:
                            throw new Exception($"Неизвестный токен: {token}");
                    }
                }
            }
            if (stack.Count != 1) throw new Exception("Некорректное выражение");
            return stack.Pop();
        }

        #endregion

        #region Logical Operators

        private static int NOT(int a) => a == 0 ? 1 : 0;
        private static int AND(int a, int b) => (a == 1 && b == 1) ? 1 : 0;
        private static int OR(int a, int b) => (a == 1 || b == 1) ? 1 : 0;
        private static int XOR(int a, int b) => (a != b) ? 1 : 0;
        private static int IMPLICATION(int a, int b) => (a == 1 && b == 0) ? 0 : 1;
        private static int EQUIVALENCE(int a, int b) => (a == b) ? 1 : 0;

        #endregion

        #region Comparison

        public static bool CompareTruthTables(List<TruthTableRow> table1, List<TruthTableRow> table2, out TruthTableRow counterExample)
        {
            counterExample = null;
            if (table1.Count != table2.Count) return false;
            for (int i = 0; i < table1.Count; i++)
            {
                if (table1[i].Result != table2[i].Result)
                {
                    counterExample = table1[i];
                    return false;
                }
            }
            return true;
        }

        #endregion
    }

    public class TruthTableRow
    {
        // Используем словарь для хранения значений переменных
        public Dictionary<string, int> Values { get; set; } = new Dictionary<string, int>();
        public int Result { get; set; }

        public int GetValue(string varName)
        {
            return Values.TryGetValue(varName, out int val) ? val : -1;
        }

        public void SetValue(string varName, int value)
        {
            Values[varName] = value;
        }
    }
}