using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
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
            File.WriteAllText(Path.Combine(runDirectory, "overview.csv"), BuildOverviewCsv(result), Encoding.UTF8);
            File.WriteAllText(Path.Combine(runDirectory, "overview.md"), BuildOverviewMarkdown(result), Encoding.UTF8);

            if (request.IncludeMetadata && result.Metadata != null)
            {
                File.WriteAllText(
                    Path.Combine(runDirectory, "metadata.json"),
                    BuildMetadataJson(result.Metadata),
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

        private static string BuildOverviewCsv(PerfRunResult result)
        {
            List<OverviewRow> rows = BuildOverviewRows(result);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("size_label,size,category,subject,variant,case_name,median_ns,mean_ns,ops_per_sec");

            for (int i = 0; i < rows.Count; i++)
            {
                OverviewRow row = rows[i];
                AppendCsvRow(
                    builder,
                    row.SizeLabel,
                    row.Size.ToString(CultureInfo.InvariantCulture),
                    row.Category,
                    row.Subject,
                    row.Variant,
                    row.CaseName,
                    FormatDouble(row.MedianNanoseconds),
                    FormatDouble(row.MeanNanoseconds),
                    FormatDouble(row.OperationsPerSecond));
            }

            return builder.ToString();
        }

        private static string BuildOverviewMarkdown(PerfRunResult result)
        {
            List<OverviewRow> rows = BuildOverviewRows(result);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# UnityPerfLab Overview");
            builder.AppendLine();
            builder.AppendLine("- run_id: `" + result.RunId + "`");
            builder.AppendLine("- unity_version: `" + result.Metadata.UnityVersion + "`");
            builder.AppendLine("- platform: `" + result.Metadata.Platform + "`");
            builder.AppendLine("- build_config: `" + result.Metadata.BuildConfiguration + "`");
            builder.AppendLine();

            int currentSize = -1;
            for (int i = 0; i < rows.Count; i++)
            {
                OverviewRow row = rows[i];
                if (row.Size != currentSize)
                {
                    if (currentSize != -1)
                    {
                        builder.AppendLine();
                    }

                    currentSize = row.Size;
                    builder.AppendLine("## " + row.SizeLabel);
                    builder.AppendLine();
                    builder.AppendLine("| Category | Subject | Variant | Median ns/op | Mean ns/op | Ops/sec |");
                    builder.AppendLine("| --- | --- | --- | ---: | ---: | ---: |");
                }

                builder.Append("| ");
                builder.Append(EscapeMarkdownCell(row.Category));
                builder.Append(" | ");
                builder.Append(EscapeMarkdownCell(row.Subject));
                builder.Append(" | ");
                builder.Append(EscapeMarkdownCell(row.Variant));
                builder.Append(" | ");
                builder.Append(FormatDouble(row.MedianNanoseconds));
                builder.Append(" | ");
                builder.Append(FormatDouble(row.MeanNanoseconds));
                builder.Append(" | ");
                builder.Append(FormatDouble(row.OperationsPerSecond));
                builder.AppendLine(" |");
            }

            return builder.ToString();
        }

        private static string BuildMetadataJson(PerfEnvironmentMetadata metadata)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("{");
            AppendJsonProperty(builder, "runId", metadata.runId, true);
            AppendJsonProperty(builder, "timestampUtc", metadata.timestampUtc, true);
            AppendJsonProperty(builder, "suiteName", metadata.suiteName, true);
            AppendJsonProperty(builder, "unityVersion", metadata.unityVersion, true);
            AppendJsonProperty(builder, "platform", metadata.platform, true);
            AppendJsonProperty(builder, "buildConfiguration", metadata.buildConfiguration, true);
            AppendJsonProperty(builder, "scriptingBackend", metadata.scriptingBackend, true);
            AppendJsonProperty(builder, "isDevelopmentBuild", metadata.isDevelopmentBuild ? "true" : "false", false, true);
            AppendJsonProperty(builder, "commandLine", metadata.commandLine, true);
            AppendJsonProperty(builder, "machineName", metadata.machineName, true);
            AppendJsonProperty(builder, "operatingSystem", metadata.operatingSystem, true);
            AppendJsonProperty(builder, "processorType", metadata.processorType, true);
            AppendJsonProperty(builder, "processorCount", metadata.processorCount.ToString(CultureInfo.InvariantCulture), false, false);
            builder.AppendLine("}");
            return builder.ToString();
        }

        private static List<OverviewRow> BuildOverviewRows(PerfRunResult result)
        {
            List<OverviewRow> rows = new List<OverviewRow>(result.CaseResults.Count);
            for (int i = 0; i < result.CaseResults.Count; i++)
            {
                PerfCaseRunResult caseResult = result.CaseResults[i];
                PerfCaseDescriptor descriptor = caseResult.Descriptor;
                int size = GetSize(descriptor.Parameters);

                rows.Add(
                    new OverviewRow(
                        size,
                        GetSizeLabel(size),
                        descriptor.Category,
                        GetSubject(descriptor),
                        GetVariant(descriptor),
                        descriptor.Name,
                        caseResult.Summary.MedianNanoseconds,
                        caseResult.Summary.MeanNanoseconds,
                        caseResult.Summary.OperationsPerSecond));
            }

            rows.Sort(CompareOverviewRows);
            return rows;
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

        private static int CompareOverviewRows(OverviewRow left, OverviewRow right)
        {
            int sizeComparison = left.Size.CompareTo(right.Size);
            if (sizeComparison != 0)
            {
                return sizeComparison;
            }

            int categoryComparison = string.Compare(left.Category, right.Category, StringComparison.Ordinal);
            if (categoryComparison != 0)
            {
                return categoryComparison;
            }

            int subjectComparison = string.Compare(left.Subject, right.Subject, StringComparison.Ordinal);
            if (subjectComparison != 0)
            {
                return subjectComparison;
            }

            int variantComparison = string.Compare(left.Variant, right.Variant, StringComparison.Ordinal);
            if (variantComparison != 0)
            {
                return variantComparison;
            }

            return string.Compare(left.CaseName, right.CaseName, StringComparison.Ordinal);
        }

        private static void AppendJsonProperty(StringBuilder builder, string name, string value, bool quoteValue, bool hasTrailingComma = true)
        {
            builder.Append("  \"");
            builder.Append(EscapeJson(name));
            builder.Append("\": ");

            if (quoteValue)
            {
                builder.Append("\"");
                builder.Append(EscapeJson(value));
                builder.Append("\"");
            }
            else
            {
                builder.Append(value ?? "null");
            }

            if (hasTrailingComma)
            {
                builder.Append(",");
            }

            builder.AppendLine();
        }

        private static string EscapeJson(string value)
        {
            string safe = value ?? string.Empty;
            return safe
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }

        private static string EscapeMarkdownCell(string value)
        {
            return (value ?? string.Empty)
                .Replace("|", "\\|")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        private static int GetSize(IReadOnlyDictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.TryGetValue("size", out string sizeValue))
            {
                return 0;
            }

            return int.TryParse(sizeValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int size)
                ? size
                : 0;
        }

        private static string GetSizeLabel(int size)
        {
            if (size >= 1000000 && size % 1000000 == 0)
            {
                return (size / 1000000).ToString(CultureInfo.InvariantCulture) + "M";
            }

            if (size >= 1000 && size % 1000 == 0)
            {
                return (size / 1000).ToString(CultureInfo.InvariantCulture) + "K";
            }

            return size > 0
                ? size.ToString(CultureInfo.InvariantCulture)
                : "Unknown";
        }

        private static string GetSubject(PerfCaseDescriptor descriptor)
        {
            if (descriptor.Parameters != null && descriptor.Parameters.TryGetValue("collection", out string collection))
            {
                return collection;
            }

            return descriptor.Category;
        }

        private static string GetVariant(PerfCaseDescriptor descriptor)
        {
            if (descriptor.Parameters == null)
            {
                return string.Empty;
            }

            if (descriptor.Parameters.TryGetValue("loop", out string loop))
            {
                return loop;
            }

            if (descriptor.Parameters.TryGetValue("dispatch", out string dispatch))
            {
                return dispatch;
            }

            if (descriptor.Parameters.TryGetValue("closure", out string closure))
            {
                return closure;
            }

            return string.Empty;
        }

        private static string FormatDouble(double value)
        {
            return value.ToString("F4", CultureInfo.InvariantCulture);
        }

        private sealed class OverviewRow
        {
            public OverviewRow(
                int size,
                string sizeLabel,
                string category,
                string subject,
                string variant,
                string caseName,
                double medianNanoseconds,
                double meanNanoseconds,
                double operationsPerSecond)
            {
                Size = size;
                SizeLabel = sizeLabel;
                Category = category;
                Subject = subject;
                Variant = variant;
                CaseName = caseName;
                MedianNanoseconds = medianNanoseconds;
                MeanNanoseconds = meanNanoseconds;
                OperationsPerSecond = operationsPerSecond;
            }

            public int Size { get; private set; }

            public string SizeLabel { get; private set; }

            public string Category { get; private set; }

            public string Subject { get; private set; }

            public string Variant { get; private set; }

            public string CaseName { get; private set; }

            public double MedianNanoseconds { get; private set; }

            public double MeanNanoseconds { get; private set; }

            public double OperationsPerSecond { get; private set; }
        }
    }
}
