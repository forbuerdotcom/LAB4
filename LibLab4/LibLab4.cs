// Импортируем необходимые пространства имен для работы с коллекциями, LINQ и базовыми функциями.
using System;
using System.Collections.Generic;
using System.Linq;

// Определяем пространство имен для нашей библиотеки.
namespace LibLab4
{
    // Объявляем статический класс LibLab4. Статический класс означает, что его методы можно вызывать 
    // без создания экземпляра класса (например, LibLab4.GenerateTruthTable(...)).
    public static class LibLab4
    {
        // Регион для методов, генерирующих таблицы истинности.
        #region Truth Table Generation

        /// <summary>
        /// Генерирует таблицу истинности по номеру функции (совершенная дизъюнктивная нормальная форма - СДНФ).
        /// </summary>
        /// <param name="numVariables">Количество переменных в функции (от 1 до 20).</param>
        /// <param name="functionNumber">Номер функции, который определяет значения в столбце результата.</param>
        /// <param name="variableNames">Список имен переменных (например, ["x1", "x2", "x3"]).</param>
        /// <returns>Список объектов TruthTableRow, представляющих строки таблицы истинности.</returns>
        public static List<TruthTableRow> GenerateTruthTableByNumber(int numVariables, int functionNumber, List<string> variableNames)
        {
            // Проверяем, что количество переменных находится в допустимом диапазоне (от 1 до 20).
            if (numVariables < 1 || numVariables > 20)
                throw new ArgumentOutOfRangeException(nameof(numVariables), "Количество переменных должно быть от 1 до 20");

            // Вычисляем количество строк в таблице истинности. Для n переменных существует 2^n комбинаций.
            int numRows = (int)Math.Pow(2, numVariables);
            // Вычисляем максимальный возможный номер функции. 
            // Например, для 2 переменных (4 строки) максимальный номер - 2^4 - 1 = 15 (1111 в двоичной системе).
            long maxFunctionNumber = (long)Math.Pow(2, numRows) - 1;
            // Проверяем, что номер функции находится в допустимом диапазоне.
            if (functionNumber < 0 || functionNumber > maxFunctionNumber)
                throw new ArgumentOutOfRangeException(nameof(functionNumber), $"Номер функции должен быть от 0 до {maxFunctionNumber}");

            // Создаем список для хранения строк таблицы истинности.
            var tableData = new List<TruthTableRow>();
            // Преобразуем номер функции в двоичную строку. Эта строка будет представлять столбец результатов.
            // PadLeft добавляет нули слева, чтобы длина строки совпадала с количеством строк.
            string binaryFunction = Convert.ToString(functionNumber, 2).PadLeft(numRows, '0');

            // Цикл по всем строкам таблицы истинности.
            for (int i = 0; i < numRows; i++)
            {
                // Создаем новый объект для представления текущей строки.
                var row = new TruthTableRow();
                // Внутренний цикл для генерации значений для каждой переменной в текущей строке.
                for (int j = 0; j < numVariables; j++)
                {
                    // Используем битовые сдвиги для эффективной генерации комбинаций 0 и 1.
                    // (i >> (numVariables - 1 - j)) сдвигает биты числа i вправо.
                    // & 1 извлекает самый младший бит (0 или 1).
                    // Это позволяет получить значение j-й переменной для i-й строки.
                    int bitValue = (i >> (numVariables - 1 - j)) & 1;
                    // Устанавливаем значение для переменной с именем variableNames[j].
                    row.SetValue(variableNames[j], bitValue);
                }
                // Устанавливаем результат для текущей строки, беря i-й символ из двоичного представления функции.
                row.Result = int.Parse(binaryFunction[i].ToString());
                // Добавляем заполненную строку в таблицу.
                tableData.Add(row);
            }

            // Возвращаем сгенерированную таблицу истинности.
            return tableData;
        }

