namespace UnityPerfLab.Runtime.Core
{
    public interface IPerfCase
    {
        PerfCaseDescriptor Descriptor { get; }

        void GlobalSetup();

        void Setup();

        void Run(int iterations);

        void Teardown();

        void GlobalTeardown();
    }
}
