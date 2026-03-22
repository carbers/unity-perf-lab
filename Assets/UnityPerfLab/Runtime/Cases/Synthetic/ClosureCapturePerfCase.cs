using System;
using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public sealed class ClosureCapturePerfCase : PerfCaseBase
    {
        private readonly bool useCapturedLambda;
        private readonly int workloadSize;
        private readonly PerfCaseDescriptor descriptor;
        private static readonly Func<int, int> NoCaptureDelegate = StaticIncrement;

        public ClosureCapturePerfCase(bool useCapturedLambda, int workloadSize)
        {
            this.useCapturedLambda = useCapturedLambda;
            this.workloadSize = workloadSize;

            descriptor = new PerfCaseDescriptor(
                useCapturedLambda ? "Closure_Capture_" + FormatSizeLabel(workloadSize) : "Closure_NoCapture_" + FormatSizeLabel(workloadSize),
                "Synthetic",
                SyntheticMeasurementDefaults.CreateForClosure(workloadSize),
                new Dictionary<string, string>
                {
                    { "closure", useCapturedLambda ? "capture" : "no-capture" },
                    { "size", workloadSize.ToString() }
                },
                "Capture path allocates a delegate per inner invocation.");
        }

        public override PerfCaseDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public override void Run(int iterations)
        {
            long total = 0;

            if (useCapturedLambda)
            {
                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    for (int i = 0; i < workloadSize; i++)
                    {
                        int captured = i;
                        Func<int> closure = delegate { return captured + 1; };
                        total += closure();
                    }
                }
            }
            else
            {
                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    for (int i = 0; i < workloadSize; i++)
                    {
                        total += NoCaptureDelegate(i);
                    }
                }
            }

            PerfVisibleSink.Write(total);
        }

        private static int StaticIncrement(int value)
        {
            return value + 1;
        }
    }
}