        /// <summary>
        /// Генерирует таблицу истинности на основе логической формулы.
        /// </summary>
        /// <param name="formula">Логическая формула (например, "x1 & x2 | !x3").</param>
        /// <param name="variables">Список переменных, используемых в формуле.</param>
        /// <returns>Список объектов TruthTableRow, представляющих строки таблицы истинности.</returns>
        public static List<TruthTableRow> GenerateTruthTableByFormula(string formula, List<string> variables)
        {
            // Получаем количество переменных.
            int numVariables = variables.Count;
            // Проверяем, что в формуле есть переменные.
            if (numVariables == 0) throw new Exception("В формуле нет переменных");

            // Создаем список для хранения строк таблицы истинности.
            var tableData = new List<TruthTableRow>();
            // Вычисляем количество строк.
            int numRows = (int)Math.Pow(2, numVariables);
            // Преобразуем инфиксную формулу в постфиксную (обратную польскую нотацию - RPN) для упрощения вычислений.
            var rpn = ConvertToRPN(formula);

            // Цикл по всем строкам таблицы истинности.
            for (int i = 0; i < numRows; i++)
            {
                // Создаем новый объект для представления текущей строки.
                var row = new TruthTableRow();
                // Создаем словарь для хранения значений переменных для текущей строки.
                var variableValues = new Dictionary<string, int>();

                // Внутренний цикл для генерации значений для каждой переменной.
                for (int j = 0; j < numVariables; j++)
                {
                    // Генерируем значение (0 или 1) для j-й переменной в i-й строке.
                    int bitValue = (i >> (numVariables - 1 - j)) & 1;
                    // Получаем имя переменной.
                    string varName = variables[j];
                    // Сохраняем значение в словаре для последующего вычисления формулы.
                    variableValues[varName] = bitValue;
                    // Устанавливаем значение для переменной в объекте строки.
                    row.SetValue(varName, bitValue);
                }

                // Вычисляем результат формулы для текущего набора значений переменных.
                row.Result = EvaluateRPN(rpn, variableValues);
                // Добавляем заполненную строку в таблицу.
                tableData.Add(row);
            }

            // Возвращаем сгенерированную таблицу истинности.
            return tableData;
        }

        #endregion

        // Регион для методов, генерирующих ДНФ и КНФ.
        #region DNF and KNF Generation

        /// <summary>
        /// Генерирует совершенные дизъюнктивную (СДНФ) и конъюнктивную (СКНФ) нормальные формы по таблице истинности.
        /// </summary>
        /// <param name="tableData">Таблица истинности.</param>
        /// <param name="variables">Список имен переменных.</param>
        /// <returns>Кортеж, содержащий строки ДНФ и КНФ.</returns>
        public static (string DNF, string KNF) GenerateDNFAndKNF(List<TruthTableRow> tableData, List<string> variables)
        {
            // Создаем списки для хранения членов (термов) ДНФ и КНФ.
            var dnfTerms = new List<string>();
            var knfTerms = new List<string>();

            // Проходим по каждой строке таблицы истинности.
            foreach (var row in tableData)
            {
                // Если результат в строке равен 1, создаем член для ДНФ.
                if (row.Result == 1)
                {
                    // Создаем список для хранения частей текущего члена ДНФ.
                    var term = new List<string>();
                    // Проходим по всем переменным.
                    foreach (var varName in variables)
                    {
                        // Получаем значение переменной в текущей строке.
                        int value = row.GetValue(varName);
                        // Если значение переменной равно 0, в член ДНФ идет ее отрицание.
                        // Если значение равно 1, идет сама переменная.
                        term.Add(value == 0 ? $"!{varName}" : varName);
                    }
                    // Соединяем все части члена через конъюнкцию ("&") и добавляем в список членов ДНФ.
                    dnfTerms.Add($"({string.Join(" & ", term)})");
                }

                // Если результат в строке равен 0, создаем член для КНФ.
                if (row.Result == 0)
                {
                    // Создаем список для хранения частей текущего члена КНФ.
                    var term = new List<string>();
                    // Проходим по всем переменным.
                    foreach (var varName in variables)
                    {
                        // Получаем значение переменной в текущей строке.
                        int value = row.GetValue(varName);
                        // Если значение переменной равно 1, в член КНФ идет ее отрицание.
                        // Если значение равно 0, идет сама переменная.
                        term.Add(value == 1 ? $"!{varName}" : varName);
                    }
                    // Соединяем все части члена через дизъюнкцию ("|") и добавляем в список членов КНФ.
                    knfTerms.Add($"({string.Join(" | ", term)})");
                }
            }

            // Соединяем все члены ДНФ через дизъюнкцию ("|").
            // Если членов нет (функция всегда ложна), ДНФ равна "0".
            string dnf = dnfTerms.Count > 0 ? string.Join(" | ", dnfTerms) : "0";
            // Соединяем все члены КНФ через конъюнкцию ("&").
            // Если членов нет (функция всегда истинна), КНФ равна "1".
            string knf = knfTerms.Count > 0 ? string.Join(" & ", knfTerms) : "1";

            // Возвращаем ДНФ и КНФ в виде кортежа.
            return (dnf, knf);
        }

