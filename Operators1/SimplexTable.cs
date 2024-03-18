
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
            List<List<double>> newValues = new();

            for (int i = 0; i < Table.Count; i++)
            {
                List<double> recounted = new();
                if (i == strIndex)
                    foreach (var number in Table[strIndex])
                        recounted.Add(number / leadingElement);
                else
                    recounted = RecountString(Table[i], strIndex, colIndex, leadingElement);

                newValues.Add(recounted);
            }


            IndexString = RecountString(IndexString, strIndex, colIndex, leadingElement);
            for (int i = 0; i < Table.Count; i++)
                Table[i] = newValues[i];

        }

        private List<double> RecountString(List<double> curStr, int str, int col, double leadingElement)
        {
            var result = new List<double>();
            for (int i = 0; i < curStr.Count; i++)
            {
                var newValue = curStr[i] -
                    Table[str][i] *
                    curStr[col] /
                    leadingElement;
                result.Add(newValue);
            }

            return result;
        }
    }
}
