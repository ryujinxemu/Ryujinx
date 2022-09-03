using System;
using System.Collections.Generic;

namespace Ryujinx.Graphics.Vulkan
{
    interface ICacheKey
    {
        bool KeyEqual(ICacheKey other);
    }

    struct I8ToI16CacheKey : ICacheKey
    {
        public bool KeyEqual(ICacheKey other)
        {
            return other is I8ToI16CacheKey;
        }
    }

    struct AlignedVertexBufferCacheKey : ICacheKey
    {
        private int _stride;
        private int _alignment;

        public AlignedVertexBufferCacheKey(int stride, int alignment)
        {
            _stride = stride;
            _alignment = alignment;
        }

        public bool KeyEqual(ICacheKey other)
        {
            return other is AlignedVertexBufferCacheKey entry &&
                entry._stride == _stride &&
                entry._alignment == _alignment;
        }
    }

    struct CacheByRange<T> where T : IDisposable
    {
        private struct Entry<T> where T : IDisposable
        {
            public ICacheKey Key;
            public T Value;

            public Entry(ICacheKey key, T value)
            {
                Key = key;
                Value = value;
            }
        }

        private Dictionary<ulong, List<Entry<T>>> _ranges;

        public void Add(int offset, int size, ICacheKey key, T value)
        {
            List<Entry<T>> entries = GetEntries(offset, size);

            entries.Add(new Entry<T>(key, value));
        }

        public bool TryGetValue(int offset, int size, ICacheKey key, out T value)
        {
            List<Entry<T>> entries = GetEntries(offset, size);

            foreach (Entry<T> entry in entries)
            {
                if (entry.Key.KeyEqual(key))
                {
                    value = entry.Value;

                    return true;
                }
            }

            value = default;
            return false;
        }

        public void Clear()
        {
            if (_ranges != null)
            {
                foreach (List<Entry<T>> entries in _ranges.Values)
                {
                    foreach (Entry<T> entry in entries)
                    {
                        entry.Value.Dispose();
                    }
                }

                _ranges.Clear();
                _ranges = null;
            }
        }

        private List<Entry<T>> GetEntries(int offset, int size)
        {
            if (_ranges == null)
            {
                _ranges = new Dictionary<ulong, List<Entry<T>>>();
            }

            ulong key = PackRange(offset, size);

            List<Entry<T>> value;
            if (!_ranges.TryGetValue(key, out value))
            {
                value = new List<Entry<T>>();
                _ranges.Add(key, value);
            }

            return value;
        }

        private static ulong PackRange(int offset, int size)
        {
            return (uint)offset | ((ulong)size << 32);
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
