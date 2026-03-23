using System;
using System.Collections;
using System.Collections.Generic;

namespace Perf.Util
{
    public class SimpleStringDictionary<TValue>:IDisposable
    {
    public SimpleStringDictionary() : this(11)
    {
    }

    public SimpleStringDictionary(int capacity)
    {
        _Reverse(capacity);
    }

    public int Count
    {
        get { return m_count; }
    }

    public TValue this[string key]
    {
        get
        {
            TValue value;
            if (!TryGetValue(key, out value))
            {
                throw new Exception("[SDicionary] read undefine value.");
            }

            return value;
        }
        set { Add(key, value); }
    }

    public void Add(string key, TValue value)
    {
        int hash = key.GetHashCode() & 0x7FFFFFFF;
        int slot = _FindSlot(hash, key);
        if (slot == -1)
        {
            if (m_count >= m_threshold)
            {
                _Reverse(m_count * 2);
            }

            _AddNoCheck(hash, key, value, m_count);
            ++m_count;
        }
        else
        {
            m_entryValue[slot] = value;
        }
    }

    public void Clear()
    {
        m_count = 0;
        for (int i = 0; i < m_buckets.Length; ++i)
        {
            m_buckets[i] = -1;
            m_entryHash[i] = -1;
        }

        Array.Clear(m_entryKey, 0, m_entryKey.Length);
        Array.Clear(m_entryValue, 0, m_entryValue.Length);
    }

    public bool ContainsKey(string key)
    {
        int hash = key.GetHashCode() & 0x7FFFFFFF;
        return _FindSlot(hash, key) != -1;
    }

    public bool Remove(string key)
    {
        int hash = key.GetHashCode() & 0x7FFFFFFF;
        return _Remove(hash, key);
    }

    public bool TryGetValue(string key, out TValue value)
    {
        int hash = key.GetHashCode() & 0x7FFFFFFF;
        int slot = _FindSlot(hash, key);
        if (slot != -1)
        {
            value = m_entryValue[slot];
            return true;
        }

        value = default(TValue);
        return false;
    }

    public bool TryAdd(string key, TValue value)
    {
        int hash = key.GetHashCode() & 0x7FFFFFFF;
        int slot = _FindSlot(hash, key);
        if (slot == -1)
        {
            if (m_count >= m_threshold)
            {
                _Reverse(m_count * 2);
            }

            _AddNoCheck(hash, key, value, m_count);
            ++m_count;
            return true;
        }
        else
        {
            return false;
        }
    }

    public TValue GetValueOrDefault(string key, TValue defaultValue = default(TValue))
    {
        int hash = key.GetHashCode() & 0x7FFFFFFF;
        int slot = _FindSlot(hash, key);
        if (slot != -1)
        {
            return m_entryValue[slot];
        }

        return defaultValue;
    }

    public SimpleStringDictionary<TValue>.Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    [Serializable]
    public struct Enumerator : IEnumerator, IDisposable, IEnumerator<KeyValuePair<string, TValue>>
    {
        public KeyValuePair<string, TValue> Current
        {
            get
            {
                return m_current;

            }
        }
        public void Dispose()
        {
            m_dict = null;
            m_current = new KeyValuePair<string, TValue>();
            m_index = 0;
        }

        public bool MoveNext()
        {
            if (m_index < m_dict.m_count)
            {
                m_current = new KeyValuePair<string, TValue>(m_dict.m_entryKey[m_index], m_dict.m_entryValue[m_index]);
                m_index++;
                return true;
            }

            m_index = m_dict.m_count + 1;
            m_current = new KeyValuePair<string, TValue>();
            return false;
        }

        object IEnumerator.Current
        {
            get { return new KeyValuePair<string, TValue>(m_current.Key, m_current.Value); }
        }

        void IEnumerator.Reset()
        {
            m_index = 0;
            m_current = new KeyValuePair<string, TValue>();
        }

        internal Enumerator(SimpleStringDictionary<TValue> dict)
        {
            m_dict = dict;
            m_current = new KeyValuePair<string, TValue>();
            m_index = 0;
        }

        private int m_index;
        private SimpleStringDictionary<TValue> m_dict;
        private KeyValuePair<string, TValue> m_current;
    }
    public string[] Keys
    {
        get
        {
            var keys = new string[m_count];
            for (var i = 0; i < m_count; i++)
                keys[i] = m_entryKey[i];
            return keys;
        }
    }

    public TValue[] Values
    {
        get
        {
            var values = new TValue[m_count];
            for (var i = 0; i < m_count; i++)
                values[i] = m_entryValue[i];
            return values;
        }
    }
    private void _Reverse(int size)
    {
        size = SimpleHelper.FindLastPrimes(size);

        int[] tempBuckets = m_buckets;
        int[] tempEntryNext = m_entryNext;
        int[] tempEntryHash = m_entryHash;
        string[] tempEntrykey = m_entryKey;
        TValue[] tempEntryValue = m_entryValue;

        m_mod = size;
        m_threshold = (int)(size * 0.9) + 1;
        if (m_threshold >= size)
        {
            m_threshold = size;
        }

        m_buckets = new int[size];
        m_entryNext = new int[size];
        m_entryHash = new int[size];
        m_entryKey = new string[size];
        m_entryValue = new TValue[size];
        Clear();

        if (tempBuckets == null)
        {
            return;
        }

        for (int i = 0; i < tempBuckets.Length; ++i)
        {
            for (int h = tempBuckets[i]; h != -1; h = tempEntryNext[h])
            {
                _AddNoCheck(tempEntryHash[h], tempEntrykey[h], tempEntryValue[h], m_count);
                ++m_count;
            }
        }
    }

    private void _AddNoCheck(int hash, string key, TValue value, int pos)
    {
        int slot = (hash & 0x7FFFFFFF) % m_mod;
        m_entryHash[pos] = hash;
        m_entryKey[pos] = key;
        m_entryValue[pos] = value;
        m_entryNext[pos] = m_buckets[slot];
        m_buckets[slot] = pos;
    }

    private int _FindSlot(int hash, string key)
    {
        int slot = hash % m_mod;

        for (int h = m_buckets[slot]; h != -1; h = m_entryNext[h])
        {
            if (key == m_entryKey[h])
            {
                return h;
            }
        }

        return -1;
    }

    private bool _Remove(int hash, string key)
    {
        int slot = hash % m_mod;
        int dstSlot = _FindSlot(hash, key);

        if (dstSlot == -1) return false;

        SimpleHelper.RemoveSlot(m_buckets, m_entryNext, slot, dstSlot);
        --m_count;

        if (m_count > 0 && dstSlot != m_count)
        {
            int lastSlot = m_entryHash[m_count] % m_mod;
            SimpleHelper.RemoveSlot(m_buckets, m_entryNext, lastSlot, m_count);
            _AddNoCheck(m_entryHash[m_count], m_entryKey[m_count], m_entryValue[m_count], dstSlot);
        }

        m_entryKey[m_count] = default(string);
        m_entryValue[m_count] = default(TValue);
        m_entryHash[m_count] = -1;

        return true;
    }

    public void Dispose()
    {
        m_entryKey = null;
        m_entryValue = null;
        m_buckets = null;
        m_entryNext = null;
        m_entryHash = null;
        m_threshold = 0;
        m_count = 0;
        m_mod = 0;
    }

    private int m_threshold = 0;
    private int m_count = 0;
    private int m_mod = 0;
    private int[] m_buckets = null;
    private int[] m_entryNext = null;
    private int[] m_entryHash = null;
    private string[] m_entryKey = null;
    private TValue[] m_entryValue = null;
    }
}
