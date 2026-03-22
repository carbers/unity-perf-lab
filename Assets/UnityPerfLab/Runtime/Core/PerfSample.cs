namespace UnityPerfLab.Runtime.Core
{
    public sealed class PerfSample
    {
        public PerfSample(int sampleIndex, bool isWarmup, int iterations, long elapsedNanoseconds)
        {
            SampleIndex = sampleIndex;
            IsWarmup = isWarmup;
            Iterations = iterations;
            ElapsedNanoseconds = elapsedNanoseconds;
            NanosecondsPerOperation = iterations > 0
                ? (double)elapsedNanoseconds / iterations
                : 0d;
        }

        public int SampleIndex { get; private set; }

        public bool IsWarmup { get; private set; }

        public int Iterations { get; private set; }

        public long ElapsedNanoseconds { get; private set; }

        public double NanosecondsPerOperation { get; private set; }
    }
}
