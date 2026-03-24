using Perf.Util;
using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public sealed class SimpleListTraversalPerfCase : PerfCaseBase
    {
        private readonly CollectionPayloadKind payloadKind;
        private readonly int workloadSize;
        private readonly PerfCaseDescriptor descriptor;
        private SimpleList<int> intValues;
        private SimpleList<CollectionPayloadClass> classValues;
        private SimpleList<CollectionPayloadStruct> structValues;

        public SimpleListTraversalPerfCase(CollectionPayloadKind payloadKind, int workloadSize)
        {
            this.payloadKind = payloadKind;
            this.workloadSize = workloadSize;

            descriptor = new PerfCaseDescriptor(
                "SimpleList_" + CollectionPayloadFactory.GetPayloadLabel(payloadKind) + "_Traverse_" + FormatSizeLabel(workloadSize),
                "Collections",
                SyntheticMeasurementDefaults.CreateForTraversal(workloadSize),
                new Dictionary<string, string>
                {
                    { "collection", "SimpleList<" + CollectionPayloadFactory.GetCollectionLabel(payloadKind) + ">" },
                    { "loop", "buffer-traverse" },
                    { "size", workloadSize.ToString() }
                },
                "Traversal uses SimpleList.buffer plus SimpleList.size, which is the intended fast path.");
        }

        public override PerfCaseDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public override void GlobalSetup()
        {
            switch (payloadKind)
            {
                case CollectionPayloadKind.Int:
                    intValues = new SimpleList<int>(workloadSize);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        intValues.Add(i);
                    }

                    break;

                case CollectionPayloadKind.Class:
                    classValues = new SimpleList<CollectionPayloadClass>(workloadSize);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        classValues.Add(CollectionPayloadFactory.CreateClassPayload(i));
                    }

                    break;

                default:
                    structValues = new SimpleList<CollectionPayloadStruct>(workloadSize);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        structValues.Add(CollectionPayloadFactory.CreateStructPayload(i));
                    }

                    break;
            }
        }

        public override void Run(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                switch (payloadKind)
                {
                    case CollectionPayloadKind.Int:
                        for (int i = 0; i < intValues.size; i++)
                        {
                            total += intValues.buffer[i];
                        }

                        break;

                    case CollectionPayloadKind.Class:
                        for (int i = 0; i < classValues.size; i++)
                        {
                            CollectionPayloadClass payload = classValues.buffer[i];
                            total += payload.a + payload.b + payload.c;
                        }

                        break;

                    default:
                        for (int i = 0; i < structValues.size; i++)
                        {
                            CollectionPayloadStruct payload = structValues.buffer[i];
                            total += payload.a + payload.b + payload.c;
                        }

                        break;
                }
            }

            PerfVisibleSink.Write(total);
        }

        public override void GlobalTeardown()
        {
            intValues = null;
            classValues = null;
            structValues = null;
        }
    }
}
