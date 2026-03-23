using System;
using System.Runtime.CompilerServices;

namespace Perf.Util
{

    public static class CollectionConst
    {
        public const int INVALID_HEAD = -1;
    }


    public class SimpleLinkList<T>
    {
        private const int INVALID_HEAD = CollectionConst.INVALID_HEAD;

        private const int DefaultBufferSize = 32;

        /// <summary>
        /// Direct access to the buffer's size. Note that it's only public for speed and efficiency. You shouldn't modify it.
        /// </summary>

        public int size;

        /// <summary>
        /// Direct access to the buffer. Note that you should not use its 'Length' parameter, but instead use BetterList.size.
        /// </summary>
        public SimpleList<T> value = null;

        private SimpleList<int> m_freeList = null;
        private SimpleList<int> m_prev = null;
        private SimpleList<int> m_next = null;
        private SimpleList<bool> m_isValidList = null;

        public int First
        {
            get { return m_next.buffer[0] == 0 ? INVALID_HEAD : m_next.buffer[0]; }
        }

        public int Last
        {
            get { return m_prev.buffer[0] == 0 ? INVALID_HEAD : m_prev.buffer[0]; }
        }

        public T FirstValue
        {
            get { return m_next.buffer[0] == 0 ? default : value.buffer[m_next.buffer[0]]; }
        }

        public T LastValue
        {
            get { return m_prev.buffer[0] == 0 ? default : value.buffer[m_prev.buffer[0]]; }
        }

        public int Count
        {
            get { return size; }
        }

        public SimpleLinkList(int capacity = DefaultBufferSize)
        {
            m_prev = new SimpleList<int>(capacity);
            m_next = new SimpleList<int>(capacity);
            m_isValidList = new SimpleList<bool>(capacity);
            value = new SimpleList<T>(capacity);
            m_freeList = new SimpleList<int>(8);

            _RequestFirstNew(default);
            size = 0;
        }

        public int AddLast(T value)
        {
            int nid = _RequestFirstNew(value);
            _InsertAfter(m_prev.buffer[0], nid);
            return nid;
        }

        public int AddFirst(T value)
        {
            int nid = _RequestFirstNew(value);
            _InsertAfter(0, nid);
            return nid;
        }

        public int AddBefore(T value, int index)
        {
            //Add Last Node
            if (index < 0 || index >= this.value.size)
                index = 0;

            int nid = _RequestFirstNew(value);
            _InsertBefore(index, nid);
            return nid;
        }

        public int AddAfter(T value, int index)
        {
            //Add First Node
            if (index < 0 || index >= this.value.size)
                index = 0;

            int nid = _RequestFirstNew(value);
            _InsertAfter(index, nid);
            return nid;
        }

        public T PopLast()
        {
            if (value.size == 0 || m_prev.buffer[0] == 0)
                return default;

            var res = value.buffer[m_prev.buffer[0]];
            RemoveAt(m_prev.buffer[0]);

            return res;
        }

        public T PopFirst()
        {
            if (value.size == 0 || m_next.buffer[0] == 0)
                return default;

            var res = value.buffer[m_next.buffer[0]];
            RemoveAt(m_next.buffer[0]);

            return res;
        }

        public bool Contains(T value)
        {
            for (int h = m_next.buffer[0]; h != 0; h = m_next.buffer[h])
            {
                if (value.Equals(this.value.buffer[h])) return true;
            }
            return false;
        }

        private int IndexOf(T value)
        {
            for (int h = m_next.buffer[0]; h != 0; h = m_next.buffer[h])
            {
                if (value.Equals(this.value.buffer[h])) return h;
            }
            return INVALID_HEAD;
        }

        public int Remove(T value)
        {
            if (this.value.size == 0 || value == null)
                return INVALID_HEAD;

            for (int h = m_next.buffer[0]; h != 0; h = m_next.buffer[h])
            {
                if (value.Equals(this.value.buffer[h])) return RemoveAt(h);
            }

            return INVALID_HEAD;
        }

        public int RemoveAt(int id)
        {
            if (id == INVALID_HEAD || id >= value.size)
            {
                return INVALID_HEAD;
            }

            int idNext = m_next.buffer[id];
            m_next.buffer[m_prev.buffer[id]] = idNext;
            m_prev.buffer[idNext] = m_prev.buffer[id];
            //m_next.buffer[id] = m_prev.buffer[id] = -id;
            m_isValidList.buffer[id] = false;
            value.buffer[id] = default;
            m_freeList.Add(id);
            --size;
            return idNext == 0 ? INVALID_HEAD : idNext;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetValue(int id)
        {
            return id == INVALID_HEAD || id >= value.size || id < 0 ? default : value.buffer[id];
        }

        public bool TryGetValue(int id, out T retValue)
        {
            if (id == INVALID_HEAD || id >= this.value.size || id < 0)
            {
                retValue = default;
                return false;
            }

            retValue = value.buffer[id];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetValue(int id, T tValue)
        {
            if (id == INVALID_HEAD || id >= this.value.size || id < 0)
            {
                return false;
            }

            value.buffer[id] = tValue;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next(int id)
        {
            while (true)
            {
                id = NextStep(id);
                if (id == INVALID_HEAD || m_isValidList.buffer[id])
                {
                    return id;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int NextStep(int id)
        {
            return id == INVALID_HEAD || id >= m_next.size || m_next.buffer[id] == 0 ? INVALID_HEAD : m_next.buffer[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Prev(int id)
        {
            while (true)
            {
                id = PrevStep(id);
                if (id == INVALID_HEAD || m_isValidList.buffer[id])
                {
                    return id;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int PrevStep(int id)
        {
            return id == INVALID_HEAD || id < 0 || m_prev.buffer[id] == 0 ? INVALID_HEAD : m_prev.buffer[id];
        }

        public void Clear()
        {
            if (size == 0)
                return;

            int id = m_next.buffer[0];
            while (id != 0)
            {
                value.buffer[id] = default;
                m_isValidList.buffer[id] = false;
                m_freeList.Add(id);
                id = m_next.buffer[id];
            }

            m_next.buffer[0] = 0;
            m_prev.buffer[0] = 0;
            size = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _InsertBefore(int oid, int nid)
        {
            m_next.buffer[nid] = oid;
            m_prev.buffer[nid] = m_prev.buffer[oid];
            m_prev.buffer[oid] = nid;
            m_next.buffer[m_prev.buffer[nid]] = nid;
            ++size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _InsertAfter(int oid, int nid)
        {
            m_prev.buffer[nid] = oid;
            m_next.buffer[nid] = m_next.buffer[oid];
            m_next.buffer[oid] = nid;
            m_prev.buffer[m_next.buffer[nid]] = nid;
            ++size;
        }

        private int _RequestFirstNew(T valueNew)
        {
            int id = INVALID_HEAD;
            if (m_freeList.size > 0)
            {
                id = m_freeList.PopBack();
                m_next.buffer[id] = id;
                m_prev.buffer[id] = id;
                this.value.buffer[id] = valueNew;
                m_isValidList.buffer[id] = true;

                return id;
            }

            id = this.value.size;
            m_next.Add(id);
            m_prev.Add(id);
            this.value.Add(valueNew);
            m_isValidList.Add(true);

            return id;
        }
    }

}
