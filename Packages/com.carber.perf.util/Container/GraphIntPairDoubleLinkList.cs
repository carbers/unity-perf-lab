using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Perf.Util
{
    public class GraphIntPairDoubleLinkList<TValue>
    {
        private readonly SimpleIntPairDictionary<(int, int, int)> m_dict;
        private readonly SimpleList<int> m_head;
        private readonly SimpleList<int> m_tail;
        private readonly SimpleList<int> m_prev;
        private readonly SimpleList<int> m_nxt;
        private readonly SimpleList<(int, int, TValue)> m_value;
        private readonly Queue<int> m_freeValueIDQueue;
        private readonly Queue<int> m_freeKeyIDQueue;
        private int m_usedValueNodeCount;
        private int m_usedKeyIDNodeCount;

        private readonly SimpleList<(int, int)> m_debugValue;


        public GraphIntPairDoubleLinkList() : this(0, 0)
        {
        }

        public GraphIntPairDoubleLinkList(int V, int E)
        {
            m_dict = new SimpleIntPairDictionary<(int, int, int)>();
            m_head = new SimpleList<int>(V);
            m_tail = new SimpleList<int>(V);
            m_nxt = new SimpleList<int>(E);
            m_prev = new SimpleList<int>(E);
            m_freeValueIDQueue = new Queue<int>();
            m_freeKeyIDQueue = new Queue<int>();
            m_value = new SimpleList<(int, int, TValue)>(E);
            m_usedValueNodeCount = 0;
            m_usedKeyIDNodeCount = 0;
            for (int i = 0; i < V; ++i)
            {
                m_head.Add(-1);
                m_tail.Add(-1);
            }
        }

        public int AddLast((int,int) key, TValue value)
        {
            var (hid, patchId, keyId) = InternalAddKey(key);
            if (hid < 0 || hid >= m_tail.size)
            {
                return -1;
            }

            if (patchId != key.Item1 || keyId != key.Item2)
            {
                throw new Exception("[GraphIntPairDoubleLinkList] Invalid key, data maybe modify by other");
            }

            if (m_tail.buffer[hid] == -1)
            {
                return InternalAddValue(key, hid, value);
            }
            return InternalAddValueByID(key, hid, m_tail.buffer[hid], value);
        }

        public bool RemoveKey((int,int) key)
        {
            var (hid, patchId, keyId) = GetKeyID(key);
            if (hid == -1)
            {
                return false;
            }

            if (patchId != key.Item1 || keyId != key.Item2)
            {
                throw new Exception("[GraphIntPairDoubleLinkList] Invalid key, data maybe modify by other");
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

        public bool ClearValues((int,int) key)
        {
            var (hid, patchId, keyId) = GetKeyID(key);

            if (hid != -1)
            {
                if (patchId != key.Item1 || keyId != key.Item2)
                {
                    throw new Exception("[GraphIntPairDoubleLinkList] Invalid key, data maybe modify by other");
                }
            }

            for (int vid = GetFirstValueID(key); vid != -1; )
            {
                int nid = GetNextValueID(vid);
                if (!RemoveValueByID(key, vid))
                {
                    Debug.LogError($"[GraphicsLinkList] Remove Value By ID ({hid}-{vid}) failed");
                    return false;
                }
                vid = nid;
            }

            return true;
        }

        public bool RemoveValueByID((int,int) key, int vid)
        {
            int hid = GetKeyID(key).Item1;
            if (hid < 0 || hid >= m_head.size)
            {
                return false;
            }

            if (vid < 0 || vid >= m_nxt.size)
            {
                return false;
            }

            if (m_value.buffer[vid].Item1 == -1 && m_value.buffer[vid].Item2 == -1)
            {
                return false;
            }

            CheckNodeValid(key, hid, vid);
            CheckNodeValid(key, hid, m_prev.buffer[vid]);
            CheckNodeValid(key, hid, m_nxt.buffer[vid]);

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

            CheckNodeValid(key, hid, m_prev.buffer[vid]);
            CheckNodeValid(key, hid, m_nxt.buffer[vid]);
            CheckNodeValid(key, hid, m_head.buffer[hid]);
            CheckNodeValid(key, hid, m_tail.buffer[hid]);

            m_prev.buffer[vid] = -1;
            m_nxt.buffer[vid] = -1;
            m_value.buffer[vid] = (-1, -1, default);
            m_freeValueIDQueue.Enqueue(vid);

            return true;
        }

        public bool UpdateValueByID(int vid, TValue value)
        {
            if (vid < 0 || vid >= m_nxt.size)
            {
                return false;
            }

            m_value.buffer[vid] = (m_value.buffer[vid].Item1, m_value.buffer[vid].Item2, value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int, int, int) GetKeyID((int,int) key)
        {
            var data =  m_dict.GetValueOrDefault(key, (-1, -1, -1));
            if (data.Item1 != -1)
            {
                if (data.Item2 != key.Item1 || data.Item3 != key.Item2)
                {
                    throw new Exception("[GraphIntPairDoubleLinkList] Invalid key, data maybe modify by other");
                }
            }

            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetFirstValueID((int,int) key)
        {
            var (hid, patchId, keyId) = GetKeyID(key);
            if (hid >= 0 && hid < m_head.size)
            {
                if (patchId != key.Item1 || keyId != key.Item2)
                {
                    throw new Exception("[GraphIntPairDoubleLinkList] Invalid key, data maybe modify by other");
                }

                return m_head.buffer[hid];
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLastValueID((int,int) key)
        {
            var (hid, patchId, keyId) = GetKeyID(key);
            if (hid >= 0 && hid < m_head.size)
            {
                if (patchId != key.Item1 || keyId != key.Item2)
                {
                    throw new Exception("[GraphIntPairDoubleLinkList] Invalid key, data maybe modify by other");
                }

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
        public int GetPrevValueID(int vid)
        {
            if (vid >= 0 && vid < m_prev.size)
            {
                return m_prev.buffer[vid];
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetValueByID(int vid, out TValue retValue)
        {
            if (vid >= 0 && vid < m_value.size)
            {
                retValue = m_value.buffer[vid].Item3;
                return true;
            }

            retValue = default(TValue);
            return false;
        }

        private (int, int, int) InternalAddKey((int,int) key)
        {
            if (!m_dict.TryGetValue(key, out var keyData))
            {
                if (!m_freeKeyIDQueue.TryDequeue(out keyData.Item1))
                {
                    keyData.Item1 = m_usedKeyIDNodeCount;
                    m_usedKeyIDNodeCount += 1;
                }

                keyData.Item2 = key.Item1;
                keyData.Item3 = key.Item2;

                m_dict.Add(key, keyData);
            }

            if (keyData.Item1 >= m_head.size)
            {
                m_head.Add(-1);
                m_tail.Add(-1);
            }

            return GetKeyID(key);
        }

        private bool CheckNodeValid((int, int) key, int hid, int vid)
        {
#if !FORMAL_RELEASE
            if (vid >= 0 && vid < m_value.size)
            {
                if (m_value.buffer[vid].Item1 != key.Item1 || m_value.buffer[vid].Item2 != key.Item2)
                {
                    Debug.LogException(new Exception($"[GraphIntPairDoubleLinkList] Invalid node: key={key}, hid={hid}, vid={vid}, m_value={m_value.buffer[vid]}."));
#if !FORMAL_RELEASE && !UNITY_EDITOR
                    UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.FatalError);
#endif
                }
            }
#endif
            return true;
        }

        private int InternalAddValue((int, int) key, int hid, TValue value)
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

            CheckNodeValid(key, hid, m_head.buffer[hid]);
            if (vid < m_nxt.size)
            {
                m_nxt.buffer[vid] = m_head.buffer[hid];
                m_prev.buffer[vid] = -1;
                m_value.buffer[vid] = (key.Item1, key.Item2, value);
            }
            else
            {
                m_nxt.Add(m_head.buffer[hid]);
                m_prev.Add(-1);
                m_value.Add((key.Item1, key.Item2, value));
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
            CheckNodeValid(key, hid, m_tail.buffer[hid]);
            CheckNodeValid(key, hid, vid);

            return vid;
        }

        private int InternalAddValueByID((int, int) key, int hid, int lastVid, TValue value)
        {
            if (hid < 0 || hid >= m_head.size)
            {
                return -1;
            }

            if (lastVid < 0 || lastVid >= m_nxt.size)
            {
                return -1;
            }

            CheckNodeValid(key, hid, lastVid);

            if (!m_freeValueIDQueue.TryDequeue(out var vid))
            {
                vid = m_usedValueNodeCount;
                m_usedValueNodeCount += 1;
            }

            if (vid < m_nxt.size)
            {
                m_nxt.buffer[vid] = -1;
                m_prev.buffer[vid] = lastVid;
                m_value.buffer[vid] = (key.Item1, key.Item2, value);
            }
            else
            {
                m_nxt.Add(-1);
                m_prev.Add(lastVid);
                m_value.Add((key.Item1, key.Item2, value));
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

            CheckNodeValid(key, hid, vid);

            return vid;
        }
    }
}
