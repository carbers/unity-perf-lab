using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public sealed class ListIntIterationPerfCase : PerfCaseBase
    {
        private readonly bool useForeach;
        private readonly int workloadSize;
        private readonly PerfCaseDescriptor descriptor;
        private List<int> values;

        public ListIntIterationPerfCase(bool useForeach, int workloadSize)
        {
            this.useForeach = useForeach;
            this.workloadSize = workloadSize;

            descriptor = new PerfCaseDescriptor(
                useForeach ? "List_Int_ForEach_" + FormatSizeLabel(workloadSize) : "List_Int_For_" + FormatSizeLabel(workloadSize),
                "Looping",
                SyntheticMeasurementDefaults.CreateForTraversal(workloadSize),
                new Dictionary<string, string>
                {
                    { "collection", "List<int>" },
                    { "loop", useForeach ? "foreach" : "for" },
                    { "size", workloadSize.ToString() }
                });
        }

        public override PerfCaseDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public override void GlobalSetup()
        {
            values = new List<int>(workloadSize);
            for (int i = 0; i < workloadSize; i++)
            {
                values.Add((i % 113) + 1);
            }
        }

        public override void Run(int iterations)
        {
            long total = 0;
            if (useForeach)
            {
                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    foreach (int value in values)
                    {
                        total += value;
                    }
                }
            }
            else
            {
                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    for (int i = 0; i < values.Count; i++)
                    {
                        total += values[i];
                    }
                }
            }

            PerfVisibleSink.Write(total);
        }

        public override void GlobalTeardown()
        {
            values = null;
        }
    }
}
