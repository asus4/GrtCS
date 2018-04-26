using System.Runtime.CompilerServices;

namespace GRT
{
    public static class GRT
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sqr(double x) => x * x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sqrt(double x) => System.Math.Sqrt(x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Antilog(double x) => System.Math.Exp(x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Exp(double x) => System.Math.Exp(x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log(double x) => System.Math.Log(x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sigmoid(double x) => 1.0 / (1.0 + System.Math.Exp(x));


        /// <summary>
        /// Scales the input value x (which should be in the range [minSource maxSource]) to a value in the new target range of [minTarget maxTarget].
        /// </summary>
        /// <param name="x">the value that should be scaled</param>
        /// <param name="minSource">the minimum range that x originates from</param>
        /// <param name="maxSource">the maximum range that x originates from</param>
        /// <param name="minTarget">the minimum range that x should be scaled to</param>
        /// <param name="maxTarget">the maximum range that x should be scaled to</param>
        /// <param name="constrain">sets if the scaled value should be constrained to the target range</param>
        /// <returns>returns a new value that has been scaled based on the input parameters</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Scale(double x, double minSource, double maxSource, double minTarget, double maxTarget, bool constrain = false)
        {
            if (constrain)
            {
                if (x <= minSource) return minTarget;
                if (x >= maxSource) return maxTarget;
            }
            if (minSource == maxSource) return minTarget;
            return (((x - minSource) * (maxTarget - minTarget)) / (maxSource - minSource)) + minTarget;
        }
    }

}
