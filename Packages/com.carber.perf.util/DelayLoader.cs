
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace Perf.Util
{
    public class DelayLoader
    {
        private static DelayLoader m_instance = null;
        public static DelayLoader Instance
        {
            get
            {
                if (m_instance != null)
                {
                    return m_instance;
                }
                m_instance = new DelayLoader();
                if (!m_instance.Init())
                {
                   Debug.LogErrorFormat("[ResourceSingleton] getInstance().Init() failed, type={0}",
                        m_instance.GetType().ToString()
                    );
                }
                return m_instance;
            }
        }

        public static void ReleaseInstance()
        {
            if (m_instance != null)
            {
                m_instance.UnInit();
                m_instance = null;
            }
        }
        public delegate long DelayCallDelegate(GameObject gameObject, System.Action action);

        public delegate bool CancelDelayCallDelegate(long callId);

        class DelayData
        {
            public GameObject gameObject;
            public System.Action action;
            public long callId = 0;
            public long frameIndex = 0;
        }

        private Queue<DelayData> m_queue = new Queue<DelayData>();
        // 0, queueing, 1 cancled.
        private Dictionary<long, int> m_callIdStatus = new Dictionary<long, int>();
        private long m_currentCallId = 0;
        private long m_frameIndex = 0;
        private Stopwatch m_stopwatch = new Stopwatch();

        public long timeOut = 5;

        public bool Reset()
        {
            m_stopwatch.Reset();
            m_stopwatch.Start();
            return true;
        }
        public long DelayCall(GameObject gameObject, System.Action action)
        {
            if (action == null || gameObject == null)
            {
                return -1;
            }
#if UNITY_EDITOR
            action.Invoke();
            return 0;
#endif

            ++m_currentCallId;

            DelayData delayData = new DelayData()
            {
                gameObject = gameObject,
                action = action,
                callId = m_currentCallId,
                frameIndex = m_frameIndex
            };

            m_callIdStatus.Add(m_currentCallId, 0);
            m_queue.Enqueue(delayData);

            return m_currentCallId;
        }

        public bool CancelDelayCall(long callId)
        {
            return m_callIdStatus.Remove(callId);
        }

        private bool Init()
        {
            bool bResult = false;
            bool bRetCode = false;
            bResult = true;
        Exit0:
            return bResult;
        }

        private bool UnInit()
        {
            m_instance = null;
            return true;
        }

        public void Update()
        {
            while (m_queue.Count > 0)
            {
                DelayData delayData = m_queue.Peek();


                if (delayData.frameIndex >= m_frameIndex)
                {
                    break;
                }

                m_queue.Dequeue();

                if (!m_callIdStatus.ContainsKey(delayData.callId))
                {

                    continue;
                }

                if (delayData.gameObject == null)
                {
                    continue;
                }
                Profiler.BeginSample(delayData.gameObject.name);
                m_callIdStatus.Remove(delayData.callId);
                delayData.action.Invoke();
                Profiler.EndSample();
                if (m_stopwatch.ElapsedMilliseconds > timeOut)
                {
                    break;
                }

            }

            ++m_frameIndex;
        }
    }
}
