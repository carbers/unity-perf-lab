using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityPerfLab.Runtime.Environment;

namespace UnityPerfLab.Runtime.Core
{
    public sealed class PerfRunner
    {
        public PerfRunResult Run(PerfRunRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.Cases == null)
            {
                throw new ArgumentException("PerfRunRequest.Cases cannot be null.", "request");
            }

            if (request.Cases.Count == 0)
            {
                throw new ArgumentException("PerfRunRequest.Cases cannot be empty.", "request");
            }

            DateTime timestampUtc = DateTime.UtcNow;
            string runId = timestampUtc.ToString("yyyyMMddTHHmmssfffZ") + "-" + Guid.NewGuid().ToString("N").Substring(0, 6);
            PerfEnvironmentMetadata metadata = PerfEnvironmentCollector.Capture(runId, timestampUtc, request);
            List<PerfCaseRunResult> caseResults = new List<PerfCaseRunResult>(request.Cases.Count);

            for (int i = 0; i < request.Cases.Count; i++)
            {
                if (request.Cases[i] == null)
                {
                    throw new ArgumentException("PerfRunRequest.Cases cannot contain null entries.", "request");
                }

                caseResults.Add(ExecuteCase(request.Cases[i]));
            }

            return new PerfRunResult(runId, timestampUtc, request.SuiteName, caseResults, metadata);
        }

        private static PerfCaseRunResult ExecuteCase(IPerfCase perfCase)
        {
            PerfCaseDescriptor descriptor = perfCase.Descriptor;
            PerfMeasurementConfig config = descriptor.Measurement;
            List<PerfSample> allSamples = new List<PerfSample>(config.WarmupSamples + config.MeasureSamples);
            List<PerfSample> measuredSamples = new List<PerfSample>(config.MeasureSamples);
            int iterationsPerSample = 0;
            int totalMeasuredIterations = 0;

            try
            {
                perfCase.GlobalSetup();
                iterationsPerSample = ResolveIterationsPerSample(perfCase, config);

                int sampleIndex = 0;
                for (int i = 0; i < config.WarmupSamples; i++)
                {
                    PerfSample sample = ExecuteSample(perfCase, sampleIndex++, true, iterationsPerSample);
                    allSamples.Add(sample);
                }

                for (int i = 0; i < config.MeasureSamples; i++)
                {
                    PerfSample sample = ExecuteSample(perfCase, sampleIndex++, false, iterationsPerSample);
                    allSamples.Add(sample);
                    measuredSamples.Add(sample);
                    totalMeasuredIterations += iterationsPerSample;
                }
            }
            finally
            {
                perfCase.GlobalTeardown();
            }

            return new PerfCaseRunResult(
                descriptor,
                allSamples,
                PerfSummaryStats.FromSamples(measuredSamples),
                totalMeasuredIterations,
                iterationsPerSample);
        }

        private static int ResolveIterationsPerSample(IPerfCase perfCase, PerfMeasurementConfig config)
        {
            if (config.Mode == PerfMeasurementMode.FixedIterations)
            {
                return config.IterationsPerSample;
            }

            perfCase.Setup();
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                perfCase.Run(config.ProbeIterations);
                stopwatch.Stop();
                long elapsedNanoseconds = PerfMath.StopwatchTicksToNanoseconds(stopwatch.ElapsedTicks);
                if (elapsedNanoseconds <= 0)
                {
                    return config.MaxIterations;
                }

                double targetNanoseconds = config.TargetSampleDurationMs * 1000000d;
                double estimatedIterations = targetNanoseconds / elapsedNanoseconds * config.ProbeIterations;
                int resolved = (int)Math.Ceiling(estimatedIterations);
                return PerfMath.Clamp(resolved, config.MinIterations, config.MaxIterations);
            }
            finally
            {
                perfCase.Teardown();
            }
        }

        private static PerfSample ExecuteSample(IPerfCase perfCase, int sampleIndex, bool isWarmup, int iterationsPerSample)
        {
            perfCase.Setup();
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                perfCase.Run(iterationsPerSample);
                stopwatch.Stop();
                return new PerfSample(sampleIndex, isWarmup, iterationsPerSample, PerfMath.StopwatchTicksToNanoseconds(stopwatch.ElapsedTicks));
            }
            finally
            {
                perfCase.Teardown();
            }
        }
    }
}
