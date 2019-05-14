using System;
using Xunit;

namespace Bl.Tests
{
    public class UnconditionalOptimizationTests
    {
        [Theory]
        [InlineData(30, 5)]
        [InlineData(150, 5)]
        public void Minimum_In_100(double startPoint, double step)
        {
            var optimization = new UnconditionalOptimization(MinimizationFunction);
            var (leftBound, rightBound) = optimization.GetBoundForMinimization(startPoint, step);
            Assert.True(leftBound <= 100);
            Assert.True(rightBound >= 100);
        }

        [Theory]
        [InlineData(-5, .5)]
        [InlineData(5, .5)]
        public void Maximum_In_0(double startPoint, double step)
        {
            var optimization = new UnconditionalOptimization(MaximizationFunction);
            var (leftBound, rightBound) = optimization.GetBoundForMaximization(startPoint, step);
            Assert.True(leftBound <= 0);
            Assert.True(rightBound >= 0);
        }

        [Theory]
        [InlineData(-5, 5, 0.001)]
        [InlineData(-1, 1, 0.00001)]
        public void GoldenSection_Maximum_In_0(double a, double b, double eps)
        {
            const double expectedValue = 0;
            var goldenSection = new GoldenSection(MaximizationFunction);
            var max = goldenSection.FindMax(a, b, eps);
            Assert.True(Math.Abs(expectedValue - (max.LeftBound - max.RightBound) / 2) <= eps);
        }

        //[Theory]
        //[InlineData(30, 130, 0.001)]
        //[InlineData(90, 150, 0.001)]
        //public void GoldenSection_Minimum_In_100(double a, double b, double eps)
        //{
        //    const double expectedValue = 100;
        //    var goldenSection = new GoldenSection(MinimizationFunction);
        //    var min = goldenSection.FindMin(a, b, eps);
        //    Assert.True(Math.Abs(expectedValue - (min.LeftBound - min.RightBound) / 2) <= eps);
        //}

        [Theory]
        [InlineData(-9, 0.00001)]
        [InlineData(3, 0.00001)]
        public void Mewtoon_Maximum_In_0(double startPoint, double eps)
        {
            const double expectedValue = 0;
            var newtonsMethod = new NewtonsMethod(MaximizationFunctionDerivative1, MaximizationFunctionDerivative2);
            var root = newtonsMethod.Newton(startPoint, eps);
            Assert.True(Math.Abs(expectedValue - root) < eps);
        }

        private static double MaximizationFunction(double x) => 30 - Math.Pow(x, 2) - 0.04 * Math.Pow(x, 5) - 0.06 * Math.Pow(x, 6);
        private static double MaximizationFunctionDerivative1(double x) => -0.36 * Math.Pow(x, 5) - 0.2 * Math.Pow(x, 4) - 2 * x;
        private static double MaximizationFunctionDerivative2(double x) => -1.8 * Math.Pow(x, 4) - 0.8 * Math.Pow(x, 3) - 2;

        private static double MinimizationFunction(double x) => Math.Pow(100 - x, 2);
    }
}