using System;
using Bl;

namespace UI
{
    class Program
    {
        static void Main(string[] args)
        {
            const double eps = 0.001;

            Console.WriteLine("Установление первоначальных границ");
            var boundFoundary = new UnconditionalOptimization(MaximizationFunction);
            boundFoundary.OnIteration += BoundFoundary_OnIteration;
            var (startLeftBound, startRightBound) = boundFoundary.GetBoundForMaximization(5, .5);
            DisplayResult((startLeftBound, startRightBound), boundFoundary.Iterations, eps);

            Console.WriteLine("Уточнение границ методом Золотого сечения");
            var goldenSection = new GoldenSection(MaximizationFunction);
            goldenSection.OnIteration += BoundFoundary_OnIteration;
            var goldenSectionBounds = goldenSection.FindMax(startLeftBound, startRightBound, eps);
            DisplayResult(goldenSectionBounds, goldenSection.Iterations, eps);

            Console.WriteLine("Квадратичная апроксимация");
            var quadraticApproximation = new QuadraticApproximation(MaximizationFunction);
            quadraticApproximation.OnIteration += BoundFoundary_OnIteration;
            var quadApproxBounds = quadraticApproximation.Calculate(startLeftBound, startRightBound, eps);
            DisplayResult(quadApproxBounds, quadraticApproximation.Iterations, eps);

            Console.WriteLine("Метод Ньютона");
            var newtonsMethod = new NewtonsMethod(MaximizationFunctionDerivative1, MaximizationFunctionDerivative2);
            var rootNewtoon = newtonsMethod.Newton(-9, eps);
            Console.WriteLine($"Корень {rootNewtoon}");
            Console.WriteLine();

            Console.ReadLine();
        }

        private static void BoundFoundary_OnIteration(object sender, IterationInfoEventArgs iterationInfoEventArgs)
        {
            Console.WriteLine("Итерация {0}\t[{1:f5};{2:f5}]", iterationInfoEventArgs.Iteration, iterationInfoEventArgs.LeftBound, iterationInfoEventArgs.RightBound);
        }

        private static void DisplayResult((double LeftBound, double RightBound) bounds, int iterationsCount, double eps)
        {
            Console.WriteLine($"Приблизительный максимум находится в пределах [{bounds.LeftBound};{bounds.RightBound}]");
            Console.WriteLine($"Решение получено за {iterationsCount} итераций с точностью {eps}");
            Console.WriteLine();
        }

        private static double MaximizationFunction(double x) => 30 - Math.Pow(x, 2) - 0.04 * Math.Pow(x, 5) - 0.06 * Math.Pow(x, 6);
        private static double MaximizationFunctionDerivative1(double x) => -0.36 * Math.Pow(x, 5) - 0.2 * Math.Pow(x, 4) - 2 * x;
        private static double MaximizationFunctionDerivative2(double x) => -1.8 * Math.Pow(x, 4) - 0.8 * Math.Pow(x, 3) - 2;
    }
}
