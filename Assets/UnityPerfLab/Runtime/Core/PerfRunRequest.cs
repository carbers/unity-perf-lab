using System.Collections.Generic;

namespace UnityPerfLab.Runtime.Core
{
    public sealed class PerfRunRequest
    {
        public PerfRunRequest(
            string suiteName,
            IList<IPerfCase> cases,
            string buildConfiguration,
            string outputDirectoryOverride = "",
            bool includeMetadata = true)
        {
            SuiteName = suiteName;
            Cases = cases ?? new List<IPerfCase>();
            BuildConfiguration = buildConfiguration;
            OutputDirectoryOverride = outputDirectoryOverride ?? string.Empty;
            IncludeMetadata = includeMetadata;
        }

        public string SuiteName { get; private set; }

        public IList<IPerfCase> Cases { get; private set; }

        public string BuildConfiguration { get; private set; }

        public string OutputDirectoryOverride { get; private set; }

        public bool IncludeMetadata { get; private set; }
    }
}
