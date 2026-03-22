using System;

namespace UnityPerfLab.Runtime.Core
{
    public abstract class PerfCaseBase : IPerfCase
    {
        public abstract PerfCaseDescriptor Descriptor { get; }

        public virtual void GlobalSetup()
        {
        }

        public virtual void Setup()
        {
        }

        public abstract void Run(int iterations);

        public virtual void Teardown()
        {
        }

        public virtual void GlobalTeardown()
        {
        }

        protected static string FormatSizeLabel(int size)
        {
            if (size >= 1000000 && size % 1000000 == 0)
            {
                return (size / 1000000).ToString() + "M";
            }

            if (size >= 1000 && size % 1000 == 0)
            {
                return (size / 1000).ToString() + "K";
            }

            return size.ToString();
        }
    }
}
