using System;
using Xunit;

namespace Bl.Tests
{
    public class UnconditionalOptimizationTests
    {
        [Theory]
        [InlineData(30, 5)]
        [InlineData(150, 5)]
        public void Maximum_Between_100(double startPoint, double step)
        {
            var optimization = new UnconditionalOptimization(TestFunction);
            var (leftBound, rightBound) = optimization.GetBoundForMinimization(startPoint, step);
            Assert.True(leftBound < 100);
            Assert.True(rightBound > 100);
        }

        private static double TestFunction(double x)
        {
            return Math.Pow(100 - x, 2);
        }
    }
}