using UnityEditor;

namespace UnityPerfLab.Editor.Runner
{
    public static class PerfLabMenu
    {
        [MenuItem("UnityPerfLab/Run Synthetic Suite")]
        public static void RunSyntheticSuite()
        {
            PerfLabEditorRunner.RunSyntheticSuite();
        }

        [MenuItem("UnityPerfLab/Run All Available Cases")]
        public static void RunAllAvailableCases()
        {
            PerfLabEditorRunner.RunAllAvailableCases();
        }
    }
}
