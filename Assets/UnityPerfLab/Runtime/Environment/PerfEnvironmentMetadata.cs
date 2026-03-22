using System;

namespace UnityPerfLab.Runtime.Environment
{
    [Serializable]
    public sealed class PerfEnvironmentMetadata
    {
        public string runId;
        public string timestampUtc;
        public string suiteName;
        public string unityVersion;
        public string platform;
        public string buildConfiguration;
        public string scriptingBackend;
        public bool isDevelopmentBuild;
        public string commandLine;
        public string machineName;
        public string operatingSystem;
        public string processorType;
        public int processorCount;

        public string RunId
        {
            get { return runId; }
        }

        public string TimestampUtc
        {
            get { return timestampUtc; }
        }

        public string SuiteName
        {
            get { return suiteName; }
        }

        public string UnityVersion
        {
            get { return unityVersion; }
        }

        public string Platform
        {
            get { return platform; }
        }

        public string BuildConfiguration
        {
            get { return buildConfiguration; }
        }

        public string ScriptingBackend
        {
            get { return scriptingBackend; }
        }
    }
}
