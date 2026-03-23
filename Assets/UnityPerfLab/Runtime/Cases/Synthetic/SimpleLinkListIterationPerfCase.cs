using Perf.Util;
using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public sealed class SimpleLinkListIterationPerfCase : PerfCaseBase
    {
        private readonly CollectionPayloadKind payloadKind;
        private readonly int workloadSize;
        private readonly PerfCaseDescriptor descriptor;
        private SimpleLinkList<int> intValues;
        private SimpleLinkList<CollectionPayloadClass> classValues;
        private SimpleLinkList<CollectionPayloadStruct> structValues;

        public SimpleLinkListIterationPerfCase(CollectionPayloadKind payloadKind, int workloadSize)
        {
            this.payloadKind = payloadKind;
            this.workloadSize = workloadSize;

            descriptor = new PerfCaseDescriptor(
                "SimpleLinkList_" + CollectionPayloadFactory.GetPayloadLabel(payloadKind) + "_Traverse_" + FormatSizeLabel(workloadSize),
                "Collections",
                SyntheticMeasurementDefaults.CreateForTraversal(workloadSize),
                new Dictionary<string, string>
                {
                    { "collection", "SimpleLinkList<" + CollectionPayloadFactory.GetCollectionLabel(payloadKind) + ">" },
                    { "loop", "node-traverse" },
                    { "size", workloadSize.ToString() }
                });
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
                    intValues = new SimpleLinkList<int>(workloadSize);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        intValues.AddLast(i);
                    }

                    break;

                case CollectionPayloadKind.Class:
                    classValues = new SimpleLinkList<CollectionPayloadClass>(workloadSize);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        classValues.AddLast(CollectionPayloadFactory.CreateClassPayload(i));
                    }

                    break;

                default:
                    structValues = new SimpleLinkList<CollectionPayloadStruct>(workloadSize);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        structValues.AddLast(CollectionPayloadFactory.CreateStructPayload(i));
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
                        for (int node = intValues.First; node != CollectionConst.INVALID_HEAD; node = intValues.Next(node))
                        {
                            total += intValues.GetValue(node);
                        }

                        break;

                    case CollectionPayloadKind.Class:
                        for (int node = classValues.First; node != CollectionConst.INVALID_HEAD; node = classValues.Next(node))
                        {
                            CollectionPayloadClass payload = classValues.GetValue(node);
                            total += payload.a + payload.b + payload.c;
                        }

                        break;

                    default:
                        for (int node = structValues.First; node != CollectionConst.INVALID_HEAD; node = structValues.Next(node))
                        {
                            CollectionPayloadStruct payload = structValues.GetValue(node);
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
