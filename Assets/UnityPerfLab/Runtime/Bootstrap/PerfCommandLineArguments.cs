using System;

namespace UnityPerfLab.Bootstrap
{
    public sealed class PerfCommandLineArguments
    {
        public string SuiteName { get; private set; }

        public string OutputDirectory { get; private set; }

        public static PerfCommandLineArguments Parse(string[] args)
        {
            PerfCommandLineArguments parsed = new PerfCommandLineArguments();
            parsed.SuiteName = string.Empty;
            parsed.OutputDirectory = string.Empty;

            if (args == null)
            {
                return parsed;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string argument = args[i] ?? string.Empty;
                if (argument.StartsWith("--suite=", StringComparison.OrdinalIgnoreCase))
                {
                    parsed.SuiteName = argument.Substring("--suite=".Length);
                }
                else if (argument.StartsWith("--outputDir=", StringComparison.OrdinalIgnoreCase))
                {
                    parsed.OutputDirectory = argument.Substring("--outputDir=".Length);
                }
            }

            return parsed;
        }
    }
}
