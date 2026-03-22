namespace UnityPerfLab.Runtime.Core
{
    public sealed class PerfMeasurementConfig
    {
        private PerfMeasurementConfig()
        {
        }

        public PerfMeasurementMode Mode { get; private set; }

        public int WarmupSamples { get; private set; }

        public int MeasureSamples { get; private set; }

        public int IterationsPerSample { get; private set; }

        public int ProbeIterations { get; private set; }

        public int TargetSampleDurationMs { get; private set; }

        public int MinIterations { get; private set; }

        public int MaxIterations { get; private set; }

        public static PerfMeasurementConfig CreateFixedIterations(int iterationsPerSample, int warmupSamples, int measureSamples)
        {
            return new PerfMeasurementConfig
            {
                Mode = PerfMeasurementMode.FixedIterations,
                IterationsPerSample = PerfMath.Max(1, iterationsPerSample),
                WarmupSamples = PerfMath.Max(0, warmupSamples),
                MeasureSamples = PerfMath.Max(1, measureSamples),
                ProbeIterations = 1,
                TargetSampleDurationMs = 0,
                MinIterations = PerfMath.Max(1, iterationsPerSample),
                MaxIterations = PerfMath.Max(1, iterationsPerSample)
            };
        }

        public static PerfMeasurementConfig CreateTargetDuration(
            int targetSampleDurationMs,
            int minIterations,
            int maxIterations,
            int warmupSamples,
            int measureSamples,
            int probeIterations)
        {
            return new PerfMeasurementConfig
            {
                Mode = PerfMeasurementMode.TargetDuration,
                WarmupSamples = PerfMath.Max(0, warmupSamples),
                MeasureSamples = PerfMath.Max(1, measureSamples),
                IterationsPerSample = 0,
                ProbeIterations = PerfMath.Max(1, probeIterations),
                TargetSampleDurationMs = PerfMath.Max(1, targetSampleDurationMs),
                MinIterations = PerfMath.Max(1, minIterations),
                MaxIterations = PerfMath.Max(PerfMath.Max(1, minIterations), maxIterations)
            };
        }
    }
}
