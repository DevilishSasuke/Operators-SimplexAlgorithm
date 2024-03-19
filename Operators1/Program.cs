using System;

namespace Operators
{
    class Program
    {
        public static void Main()
        {
            List<decimal> x = new(){ 3, 7, 3, 5, 7, 8 };
            List<List<decimal>> limits = new()
            {
                new(){ 1, 3, 0, 3, 1, 4 },
                new(){ 2, 5, 5, -2, 2, 5 },
                new(){ 6, 0, 5, 0, 1, 5 },
            };
            decimal[] bounds = { 6, 7, 4};

            var optimalPlan = new OptimalPlan(x.ToList());
            for (int i = 0; i < limits.Count; i++)
                optimalPlan.AddLimitation(limits[i], bounds[i]);

            optimalPlan.ReduceToCanonical();
            var table = optimalPlan.GetOptimailPlan();
            optimalPlan.ShowPlan(table);
        }

    }
}