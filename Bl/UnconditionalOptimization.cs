using System;

namespace Bl
{
    public class IterationInfoEventArgs : EventArgs
    {
        public IterationInfoEventArgs(double leftBound, double rightBound, int iteration)
        {
            LeftBound = leftBound;
            RightBound = rightBound;
            Iteration = iteration;
        }

        public double LeftBound { get; }
        public double RightBound { get; }
        public int Iteration { get; }
    }

    public class UnconditionalOptimization
    {
        public delegate void IterationInfoDelegate(object sender, IterationInfoEventArgs iterationInfoEventArgs);

        private readonly SingleVariableFunctionDelegate _function;

        public UnconditionalOptimization(SingleVariableFunctionDelegate function)
        {
            _function = function;
        }

        public event IterationInfoDelegate OnIteration;

        /// <summary>
        /// Get bound for Maximization
        /// </summary>
        /// <param name="startPoint">X0 - start point of search</param>
        /// <param name="step">Step to find range</param>
        /// <param name="maxIterations">Maximum iteration count</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public (double LeftBound, double RightBound) GetBoundForMinimization(
            double startPoint,
            double step,
            int maxIterations = 1000)
        {
            return GetBoundInternal(startPoint, step, true, maxIterations);
        }

        /// <summary>
        /// Get bound for Minimization
        /// </summary>
        /// <param name="startPoint">X0 - start point of search</param>
        /// <param name="step">Step to find range</param>
        /// <param name="maxIterations">Maximum iteration count</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public (double LeftBound, double RightBound) GetBoundForMaximization(
            double startPoint,
            double step,
            int maxIterations = 1000)
        {
            return GetBoundInternal(startPoint, step, false, maxIterations);
        }

        #region Private helpers

        private (double LeftBound, double RightBound) GetBoundInternal(
            double startPoint,
            double step,
            bool minimization,
            int maxIterations)
        {
            // Get initial range
            var innerPoint = startPoint;
            var leftPoint = startPoint - step;
            var rightPoint = startPoint + step;

            DirectionDelegate getDirection;
            if (minimization)
            {
                // Min function on left side
                getDirection = GetDirectionForMinimization;
            }
            else
            {
                // Max function on right side
                getDirection = GetDirectionForMaximization;
            }

            var direction = getDirection(leftPoint, innerPoint);

            for (int iteration = 1; iteration < maxIterations; iteration++)
            {
                // If function changed direction, then found extremum
                if (direction != getDirection(leftPoint, rightPoint))
                    return (LeftBound: leftPoint, RightBound: rightPoint);

                // Calculating step to shift range bounds
                var h = Math.Pow(2, iteration + 1) * step;

                // Set new bounds, depended of direction
                // WARNING!!! Do not change position of set range
                if (direction == Direction.Right)
                {
                    // Shift to right
                    leftPoint = innerPoint;
                    innerPoint = rightPoint;
                    rightPoint = innerPoint + h;
                }
                else
                {
                    // Shift to left
                    rightPoint = innerPoint;
                    innerPoint = leftPoint;
                    leftPoint = innerPoint - h;
                }

                OnIteration?.Invoke(this, new IterationInfoEventArgs(leftPoint, rightPoint, iteration));
            }

            throw new Exception($"Maximum number of iterations ({maxIterations}) reached");
        }

        #endregion

        private enum Direction
        {
            Left,
            Right
        }

        private delegate Direction DirectionDelegate(double leftBound, double rightBound);

        #region Direction functions

        private Direction GetDirectionForMinimization(double leftBound, double rightBound)
        {
            var leftY = _function(leftBound);
            var rightY = _function(rightBound);
            var direction = leftY < rightY ? Direction.Left : Direction.Right;
            return direction;
        }

        private Direction GetDirectionForMaximization(double leftBound, double rightBound)
        {
            var leftY = _function(leftBound);
            var rightY = _function(rightBound);
            var direction = leftY > rightY ? Direction.Left : Direction.Right;
            return direction;
        }

        #endregion
    }
}