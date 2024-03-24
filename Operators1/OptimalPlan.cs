namespace Operators 
{
    public class OptimalPlan
    {
        public List<decimal> Coeffs { get; private set; } = new();      // Коэффициенты переменных
        public List<Limitation> Limits { get; private set; } = new();   // Ограничения
        public SimplexTable Solution { get; private set; }              // Решение в виде симплексной таблицы
        public bool IsEquality { get; private set; }                    // Равенство или неравенство
        private Dictionary<int, int> Variables = new();                 // Словарь смены переменных

        public OptimalPlan(List<decimal> coeffs, bool isEquality = false)
        {
            Coeffs = coeffs;
            IsEquality = isEquality;
        }

        // Получуение оптимального плана в симплексной таблице - lab.1
        public void GetOptimalPlan()
        {
            SimplexTable table = new(Limits, Coeffs, IsEquality);
            InitVars(Limits.Count, Coeffs.Count + Limits.Count);

            // Пока находятся отрицательные элементы в индекс строке
            while (!AreNonNegative(table.IndexString))
            {
                var columnIndex = table.MaxAbsValue();          // Находим решающий столбец
                var rowIndex = table.MinRelation(columnIndex);  // Находим рещающий элемент
                table.Recount(rowIndex, columnIndex);           // Пересчитываем симплекс таблицу правилом прямоугольника
                RememberVariables(rowIndex, columnIndex);       // Запоминаем номер новой переменной
            }

            Solution = table;
        }

        // Задача распределения ресурсов - lab. 2
        public void ManageRecources()
        {
            SimplexTable table = new(Limits, Coeffs, IsEquality);
            InitVars(Limits.Count, Coeffs.Count);

            // Получаем первый опорный план
            var changes = table.GetReferencePlan();
            foreach (var change in changes)
                RememberVariables(change.Item1, change.Item2);

            while (!AreNonNegative(table.IndexString))
            {
                var columnIndex = table.MaxAbsValue();
                var rowIndex = table.MinRelation(columnIndex);
                table.Recount(rowIndex, columnIndex);
                RememberVariables(rowIndex, columnIndex);
            }

            Solution = table;
        }

        // Приведение к канонической форме
        public void ReduceToCanonical()
        {
            int size = Limits.Count;
            for (int i = 0; i < size; ++i)
                Limits[i].Expand(size, i);
        }

        private void InitVars(int rows, int columns)
        {
            int offset = columns - rows;
            for (int i = 0; i < rows; i++)
                RememberVariables(i, offset + i);
        }

        // Добавление ограничений
        public void AddLimitation(List<decimal> coeffs, decimal bound) => Limits.Add(new Limitation(coeffs, bound));
        private void RememberVariables(int rowIndex, int columnIndex) => Variables[rowIndex] = columnIndex + 1;
        private static bool AreNonNegative(List<decimal> array) => array.All(x => x >= 0);
        
        // Вывод списка переменных и итоговой функции
        public void ShowPlan()
        {
            var setOfX = new decimal[Solution.IndexString.Count - 1];
            var size = Math.Min(setOfX.Length, Coeffs.Count);

            foreach (var item in Variables)
                setOfX[item.Value - 1] = Solution.Table[item.Key].Last();

            for (int i = 0; i < size; i++)
                Console.Write($"x{i + 1} = {setOfX[i]:f3}" +
                    (i == size - 1 ? ";" : ", "));

            Console.Write("\nF(X) =");
            for (int i = 0; i < size; i++)
                Console.Write($" {Coeffs[i]} * {setOfX[i]:f3} " +
                    (i == size - 1 ? "" : "+"));
            decimal value = 0;
            for (int i = 0; i < size; i++)
                value += Coeffs[i] * setOfX[i];
            Console.Write($"= {value:f3}");
        }
    }
}
