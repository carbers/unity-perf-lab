using System;
using System.Collections.Generic;

namespace UnityPerfLab.Runtime.Core
{
    public sealed class PerfSummaryStats
    {
        public static readonly PerfSummaryStats Empty = new PerfSummaryStats();

        public double MeanNanoseconds { get; private set; }

        public double MedianNanoseconds { get; private set; }

        public double MinNanoseconds { get; private set; }

        public double MaxNanoseconds { get; private set; }

        public double StandardDeviationNanoseconds { get; private set; }

        public double P95Nanoseconds { get; private set; }

        public double OperationsPerSecond { get; private set; }

        public static PerfSummaryStats FromSamples(IList<PerfSample> measuredSamples)
        {
            if (measuredSamples == null || measuredSamples.Count == 0)
            {
                return Empty;
            }

            double[] values = new double[measuredSamples.Count];
            for (int i = 0; i < measuredSamples.Count; i++)
            {
                values[i] = measuredSamples[i].NanosecondsPerOperation;
            }

            Array.Sort(values);

            double sum = 0d;
            double min = values[0];
            double max = values[values.Length - 1];
            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }

            double mean = sum / values.Length;
            double varianceSum = 0d;
            for (int i = 0; i < values.Length; i++)
            {
                double delta = values[i] - mean;
                varianceSum += delta * delta;
            }

            double median;
            int middle = values.Length / 2;
            if ((values.Length & 1) == 0)
            {
                median = (values[middle - 1] + values[middle]) * 0.5d;
            }
            else
            {
                median = values[middle];
            }

            int p95Index = (int)Math.Ceiling(values.Length * 0.95d) - 1;
            p95Index = PerfMath.Clamp(p95Index, 0, values.Length - 1);

            return new PerfSummaryStats
            {
                MeanNanoseconds = mean,
                MedianNanoseconds = median,
                MinNanoseconds = min,
                MaxNanoseconds = max,
                StandardDeviationNanoseconds = Math.Sqrt(varianceSum / values.Length),
                P95Nanoseconds = values[p95Index],
                OperationsPerSecond = mean > 0d ? 1000000000d / mean : 0d
            };
        }
    }
}
