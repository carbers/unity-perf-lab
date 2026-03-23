using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public sealed class DictionaryLookupPerfCase : PerfCaseBase
    {
        private readonly int workloadSize;
        private readonly PerfCaseDescriptor descriptor;
        private Dictionary<int, int> values;

        public DictionaryLookupPerfCase(int workloadSize)
        {
            this.workloadSize = workloadSize;

            descriptor = new PerfCaseDescriptor(
                "Dictionary_Int_Lookup_" + FormatSizeLabel(workloadSize),
                "Collections",
                SyntheticMeasurementDefaults.CreateForTraversal(workloadSize),
                new Dictionary<string, string>
                {
                    { "collection", "Dictionary<int,int>" },
                    { "loop", "lookup" },
                    { "size", workloadSize.ToString() }
                });
        }

        public override PerfCaseDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public override void GlobalSetup()
        {
            values = new Dictionary<int, int>(workloadSize);
            for (int i = 0; i < workloadSize; i++)
            {
                values.Add(i, i);
            }
        }

        public override void Run(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < workloadSize; i++)
                {
                    total += values[i];
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
