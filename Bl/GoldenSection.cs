using System;

namespace Bl
{
    public class GoldenSection
    {
        private readonly double PHI = (1 + Math.Sqrt(5)) / 2;
        private readonly SingleVariableFunctionDelegate _function;

        public GoldenSection(SingleVariableFunctionDelegate function)
        {
            _function = function;
        }

        public double FindMin(double a, double b, double e)
        {
            double x1, x2;
            while (true)
            {
                x1 = b - (b - a) / PHI;
                x2 = a + (b - a) / PHI;
                if (_function(x1) >= _function(x2))
                    a = x1;
                else
                    b = x2;
                if (Math.Abs(b - a) < e)
                    break;
            }
            return (a + b) / 2;
        }

        public double FindMax(double a, double b, double e)
        {
            double x1, x2;
            while (true)
            {
                x1 = b - (b - a) / PHI;
                x2 = a + (b - a) / PHI;
                if (_function(x1) <= _function(x2))
                    a = x1;
                else
                    b = x2;
                if (Math.Abs(b - a) < e)
                    break;
            }
            return (a + b) / 2;
        }

    }
}