        #endregion

        // Регион для методов, связанных с парсингом и вычислением формул.
        #region Formula Parsing and Evaluation

        /// <summary>
        /// Извлекает все переменные из строки формулы.
        /// </summary>
        /// <param name="formula">Строка с логической формулой.</param>
        /// <returns>Отсортированный список имен переменных.</returns>
        public static List<string> GetVariablesFromFormula(string formula)
        {
            // Используем HashSet для автоматического удаления дубликатов переменных.
            var variables = new HashSet<string>();
            // Проходим по каждому символу в формуле.
            for (int i = 0; i < formula.Length; i++)
            {
                // Ищем переменные, которые начинаются с 'x' и за которыми следуют цифры.
                if (formula[i] == 'x' && i + 1 < formula.Length && char.IsDigit(formula[i + 1]))
                {
                    // Начинаем формировать имя переменной.
                    string varName = "x";
                    i++; // Переходим к следующему символу (цифре).
                    // Продолжаем считывать цифры, пока они есть.
                    while (i < formula.Length && char.IsDigit(formula[i]))
                    {
                        varName += formula[i];
                        i++;
                    }
                    i--; // Возвращаемся на один шаг назад, так как внешний цикл сделает i++.
                    // Добавляем найденное имя переменной в HashSet.
                    variables.Add(varName);
                }
            }
            // Преобразуем HashSet в список и сортируем его по числовой части имени переменной.
            return variables.OrderBy(v => int.Parse(v.Substring(1))).ToList();
        }

        /// <summary>
        /// Преобразует инфиксную формулу в обратную польскую нотацию (RPN) с помощью алгоритма "сортировочной станции".
        /// </summary>
        /// <param name="formula">Инфиксная формула.</param>
        /// <returns>Список токенов в RPN.</returns>
        public static List<string> ConvertToRPN(string formula)
        {
            // Разбиваем формулу на токены (переменные, операторы, скобки).
            var tokens = Tokenize(formula);
            // Создаем выходную очередь для RPN.
            var output = new List<string>();
            // Создаем стек для операторов.
            var operators = new Stack<string>();

            // Проходим по каждому токену.
            foreach (var token in tokens)
            {
                // Если токен - это переменная, добавляем его в выходную очередь.
                if (IsVariable(token))
                {
                    output.Add(token);
                }
                // Если токен - открывающая скобка, помещаем ее в стек операторов.
                else if (token == "(")
                {
                    operators.Push(token);
                }
                // Если токен - закрывающая скобка:
                else if (token == ")")
                {
                    // Перекладываем операторы из стека в выходную очередь, пока не встретим открывающую скобку.
                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        output.Add(operators.Pop());
                    }
                    // Если стек пуст, значит скобки несогласованы.
                    if (operators.Count == 0) throw new Exception("Несогласованные скобки в формуле");
                    // Удаляем открывающую скобку из стека.
                    operators.Pop(); // Выкидываем открывающую скобку
                }
                // Если токен - это оператор:
                else // Если токен - это оператор
                {
                    // Унарный оператор "!" имеет правую ассоциативность.
                    bool isRightAssociative = (token == "!");

                    // Пока на вершине стека оператор с большим или равным приоритетом (для лево-ассоциативных)
                    // или с большим приоритетом (для право-ассоциативных), перекладываем его в выходную очередь.
                    while (operators.Count > 0 && operators.Peek() != "(" &&
                           (isRightAssociative ? GetOperatorPriority(operators.Peek()) > GetOperatorPriority(token)
                                               : GetOperatorPriority(operators.Peek()) >= GetOperatorPriority(token)))
                    {
                        output.Add(operators.Pop());
                    }
                    // Помещаем текущий оператор в стек.
                    operators.Push(token);
                }
            }

