using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    internal static class SyntheticMeasurementDefaults
    {
        public static PerfMeasurementConfig CreateForTraversal(int workloadSize)
        {
            if (workloadSize >= 1000000)
            {
                return PerfMeasurementConfig.CreateTargetDuration(500, 1, 16, 3, 10, 1);
            }

            if (workloadSize >= 100000)
            {
                return PerfMeasurementConfig.CreateTargetDuration(200, 1, 128, 5, 20, 1);
            }

            return PerfMeasurementConfig.CreateTargetDuration(200, 32, 200000, 5, 20, 8);
        }

        public static PerfMeasurementConfig CreateForConstruction(int workloadSize)
        {
            if (workloadSize >= 1000000)
            {
                return PerfMeasurementConfig.CreateFixedIterations(1, 2, 6);
            }

            if (workloadSize >= 100000)
            {
                return PerfMeasurementConfig.CreateFixedIterations(1, 3, 10);
            }

            return PerfMeasurementConfig.CreateTargetDuration(200, 8, 256, 5, 20, 4);
        }

        public static PerfMeasurementConfig CreateForClosure(int workloadSize)
        {
            if (workloadSize >= 1000000)
            {
                return PerfMeasurementConfig.CreateFixedIterations(1, 3, 10);
            }

            if (workloadSize >= 100000)
            {
                return PerfMeasurementConfig.CreateFixedIterations(4, 5, 20);
            }

            return PerfMeasurementConfig.CreateFixedIterations(64, 5, 20);
        }
    }
}
