using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Lab4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Добавляем обработчики для вкладок
            Tab1.MouseDown += Tab1_MouseDown;
            Tab2.MouseDown += Tab2_MouseDown;
            Tab3.MouseDown += Tab3_MouseDown;

            // Устанавливаем первую вкладку как активную при запуске
            ActivateTab(Tab1, Tab1Content);
        }

        private void Tab1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ActivateTab(Tab1, Tab1Content);
        }

        private void Tab2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ActivateTab(Tab2, Tab2Content);
        }

        private void Tab3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ActivateTab(Tab3, Tab3Content);
        }

        private void ActivateTab(System.Windows.Controls.Border activeTab, System.Windows.Controls.Border activeContent)
        {
            ResetAllTabs();

            activeTab.Background = new SolidColorBrush(Color.FromRgb(0xCA, 0x90, 0x8E));
            var textBlock = (System.Windows.Controls.TextBlock)activeTab.Child;
            textBlock.Foreground = Brushes.White;
            textBlock.FontWeight = FontWeights.SemiBold;

            Tab1Content.Visibility = Visibility.Collapsed;
            Tab2Content.Visibility = Visibility.Collapsed;
            Tab3Content.Visibility = Visibility.Collapsed;

            activeContent.Visibility = Visibility.Visible;
        }

        private void ResetAllTabs()
        {
            var tabs = new[] { Tab1, Tab2, Tab3 };
            foreach (var tab in tabs)
            {
                tab.Background = Brushes.Transparent;
                var textBlock = (System.Windows.Controls.TextBlock)tab.Child;
                textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
                textBlock.FontWeight = FontWeights.Normal;
            }
        }

        private void SetupDataGridColumns(List<string> variableNames)
        {
            // Очищаем предыдущие колонки
            TruthTableDataGrid.Columns.Clear();

            // Создаем стиль для заголовков
            var headerStyle = new Style(typeof(DataGridColumnHeader))
            {
                Setters = {
                    new Setter(DataGridColumnHeader.BackgroundProperty, new SolidColorBrush(Color.FromRgb(0xF1, 0xC4, 0xBE))),
                    new Setter(DataGridColumnHeader.ForegroundProperty, Brushes.Black),
                    new Setter(DataGridColumnHeader.FontWeightProperty, FontWeights.Bold),
                    new Setter(DataGridColumnHeader.PaddingProperty, new Thickness(8))
                }
            };

            // Создаем стиль для ячеек
            var cellStyle = new Style(typeof(DataGridCell))
            {
                Setters = {
                    new Setter(DataGridCell.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0))),
                    new Setter(DataGridCell.BorderThicknessProperty, new Thickness(0, 0, 1, 1)),
                    new Setter(DataGridCell.PaddingProperty, new Thickness(8)),
                    new Setter(DataGridCell.ForegroundProperty, Brushes.Black)
                }
            };

            // Применяем стили
            TruthTableDataGrid.ColumnHeaderStyle = headerStyle;
            TruthTableDataGrid.CellStyle = cellStyle;

            // Создаем колонку для каждой переменной
            foreach (var varName in variableNames)
            {
                // Преобразуем "x1" в "X1" для привязки к свойству класса TruthTableRow
                string bindingPath = char.ToUpper(varName[0]) + varName.Substring(1);

                var column = new DataGridTextColumn
                {
                    Header = varName, // Заголовок будет "x1"
                    Binding = new Binding(bindingPath) // Привязка к свойству "X1"
                };
                TruthTableDataGrid.Columns.Add(column);
            }

            // Создаем колонку для результата
            var resultColumn = new DataGridTextColumn
            {
                Header = "Result",
                Binding = new Binding("Result")
            };
            TruthTableDataGrid.Columns.Add(resultColumn);
        }

        private void TableTrueFalseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int numVariables = int.Parse(NumCountTextBox.Text);
                int functionNumber = int.Parse(FunctionName.Text);

                if (numVariables < 1 || numVariables > 20)
                {
                    MessageBox.Show("Количество переменных должно быть от 1 до 20");
                    return;
                }

                int maxFunctionNumber = (int)Math.Pow(2, Math.Pow(2, numVariables));
                if (functionNumber < 0 || functionNumber >= maxFunctionNumber)
                {
                    MessageBox.Show($"Номер функции должен быть от 0 до {maxFunctionNumber - 1}");
                    return;
                }

                var variableNames = Enumerable.Range(1, numVariables).Select(i => $"x{i}").ToList();

                // Вызываем новый метод для настройки колонок
                SetupDataGridColumns(variableNames);

                var tableData = GenerateTruthTableByNumber(numVariables, functionNumber, variableNames);

                TruthTableDataGrid.ItemsSource = tableData;
                GenerateDNFAndKNF(tableData, variableNames);
            }
            catch (FormatException)
            {
                MessageBox.Show("Пожалуйста, введите корректные числовые значения");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private List<TruthTableRow> GenerateTruthTableByNumber(int numVariables, int functionNumber, List<string> variableNames)
        {
            var tableData = new List<TruthTableRow>();
            int numRows = (int)Math.Pow(2, numVariables);

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

        private void GenerateDNFAndKNF(List<TruthTableRow> tableData, List<string> variables)
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

            DNFResultText.Text = dnfTerms.Count > 0 ? string.Join(" | ", dnfTerms) : "0";
            KNFResultText.Text = knfTerms.Count > 0 ? string.Join(" & ", knfTerms) : "1";

            int dnfLiterals = dnfTerms.Sum(t => t.Count(c => c == 'x') + t.Count(c => c == '!'));
            int knfLiterals = knfTerms.Sum(t => t.Count(c => c == 'x') + t.Count(c => c == '!'));

            DNFStatsText.Text = $"Литералов: {dnfLiterals}, Конъюнкций: {dnfTerms.Count}";
            KNFStatsText.Text = $"Литералов: {knfLiterals}, Дизъюнкций: {knfTerms.Count}";
        }

        private void ParseFormulaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string formula = FormulaTextBox.Text.Trim();
                var variables = GetVariablesFromFormula(formula);

                if (variables.Count == 0)
                {
                    MessageBox.Show("В формуле нет переменных");
                    return;
                }

                // Вызываем новый метод для настройки колонок
                SetupDataGridColumns(variables);

                var tableData = GenerateTruthTableByFormula(formula, variables);
                TruthTableDataGrid.ItemsSource = tableData;
                GenerateDNFAndKNF(tableData, variables);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при разборе формулы: {ex.Message}");
            }
        }

        private List<string> GetVariablesFromFormula(string formula)
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

        // ИСПРАВЛЕННЫЙ МЕТОД
        private List<string> ConvertToRPN(string formula)
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

                    if (operators.Count == 0)
                        throw new Exception("Несогласованные скобки в формуле");

                    operators.Pop();
                }
                else // Оператор
                {
                    // Стандартная логика алгоритма сортировочной станции
                    while (operators.Count > 0 && operators.Peek() != "(" && GetOperatorPriority(operators.Peek()) >= GetOperatorPriority(token))
                    {
                        output.Add(operators.Pop());
                    }

                    operators.Push(token);
                }
            }

            while (operators.Count > 0)
            {
                string op = operators.Pop();
                if (op == "(" || op == ")")
                    throw new Exception("Несогласованные скобки в формуле");

                output.Add(op);
            }

            return output;
        }

        private List<string> Tokenize(string formula)
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

        private bool IsVariable(string token) => token.StartsWith("x") && token.Length > 1 && token.Substring(1).All(char.IsDigit);

        private int GetOperatorPriority(string op)
        {
            switch (op)
            {
                case "!": return 4;
                case "&": return 3;
                case "|": return 2;
                case "^": return 2;
                case "->": return 1;
                case "=": return 1;
                default: return 0;
            }
        }

        private int EvaluateRPN(List<string> rpn, Dictionary<string, int> variableValues)
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
                        case "!": stack.Push(NOT(stack.Pop())); break;
                        case "&": stack.Push(AND(stack.Pop(), stack.Pop())); break;
                        case "|": stack.Push(OR(stack.Pop(), stack.Pop())); break;
                        case "^": stack.Push(XOR(stack.Pop(), stack.Pop())); break;
                        case "->": stack.Push(IMPLICATION(stack.Pop(), stack.Pop())); break;
                        case "=": stack.Push(EQUIVALENCE(stack.Pop(), stack.Pop())); break;
                        default: throw new Exception($"Неизвестный токен: {token}");
                    }
                }
            }
            if (stack.Count != 1) throw new Exception("Некорректное выражение");
            return stack.Pop();
        }

        private int NOT(int a) => a == 0 ? 1 : 0;
        private int AND(int a, int b) => (a == 1 && b == 1) ? 1 : 0;
        private int OR(int a, int b) => (a == 1 || b == 1) ? 1 : 0;
        private int XOR(int a, int b) => (a != b) ? 1 : 0;
        private int IMPLICATION(int a, int b) => (a == 1 && b == 0) ? 0 : 1;
        private int EQUIVALENCE(int a, int b) => (a == b) ? 1 : 0;

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string function1 = CompareFunction1.Text.Trim();
                string function2 = CompareFunction2.Text.Trim();

                bool isNumber1 = int.TryParse(function1, out int num1);
                bool isNumber2 = int.TryParse(function2, out int num2);

                var allVariablesSet = new HashSet<string>();
                if (isNumber1) for (int i = 1; i <= 3; i++) allVariablesSet.Add($"x{i}");
                else GetVariablesFromFormula(function1).ForEach(v => allVariablesSet.Add(v));

                if (isNumber2) for (int i = 1; i <= 3; i++) allVariablesSet.Add($"x{i}");
                else GetVariablesFromFormula(function2).ForEach(v => allVariablesSet.Add(v));

                var allVariables = allVariablesSet.OrderBy(v => int.Parse(v.Substring(1))).ToList();

                // Вызываем новый метод для настройки колонок и показываем таблицу для наглядности
                SetupDataGridColumns(allVariables);

                List<TruthTableRow> table1 = isNumber1 ? GenerateTruthTableByNumber(allVariables.Count, num1, allVariables) : GenerateTruthTableByFormula(function1, allVariables);
                List<TruthTableRow> table2 = isNumber2 ? GenerateTruthTableByNumber(allVariables.Count, num2, allVariables) : GenerateTruthTableByFormula(function2, allVariables);

                // Показываем первую таблицу в DataGrid для контекста
                TruthTableDataGrid.ItemsSource = table1;

                bool areEquivalent = CompareTruthTables(table1, table2, out TruthTableRow counterExample);

                if (areEquivalent)
                {
                    CompareResult.Text = "Функции эквивалентны";
                    CounterExample.Text = "";
                }
                else
                {
                    CompareResult.Text = "Функции не эквивалентны";
                    var sb = new StringBuilder("Контрпример: ");
                    bool firstVar = true;
                    foreach (var varName in allVariables)
                    {
                        if (!firstVar) sb.Append(", ");
                        sb.Append($"{varName}={counterExample.GetValue(varName)}");
                        firstVar = false;
                    }
                    var row2 = table2.FirstOrDefault(r => allVariables.All(var => r.GetValue(var) == counterExample.GetValue(var)));
                    if (row2 != null) sb.Append($", F1={counterExample.Result}, F2={row2.Result}");
                    CounterExample.Text = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сравнении функций: {ex.Message}");
            }
        }

        private List<TruthTableRow> GenerateTruthTableByFormula(string formula, List<string> variables)
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

        private bool CompareTruthTables(List<TruthTableRow> table1, List<TruthTableRow> table2, out TruthTableRow counterExample)
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
    }

    public class TruthTableRow
    {
        public int X1 { get; set; } = -1;
        public int X2 { get; set; } = -1;
        public int X3 { get; set; } = -1;
        public int X4 { get; set; } = -1;
        public int X5 { get; set; } = -1;
        public int X6 { get; set; } = -1;
        public int X7 { get; set; } = -1;
        public int X8 { get; set; } = -1;
        public int X9 { get; set; } = -1;
        public int X10 { get; set; } = -1;
        public int Result { get; set; }

        public int GetValue(string varName)
        {
            switch (varName) { case "x1": return X1; case "x2": return X2; case "x3": return X3; case "x4": return X4; case "x5": return X5; case "x6": return X6; case "x7": return X7; case "x8": return X8; case "x9": return X9; case "x10": return X10; default: return -1; }
        }
        public void SetValue(string varName, int value)
        {
            switch (varName) { case "x1": X1 = value; break; case "x2": X2 = value; break; case "x3": X3 = value; break; case "x4": X4 = value; break; case "x5": X5 = value; break; case "x6": X6 = value; break; case "x7": X7 = value; break; case "x8": X8 = value; break; case "x9": X9 = value; break; case "x10": X10 = value; break; }
        }
    }
}