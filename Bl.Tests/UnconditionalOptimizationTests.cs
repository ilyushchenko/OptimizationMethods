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

        private static double MaximizationFunction(double x) => 30 - Math.Pow(x, 2) - 0.04 * Math.Pow(x, 5) - 0.06 * Math.Pow(x, 6);

        private static double MinimizationFunction(double x) => Math.Pow(100 - x, 2);
    }
}