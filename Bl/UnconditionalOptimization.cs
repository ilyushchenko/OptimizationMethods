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
            Direction direction;

            var leftY = _function(leftPoint);
            var centerY = _function(innerPoint);

            if (minimization)
            {
                getDirection = GetDirectionForMinimization;
                // Min function on left side
                direction = leftY < centerY ? Direction.Left : Direction.Right;
            }
            else
            {
                getDirection = GetDirectionForMaximization;
                // Max function on right side
                direction = leftY < centerY ? Direction.Right : Direction.Left;
            }

            var iteration = 0;
            do
            {
                if (iteration > maxIterations)
                    throw new Exception($"Maximum number of iterations ({maxIterations}) reached");

                iteration++;
                var h = Math.Pow(2, iteration) * step;

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

                // If function changed direction, then found extremum
            } while (direction == getDirection(leftPoint, rightPoint));

            return (LeftBound: leftPoint, RightBound: rightPoint);
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