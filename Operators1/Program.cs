namespace Operators
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("Lab 1:\n"); Lab1();
            Console.WriteLine("\n\nLab 2:\n"); Lab2();
            Console.WriteLine("");
        }

        private static void Lab1()
        {
            List<decimal> x = new() { 3, 7, 3, 5, 7, 8 };
            List<List<decimal>> limits = new()
            {
                new(){ 1, 3, 0, 3, 1, 4 },
                new(){ 2, 5, 5, -2, 2, 5 },
                new(){ 6, 0, 5, 0, 1, 5 },
            };
            decimal[] bounds = { 6, 7, 4 };

            var optimalPlan = new OptimalPlan(x);
            for (int i = 0; i < limits.Count; i++)
                optimalPlan.AddLimitation(limits[i], bounds[i]);

            optimalPlan.ReduceToCanonical();
            optimalPlan.GetOptimalPlan();
            optimalPlan.ShowPlan();
        }

        private static void Lab2()
        {
            List<decimal> x = new() { 8, 6, 5, 3, 7, 8, 4, 5 };
            List<List<decimal>> limits = new()
            {
                new(){ 3, -1, 2, 6, 5, 5, 5, 3 },
                new(){ 1, 3, 0, 1, 4, 6, 1, 3 },
                new(){ 1, 5, -1, 2, 3, 4, 3, 4 },
            };
            decimal[] bounds = { 9, 6, 12 };

            var optimalPlan = new OptimalPlan(x, true);
            for (int i = 0; i < limits.Count; i++)
                optimalPlan.AddLimitation(limits[i], bounds[i]);

            optimalPlan.ManageRecources();
            optimalPlan.ShowPlan();
        }
    }
}