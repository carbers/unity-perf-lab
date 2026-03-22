using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public static class SyntheticPerfCaseCatalog
    {
        private static readonly int[] WorkloadSizes = { 1000, 100000, 1000000 };

        public static List<IPerfCase> CreateAll()
        {
            List<IPerfCase> cases = new List<IPerfCase>();

            for (int i = 0; i < WorkloadSizes.Length; i++)
            {
                int workloadSize = WorkloadSizes[i];
                cases.Add(new ArrayIntIterationPerfCase(true, workloadSize));
                cases.Add(new ArrayIntIterationPerfCase(false, workloadSize));
                cases.Add(new ListIntIterationPerfCase(true, workloadSize));
                cases.Add(new ListIntIterationPerfCase(false, workloadSize));
                cases.Add(new ListStructIterationPerfCase(true, workloadSize));
                cases.Add(new ListStructIterationPerfCase(false, workloadSize));
                cases.Add(new CallDispatchPerfCase(false, workloadSize));
                cases.Add(new CallDispatchPerfCase(true, workloadSize));
                cases.Add(new ClosureCapturePerfCase(false, workloadSize));
                cases.Add(new ClosureCapturePerfCase(true, workloadSize));
            }

            return cases;
        }
    }
}
