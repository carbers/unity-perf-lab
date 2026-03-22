using System;
using System.Collections.Generic;
using UnityPerfLab.Runtime.Environment;

namespace UnityPerfLab.Runtime.Core
{
    public sealed class PerfRunResult
    {
        public PerfRunResult(
            string runId,
            DateTime timestampUtc,
            string suiteName,
            IList<PerfCaseRunResult> caseResults,
            PerfEnvironmentMetadata metadata)
        {
            RunId = runId;
            TimestampUtc = timestampUtc;
            SuiteName = suiteName;
            CaseResults = caseResults;
            Metadata = metadata;
            OutputDirectory = string.Empty;
        }

        public string RunId { get; private set; }

        public DateTime TimestampUtc { get; private set; }

        public string SuiteName { get; private set; }

        public IList<PerfCaseRunResult> CaseResults { get; private set; }

        public PerfEnvironmentMetadata Metadata { get; private set; }

        public string OutputDirectory { get; set; }
    }
}
