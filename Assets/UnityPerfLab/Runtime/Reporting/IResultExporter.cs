using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Runtime.Reporting
{
    public interface IResultExporter
    {
        string Export(PerfRunResult result, PerfRunRequest request);
    }
}
