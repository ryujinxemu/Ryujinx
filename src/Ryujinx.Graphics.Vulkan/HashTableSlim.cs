﻿using System;
using System.Collections.Generic;

namespace Ryujinx.Graphics.Vulkan
{
    interface IRefEquatable<T>
    {
        bool Equals(ref T other);
    }

    class HashTableSlim<TK, TV> where TK : IRefEquatable<TK>
    {
        private const int TotalBuckets = 16; // Must be power of 2
        private const int TotalBucketsMask = TotalBuckets - 1;

        private struct Entry
        {
            public int Hash;
            public TK Key;
            public TV Value;
        }

        private readonly Entry[][] _hashTable = new Entry[TotalBuckets][];

        public IEnumerable<TK> Keys
        {
            get
            {
                foreach (Entry[] bucket in _hashTable)
                {
                    if (bucket != null)
                    {
                        foreach (Entry entry in bucket)
                        {
                            yield return entry.Key;
                        }
                    }
                }
            }
        }

        public IEnumerable<TV> Values
        {
            get
            {
                foreach (Entry[] bucket in _hashTable)
                {
                    if (bucket != null)
                    {
                        foreach (Entry entry in bucket)
                        {
                            yield return entry.Value;
                        }
                    }
                }
            }
        }

        public void Add(ref TK key, TV value)
        {
            var entry = new Entry()
            {
                Hash = key.GetHashCode(),
                Key = key,
                Value = value
            };

            int hashCode = key.GetHashCode();
            int bucketIndex = hashCode & TotalBucketsMask;

            var bucket = _hashTable[bucketIndex];
            if (bucket != null)
            {
                int index = bucket.Length;

                Array.Resize(ref _hashTable[bucketIndex], index + 1);

                _hashTable[bucketIndex][index] = entry;
            }
            else
            {
                _hashTable[bucketIndex] = new Entry[]
                {
                    entry
                };
            }
        }

        public bool TryGetValue(ref TK key, out TV value)
        {
            int hashCode = key.GetHashCode();

            var bucket = _hashTable[hashCode & TotalBucketsMask];
            if (bucket != null)
            {
                for (int i = 0; i < bucket.Length; i++)
                {
                    ref var entry = ref bucket[i];

                    if (entry.Hash == hashCode && entry.Key.Equals(ref key))
                    {
                        value = entry.Value;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }
    }
}
