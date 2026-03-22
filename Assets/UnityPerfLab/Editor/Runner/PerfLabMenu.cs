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
    }
}
