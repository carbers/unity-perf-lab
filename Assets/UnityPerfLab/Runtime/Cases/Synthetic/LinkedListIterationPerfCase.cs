using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public sealed class LinkedListIterationPerfCase : PerfCaseBase
    {
        private readonly CollectionPayloadKind payloadKind;
        private readonly int workloadSize;
        private readonly PerfCaseDescriptor descriptor;
        private LinkedList<int> intValues;
        private LinkedList<CollectionPayloadClass> classValues;
        private LinkedList<CollectionPayloadStruct> structValues;

        public LinkedListIterationPerfCase(CollectionPayloadKind payloadKind, int workloadSize)
        {
            this.payloadKind = payloadKind;
            this.workloadSize = workloadSize;

            descriptor = new PerfCaseDescriptor(
                "LinkedList_" + CollectionPayloadFactory.GetPayloadLabel(payloadKind) + "_ForEach_" + FormatSizeLabel(workloadSize),
                "Collections",
                SyntheticMeasurementDefaults.CreateForTraversal(workloadSize),
                new Dictionary<string, string>
                {
                    { "collection", "LinkedList<" + CollectionPayloadFactory.GetCollectionLabel(payloadKind) + ">" },
                    { "loop", "foreach" },
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
                    intValues = new LinkedList<int>();
                    for (int i = 0; i < workloadSize; i++)
                    {
                        intValues.AddLast(i);
                    }

                    break;

                case CollectionPayloadKind.Class:
                    classValues = new LinkedList<CollectionPayloadClass>();
                    for (int i = 0; i < workloadSize; i++)
                    {
                        classValues.AddLast(CollectionPayloadFactory.CreateClassPayload(i));
                    }

                    break;

                default:
                    structValues = new LinkedList<CollectionPayloadStruct>();
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
                        foreach (int value in intValues)
                        {
                            total += value;
                        }

                        break;

                    case CollectionPayloadKind.Class:
                        foreach (CollectionPayloadClass payload in classValues)
                        {
                            total += payload.a + payload.b + payload.c;
                        }

                        break;

                    default:
                        foreach (CollectionPayloadStruct payload in structValues)
                        {
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
