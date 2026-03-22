using System;

namespace UnityPerfLab.Runtime.Core
{
    public static class PerfMath
    {
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        public static int Max(int left, int right)
        {
            return left > right ? left : right;
        }

        public static long StopwatchTicksToNanoseconds(long ticks)
        {
            return (long)Math.Round(ticks * (1000000000d / System.Diagnostics.Stopwatch.Frequency));
        }
    }
}
