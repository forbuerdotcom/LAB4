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
    // Класс главного окна приложения, наследуется от базового класса Window
    public partial class MainWindow : Window
    {
        // Конструктор главного окна. Вызывается при создании окна.
        public MainWindow()
        {
            // Инициализирует компоненты, определенные в XAML-файле (MainWindow.xaml).
            InitializeComponent();

            // Подписываемся на события нажатия кнопки мыши на наши кастомные "вкладки".
            // Это позволяет переключаться между разделами приложения.
            Tab1.MouseDown += Tab1_MouseDown;
            Tab2.MouseDown += Tab2_MouseDown;
            Tab3.MouseDown += Tab3_MouseDown;

            // Активируем первую вкладку при запуске приложения.
            ActivateTab(Tab1, Tab1Content);
        }

        // Обработчик события нажатия на кнопку с вопросом (инструкция).
        private void QuestionButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем многострочную строку с инструкциями для пользователя.
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

            // Показываем инструкцию в модальном окне (MessageBox).
            MessageBox.Show(instructions, "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Обработчики нажатия на вкладки. Каждый из них вызывает универсальный метод ActivateTab.
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

        // Метод для активации выбранной вкладки.
        private void ActivateTab(Border activeTab, Border activeContent)
        {
            // Сначала сбрасываем стили всех вкладок к неактивному состоянию.
            ResetAllTabs();

            // Применяем стили для активной вкладки: меняем фон, цвет текста и жирность шрифта.
            activeTab.Background = new SolidColorBrush(Color.FromRgb(0xCA, 0x90, 0x8E));
            var textBlock = (TextBlock)activeTab.Child; // Получаем TextBlock внутри Border
            textBlock.Foreground = Brushes.White;
            textBlock.FontWeight = FontWeights.SemiBold;

            // Скрываем все панели с содержимым.
            Tab1Content.Visibility = Visibility.Collapsed;
            Tab2Content.Visibility = Visibility.Collapsed;
            Tab3Content.Visibility = Visibility.Collapsed;

            // Отображаем только панель содержимого, соответствующую активной вкладке.
            activeContent.Visibility = Visibility.Visible;
        }

        // Вспомогательный метод для сброса стилей всех вкладок.
        private void ResetAllTabs()
        {
            // Создаем массив всех вкладок для удобства перебора.
            var tabs = new[] { Tab1, Tab2, Tab3 };
            foreach (var tab in tabs)
            {
                // Устанавливаем стиль по умолчанию: прозрачный фон, серый текст, нормальный вес.
                tab.Background = Brushes.Transparent;
                var textBlock = (TextBlock)tab.Child;
                textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
                textBlock.FontWeight = FontWeights.Normal;
            }
        }

        // Метод для настройки столбцов в DataGrid.
        private void SetupDataGridColumns(List<string> variableNames)
        {
            // Очищаем предыдущие столбцы, если они были.
            TruthTableDataGrid.Columns.Clear();

            // Создаем и применяем стиль для заголовков столбцов (фон, цвет текста, отступы).
            var headerStyle = new Style(typeof(DataGridColumnHeader))
            {
                Setters = {
                    new Setter(DataGridColumnHeader.BackgroundProperty, new SolidColorBrush(Color.FromRgb(0xF1, 0xC4, 0xBE))),
                    new Setter(DataGridColumnHeader.ForegroundProperty, Brushes.Black),
                    new Setter(DataGridColumnHeader.FontWeightProperty, FontWeights.Bold),
                    new Setter(DataGridColumnHeader.PaddingProperty, new Thickness(8))
                }
            };

            // Создаем и применяем стиль для ячеек таблицы (границы, отступы, цвет текста).
            var cellStyle = new Style(typeof(DataGridCell))
            {
                Setters = {
                    new Setter(DataGridCell.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0))),
                    new Setter(DataGridCell.BorderThicknessProperty, new Thickness(0, 0, 1, 1)),
                    new Setter(DataGridCell.PaddingProperty, new Thickness(8)),
                    new Setter(DataGridCell.ForegroundProperty, Brushes.Black)
                }
            };

            // Применяем созданные стили к DataGrid.
            TruthTableDataGrid.ColumnHeaderStyle = headerStyle;
            TruthTableDataGrid.CellStyle = cellStyle;

            // Динамически создаем столбцы для каждой переменной.
            foreach (var varName in variableNames)
            {
                // Путь для привязки данных. Он указывает, что значение для ячейки нужно брать из словаря "Values" 
                // объекта TruthTableRow по ключу, равному имени переменной (например, "x1").
                string bindingPath = $"Values[{varName}]";

                var column = new DataGridTextColumn
                {
                    Header = varName, // Заголовок столбца - имя переменной
                    Binding = new Binding(bindingPath) // Привязка к данным
                };
                TruthTableDataGrid.Columns.Add(column);
            }

            // Добавляем финальный столбец для результата функции.
            var resultColumn = new DataGridTextColumn
            {
                Header = "Result",
                Binding = new Binding("Result") // Привязка к свойству "Result"
            };
            TruthTableDataGrid.Columns.Add(resultColumn);
        }

        // Обработчик нажатия на кнопку "Построить" на вкладке "Таблица по номеру".
        private void TableTrueFalseButton_Click(object sender, RoutedEventArgs e)
        {
            try // Используем блок try-catch для отлова ошибок, например, некорректного ввода.
            {
                // Получаем количество переменных и номер функции из текстовых полей.
                int numVariables = int.Parse(NumCountTextBox.Text);
                int functionNumber = int.Parse(FunctionName.Text);
                // Генерируем список имен переменных (x1, x2, ..., xn).
                var variableNames = Enumerable.Range(1, numVariables).Select(i => $"x{i}").ToList();

                // Настраиваем столбцы в DataGrid в соответствии с количеством переменных.
                SetupDataGridColumns(variableNames);
                // Генерируем таблицу истинности с помощью метода из нашей библиотеки.
                var tableData = LibLab4.LibLab4.GenerateTruthTableByNumber(numVariables, functionNumber, variableNames);
                // Устанавливаем сгенерированные данные как источник для DataGrid.
                TruthTableDataGrid.ItemsSource = tableData;

                // Генерируем ДНФ и КНФ по полученной таблице.
                (string dnf, string knf) = LibLab4.LibLab4.GenerateDNFAndKNF(tableData, variableNames);
                // Отображаем полученные формулы в текстовых полях.
                DNFResultText.Text = dnf;
                KNFResultText.Text = knf;

                // --- Вычисление и отображение статистики для ДНФ и КНФ ---
                numVariables = variableNames.Count;

                // ДНФ: считаем количество термов (конъюнкций), разделяя строку по '|'.
                int dnfTermsCount = dnf.Split('|', StringSplitOptions.RemoveEmptyEntries).Length;
                // Общее количество литералов = количество термов * количество переменных в каждом.
                int dnfLiterals = dnfTermsCount * numVariables;

                // КНФ: считаем количество термов (дизъюнкций), разделяя строку по '&'.
                int knfTermsCount = knf.Split('&', StringSplitOptions.RemoveEmptyEntries).Length;
                // Общее количество литералов = количество термов * количество переменных в каждом.
                int knfLiterals = knfTermsCount * numVariables;

                // Отображаем статистику.
                DNFStatsText.Text = $"Литералов: {dnfLiterals}, Конъюнкций: {dnfTermsCount}";
                KNFStatsText.Text = $"Литералов: {knfLiterals}, Дизъюнкций: {knfTermsCount}";
            }
            catch (FormatException) // Ловим ошибку, если введено не число.
            {
                MessageBox.Show("Пожалуйста, введите корректные числовые значения");
            }
            catch (Exception ex) // Ловим все остальные возможные ошибки (например, из библиотеки).
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        // Обработчик нажатия на кнопку "Построить" на вкладке "Таблица по формуле".
        private void ParseFormulaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем формулу из текстового поля и убираем лишние пробелы.
                string formula = FormulaTextBox.Text.Trim();
                // Извлекаем имена переменных из строки формулы с помощью метода из библиотеки.
                var variableNames = LibLab4.LibLab4.GetVariablesFromFormula(formula);
                if (variableNames.Count == 0) { MessageBox.Show("В формуле нет переменных"); return; }

                // Настраиваем столбцы DataGrid.
                SetupDataGridColumns(variableNames);
                // Генерируем таблицу истинности по формуле.
                var tableData = LibLab4.LibLab4.GenerateTruthTableByFormula(formula, variableNames);
                // Отображаем таблицу.
                TruthTableDataGrid.ItemsSource = tableData;

                // Генерируем и отображаем ДНФ и КНФ (аналогично предыдущему методу).
                (string dnf, string knf) = LibLab4.LibLab4.GenerateDNFAndKNF(tableData, variableNames);
                DNFResultText.Text = dnf;
                KNFResultText.Text = knf;

                // Вычисляем и отображаем статистику (аналогично предыдущему методу).
                int numVariables = variableNames.Count;
                int dnfTermsCount = dnf.Split('|', StringSplitOptions.RemoveEmptyEntries).Length;
                int dnfLiterals = dnfTermsCount * numVariables;
                int knfTermsCount = knf.Split('&', StringSplitOptions.RemoveEmptyEntries).Length;
                int knfLiterals = knfTermsCount * numVariables;

                DNFStatsText.Text = $"Литералов: {dnfLiterals}, Конъюнкций: {dnfTermsCount}";
                KNFStatsText.Text = $"Литералов: {knfLiterals}, Дизъюнкций: {knfTermsCount}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при разборе формулы: {ex.Message}");
            }
        }

        // Обработчик нажатия на кнопку "Сравнить" на третьей вкладке.
        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем строки с функциями для сравнения.
                string function1 = CompareFunction1.Text.Trim();
                string function2 = CompareFunction2.Text.Trim();
                // Пытаемся преобразовать строки в числа, чтобы определить тип ввода (номер или формула).
                bool isNumber1 = int.TryParse(function1, out int num1);
                bool isNumber2 = int.TryParse(function2, out int num2);

                // Создаем единый набор переменных для обеих функций. Это необходимо для корректного сравнения.
                var allVariablesSet = new HashSet<string>();
                // Если первая функция - номер, добавляем стандартный набор x1, x2, x3.
                if (isNumber1) for (int i = 1; i <= 3; i++) allVariablesSet.Add($"x{i}");
                // Если это формула, извлекаем переменные из нее.
                else LibLab4.LibLab4.GetVariablesFromFormula(function1).ForEach(v => allVariablesSet.Add(v));
                // То же самое для второй функции.
                if (isNumber2) for (int i = 1; i <= 3; i++) allVariablesSet.Add($"x{i}");
                else LibLab4.LibLab4.GetVariablesFromFormula(function2).ForEach(v => allVariablesSet.Add(v));

                // Преобразуем HashSet в отсортированный список.
                var allVariables = allVariablesSet.OrderBy(v => int.Parse(v.Substring(1))).ToList();
                // Настраиваем DataGrid для отображения всех переменных.
                SetupDataGridColumns(allVariables);

                // Генерируем первую таблицу истинности.
                List<LibLab4.TruthTableRow> table1 = isNumber1 ?
                    LibLab4.LibLab4.GenerateTruthTableByNumber(allVariables.Count, num1, allVariables) :
                    LibLab4.LibLab4.GenerateTruthTableByFormula(function1, allVariables);
                // Генерируем вторую таблицу истинности.
                List<LibLab4.TruthTableRow> table2 = isNumber2 ?
                    LibLab4.LibLab4.GenerateTruthTableByNumber(allVariables.Count, num2, allVariables) :
                    LibLab4.LibLab4.GenerateTruthTableByFormula(function2, allVariables);

                // Отображаем первую таблицу в DataGrid.
                TruthTableDataGrid.ItemsSource = table1;
                // Сравниваем две таблицы с помощью метода из библиотеки.
                bool areEquivalent = LibLab4.LibLab4.CompareTruthTables(table1, table2, out LibLab4.TruthTableRow counterExample);

                // Отображаем результат сравнения.
                if (areEquivalent)
                {
                    CompareResult.Text = "Функции эквивалентны";
                    CounterExample.Text = ""; // Контрпример не нужен.
                }
                else
                {
                    CompareResult.Text = "Функции не эквивалентны";
                    // Формируем строку с контрпримером.
                    var sb = new StringBuilder("Контрпример: ");
                    bool firstVar = true;
                    // Перебираем все переменные и их значения в строке-контрпримере.
                    foreach (var varName in allVariables)
                    {
                        if (!firstVar) sb.Append(", ");
                        sb.Append($"{varName}={counterExample.GetValue(varName)}");
                        firstVar = false;
                    }
                    // Находим соответствующую строку во второй таблице, чтобы показать результат второй функции.
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
    }

    public class TruthTableRow
    {
        // Словарь для хранения пар "имя переменной - значение".
        public Dictionary<string, int> Values { get; set; } = new Dictionary<string, int>();
        // Свойство для результата функции.
        public int Result { get; set; }

        // Метод для получения значения переменной.
        public int GetValue(string varName)
        {
            return Values.TryGetValue(varName, out int val) ? val : -1;
        }

        // Метод для установки значения переменной.
        public void SetValue(string varName, int value)
        {
            Values[varName] = value;
        }
    }
}