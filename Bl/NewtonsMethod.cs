using System;

namespace Bl
{
    public class NewtonsMethod
    {
        private readonly SingleVariableFunctionDelegate dfdx;
        private readonly SingleVariableFunctionDelegate d2fdx2;

        public NewtonsMethod(SingleVariableFunctionDelegate dfdx, SingleVariableFunctionDelegate d2fdx2)
        {
            this.dfdx = dfdx;
            this.d2fdx2 = d2fdx2;
        }

        public double Newton(double start, double eps)
        {
            double x1, dx;
            double x0 = start;
            do
            {
                x1 = x0 - dfdx(x0) / d2fdx2(x0);
                dx = Math.Abs(x1 - x0);
                x0 = x1;
            }
            while (dx > eps);
            return x1;
        }
    }
}
