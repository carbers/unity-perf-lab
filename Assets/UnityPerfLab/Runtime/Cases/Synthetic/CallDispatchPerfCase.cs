using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public sealed class CallDispatchPerfCase : PerfCaseBase
    {
        private readonly bool useInterfaceDispatch;
        private readonly int workloadSize;
        private readonly PerfCaseDescriptor descriptor;
        private DirectCounter directCounter;
        private ICounter interfaceCounter;

        private interface ICounter
        {
            int AddOne(int value);
        }

        private sealed class DirectCounter : ICounter
        {
            public int AddOne(int value)
            {
                return value + 1;
            }
        }

        public CallDispatchPerfCase(bool useInterfaceDispatch, int workloadSize)
        {
            this.useInterfaceDispatch = useInterfaceDispatch;
            this.workloadSize = workloadSize;

            descriptor = new PerfCaseDescriptor(
                useInterfaceDispatch ? "InterfaceCall_" + FormatSizeLabel(workloadSize) : "DirectCall_" + FormatSizeLabel(workloadSize),
                "Dispatch",
                SyntheticMeasurementDefaults.CreateForTraversal(workloadSize),
                new Dictionary<string, string>
                {
                    { "dispatch", useInterfaceDispatch ? "interface" : "direct" },
                    { "size", workloadSize.ToString() }
                });
        }

        public override PerfCaseDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public override void GlobalSetup()
        {
            directCounter = new DirectCounter();
            interfaceCounter = directCounter;
        }

        public override void Run(int iterations)
        {
            long total = 0;
            if (useInterfaceDispatch)
            {
                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    for (int i = 0; i < workloadSize; i++)
                    {
                        total += interfaceCounter.AddOne(i);
                    }
                }
            }
            else
            {
                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    for (int i = 0; i < workloadSize; i++)
                    {
                        total += directCounter.AddOne(i);
                    }
                }
            }

            PerfVisibleSink.Write(total);
        }

        public override void GlobalTeardown()
        {
            directCounter = null;
            interfaceCounter = null;
        }
    }
}
