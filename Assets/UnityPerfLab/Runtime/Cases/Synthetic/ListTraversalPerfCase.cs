using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public sealed class ListTraversalPerfCase : PerfCaseBase
    {
        private readonly CollectionPayloadKind payloadKind;
        private readonly int workloadSize;
        private readonly PerfCaseDescriptor descriptor;
        private List<int> intValues;
        private List<CollectionPayloadClass> classValues;
        private List<CollectionPayloadStruct> structValues;

        public ListTraversalPerfCase(CollectionPayloadKind payloadKind, int workloadSize)
        {
            this.payloadKind = payloadKind;
            this.workloadSize = workloadSize;

            descriptor = new PerfCaseDescriptor(
                "List_" + CollectionPayloadFactory.GetPayloadLabel(payloadKind) + "_Traverse_" + FormatSizeLabel(workloadSize),
                "Collections",
                SyntheticMeasurementDefaults.CreateForTraversal(workloadSize),
                new Dictionary<string, string>
                {
                    { "collection", "List<" + CollectionPayloadFactory.GetCollectionLabel(payloadKind) + ">" },
                    { "loop", "index-traverse" },
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
                    intValues = new List<int>(workloadSize);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        intValues.Add(i);
                    }

                    break;

                case CollectionPayloadKind.Class:
                    classValues = new List<CollectionPayloadClass>(workloadSize);
                    for (int i = 0; i < workloadSize; i++)
                    {
                        classValues.Add(CollectionPayloadFactory.CreateClassPayload(i));
                    }

                    break;

                default:
                    structValues = new List<CollectionPayloadStruct>(workloadSize);
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
                        for (int i = 0; i < intValues.Count; i++)
                        {
                            total += intValues[i];
                        }

                        break;

                    case CollectionPayloadKind.Class:
                        for (int i = 0; i < classValues.Count; i++)
                        {
                            CollectionPayloadClass payload = classValues[i];
                            total += payload.a + payload.b + payload.c;
                        }

                        break;

                    default:
                        for (int i = 0; i < structValues.Count; i++)
                        {
                            CollectionPayloadStruct payload = structValues[i];
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
