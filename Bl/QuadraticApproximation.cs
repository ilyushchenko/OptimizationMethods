using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bl
{
    public class QuadraticApproximation : IterationMethod
    {
        private readonly SingleVariableFunctionDelegate _function;

        public QuadraticApproximation(SingleVariableFunctionDelegate function)
        {
            _function = function;
        }

        public (double LeftBound, double RightBound) Calculate(double leftBound, double rightBound, double eps)
        {
            //var x = new List<double>();
            //var f = new List<double>();
            //for (int i = 0; i < x.Count; i++)
            //{
            //    f.Add(_function(x[i]));
            //}
            double x1 = leftBound;

            double x3 = rightBound;

            double x2 = rightBound - (rightBound - leftBound) / 2;
            var iteration = 0;
            double fAVG;

            do
            {
                var f1 = _function(x1);
                var f2 = _function(x2);
                var f3 = _function(x3);
                var a0 = f1;
                var a1 = (f2 - f1) / (x2 - x1);
                var a2 = (1 / (x3 - x2)) * ((f3 - f1) / (x3 - x1) - a1);
                var xNew = ((x1 + x2) / 2) - (a1 / (2 * a2));
                var fNew = _function(xNew);

                var values = new List<(double X, double Y)> { (x1, f1), (x2, f2), (x3, f3), (xNew, fNew) };
                var valuesWithoutMinValue = values.OrderBy(t => t.Y).Skip(1).ToList();
                var xValues = valuesWithoutMinValue.Select(t => t.X).OrderBy(x => x).ToList();

                x1 = xValues[0];
                x2 = xValues[1];
                x3 = xValues[2];

                double xAVG = (x1 + x3) / 2 - a1 / (2 * a2);
                fAVG = _function(xAVG);
                Notify(x1, x3, iteration);
                iteration++;

            } while (Math.Abs((_function(x1) - fAVG) / fAVG) > eps);
            return (x1, x3);
        }

        public struct PointData
        {
            public double _x;
            public double _y;

            public PointData(double x, double y)
            {
                _x = x;
                _y = y;
            }
        }
    }
}
