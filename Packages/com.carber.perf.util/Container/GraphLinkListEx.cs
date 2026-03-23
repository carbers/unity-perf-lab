using System;
using System.Collections.Generic;
using UnityEngine;

namespace Perf.Util
{
    public class GraphLinkListEx<TKey, TValue>
    {
        private readonly Dictionary<TKey, SimpleLinkList<TValue>> m_dictionary = new();
        private EqualityComparer<TValue> m_comparer = EqualityComparer<TValue>.Default;

        public int AddLast(TKey key, TValue value)
        {
            if (m_dictionary.TryGetValue(key, out var linkList))
            {
                return linkList.AddLast(value);
            }

            linkList = new SimpleLinkList<TValue>();
            if (!m_dictionary.TryAdd(key, linkList))
            {
                return -1;
            }

            return linkList.AddLast(value);
        }

        public bool RemoveKey(TKey key)
        {
            return m_dictionary.Remove(key);
        }

        public bool CompareAndChange(TKey key, int vid, TValue oldValue, TValue newValue)
        {
            var linkList = GetValues(key);
            if (linkList == null)
            {
                return false;
            }

            var value = linkList.GetValue(vid);
            if (!m_comparer.Equals(value, oldValue))
            {
                Debug.LogError("[GraphLinkListEx] Comparing values do not match.");
                return false;
            }
            return linkList.SetValue(vid, newValue);
        }

        public bool RemoveAtWithCompare(TKey key, int vid, TValue oldValue)
        {
            var linkList = GetValues(key);
            if (linkList == null)
            {
                return false;
            }

            var value = linkList.GetValue(vid);
            if (!m_comparer.Equals(value, oldValue))
            {
                Debug.LogError("[GraphLinkListEx] Comparing values do not match.");
                return false;
            }

            linkList.RemoveAt(vid);
            return true;
        }

        public bool TryGetValue(TKey key, int vid, out TValue value)
        {
            var linkList = GetValues(key);
            if (linkList == null)
            {
                value = default;
                return false;
            }

            return linkList.TryGetValue(vid, out value);
        }

        public SimpleLinkList<TValue> GetValues(TKey key, bool createIfNotExists = false)
        {
            if (m_dictionary.TryGetValue(key, out var linkList))
            {
                return linkList;
            }

            if (!createIfNotExists)
            {
                return null;
            }

            linkList = new SimpleLinkList<TValue>();
            if (m_dictionary.TryAdd(key, linkList))
            {
                return linkList;
            }

            return null;
        }

        public bool Traverse(TKey key, Action<TValue> action)
        {
            if (action == null)
            {
                return false;
            }

            var linkList = GetValues(key);
            if (linkList == null)
            {
                return true;
            }

            for (int cur = linkList.First; cur != CollectionConst.INVALID_HEAD; cur = linkList.Next(cur))
            {
                var value = linkList.GetValue(cur);
                action.Invoke(value);
            }

            return true;
        }
    }
}
