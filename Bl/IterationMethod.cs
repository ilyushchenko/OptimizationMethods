using System;
using System.Collections.Generic;
using System.Text;

namespace Bl
{
    public delegate void IterationInfoDelegate(object sender, IterationInfoEventArgs iterationInfoEventArgs);

    public abstract class IterationMethod
    {
        public event IterationInfoDelegate OnIteration;
        public int Iterations { get; private set; }

        protected void Notify(double leftBound, double rightBound, int iteration)
        {
            OnIteration?.Invoke(this, new IterationInfoEventArgs(leftBound, rightBound, iteration));
            Iterations++;
        }
    }
}
