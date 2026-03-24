using Perf.Util;
using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public sealed class SimpleListAddPerfCase : PerfCaseBase
    {
        private readonly CollectionPayloadKind payloadKind;
        private readonly int workloadSize;
        private readonly PerfCaseDescriptor descriptor;
        private SimpleList<int> intValues;
        private SimpleList<CollectionPayloadClass> classValues;
        private SimpleList<CollectionPayloadStruct> structValues;

        public SimpleListAddPerfCase(CollectionPayloadKind payloadKind, int workloadSize)
        {
            this.payloadKind = payloadKind;
            this.workloadSize = workloadSize;

            descriptor = new PerfCaseDescriptor(
                "SimpleList_" + CollectionPayloadFactory.GetPayloadLabel(payloadKind) + "_Add_" + FormatSizeLabel(workloadSize),
                "Collections",
                SyntheticMeasurementDefaults.CreateForConstruction(workloadSize),
                new Dictionary<string, string>
                {
                    { "collection", "SimpleList<" + CollectionPayloadFactory.GetCollectionLabel(payloadKind) + ">" },
                    { "loop", "add" },
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
                    intValues = new SimpleList<int>(workloadSize);
                    break;

                case CollectionPayloadKind.Class:
                    classValues = new SimpleList<CollectionPayloadClass>(workloadSize);
                    break;

                default:
                    structValues = new SimpleList<CollectionPayloadStruct>(workloadSize);
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
                        intValues.Clear();
                        for (int i = 0; i < workloadSize; i++)
                        {
                            intValues.Add(i);
                        }

                        total += intValues.Count;
                        break;

                    case CollectionPayloadKind.Class:
                        classValues.Clear();
                        for (int i = 0; i < workloadSize; i++)
                        {
                            classValues.Add(CollectionPayloadFactory.CreateClassPayload(i));
                        }

                        total += classValues.Count;
                        break;

                    default:
                        structValues.Clear();
                        for (int i = 0; i < workloadSize; i++)
                        {
                            structValues.Add(CollectionPayloadFactory.CreateStructPayload(i));
                        }

                        total += structValues.Count;
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
