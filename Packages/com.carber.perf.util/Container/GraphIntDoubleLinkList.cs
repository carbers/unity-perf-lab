using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Perf.Util
{
    public class GraphIntDoubleLinkList<TValue>
    {
        private readonly SimpleIntDictionary<int> m_dict;
        private readonly SimpleList<int> m_head;
        private readonly SimpleList<int> m_tail;
        private readonly SimpleList<int> m_prev;
        private readonly SimpleList<int> m_nxt;
        private readonly SimpleList<TValue> m_value;
        private readonly Queue<int> m_freeValueIDQueue;
        private readonly Queue<int> m_freeKeyIDQueue;
        private int m_usedValueNodeCount;
        private int m_usedKeyIDNodeCount;

        public GraphIntDoubleLinkList() : this(0, 0)
        {
        }

        public GraphIntDoubleLinkList(int V, int E)
        {
            m_dict = new SimpleIntDictionary<int>();
            m_head = new SimpleList<int>(V);
            m_tail = new SimpleList<int>(V);
            m_nxt = new SimpleList<int>(E);
            m_prev = new SimpleList<int>(E);
            m_freeValueIDQueue = new Queue<int>();
            m_freeKeyIDQueue = new Queue<int>();
            m_value = new SimpleList<TValue>(E);
            m_usedValueNodeCount = 0;
            m_usedKeyIDNodeCount = 0;
            for (int i = 0; i < V; ++i)
            {
                m_head.Add(-1);
                m_tail.Add(-1);
            }
        }

        public bool AddValues(int key, List<TValue> values)
        {
            int hid = InternalAddKey(key);

            for (int i = 0, count = values.Count; i < count; ++i)
            {
                int nRetCode = InternalAddValue(hid, values[i]);
                if (nRetCode == -1)
                {
                    return false;
                }
            }

            return true;
        }

        public int Add(int key, TValue value)
        {
            int hid = InternalAddKey(key);
            return InternalAddValue(hid, value);
        }

        public int AddLast(int key, TValue value)
        {
            int hid = InternalAddKey(key);
            if (hid < 0 || hid >= m_tail.size)
            {
                return -1;
            }

            if (m_tail.buffer[hid] == -1)
            {
                return InternalAddValue(hid, value);
            }

            return InternalAddValueByID(hid, m_tail.buffer[hid], value);
        }

        public bool RemoveKey(int key)
        {
            int hid = GetKeyID(key);
            if (hid == -1)
            {
                return false;
            }

            if (!ClearValues(key))
            {
                return false;
            }

            m_dict.Remove(key);
            m_freeKeyIDQueue.Enqueue(hid);
            m_head.buffer[hid] = -1;
            m_tail.buffer[hid] = -1;

            return true;
        }

        public bool ClearValues(int key)
        {
            int hid = GetKeyID(key);
            for (int vid = GetFirstValueID(key); vid != -1;)
            {
                int nid = GetNextValueID(vid);
                if (!RemoveValueByID(hid, vid))
                {
                    return false;
                }

                vid = nid;
            }

            return true;
        }

        public bool RemoveValueByID(int hid, int vid)
        {
            if (hid < 0 || hid >= m_head.size)
            {
                return false;
            }

            if (vid < 0 || vid >= m_nxt.size)
            {
                return false;
            }

            if (vid == m_tail.buffer[hid])
            {
                m_tail.buffer[hid] = m_prev.buffer[vid];
            }

            if (m_prev.buffer[vid] != -1)
            {
                m_nxt.buffer[m_prev.buffer[vid]] = m_nxt.buffer[vid];
            }

            if (m_nxt.buffer[vid] != -1)
            {
                m_prev.buffer[m_nxt.buffer[vid]] = m_prev.buffer[vid];
            }

            if (m_head.buffer[hid] == vid)
            {
                m_head.buffer[hid] = m_nxt.buffer[vid];
            }

            m_prev.buffer[vid] = -1;
            m_nxt.buffer[vid] = -1;
            m_value.buffer[vid] = default;
            m_freeValueIDQueue.Enqueue(vid);

            return true;
        }

        public bool UpdateValueByID(int vid, TValue value)
        {
            if (vid < 0 || vid >= m_nxt.size)
            {
                return false;
            }

            m_value.buffer[vid] = value;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetKeyID(int key)
        {
            return m_dict.GetValueOrDefault(key, -1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetFirstValueID(int key)
        {
            int hid = GetKeyID(key);
            if (hid >= 0 && hid < m_head.size)
            {
                return m_head.buffer[hid];
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLastValueID(int key)
        {
            int hid = GetKeyID(key);
            if (hid >= 0 && hid < m_head.size)
            {
                return m_tail.buffer[hid];
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetNextValueID(int vid)
        {
            if (vid >= 0 && vid < m_nxt.size)
            {
                return m_nxt.buffer[vid];
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetValueByID(int vid, out TValue retValue)
        {
            if (vid >= 0 && vid < m_value.size)
            {
                retValue = m_value.buffer[vid];
                return true;
            }

            retValue = default(TValue);
            return false;
        }

        private int InternalAddKey(int key)
        {
            if (!m_dict.TryGetValue(key, out var hid))
            {
                if (!m_freeKeyIDQueue.TryDequeue(out hid))
                {
                    hid = m_usedKeyIDNodeCount;
                    m_usedKeyIDNodeCount += 1;
                }

                m_dict.Add(key, hid);
            }

            if (hid >= m_head.size)
            {
                m_head.Add(-1);
                m_tail.Add(-1);
            }

            return hid;
        }

        private int InternalAddValue(int hid, TValue value)
        {
            if (hid < 0 || hid >= m_head.size)
            {
                return -1;
            }

            if (!m_freeValueIDQueue.TryDequeue(out var vid))
            {
                vid = m_usedValueNodeCount;
                m_usedValueNodeCount += 1;
            }

            if (vid < m_nxt.size)
            {
                m_nxt.buffer[vid] = m_head.buffer[hid];
                m_prev.buffer[vid] = -1;
                m_value.buffer[vid] = value;
            }
            else
            {
                m_nxt.Add(m_head.buffer[hid]);
                m_prev.Add(-1);
                m_value.Add(value);
            }

            if (m_head.buffer[hid] != -1)
            {
                m_prev.buffer[vid] = m_prev.buffer[m_head.buffer[hid]];
                m_prev.buffer[m_head.buffer[hid]] = vid;
            }

            m_head.buffer[hid] = vid;

            if (m_nxt.buffer[vid] == -1)
            {
                m_tail.buffer[hid] = vid;
            }

            return vid;
        }

        private int InternalAddValueByID(int hid, int lastVid, TValue value)
        {
            if (hid < 0 || hid >= m_head.size)
            {
                return -1;
            }

            if (lastVid < 0 || lastVid >= m_nxt.size)
            {
                return -1;
            }

            if (!m_freeValueIDQueue.TryDequeue(out var vid))
            {
                vid = m_usedValueNodeCount;
                m_usedValueNodeCount += 1;
            }

            if (vid < m_nxt.size)
            {
                m_nxt.buffer[vid] = -1;
                m_prev.buffer[vid] = lastVid;
                m_value.buffer[vid] = value;
            }
            else
            {
                m_nxt.Add(-1);
                m_prev.Add(lastVid);
                m_value.Add(value);
            }

            if (m_nxt.buffer[lastVid] != -1)
            {
                m_nxt.buffer[vid] = m_nxt.buffer[lastVid];
                m_prev.buffer[m_nxt.buffer[lastVid]] = vid;
            }

            m_nxt.buffer[lastVid] = vid;

            if (m_nxt.buffer[vid] == -1)
            {
                m_tail.buffer[hid] = vid;
            }

            return vid;
        }
    }
}
