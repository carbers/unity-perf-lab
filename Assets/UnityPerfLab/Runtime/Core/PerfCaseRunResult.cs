using System.Collections.Generic;

namespace UnityPerfLab.Runtime.Core
{
    public sealed class PerfCaseRunResult
    {
        public PerfCaseRunResult(
            PerfCaseDescriptor descriptor,
            IList<PerfSample> samples,
            PerfSummaryStats summary,
            int totalMeasuredIterations,
            int resolvedIterationsPerSample)
        {
            Descriptor = descriptor;
            Samples = samples;
            Summary = summary;
            TotalMeasuredIterations = totalMeasuredIterations;
            ResolvedIterationsPerSample = resolvedIterationsPerSample;
        }

        public PerfCaseDescriptor Descriptor { get; private set; }

        public IList<PerfSample> Samples { get; private set; }

        public PerfSummaryStats Summary { get; private set; }

        public int TotalMeasuredIterations { get; private set; }

        public int ResolvedIterationsPerSample { get; private set; }
    }
}
