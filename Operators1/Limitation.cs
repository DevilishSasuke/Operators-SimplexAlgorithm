using System.Runtime.CompilerServices;

namespace Operators
{
    public class Limitation
    {
        public List<decimal> Coeffs { get; private set; }
        public decimal Bound { get; private set; }

        public Limitation(List<decimal> coeffs, decimal bound)
        {
            Coeffs = coeffs;
            this.Bound = bound;
        }

        // Расширение ряда до канонической формы
        public void Expand(int amount, int posOfVariable)
        {
            for (int i = 0; i < amount; ++i)
                Coeffs.Add(i == posOfVariable ? 1 : 0);
        }
    }
}
