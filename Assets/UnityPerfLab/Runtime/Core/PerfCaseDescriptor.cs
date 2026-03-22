using System.Collections.Generic;
using System.Text;

namespace UnityPerfLab.Runtime.Core
{
    public sealed class PerfCaseDescriptor
    {
        public PerfCaseDescriptor(
            string name,
            string category,
            PerfMeasurementConfig measurement,
            IDictionary<string, string> parameters = null,
            string notes = "")
        {
            Name = name;
            Category = category;
            Measurement = measurement;
            Notes = notes ?? string.Empty;
            Parameters = parameters != null
                ? new SortedDictionary<string, string>(parameters)
                : new SortedDictionary<string, string>();
        }

        public string Name { get; private set; }

        public string Category { get; private set; }

        public PerfMeasurementConfig Measurement { get; private set; }

        public IReadOnlyDictionary<string, string> Parameters { get; private set; }

        public string Notes { get; private set; }

        public string GetParametersDisplay()
        {
            if (Parameters.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            bool isFirst = true;
            foreach (KeyValuePair<string, string> pair in Parameters)
            {
                if (!isFirst)
                {
                    builder.Append(';');
                }

                builder.Append(pair.Key);
                builder.Append('=');
                builder.Append(pair.Value);
                isFirst = false;
            }

            return builder.ToString();
        }
    }
}