            // После обработки всех токенов перекладываем оставшиеся операторы из стека в выходную очередь.
            while (operators.Count > 0)
            {
                string op = operators.Pop();
                // Если в стеке остались скобки, значит они несогласованы.
                if (op == "(" || op == ")") throw new Exception("Несогласованные скобки в формуле");
                output.Add(op);
            }
            // Возвращаем список токенов в RPN.
            return output;
        }

        /// <summary>
        /// Разбивает строку формулы на токены.
        /// </summary>
        /// <param name="formula">Строка формулы.</param>
        /// <returns>Список токенов.</returns>
        private static List<string> Tokenize(string formula)
        {
            var tokens = new List<string>();
            int i = 0;
            // Проходим по строке формулы.
            while (i < formula.Length)
            {
                char c = formula[i];
                // Пропускаем пробелы.
                if (char.IsWhiteSpace(c)) { i++; continue; }
                // Если символ - это оператор или скобка, добавляем его как отдельный токен.
                if (c == '(' || c == ')' || c == '!' || c == '&' || c == '|' || c == '^' || c == '=')
                {
                    tokens.Add(c.ToString());
                    i++;
                }
                // Обработка оператора импликации "->".
                else if (c == '-' && i + 1 < formula.Length && formula[i + 1] == '>')
                {
                    tokens.Add("->");
                    i += 2;
                }
                // Если символ - это 'x', начинаем считывать имя переменной.
                else if (c == 'x')
                {
                    string varName = "x";
                    i++;
                    // Считываем все последующие цифры как часть имени переменной.
                    while (i < formula.Length && char.IsDigit(formula[i]))
                    {
                        varName += formula[i];
                        i++;
                    }
                    tokens.Add(varName);
                }
                else
                {
                    // Если встречен неожиданный символ, генерируем исключение.
                    throw new Exception($"Неожиданный символ '{c}' в позиции {i}");
                }
            }
            return tokens;
        }

        /// <summary>
        /// Проверяет, является ли токен переменной.
        /// </summary>
        /// <param name="token">Токен для проверки.</param>
        /// <returns>True, если токен - переменная, иначе False.</returns>
        private static bool IsVariable(string token) => token.StartsWith("x") && token.Length > 1 && token.Substring(1).All(char.IsDigit);

        /// <summary>
        /// Возвращает приоритет оператора.
        /// </summary>
        /// <param name="op">Оператор.</param>
        /// <returns>Числовое значение приоритета.</returns>
        private static int GetOperatorPriority(string op)
        {
            // Чем больше число, тем выше приоритет.
            switch (op) { case "!": return 4; case "&": return 3; case "|": return 2; case "^": return 2; case "->": return 1; case "=": return 1; default: return 0; }
        }

