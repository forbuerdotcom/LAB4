using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

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
            // Сбрасываем все табы к неактивному состоянию
            ResetAllTabs();

            // Устанавливаем активный таб
            activeTab.Background = new SolidColorBrush(Color.FromRgb(0xCA, 0x90, 0x8E));
            var textBlock = (System.Windows.Controls.TextBlock)activeTab.Child;
            textBlock.Foreground = Brushes.White;
            textBlock.FontWeight = FontWeights.SemiBold;

            // Скрываем весь контент
            Tab1Content.Visibility = Visibility.Collapsed;
            Tab2Content.Visibility = Visibility.Collapsed;
            Tab3Content.Visibility = Visibility.Collapsed;

            // Показываем активный контент
            activeContent.Visibility = Visibility.Visible;
        }

        private void ResetAllTabs()
        {
            // Сбрасываем таб 1
            Tab1.Background = Brushes.Transparent;
            var textBlock1 = (System.Windows.Controls.TextBlock)Tab1.Child;
            textBlock1.Foreground = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
            textBlock1.FontWeight = FontWeights.Normal;

            // Сбрасываем таб 2
            Tab2.Background = Brushes.Transparent;
            var textBlock2 = (System.Windows.Controls.TextBlock)Tab2.Child;
            textBlock2.Foreground = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
            textBlock2.FontWeight = FontWeights.Normal;

            // Сбрасываем таб 3
            Tab3.Background = Brushes.Transparent;
            var textBlock3 = (System.Windows.Controls.TextBlock)Tab3.Child;
            textBlock3.Foreground = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
            textBlock3.FontWeight = FontWeights.Normal;
        }

        // Обработчик кнопки для вкладки "По номеру"
        private void TableTrueFalseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int numVariables = int.Parse(NumCountTextBox.Text);
                int functionNumber = int.Parse(FunctionName.Text);

                // Проверяем, что количество переменных в разумных пределах
                if (numVariables < 1 || numVariables > 10)
                {
                    MessageBox.Show("Количество переменных должно быть от 1 до 10");
                    return;
                }

                // Проверяем, что номер функции в допустимых пределах
                int maxFunctionNumber = (int)Math.Pow(2, Math.Pow(2, numVariables));
                if (functionNumber < 0 || functionNumber >= maxFunctionNumber)
                {
                    MessageBox.Show($"Номер функции должен быть от 0 до {maxFunctionNumber - 1}");
                    return;
                }

                // Создаем таблицу истинности по номеру функции
                var tableData = GenerateTruthTableByNumber(numVariables, functionNumber);

                // Заполняем DataGrid
                TruthTableDataGrid.ItemsSource = tableData;

                // Генерируем и выводим СДНФ и СКНФ
                GenerateDNFAndKNF(tableData, numVariables);
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

        // Генерация таблицы истинности по номеру функции
        private List<TruthTableRow> GenerateTruthTableByNumber(int numVariables, int functionNumber)
        {
            var tableData = new List<TruthTableRow>();
            int numRows = (int)Math.Pow(2, numVariables);

            // Преобразуем номер функции в двоичную последовательность
            string binaryFunction = Convert.ToString(functionNumber, 2).PadLeft(numRows, '0');

            for (int i = 0; i < numRows; i++)
            {
                var row = new TruthTableRow();

                // Заполняем значения переменных
                for (int j = 0; j < numVariables; j++)
                {
                    int bitValue = (i >> (numVariables - 1 - j)) & 1;

                    switch (j)
                    {
                        case 0: row.X1 = bitValue; break;
                        case 1: row.X2 = bitValue; break;
                        case 2: row.X3 = bitValue; break;
                        case 3: row.X4 = bitValue; break;
                        case 4: row.X5 = bitValue; break;
                        case 5: row.X6 = bitValue; break;
                        case 6: row.X7 = bitValue; break;
                        case 7: row.X8 = bitValue; break;
                        case 8: row.X9 = bitValue; break;
                        case 9: row.X10 = bitValue; break;
                    }
                }

                // Устанавливаем результат функции
                row.Result = int.Parse(binaryFunction[i].ToString());

                tableData.Add(row);
            }

            return tableData;
        }

        // Генерация СДНФ и СКНФ
        private void GenerateDNFAndKNF(List<TruthTableRow> tableData, int numVariables)
        {
            var dnfTerms = new List<string>();
            var knfTerms = new List<string>();

            foreach (var row in tableData)
            {
                // Для СДНФ (результат = 1)
                if (row.Result == 1)
                {
                    var term = new List<string>();

                    for (int i = 0; i < numVariables; i++)
                    {
                        int value = 0;
                        string varName = $"x{i + 1}";

                        switch (i)
                        {
                            case 0: value = row.X1; break;
                            case 1: value = row.X2; break;
                            case 2: value = row.X3; break;
                            case 3: value = row.X4; break;
                            case 4: value = row.X5; break;
                            case 5: value = row.X6; break;
                            case 6: value = row.X7; break;
                            case 7: value = row.X8; break;
                            case 8: value = row.X9; break;
                            case 9: value = row.X10; break;
                        }

                        if (value == 0)
                            term.Add($"!{varName}");
                        else
                            term.Add(varName);
                    }

                    dnfTerms.Add($"({string.Join(" & ", term)})");
                }

                // Для СКНФ (результат = 0)
                if (row.Result == 0)
                {
                    var term = new List<string>();

                    for (int i = 0; i < numVariables; i++)
                    {
                        int value = 0;
                        string varName = $"x{i + 1}";

                        switch (i)
                        {
                            case 0: value = row.X1; break;
                            case 1: value = row.X2; break;
                            case 2: value = row.X3; break;
                            case 3: value = row.X4; break;
                            case 4: value = row.X5; break;
                            case 5: value = row.X6; break;
                            case 6: value = row.X7; break;
                            case 7: value = row.X8; break;
                            case 8: value = row.X9; break;
                            case 9: value = row.X10; break;
                        }

                        if (value == 1)
                            term.Add($"!{varName}");
                        else
                            term.Add(varName);
                    }

                    knfTerms.Add($"({string.Join(" | ", term)})");
                }
            }

            // Выводим результаты в текстовые поля (предполагается, что они добавлены в XAML)
            DNFResultText.Text = dnfTerms.Count > 0 ? string.Join(" | ", dnfTerms) : "0";
            KNFResultText.Text = knfTerms.Count > 0 ? string.Join(" & ", knfTerms) : "1";

            // Подсчитываем "стоимость" формул
            int dnfLiterals = dnfTerms.Sum(t => t.Count(c => c == 'x') + t.Count(c => c == '!'));
            int knfLiterals = knfTerms.Sum(t => t.Count(c => c == 'x') + t.Count(c => c == '!'));

            DNFStatsText.Text = $"Литералов: {dnfLiterals}, Конъюнкций: {dnfTerms.Count}";
            KNFStatsText.Text = $"Литералов: {knfLiterals}, Дизъюнкций: {knfTerms.Count}";
        }

        // Обработчик кнопки для вкладки "По формуле"
        private void ParseFormulaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string formula = FormulaTextBox.Text.Trim();

                // Получаем все переменные из формулы
                var variables = GetVariablesFromFormula(formula);
                int numVariables = variables.Count;

                if (numVariables == 0)
                {
                    MessageBox.Show("В формуле нет переменных");
                    return;
                }

                // Создаем таблицу истинности
                var tableData = new List<TruthTableRow>();
                int numRows = (int)Math.Pow(2, numVariables);

                // Лексический анализ и синтаксический анализ формулы
                var rpn = ConvertToRPN(formula);

                for (int i = 0; i < numRows; i++)
                {
                    var row = new TruthTableRow();
                    var variableValues = new Dictionary<string, int>();

                    // Заполняем значения переменных
                    for (int j = 0; j < numVariables; j++)
                    {
                        int bitValue = (i >> (numVariables - 1 - j)) & 1;
                        string varName = variables[j];
                        variableValues[varName] = bitValue;

                        // Заполняем соответствующие поля в строке таблицы
                        if (varName == "x1") row.X1 = bitValue;
                        else if (varName == "x2") row.X2 = bitValue;
                        else if (varName == "x3") row.X3 = bitValue;
                        else if (varName == "x4") row.X4 = bitValue;
                        else if (varName == "x5") row.X5 = bitValue;
                        else if (varName == "x6") row.X6 = bitValue;
                        else if (varName == "x7") row.X7 = bitValue;
                        else if (varName == "x8") row.X8 = bitValue;
                        else if (varName == "x9") row.X9 = bitValue;
                        else if (varName == "x10") row.X10 = bitValue;
                    }

                    // Вычисляем значение формулы для текущего набора переменных
                    row.Result = EvaluateRPN(rpn, variableValues);

                    tableData.Add(row);
                }

                // Заполняем DataGrid
                TruthTableDataGrid.ItemsSource = tableData;

                // Генерируем и выводим СДНФ и СКНФ
                GenerateDNFAndKNF(tableData, numVariables);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при разборе формулы: {ex.Message}");
            }
        }

        // Получение всех переменных из формулы
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

                    i--; // Возвращаемся на один символ назад, так как цикл for увеличит i

                    variables.Add(varName);
                }
            }

            return variables.OrderBy(v => int.Parse(v.Substring(1))).ToList();
        }

        // Преобразование формулы в обратную польскую запись (RPN)
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

                    operators.Pop(); // Удаляем "(" из стека
                }
                else // Оператор
                {
                    // Заменяем небазисные операции на базисные
                    if (token == "->")
                    {
                        // A -> B эквивалентно !A | B
                        operators.Push(")");
                        operators.Push("B");
                        operators.Push("|");
                        operators.Push("!");
                        operators.Push("A");
                        operators.Push("(");
                        continue;
                    }
                    else if (token == "=")
                    {
                        // A = B эквивалентно (A & B) | (!A & !B)
                        operators.Push(")");
                        operators.Push(")");
                        operators.Push("B");
                        operators.Push("!");
                        operators.Push("&");
                        operators.Push("A");
                        operators.Push("!");
                        operators.Push("(");
                        operators.Push("|");
                        operators.Push("B");
                        operators.Push("&");
                        operators.Push("A");
                        operators.Push("(");
                        continue;
                    }
                    else if (token == "^")
                    {
                        // A ^ B эквивалентно (A & !B) | (!A & B)
                        operators.Push(")");
                        operators.Push(")");
                        operators.Push("B");
                        operators.Push("&");
                        operators.Push("A");
                        operators.Push("!");
                        operators.Push("(");
                        operators.Push("|");
                        operators.Push("B");
                        operators.Push("!");
                        operators.Push("&");
                        operators.Push("A");
                        operators.Push("(");
                        continue;
                    }

                    while (operators.Count > 0 && GetOperatorPriority(operators.Peek()) >= GetOperatorPriority(token))
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

        // Лексический анализ формулы
        private List<string> Tokenize(string formula)
        {
            var tokens = new List<string>();
            int i = 0;

            while (i < formula.Length)
            {
                char c = formula[i];

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                if (c == '(' || c == ')')
                {
                    tokens.Add(c.ToString());
                    i++;
                }
                else if (c == '!')
                {
                    tokens.Add("!");
                    i++;
                }
                else if (c == '&')
                {
                    tokens.Add("&");
                    i++;
                }
                else if (c == '|')
                {
                    tokens.Add("|");
                    i++;
                }
                else if (c == '^')
                {
                    tokens.Add("^");
                    i++;
                }
                else if (c == '-')
                {
                    if (i + 1 < formula.Length && formula[i + 1] == '>')
                    {
                        tokens.Add("->");
                        i += 2;
                    }
                    else
                    {
                        throw new Exception($"Неожиданный символ '-' в позиции {i}");
                    }
                }
                else if (c == '=')
                {
                    tokens.Add("=");
                    i++;
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

        // Проверка, является ли токен переменной
        private bool IsVariable(string token)
        {
            return token.StartsWith("x") && token.Length > 1 && token.Substring(1).All(char.IsDigit);
        }

        // Получение приоритета оператора
        private int GetOperatorPriority(string op)
        {
            switch (op)
            {
                case "!": return 4;
                case "&": return 3;
                case "|": return 2;
                case "->": return 1;
                case "=": return 1;
                case "^": return 2;
                default: return 0;
            }
        }

        // Вычисление выражения в RPN
        private int EvaluateRPN(List<string> rpn, Dictionary<string, int> variableValues)
        {
            var stack = new Stack<int>();

            foreach (var token in rpn)
            {
                if (IsVariable(token))
                {
                    stack.Push(variableValues[token]);
                }
                else if (token == "!")
                {
                    if (stack.Count < 1) throw new Exception("Недостаточно операндов для операции !");
                    int a = stack.Pop();
                    stack.Push(NOT(a));
                }
                else if (token == "&")
                {
                    if (stack.Count < 2) throw new Exception("Недостаточно операндов для операции &");
                    int b = stack.Pop();
                    int a = stack.Pop();
                    stack.Push(AND(a, b));
                }
                else if (token == "|")
                {
                    if (stack.Count < 2) throw new Exception("Недостаточно операндов для операции |");
                    int b = stack.Pop();
                    int a = stack.Pop();
                    stack.Push(OR(a, b));
                }
                else if (token == "^")
                {
                    if (stack.Count < 2) throw new Exception("Недостаточно операндов для операции ^");
                    int b = stack.Pop();
                    int a = stack.Pop();
                    stack.Push(XOR(a, b));
                }
                else if (token == "->")
                {
                    if (stack.Count < 2) throw new Exception("Недостаточно операндов для операции ->");
                    int b = stack.Pop();
                    int a = stack.Pop();
                    stack.Push(IMPLICATION(a, b));
                }
                else if (token == "=")
                {
                    if (stack.Count < 2) throw new Exception("Недостаточно операндов для операции =");
                    int b = stack.Pop();
                    int a = stack.Pop();
                    stack.Push(EQUIVALENCE(a, b));
                }
                else
                {
                    throw new Exception($"Неизвестный токен: {token}");
                }
            }

            if (stack.Count != 1)
                throw new Exception("Некорректное выражение");

            return stack.Pop();
        }

        // Логические операции
        private int NOT(int a) => a == 0 ? 1 : 0;
        private int AND(int a, int b) => (a == 1 && b == 1) ? 1 : 0;
        private int OR(int a, int b) => (a == 1 || b == 1) ? 1 : 0;
        private int XOR(int a, int b) => (a != b) ? 1 : 0;
        private int IMPLICATION(int a, int b) => (a == 1 && b == 0) ? 0 : 1;
        private int EQUIVALENCE(int a, int b) => (a == b) ? 1 : 0;

        // Обработчик кнопки для сравнения функций
        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string function1 = CompareFunction1.Text.Trim();
                string function2 = CompareFunction2.Text.Trim();

                // Определяем тип функций (номер или формула)
                bool isNumber1 = int.TryParse(function1, out int num1);
                bool isNumber2 = int.TryParse(function2, out int num2);

                List<TruthTableRow> table1, table2;

                // Получаем таблицы истинности для обеих функций
                if (isNumber1)
                {
                    // Для функции по номеру нужно определить количество переменных
                    // По умолчанию используем 3 переменные, если не указано иное
                    int numVariables = 3;
                    table1 = GenerateTruthTableByNumber(numVariables, num1);
                }
                else
                {
                    table1 = GenerateTruthTableByFormula(function1);
                }

                if (isNumber2)
                {
                    int numVariables = 3;
                    table2 = GenerateTruthTableByNumber(numVariables, num2);
                }
                else
                {
                    table2 = GenerateTruthTableByFormula(function2);
                }

                // Сравниваем таблицы
                bool areEquivalent = CompareTruthTables(table1, table2, out TruthTableRow counterExample);

                // Выводим результат
                if (areEquivalent)
                {
                    CompareResult.Text = "Функции эквивалентны";
                    CounterExample.Text = "";
                }
                else
                {
                    CompareResult.Text = "Функции не эквивалентны";

                    // Формируем контрпример
                    StringBuilder sb = new StringBuilder("Контрпример: ");

                    if (table1.Count > 0)
                    {
                        sb.Append($"x1={counterExample.X1}");
                        if (table1[0].X2 != -1) sb.Append($", x2={counterExample.X2}");
                        if (table1[0].X3 != -1) sb.Append($", x3={counterExample.X3}");

                        sb.Append($", F1={counterExample.Result}");

                        // Находим соответствующую строку во второй таблице
                        var row2 = table2.FirstOrDefault(r =>
                            r.X1 == counterExample.X1 &&
                            r.X2 == counterExample.X2 &&
                            r.X3 == counterExample.X3);

                        if (row2 != null)
                            sb.Append($", F2={row2.Result}");
                    }

                    CounterExample.Text = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сравнении функций: {ex.Message}");
            }
        }

        // Генерация таблицы истинности по формуле
        private List<TruthTableRow> GenerateTruthTableByFormula(string formula)
        {
            var variables = GetVariablesFromFormula(formula);
            int numVariables = variables.Count;

            if (numVariables == 0)
            {
                throw new Exception("В формуле нет переменных");
            }

            var tableData = new List<TruthTableRow>();
            int numRows = (int)Math.Pow(2, numVariables);

            var rpn = ConvertToRPN(formula);

            for (int i = 0; i < numRows; i++)
            {
                var row = new TruthTableRow();
                var variableValues = new Dictionary<string, int>();

                // Заполняем значения переменных
                for (int j = 0; j < numVariables; j++)
                {
                    int bitValue = (i >> (numVariables - 1 - j)) & 1;
                    string varName = variables[j];
                    variableValues[varName] = bitValue;

                    // Заполняем соответствующие поля в строке таблицы
                    if (varName == "x1") row.X1 = bitValue;
                    else if (varName == "x2") row.X2 = bitValue;
                    else if (varName == "x3") row.X3 = bitValue;
                    else if (varName == "x4") row.X4 = bitValue;
                    else if (varName == "x5") row.X5 = bitValue;
                    else if (varName == "x6") row.X6 = bitValue;
                    else if (varName == "x7") row.X7 = bitValue;
                    else if (varName == "x8") row.X8 = bitValue;
                    else if (varName == "x9") row.X9 = bitValue;
                    else if (varName == "x10") row.X10 = bitValue;
                }

                // Вычисляем значение формулы для текущего набора переменных
                row.Result = EvaluateRPN(rpn, variableValues);

                tableData.Add(row);
            }

            return tableData;
        }

        // Сравнение двух таблиц истинности
        private bool CompareTruthTables(List<TruthTableRow> table1, List<TruthTableRow> table2, out TruthTableRow counterExample)
        {
            counterExample = null;

            if (table1.Count != table2.Count)
                return false;

            for (int i = 0; i < table1.Count; i++)
            {
                var row1 = table1[i];
                var row2 = table2[i];

                // Проверяем, что значения переменных совпадают
                if (row1.X1 != row2.X1 || row1.X2 != row2.X2 || row1.X3 != row2.X3)
                    return false;

                // Проверяем, что результаты совпадают
                if (row1.Result != row2.Result)
                {
                    counterExample = row1;
                    return false;
                }
            }

            return true;
        }
    }

    // Класс для хранения строк таблицы (вне класса MainWindow, но внутри namespace)
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
    }
}