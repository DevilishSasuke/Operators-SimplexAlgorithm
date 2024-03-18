
namespace Operators
{
    public class SimplexTable
    {
        public List<List<double>> Table { get; private set; } = new();
        public List<double> IndexString = new();
        private int columnLeading;
        private int stringLeading;
        private int leadingElement;

        public SimplexTable(List<Limitation> limits, List<double> indexString)
        {

            foreach (var limit in limits) 
            {
                var current = new List<double>(limit.Coeffs);
                current.Add(limit.Bound);
                Table.Add(current);
            }

            foreach (var value in indexString)
                IndexString.Add(value * -1);
            while (IndexString.Count < Table[0].Count)
                IndexString.Add(0);
        }

        public int MaxAbsValue()
        {
            int index = 0;

            for (int i = 0; i < IndexString.Count; i++)
                if (Math.Abs(IndexString[i]) > Math.Abs(IndexString[index]))
                    index = i;

            return index;
        }

        public int MinRelation(int columnIndex)
        {
            var min = double.MaxValue;
            int stringIndex = 0;

            for (int i = 0; i < Table.Count; i++)
            {
                var value = Table[i].Last() / Table[i][columnIndex];

                if (value < min)
                {
                    min = value;
                    stringIndex = i;
                }
            }

            return stringIndex;
        }

        public void Recount(int strIndex, int colIndex)
        {
            var leadingElement = Table[strIndex][colIndex];

            for (int i = 0; i < Table.Count; i++)
            {
                List<double> recounted = new();
                if (i == strIndex)
                    foreach (var number in Table[strIndex])
                        recounted.Add(number / leadingElement);
                else
                    recounted = RecountString(i, strIndex, colIndex, leadingElement);

            }
        }

        private List<double> RecountString(int index, int str, int col, double leadingElement)
        {
            var result = new List<double>();
            for (int i = 0; i < Table[index].Count; i++)
            {
                var newValue = Table[index][i] -
                    (Table[str][i] *
                    Table[index][col]) /
                    leadingElement;
                result.Add(newValue);
            }

            return result;
        }
    }
}
