using System.Runtime.CompilerServices;

namespace Operators
{
    public class Limitation
    {
        public List<double> Coeffs { get; private set; }
        public List<double> CoeffsOriginal { get; set; }
        public double Bound { get; private set; }
        public bool IsCanoncal { get; private set; } = false;

        public Limitation(List<double> coeffs, double bound)
        {
            Coeffs = coeffs;
            this.Bound = bound;
        }

        // Добавляет 
        public void Expand(int amount, int posOfVariable)
        {
            CoeffsOriginal = new(Coeffs);
            for (int i = 0; i < amount; ++i)
                Coeffs.Add(i == posOfVariable ? 1 : 0);
            IsCanoncal = true;
        }

        public bool IsCorrect(List<double> values)
        {
            if (values.Count > Coeffs.Count) 
                throw new Exception("Incorrect number of values");

            double result = 0;

            for (int i = 0; i < values.Count; ++i)
                result += Coeffs[i] * values[i];

            return result <= Bound;
        }
    }
}
