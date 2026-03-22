using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public sealed class ListStructIterationPerfCase : PerfCaseBase
    {
        private readonly bool useForeach;
        private readonly int workloadSize;
        private readonly PerfCaseDescriptor descriptor;
        private List<Payload> values;

        private struct Payload
        {
            public int Value;
        }

        public ListStructIterationPerfCase(bool useForeach, int workloadSize)
        {
            this.useForeach = useForeach;
            this.workloadSize = workloadSize;

            descriptor = new PerfCaseDescriptor(
                useForeach ? "List_Struct_ForEach_" + FormatSizeLabel(workloadSize) : "List_Struct_For_" + FormatSizeLabel(workloadSize),
                "Synthetic",
                SyntheticMeasurementDefaults.CreateForTraversal(workloadSize),
                new Dictionary<string, string>
                {
                    { "collection", "List<struct>" },
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
            values = new List<Payload>(workloadSize);
            for (int i = 0; i < workloadSize; i++)
            {
                Payload payload = new Payload();
                payload.Value = (i % 67) + 1;
                values.Add(payload);
            }
        }

        public override void Run(int iterations)
        {
            long total = 0;
            if (useForeach)
            {
                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    foreach (Payload payload in values)
                    {
                        total += payload.Value;
                    }
                }
            }
            else
            {
                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    for (int i = 0; i < values.Count; i++)
                    {
                        total += values[i].Value;
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
