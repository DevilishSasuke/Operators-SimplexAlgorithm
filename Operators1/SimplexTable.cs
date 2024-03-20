
namespace Operators
{
    public class SimplexTable
    {
        private int leadingColumn;
        private int leadingRow;
        private decimal leadingElement;
        public List<List<decimal>> Table { get; private set; } = new();
        public List<decimal> IndexString { get => Table.Last();
            set => Table[Table.Count - 1] = value; }

        public SimplexTable(List<Limitation> limits, List<decimal> indexString, bool isEquality)
        {
            if (isEquality)
                EqualityConstructor(limits, indexString);
            else
                InequalityConstructor(limits, indexString);
        }

        public void EqualityConstructor(List<Limitation> limits, List<decimal> indexString)
        {
            foreach (var limit in limits)
            {
                var current = new List<decimal>(limit.Coeffs);
                current.Add(limit.Bound);
                Table.Add(current);
            }

            ToIdentityMatrix();
            ObjectiveFunc(indexString);
        }

        public void InequalityConstructor(List<Limitation> limits, List<decimal> indexString)
        {
            foreach (var limit in limits)
            {
                var current = new List<decimal>(limit.Coeffs);
                current.Add(limit.Bound);
                Table.Add(current);
            }

            Table.Add(new());
            foreach (var value in indexString)
                IndexString.Add(value * -1);
            while (IndexString.Count < Table[0].Count)
                IndexString.Add(0);
        }

        // Максимальное по модулю значение
        public int MaxAbsValue()
        {
            int index = 0;

            for (int i = 0; i < IndexString.Count - 1; i++)
                if (Math.Abs(IndexString[i]) > Math.Abs(IndexString[index]))
                    index = i;

            return index;
        }

        // Индекс минимального отношения
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

        // Пересчёт таблицы
        public void Recount(int rowIndex, int colIndex)
        {
            leadingRow = rowIndex;
            leadingColumn = colIndex;
            leadingElement = Table[rowIndex][colIndex];
            List<List<decimal>> newValues = new();

            for (int i = 0; i < Table.Count; i++)
            {
                List<decimal> recounted = new();
                // Для всех строк кроме решающей
                if (i == rowIndex)
                    foreach (var number in Table[rowIndex])
                        recounted.Add(number / leadingElement);
                else
                    recounted = RecountString(Table[i]);

                newValues.Add(recounted);
            }

            //IndexString = RecountString(IndexString);
            Table = newValues;
        }

        // Пересчёт строки правилом прямоугольника
        private List<decimal> RecountString(List<decimal> currentRow)
        {
            var result = new List<decimal>();
            for (int i = 0; i < currentRow.Count; i++)
            {
                var newValue = currentRow[i] - // Старое значение
                    Table[leadingRow][i] * //A
                    currentRow[leadingColumn] / // B
                    leadingElement; // Решающий элемент
                result.Add(newValue);
            }

            return result;
        }

        private void ToIdentityMatrix()
        {
            for (int row = 0; row < Table.Count; row++)
            {
                var column = row + (Table[0].Count - (Table.Count + 1));
                Recount(row, column);
            }
        }

        private List<decimal> ObjectiveFunc(List<decimal> coeffs)
        {
            var size = Table[0].Count - (Table.Count + 1);
            var func = new decimal[size].ToList();

            for (int i = 0; i < size; i++)
            {
                func[i] += coeffs[i];
                for (int j = 0; j < Table.Count; j++)
                    func[i] -= Table[j][i] * coeffs[size + j];
            }

            return func;
        }
    }
}
