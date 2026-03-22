using System.IO;
using UnityEngine;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Runtime.Environment
{
    public static class PerfResultPathResolver
    {
        public static string ResolveRunDirectory(PerfRunRequest request, PerfRunResult result)
        {
            string root = request.OutputDirectoryOverride;
            if (string.IsNullOrEmpty(root))
            {
                root = Application.isEditor
                    ? Path.Combine(GetProjectRoot(), PerfLabProjectConstants.ResultFolderName)
                    : Path.Combine(Application.persistentDataPath, PerfLabProjectConstants.ResultFolderName);
            }

            string runDirectory = Path.Combine(root, result.RunId);
            Directory.CreateDirectory(runDirectory);
            return runDirectory;
        }

        private static string GetProjectRoot()
        {
            DirectoryInfo parent = Directory.GetParent(Application.dataPath);
            return parent != null ? parent.FullName : Application.dataPath;
        }
    }
}
