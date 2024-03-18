using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operators 
{
    public class OptimalPlan
    {
        public List<double> Coeffs { get; private set; } = new();
        public readonly bool Maximize;
        public List<Limitation> Limits { get; private set; } = new();

        public OptimalPlan(List<double> coeffs, bool maximize = true) 
        {
            this.Coeffs = coeffs;
            this.Maximize = maximize;
        }

        // Добавление ограниченйи
        public void AddLimitation(Limitation lim) => Limits.Add(lim);
        public void AddLimitation(List<double> coeffs, double bound)
        {
            var lim = new Limitation(coeffs, bound);
            Limits.Add(lim);
        }

        public void ReduceToCanonical()
        {
            int size = Limits.Count();
            for (int i = 0; i < size; ++i)
                Limits[i].Expand(size, i);
        }

        public void FirstReferencePlan()
        {
            SimplexTable table = new SimplexTable(Limits, Coeffs);

            while (!AreNonNegative(table.IndexString))
            {
                var columnIndex = table.MaxAbsValue();
                var stringIndex = table.MinRelation(columnIndex);
                table.Recount(stringIndex, columnIndex);
            }
        }

        private static bool AreNonNegative(double[] array) => array.All(x => x >= 0);
        private static bool AreNonNegative(List<double> array) => array.All(x => x >= 0);

    }
}
