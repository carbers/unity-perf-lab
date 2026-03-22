using System;
using UnityEngine;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Runtime.Environment
{
    public static class PerfEnvironmentCollector
    {
        public static PerfEnvironmentMetadata Capture(string runId, DateTime timestampUtc, PerfRunRequest request)
        {
            PerfEnvironmentMetadata metadata = new PerfEnvironmentMetadata();
            metadata.runId = runId;
            metadata.timestampUtc = timestampUtc.ToString("O");
            metadata.suiteName = request.SuiteName;
            metadata.unityVersion = Application.unityVersion;
            metadata.platform = Application.platform.ToString();
            metadata.buildConfiguration = request.BuildConfiguration;
            metadata.scriptingBackend = GetScriptingBackendLabel();
            metadata.isDevelopmentBuild = Debug.isDebugBuild;
            metadata.commandLine = System.Environment.CommandLine;
            metadata.machineName = System.Environment.MachineName;
            metadata.operatingSystem = SystemInfo.operatingSystem;
            metadata.processorType = SystemInfo.processorType;
            metadata.processorCount = SystemInfo.processorCount;
            return metadata;
        }

        private static string GetScriptingBackendLabel()
        {
#if ENABLE_IL2CPP
            return "IL2CPP";
#elif ENABLE_MONO
            return "Mono";
#else
            return "Unknown";
#endif
        }
    }
}
