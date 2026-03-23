using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Perf.Util
{
    public class SimpleList<T>
    {
        private const int DefaultBufferSize = 8;
        private const int DefaultMaxGrowSize = 1024 * 16;

        /// <summary>
        /// Direct access to the buffer. Note that you should not use its 'Length' parameter, but instead use BetterList.size.
        /// </summary>
        public T[] buffer = null;

        /// <summary>
        /// Direct access to the buffer's size. Note that it's only public for speed and efficiency. You shouldn't modify it.
        /// </summary>
        public int size = 0;

        //记录当前类型是否是object类型
        bool _isClass = typeof(T).IsClass;

        //当前类型的共享缓存池
        ArrayPool<T> _sharedPool = ArrayPool<T>.Shared;

        bool _bUsePool = false;

        public SimpleList(bool usePool = false)
        {
            _bUsePool = usePool;
        }

        public SimpleList(int capacity, bool usePool = false)
        {
            _bUsePool = usePool;
            Allocate(capacity);
            size = 0;
        }

        ~SimpleList()
        {
            Clear();
        }

        //public int Capacity { get; set; }
        public int Count
        {
            get { return size; }
        }

        [System.Obsolete("Access the list.buffer[index] instead -- direct array access avoids a copy, so it can be much faster")]
        public T this[int index]
        {
            get { return buffer[index]; }
            set { buffer[index] = value; }
        }

        public void Add(T item)
        {
            if (buffer == null || size == buffer.Length)
            {
                Allocate();
            }

            buffer[size++] = item;
        }

        public void Insert(int index, T item)
        {
            if (buffer == null || size == buffer.Length) Allocate();

            if (index < size)
            {
                Shift(index, 1);
                buffer[index] = item;
            }
            else Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PopBack()
        {
            if (size == 0) return default;

            T res = buffer[--size];
            buffer[size] = default;
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PeekBack()
        {
            return size == 0 ? default : buffer[size - 1];
        }

        public bool Remove(T item)
        {
            int loc = IndexOf(item);
            if (loc != -1) Shift(loc, -1);
            return loc != -1;
        }

        public void RemoveAt(int index)
        {
            if (buffer == null || index < 0 || index >= size)
            {
                return;
            }

            Shift(index, -1);
        }

        public void RemoveRange(int index, int length)
        {
            if (index < 0 || length <= 0 || index >= size)
                return;

            if (index + length > size)
                length = size - index;

            Shift(index, -length);
        }

        public int RemoveAll(Predicate<T> match)
        {
            if (buffer == null || match == null)
            {
                return 0;
            }

            int removedCount = 0;

            for (int i = 0; i < size; i++)
            {
                if (match(buffer[i]))
                {
                    // Element should be removed.
                    removedCount++;
                    continue;
                }

                if (removedCount > 0)
                {
                    // Element should not be removed. Reposition it instead.
                    buffer[i - removedCount] = buffer[i];
                }
            }

            if (removedCount > 0)
            {
                // Clear last 'removedCount' elements
                Array.Clear(buffer, size - removedCount, removedCount);
                size -= removedCount;
            }

            return removedCount;
        }

        public bool Contains(T item)
        {
            if (buffer == null || size <= 0) return false;
            return Array.IndexOf(buffer, item, 0, size) >= 0;
        }

        public int IndexOf(T item)
        {
            if (buffer == null || size <= 0) return -1;
            return Array.IndexOf(buffer, item, 0, size);
        }

        public virtual void Clear()
        {
            if (_bUsePool && buffer != null)
            {
                _sharedPool.Return(buffer, _isClass);
                buffer = null;
            }

            size = 0;
        }

        public T[] ToArray()
        {
            if (size <= 0)
                return null;

            if (size >= buffer.Length)
                return buffer;

            T[] newList = new T[size];
            Array.Copy(buffer, newList, size);

            return newList;
        }

        /// <summary>
        /// Shift buffer data from start location by delta places left or right
        /// </summary>
        private void Shift(int start, int delta)
        {
            if (delta < 0)
            {
                start -= delta;
            }

            if (start < size)
                Array.Copy(buffer, start, buffer, start + delta, size - start);

            size += delta;

            if (delta < 0)
                Array.Clear(buffer, size, -delta);
        }

        public void Sort()
        {
            if (size > 0)
                Array.Sort(buffer, 0, size);
        }

        public void Sort(IComparer<T> compareFunc)
        {
            if (size > 0)
                Array.Sort(buffer, 0, size, compareFunc);
        }

        protected virtual void Allocate(int capacity = DefaultBufferSize)
        {
            capacity = Math.Max(capacity, DefaultBufferSize);
            if (buffer != null)
            {
                int growSize = Math.Min(buffer.Length << 1, buffer.Length + DefaultMaxGrowSize);
                capacity = Math.Max(growSize, capacity);
            }

            if (_bUsePool)
            {
                var newBuffer = _sharedPool.Rent(capacity);
                if (buffer != null)
                {
                    if (size > 0)
                        Array.Copy(buffer, newBuffer, size);

                    _sharedPool.Return(buffer, _isClass);
                }

                buffer = newBuffer;
            }
            else
            {
                Array.Resize(ref buffer, capacity);
            }
        }

        public void MakeSureCapacity(int capacity)
        {
            if (buffer != null && capacity <= buffer.Length)
                return;

            Allocate(capacity);
        }
    }

}
