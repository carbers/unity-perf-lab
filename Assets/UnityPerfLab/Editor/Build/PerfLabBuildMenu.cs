using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Editor.Build
{
    public static class PerfLabBuildMenu
    {
        private const string MenuPath = "UnityPerfLab/Build/Win64 IL2CPP Release";
        private const string OutputFolderName = "Win64-IL2CPP-Release";
        private const string ExecutableName = "UnityPerfLab.exe";

        [MenuItem(MenuPath)]
        public static void BuildWin64Il2CppRelease()
        {
            string bootstrapScenePath = PerfLabProjectConstants.BootstrapScenePath;
            string absoluteBootstrapScenePath = Path.Combine(GetProjectRoot(), bootstrapScenePath);
            if (!File.Exists(absoluteBootstrapScenePath))
            {
                throw new InvalidOperationException("UnityPerfLab bootstrap scene was not found at '" + bootstrapScenePath + "'.");
            }

            string buildOutputPath = GetBuildOutputPath();
            string buildOutputDirectory = Path.GetDirectoryName(buildOutputPath);
            if (string.IsNullOrEmpty(buildOutputDirectory))
            {
                throw new InvalidOperationException("UnityPerfLab could not resolve a valid build output directory.");
            }

            Directory.CreateDirectory(buildOutputDirectory);

            BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
            ScriptingImplementation previousScriptingBackend = PlayerSettings.GetScriptingBackend(targetGroup);
            bool previousDevelopmentBuild = EditorUserBuildSettings.development;

            try
            {
                PlayerSettings.SetScriptingBackend(targetGroup, ScriptingImplementation.IL2CPP);
                EditorUserBuildSettings.development = false;

                BuildPlayerOptions buildOptions = new BuildPlayerOptions
                {
                    scenes = new[] { bootstrapScenePath },
                    locationPathName = buildOutputPath,
                    target = BuildTarget.StandaloneWindows64,
                    options = BuildOptions.None
                };

                BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
                if (report.summary.result != BuildResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        "UnityPerfLab Win64 IL2CPP Release build failed with result '" + report.summary.result + "'.");
                }

                Debug.Log("UnityPerfLab Win64 IL2CPP Release build complete. Output: " + buildOutputPath);
            }
            finally
            {
                EditorUserBuildSettings.development = previousDevelopmentBuild;
                PlayerSettings.SetScriptingBackend(targetGroup, previousScriptingBackend);
            }
        }

        private static string GetBuildOutputPath()
        {
            return Path.Combine(
                GetProjectRoot(),
                PerfLabProjectConstants.BuildRoot,
                OutputFolderName,
                ExecutableName);
        }

        private static string GetProjectRoot()
        {
            return Directory.GetParent(Application.dataPath)?.FullName
                ?? throw new InvalidOperationException("UnityPerfLab could not resolve the Unity project root.");
        }
    }
}
