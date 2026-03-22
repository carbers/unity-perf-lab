using System;
using System.Collections.Generic;
using UnityPerfLab.Cases.RealWorld;
using UnityPerfLab.Cases.Synthetic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Bootstrap
{
    public static class PerfSuiteCatalog
    {
        public const string SyntheticSuite = "synthetic";
        public const string AllSuite = "all";

        public static List<IPerfCase> CreateCases(string suiteName)
        {
            string normalizedSuite = NormalizeSuiteName(suiteName);
            if (normalizedSuite == SyntheticSuite)
            {
                return SyntheticPerfCaseCatalog.CreateAll();
            }

            if (normalizedSuite == AllSuite)
            {
                List<IPerfCase> cases = SyntheticPerfCaseCatalog.CreateAll();
                cases.AddRange(RealWorldPerfCaseCatalog.CreateAll());
                return cases;
            }

            throw new ArgumentException("Unsupported UnityPerfLab suite: " + suiteName);
        }

        public static string NormalizeSuiteName(string suiteName)
        {
            if (string.IsNullOrEmpty(suiteName))
            {
                return SyntheticSuite;
            }

            string normalized = suiteName.Trim().ToLowerInvariant();
            return string.IsNullOrEmpty(normalized) ? SyntheticSuite : normalized;
        }
    }
}
