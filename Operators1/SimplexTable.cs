
namespace Operators
{
    public class SimplexTable
    {
        public List<List<decimal>> Table { get; private set; } = new();
        public List<decimal> IndexRow = new();
        private int leadingColumn;
        private int leadingRow;
        private decimal leadingElement;

        public SimplexTable(List<Limitation> limits, List<decimal> indexString)
        {
            foreach (var limit in limits) 
            {
                var current = new List<decimal>(limit.Coeffs);
                current.Add(limit.Bound);
                Table.Add(current);
            }

            foreach (var value in indexString)
                IndexRow.Add(value * -1);
            while (IndexRow.Count < Table[0].Count)
                IndexRow.Add(0);
        }

        // Максимальное по модулю значение
        public int MaxAbsValue()
        {
            int index = 0;

            for (int i = 0; i < IndexRow.Count - 1; i++)
                if (Math.Abs(IndexRow[i]) > Math.Abs(IndexRow[index]))
                    index = i;

            return index;
        }

        // Индекс минимального отношения
        public int MinRelation(int columnIndex)
        {
            var min = decimal.MaxValue;
            int rowIndex = 0;

            for (int i = 0; i < Table.Count; i++)
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

            IndexRow = RecountString(IndexRow);
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
    }
}
