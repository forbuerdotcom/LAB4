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
using LibLab4;

namespace Lab4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Tab1.MouseDown += Tab1_MouseDown;
            Tab2.MouseDown += Tab2_MouseDown;
            Tab3.MouseDown += Tab3_MouseDown;
            ActivateTab(Tab1, Tab1Content);
        }
        private void QuestionButton_Click(object sender, RoutedEventArgs e)
        {
            string instructions = 
                "Инструкция по использованию приложения.\n\n" +
        
                "--- Вкладка \"Таблица по номеру\" ---\n" +
                "1. Введите количество переменных (от 3 до 20).\n" +
                "2. Введите номер функции в десятичной системе.\n" +
                "3. Нажмите \"Построить\" для получения таблицы, ДНФ и КНФ.\n\n" +

                "--- Вкладка \"Таблица по формуле\" ---\n" +
                "1. Введите логическую формулу (например: !(x1 & x2) | x3).\n" +
                "2. Используйте операторы: ! (НЕ), & (И), | (ИЛИ), ^ (XOR), -> (импликация), = (эквивалентность).\n" +
                "3. Нажмите \"Построить\" для анализа.\n\n" +

                "--- Вкладка \"Сравнение функций\" ---\n" +
                "1. Введите две функции (номера или формулы) в соответствующие поля.\n" +
                "2. Нажмите \"Сравнить\".\n" +
                "3. Результат покажет, эквивалентны ли функции, или предоставит контрпример.";

            MessageBox.Show(instructions, "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void ActivateTab(Border activeTab, Border activeContent)
        {
            ResetAllTabs();
            activeTab.Background = new SolidColorBrush(Color.FromRgb(0xCA, 0x90, 0x8E));
            var textBlock = (TextBlock)activeTab.Child;
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
                var textBlock = (TextBlock)tab.Child;
                textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
                textBlock.FontWeight = FontWeights.Normal;
            }
        }

        // ИЗМЕНЕННЫЙ МЕТОД
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
                // Создаем привязку к словарю Values по ключу, например "Values[x1]"
                string bindingPath = $"Values[{varName}]";

                var column = new DataGridTextColumn
                {
                    Header = varName,
                    Binding = new Binding(bindingPath)
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
                var variableNames = Enumerable.Range(1, numVariables).Select(i => $"x{i}").ToList();

                SetupDataGridColumns(variableNames);
                var tableData = LibLab4.LibLab4.GenerateTruthTableByNumber(numVariables, functionNumber, variableNames);
                TruthTableDataGrid.ItemsSource = tableData;

                (string dnf, string knf) = LibLab4.LibLab4.GenerateDNFAndKNF(tableData, variableNames);
                DNFResultText.Text = dnf;
                KNFResultText.Text = knf;

                numVariables = variableNames.Count;

                int dnfTermsCount = dnf.Split('|', StringSplitOptions.RemoveEmptyEntries).Length;
                int dnfLiterals = dnfTermsCount * numVariables;

                int knfTermsCount = knf.Split('&', StringSplitOptions.RemoveEmptyEntries).Length;
                int knfLiterals = knfTermsCount * numVariables;

                DNFStatsText.Text = $"Литералов: {dnfLiterals}, Конъюнкций: {dnfTermsCount}";
                KNFStatsText.Text = $"Литералов: {knfLiterals}, Дизъюнкций: {knfTermsCount}";
            }
            catch (FormatException) { MessageBox.Show("Пожалуйста, введите корректные числовые значения"); }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
        }

        private void ParseFormulaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string formula = FormulaTextBox.Text.Trim();
                var variableNames = LibLab4.LibLab4.GetVariablesFromFormula(formula);
                if (variableNames.Count == 0) { MessageBox.Show("В формуле нет переменных"); return; }
                SetupDataGridColumns(variableNames);
                var tableData = LibLab4.LibLab4.GenerateTruthTableByFormula(formula, variableNames);
                TruthTableDataGrid.ItemsSource = tableData;
                (string dnf, string knf) = LibLab4.LibLab4.GenerateDNFAndKNF(tableData, variableNames);
                DNFResultText.Text = dnf;
                KNFResultText.Text = knf;

                int numVariables = variableNames.Count;
                int dnfTermsCount = dnf.Split('|', StringSplitOptions.RemoveEmptyEntries).Length;
                int dnfLiterals = dnfTermsCount * numVariables;
                int knfTermsCount = knf.Split('&', StringSplitOptions.RemoveEmptyEntries).Length;
                int knfLiterals = knfTermsCount * numVariables;

                DNFStatsText.Text = $"Литералов: {dnfLiterals}, Конъюнкций: {dnfTermsCount}";
                KNFStatsText.Text = $"Литералов: {knfLiterals}, Дизъюнкций: {knfTermsCount}";
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка при разборе формулы: {ex.Message}"); }
        }

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
                else LibLab4.LibLab4.GetVariablesFromFormula(function1).ForEach(v => allVariablesSet.Add(v));
                if (isNumber2) for (int i = 1; i <= 3; i++) allVariablesSet.Add($"x{i}");
                else LibLab4.LibLab4.GetVariablesFromFormula(function2).ForEach(v => allVariablesSet.Add(v));

                var allVariables = allVariablesSet.OrderBy(v => int.Parse(v.Substring(1))).ToList();
                SetupDataGridColumns(allVariables);

                List<LibLab4.TruthTableRow> table1 = isNumber1 ?
                    LibLab4.LibLab4.GenerateTruthTableByNumber(allVariables.Count, num1, allVariables) :
                    LibLab4.LibLab4.GenerateTruthTableByFormula(function1, allVariables);
                List<LibLab4.TruthTableRow> table2 = isNumber2 ?
                    LibLab4.LibLab4.GenerateTruthTableByNumber(allVariables.Count, num2, allVariables) :
                    LibLab4.LibLab4.GenerateTruthTableByFormula(function2, allVariables);

                TruthTableDataGrid.ItemsSource = table1;
                bool areEquivalent = LibLab4.LibLab4.CompareTruthTables(table1, table2, out LibLab4.TruthTableRow counterExample);

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
            catch (Exception ex) { MessageBox.Show($"Ошибка при сравнении функций: {ex.Message}"); }
        }
    }

    public class TruthTableRow
    {
        // Используем словарь для хранения значений переменных
        public Dictionary<string, int> Values { get; set; } = new Dictionary<string, int>();
        public int Result { get; set; }

        public int GetValue(string varName)
        {
            // Просто возвращаем значение из словаря
            return Values.TryGetValue(varName, out int val) ? val : -1;
        }

        public void SetValue(string varName, int value)
        {
            // Просто устанавливаем значение в словаре
            Values[varName] = value;
        }
    }
}