using System;
using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;
using UnityPerfLab.Runtime.Reporting;

namespace UnityPerfLab.Bootstrap
{
    public static class PerfLabExecution
    {
        public static PerfRunResult Run(string suiteName, string buildConfiguration, string outputDirectoryOverride = "")
        {
            string normalizedSuite = PerfSuiteCatalog.NormalizeSuiteName(suiteName);
            List<IPerfCase> perfCases = PerfSuiteCatalog.CreateCases(normalizedSuite);
            if (perfCases.Count == 0)
            {
                throw new InvalidOperationException("UnityPerfLab has no benchmark cases registered for suite '" + normalizedSuite + "'.");
            }

            PerfRunRequest request = new PerfRunRequest(
                normalizedSuite,
                perfCases,
                buildConfiguration,
                outputDirectoryOverride,
                true);

            PerfRunner runner = new PerfRunner();
            PerfRunResult result = runner.Run(request);
            IResultExporter exporter = new CsvResultExporter();
            exporter.Export(result, request);
            return result;
        }
    }
}
