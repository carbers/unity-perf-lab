using System.Threading;

namespace UnityPerfLab.Runtime.Core
{
    public static class PerfVisibleSink
    {
        private static long value;

        public static void Write(long nextValue)
        {
            Interlocked.Exchange(ref value, nextValue);
        }

        public static long Read()
        {
            return Interlocked.Read(ref value);
        }
    }
}
