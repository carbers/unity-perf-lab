using UnityEngine;
using UnityPerfLab.Bootstrap;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Editor.Runner
{
    public static class PerfLabEditorRunner
    {
        public static void RunSyntheticSuite()
        {
            RunSuite(PerfSuiteCatalog.SyntheticSuite);
        }

        public static void RunAllAvailableCases()
        {
            RunSuite(PerfSuiteCatalog.AllSuite);
        }

        private static void RunSuite(string suiteName)
        {
            PerfRunResult result = PerfLabExecution.Run(suiteName, "Editor");
            Debug.Log("UnityPerfLab Editor run complete. Results written to: " + result.OutputDirectory);
        }
    }
}
