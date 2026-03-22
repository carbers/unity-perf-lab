using UnityEngine;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Bootstrap
{
    public sealed class PerfLabBootstrapBehaviour : MonoBehaviour
    {
        [SerializeField]
        private string suiteName = PerfSuiteCatalog.SyntheticSuite;

        [SerializeField]
        private string outputDirectoryOverride = string.Empty;

        [SerializeField]
        private bool quitOnComplete = true;

        private void Start()
        {
            PerfCommandLineArguments args = PerfCommandLineArguments.Parse(System.Environment.GetCommandLineArgs());
            string resolvedSuite = string.IsNullOrEmpty(args.SuiteName) ? suiteName : args.SuiteName;
            string resolvedOutputDirectory = string.IsNullOrEmpty(args.OutputDirectory) ? outputDirectoryOverride : args.OutputDirectory;
            string buildConfiguration = Debug.isDebugBuild ? "DevelopmentPlayer" : "ReleasePlayer";

            try
            {
                PerfRunResult result = PerfLabExecution.Run(resolvedSuite, buildConfiguration, resolvedOutputDirectory);
                Debug.Log("UnityPerfLab finished. Results written to: " + result.OutputDirectory);
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                if (quitOnComplete && !Application.isEditor)
                {
                    Application.Quit(0);
                }
            }
        }
    }
}
