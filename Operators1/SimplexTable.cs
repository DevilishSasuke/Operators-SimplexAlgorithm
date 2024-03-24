namespace Operators
{
    public class SimplexTable
    {
        private int leadingColumn { get; set; }                             // Рещающий столбец
        private int leadingRow { get; set; }                                // Решающий ряд
        private decimal leadingElement { get; set; }                        // Решающий элемент
        public List<List<decimal>> Table { get; private set; } = new();     // Таблица значений
        public List<decimal> IndexString { get => Table.Last();             // Индекс строка таблицы
            set => Table[Table.Count - 1] = value; }

        public SimplexTable(List<Limitation> limits, List<decimal> indexString, bool isEquality)
        {
            if (isEquality)
                EqualityConstructor(limits, indexString);
            else
                InequalityConstructor(limits, indexString);
        }

        // Конструктор равенства
        public void EqualityConstructor(List<Limitation> limits, List<decimal> indexString)
        {
            foreach (var limit in limits)
                Table.Add(limit);

            ToIdentityMatrix();
            var objFunc = ObjectiveFunc(indexString).Select(x => -x).ToList();
            while (objFunc.Count < Table.Last().Count) objFunc.Add(0);

            Table.Add(objFunc);
        }

        // Конструктор неравенства
        public void InequalityConstructor(List<Limitation> limits, List<decimal> indexString)
        {
            foreach (var limit in limits)
                Table.Add(limit);

            var newIndexString = indexString.Select(x => -x).ToList();
            while (newIndexString.Count < Table[0].Count)
                newIndexString.Add(0);
            Table.Add(newIndexString);
        }

        // Пересчёт таблицы правилом прямоугольника
        public void Recount(int rowIndex, int columnIndex)
        {
            leadingRow = rowIndex;
            leadingColumn = columnIndex;
            leadingElement = Table[rowIndex][columnIndex];
            List<List<decimal>> newTable = new();

            for (int i = 0; i < Table.Count; i++)
            {
                List<decimal> recounted = new();
                // Для всех строк кроме решающей
                if (i == rowIndex)
                    foreach (var number in Table[rowIndex])
                        recounted.Add(number / leadingElement);
                else
                    recounted = RecountString(Table[i]);

                newTable.Add(recounted);
            }

            Table = newTable;
        }

        // Пересчёт строки правилом прямоугольника
        private List<decimal> RecountString(List<decimal> currentRow)
        {
            var result = new List<decimal>();
            for (int i = 0; i < currentRow.Count; i++)
            {
                var newValue = currentRow[i] -  // Старое значение
                    Table[leadingRow][i] *      // A
                    currentRow[leadingColumn] / // B
                    leadingElement;             // Решающий элемент
                result.Add(newValue);
            }

            return result;
        }

        // Получение первого опорного плана
        public List<(int, int)> GetReferencePlan()
        {
            int rowIndex = 0;
            List<(int, int)> variables = new();

            // Пока есть отрицательные свободные члены
            while ((rowIndex = ConstTermNegativeIndex()) >= 0)
            {
                int columnIndex = FindNegative(Table[rowIndex]);    // Находим отрицательное в строке
                Recount(rowIndex, columnIndex);                     // Пересчитываем таблицу
                variables.Add((rowIndex, columnIndex));
            }

            return variables;
        }

        // Пересчёт целевой функции
        public List<decimal> ObjectiveFunc(List<decimal> coeffs)
        {
            var size = Table[0].Count - (Table.Count + 1);
            var func = new decimal[size];

            for (int i = 0; i < size; i++)
            {
                func[i] += coeffs[i];
                for (int j = 0; j < Table.Count; j++)
                    func[i] -= Table[j][i] * coeffs[size + j];
            }

            return func.ToList();
        }

        // Индекс отрицательного свободного члена
        private int ConstTermNegativeIndex()
        {
            int size = Table.Count - 1;

            for (int i = 0; i < size; i++)
                if (Table[i].Last() < 0) 
                    return i;

            return -1;
        }

        // Выделение базиса в матрице
        private void ToIdentityMatrix()
        {
            int offset = Table[0].Count - (Table.Count + 1);
            for (int row = 0; row < Table.Count; row++)
                Recount(row, row + offset);
        }

        // Максимальное по модулю значение в ряду
        public int MaxAbsValue()
        {
            int index = 0;

            for (int i = 0; i < IndexString.Count - 1; i++)
                if (Math.Abs(IndexString[i]) > Math.Abs(IndexString[index]))
                    index = i;

            return index;
        }

        // Индекс минимального отношения в столбце
        public int MinRelation(int columnIndex)
        {
            var min = decimal.MaxValue;
            int rowIndex = 0;

            for (int i = 0; i < Table.Count - 1; i++)
            {
                if (Table[i][columnIndex] <= 0) continue;
                var value = Table[i].Last() / Table[i][columnIndex];
                if (value < 0) continue;

                if (value < min)
                {
                    min = value;
                    rowIndex = i;
                }
            }

            if (min == decimal.MaxValue) throw new Exception("No minimal relation");
            return rowIndex;
        }

        private int FindNegative(List<decimal> row)
        {
            for (int i = 0; i < row.Count; i++)
                if (row[i] < 0) return i;

            throw new Exception("No negative");
        }
    }
}
