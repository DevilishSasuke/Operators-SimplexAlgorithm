using System;

namespace Operators
{
    class Program
    {
        public static void Main()
        {
            List<double> x = new(){ 3, 7, 3, 5, 7, 8 };
            List<List<double>> limits = new()
            {
                new(){ 1, 3, 0, 3, 1, 4 },
                new(){ 2, 5, 5, -2, 2, 5 },
                new(){ 6, 0, 5, 0, 1, 5 },
            };
            double[] bounds = { 6, 7, 4 , 5};

            var optimanPlan = new OptimalPlan(x.ToList());
            for (int i = 0; i < limits.Count; i++)
                optimanPlan.AddLimitation(limits[i], bounds[i]);

            optimanPlan.ReduceToCanonical();
            optimalPlan.FirstReferencePlan();

        }

    }
}