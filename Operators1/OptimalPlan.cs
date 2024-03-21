using System;

namespace Operators 
{
    public class OptimalPlan
    {
        public List<decimal> Coeffs { get; private set; } = new(); // Коэффициенты
        public List<Limitation> Limits { get; private set; } = new(); // Ограничения
        private Dictionary<int, int> Variables = new(); // Словарь смены переменных
        private bool IsEquality { get; set; }

        public OptimalPlan(List<decimal> coeffs, bool isEquality = false)
        {
            Coeffs = coeffs;
            IsEquality = isEquality;
        }

        // Приведение к канонической форме
        public void ReduceToCanonical()
        {
            int size = Limits.Count;
            for (int i = 0; i < size; ++i)
                Limits[i].Expand(size, i);
        }

        // Получуение оптимального плана в симплексной таблице - lab.1
        public SimplexTable GetOptimalPlan()
        {
            SimplexTable table = new(Limits, Coeffs, IsEquality);
            InitVars(Limits.Count, Coeffs.Count + Limits.Count);

            // Пока находятся отрицательные элементы в индекс строке
            while (!AreNonNegative(table.IndexString))
            {
                var columnIndex = table.MaxAbsValue(); // Находим рещающую строку
                var rowIndex = table.MinRelation(columnIndex); // Находим рещающий элемент
                table.Recount(rowIndex, columnIndex); // Пересчитываем симплекс таблицу правилом прямоугольника
                RememberVariables(rowIndex, columnIndex); // Запоминаем номер новой переменной
            }

            return table;
        }

        // Распределение ресурсов
        public SimplexTable ManageRecources()
        {
            SimplexTable table = new(Limits, Coeffs, IsEquality);
            InitVars(Limits.Count, Coeffs.Count);

            var changes = table.GetReferencePlan();
            foreach (var change in changes)
                RememberVariables(change.Item1, change.Item2);
            var objFunc = table.ObjectiveFunc(Coeffs, Variables);

            while (!AreNonNegative(table.IndexString))
            {
                var columnIndex = table.MaxAbsValue(); // Находим рещающую строку
                var rowIndex = table.MinRelation(columnIndex); // Находим рещающий элемент
                table.Recount(rowIndex, columnIndex); // Пересчитываем симплекс таблицу правилом прямоугольника
                RememberVariables(rowIndex, columnIndex); // Запоминаем номер новой переменной
            }

            return table;
        }

        // Вывод списка переменных и итоговой функции
        public void ShowPlan(SimplexTable table)
        {
            var setOfX = new decimal[table.IndexString.Count - 1];

            foreach(var item in Variables)
                setOfX[item.Value - 1] = table.Table[item.Key].Last();

            for (int i = 0; i < Coeffs.Count; i++)
                Console.Write(i == Coeffs.Count - 1 ?
                    $"x{i + 1} = {setOfX[i]:f3};" :
                    $"x{i + 1} = {setOfX[i]:f3}, ");

            Console.Write("\nF(X) =");
            for (int i = 0; i < Coeffs.Count; i++)
                Console.Write(i == Coeffs.Count - 1 ?
                    $" {Coeffs[i]} * {setOfX[i]:f3} " :
                    $" {Coeffs[i]} * {setOfX[i]:f3} +");
            Console.Write($"= {table.IndexString.Last():f3}");
        }

        // Добавление ограничений
        public void AddLimitation(Limitation lim) => Limits.Add(lim);
        public void AddLimitation(List<decimal> coeffs, decimal bound) => Limits.Add(new Limitation(coeffs, bound));
        private static bool AreNonNegative(List<decimal> array) => array.All(x => x >= 0);
        private void RememberVariables(int rowIndex, int columnIndex) => Variables[rowIndex] = columnIndex + 1;
        private void InitVars(int rows, int columns)
        {
            int offset = columns - rows;
            for (int i = 0; i < rows; i++)
                RememberVariables(i, offset + i);
        }
    }
}