        /// <summary>
        /// Вычисляет значение формулы в обратной польской нотации (RPN).
        /// </summary>
        /// <param name="rpn">Список токенов в RPN.</param>
        /// <param name="variableValues">Словарь со значениями переменных.</param>
        /// <returns>Результат вычисления (0 или 1).</returns>
        private static int EvaluateRPN(List<string> rpn, Dictionary<string, int> variableValues)
        {
            // Создаем стек для вычислений.
            var stack = new Stack<int>();
            // Проходим по каждому токену в RPN.
            foreach (var token in rpn)
            {
                // Если токен - переменная, помещаем ее значение в стек.
                if (IsVariable(token))
                {
                    stack.Push(variableValues[token]);
                }
                // Если токен - оператор:
                else
                {
                    // Выполняем соответствующую операцию.
                    switch (token)
                    {
                        case "!": // Унарное отрицание
                            stack.Push(NOT(stack.Pop()));
                            break;
                        case "&": // Конъюнкция (И)
                            int op2_and = stack.Pop();
                            int op1_and = stack.Pop();
                            stack.Push(AND(op1_and, op2_and));
                            break;
                        case "|": // Дизъюнкция (ИЛИ)
                            int op2_or = stack.Pop();
                            int op1_or = stack.Pop();
                            stack.Push(OR(op1_or, op2_or));
                            break;
                        case "^": // Исключающее ИЛИ (XOR)
                            int op2_xor = stack.Pop();
                            int op1_xor = stack.Pop();
                            stack.Push(XOR(op1_xor, op2_xor));
                            break;
                        case "->": // Импликация
                            int op2_impl = stack.Pop();
                            int op1_impl = stack.Pop();
                            stack.Push(IMPLICATION(op1_impl, op2_impl));
                            break;
                        case "=": // Эквивалентность
                            int op2_eq = stack.Pop();
                            int op1_eq = stack.Pop();
                            stack.Push(EQUIVALENCE(op1_eq, op2_eq));
                            break;
                        default:
                            throw new Exception($"Неизвестный токен: {token}");
                    }
                }
            }
            // После обработки всех токенов в стеке должен остаться один элемент - результат.
            if (stack.Count != 1) throw new Exception("Некорректное выражение");
            return stack.Pop();
        }

        #endregion

        // Регион для базовых логических операций.
        #region Logical Operators

        // Операторы реализованы как простые функции для ясности.
        private static int NOT(int a) => a == 0 ? 1 : 0;
        private static int AND(int a, int b) => (a == 1 && b == 1) ? 1 : 0;
        private static int OR(int a, int b) => (a == 1 || b == 1) ? 1 : 0;
        private static int XOR(int a, int b) => (a != b) ? 1 : 0;
        private static int IMPLICATION(int a, int b) => (a == 1 && b == 0) ? 0 : 1;
        private static int EQUIVALENCE(int a, int b) => (a == b) ? 1 : 0;

        #endregion

        // Регион для метода сравнения таблиц истинности.
        #region Comparison

        /// <summary>
        /// Сравнивает две таблицы истинности на идентичность.
        /// </summary>
        /// <param name="table1">Первая таблица.</param>
        /// <param name="table2">Вторая таблица.</param>
        /// <param name="counterExample">Возвращает первую строку, в которой таблицы различаются.</param>
        /// <returns>True, если таблицы идентичны, иначе False.</returns>
        public static bool CompareTruthTables(List<TruthTableRow> table1, List<TruthTableRow> table2, out TruthTableRow counterExample)
        {
            counterExample = null;
            // Сначала проверяем, что количество строк в таблицах совпадает.
            if (table1.Count != table2.Count) return false;
            // Проходим по всем строкам и сравниваем результаты.
            for (int i = 0; i < table1.Count; i++)
            {
                // Если результаты в строках не совпадают:
                if (table1[i].Result != table2[i].Result)
                {
                    // Сохраняем строку-контрпример и возвращаем false.
                    counterExample = table1[i];
                    return false;
                }
            }
            // Если все строки совпали, возвращаем true.
            return true;
        }

        #endregion
    }

    // Класс для представления одной строки таблицы истинности.
    public class TruthTableRow
    {
        // Используем словарь для хранения значений переменных.
        // Ключ - имя переменной (например, "x1"), значение - 0 или 1.
        public Dictionary<string, int> Values { get; set; } = new Dictionary<string, int>();
        // Свойство для хранения результата функции для данной строки.
        public int Result { get; set; }

        // Метод для получения значения переменной по ее имени.
        public int GetValue(string varName)
        {
            // Пытаемся получить значение из словаря. Если ключ не найден, возвращаем -1.
            return Values.TryGetValue(varName, out int val) ? val : -1;
        }

        // Метод для установки значения переменной.
        public void SetValue(string varName, int value)
        {
            // Добавляем или обновляем пару "ключ-значение" в словаре.
            Values[varName] = value;
        }
    }
}