using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityPerfLab.Runtime.Core;
using UnityPerfLab.Runtime.Environment;

namespace UnityPerfLab.Runtime.Reporting
{
    public sealed class CsvResultExporter : IResultExporter
    {
        public string Export(PerfRunResult result, PerfRunRequest request)
        {
            string runDirectory = PerfResultPathResolver.ResolveRunDirectory(request, result);
            result.OutputDirectory = runDirectory;

            File.WriteAllText(Path.Combine(runDirectory, "summary.csv"), BuildSummaryCsv(result), Encoding.UTF8);
            File.WriteAllText(Path.Combine(runDirectory, "raw_samples.csv"), BuildRawSamplesCsv(result), Encoding.UTF8);

            if (request.IncludeMetadata && result.Metadata != null)
            {
                File.WriteAllText(
                    Path.Combine(runDirectory, "metadata.json"),
                    JsonUtility.ToJson(result.Metadata, true),
                    Encoding.UTF8);
            }

            return runDirectory;
        }

        private static string BuildSummaryCsv(PerfRunResult result)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
                "run_id,timestamp_utc,unity_version,platform,build_config,case_name,category,parameters,measurement_mode,warmup_count,sample_count,total_iterations,target_duration_ms,mean_ns,median_ns,min_ns,max_ns,stddev_ns,p95_ns,ops_per_sec,notes");

            for (int i = 0; i < result.CaseResults.Count; i++)
            {
                PerfCaseRunResult caseResult = result.CaseResults[i];
                PerfCaseDescriptor descriptor = caseResult.Descriptor;
                PerfMeasurementConfig config = descriptor.Measurement;

                AppendCsvRow(
                    builder,
                    result.RunId,
                    result.TimestampUtc.ToString("O", CultureInfo.InvariantCulture),
                    result.Metadata.UnityVersion,
                    result.Metadata.Platform,
                    result.Metadata.BuildConfiguration,
                    descriptor.Name,
                    descriptor.Category,
                    descriptor.GetParametersDisplay(),
                    config.Mode.ToString(),
                    config.WarmupSamples.ToString(CultureInfo.InvariantCulture),
                    config.MeasureSamples.ToString(CultureInfo.InvariantCulture),
                    caseResult.TotalMeasuredIterations.ToString(CultureInfo.InvariantCulture),
                    config.TargetSampleDurationMs.ToString(CultureInfo.InvariantCulture),
                    FormatDouble(caseResult.Summary.MeanNanoseconds),
                    FormatDouble(caseResult.Summary.MedianNanoseconds),
                    FormatDouble(caseResult.Summary.MinNanoseconds),
                    FormatDouble(caseResult.Summary.MaxNanoseconds),
                    FormatDouble(caseResult.Summary.StandardDeviationNanoseconds),
                    FormatDouble(caseResult.Summary.P95Nanoseconds),
                    FormatDouble(caseResult.Summary.OperationsPerSecond),
                    descriptor.Notes);
            }

            return builder.ToString();
        }

        private static string BuildRawSamplesCsv(PerfRunResult result)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("run_id,case_name,sample_index,is_warmup,iterations,elapsed_ns,ns_per_op");

            for (int i = 0; i < result.CaseResults.Count; i++)
            {
                PerfCaseRunResult caseResult = result.CaseResults[i];
                for (int sampleIndex = 0; sampleIndex < caseResult.Samples.Count; sampleIndex++)
                {
                    PerfSample sample = caseResult.Samples[sampleIndex];
                    AppendCsvRow(
                        builder,
                        result.RunId,
                        caseResult.Descriptor.Name,
                        sample.SampleIndex.ToString(CultureInfo.InvariantCulture),
                        sample.IsWarmup ? "true" : "false",
                        sample.Iterations.ToString(CultureInfo.InvariantCulture),
                        sample.ElapsedNanoseconds.ToString(CultureInfo.InvariantCulture),
                        FormatDouble(sample.NanosecondsPerOperation));
                }
            }

            return builder.ToString();
        }

        private static void AppendCsvRow(StringBuilder builder, params string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }

                builder.Append(Escape(values[i]));
            }

            builder.AppendLine();
        }

        private static string Escape(string value)
        {
            string safe = value ?? string.Empty;
            bool requiresQuotes = safe.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0;
            if (!requiresQuotes)
            {
                return safe;
            }

            return "\"" + safe.Replace("\"", "\"\"") + "\"";
        }

        private static string FormatDouble(double value)
        {
            return value.ToString("F4", CultureInfo.InvariantCulture);
        }
    }
}
