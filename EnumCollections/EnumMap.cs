using ExtraConstraints;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EnumCollections
{
    public class EnumMap<[EnumConstraint] TKey, TValue> : IDictionary<TKey, TValue>
    {
        #region Static data

        private static readonly Dictionary<TKey, int> ordinal = EnumHelper<TKey>.Ordinal;
        private static readonly TKey[] keys = EnumHelper<TKey>.Value;

        #endregion

        #region Instance data

        private readonly TValue[] values = new TValue[EnumHelper<TKey>.Value.Length];
        private readonly bool[] occupied = new bool[EnumHelper<TKey>.Value.Length];

        #endregion

        #region IEnumerable methods

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICollection methods

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            var index = ordinal[item.Key];
            if (!occupied[index])
            {
                occupied[index] = true;
                Count++;
            }
            values[index] = item.Value;
        }

        public void Clear()
        {
            for (var i = 0; i < occupied.Length; i++)
            {
                occupied[i] = false;
            }
            Count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var index = ordinal[item.Key];
            return occupied[index] && values[index].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (var i = 0; i < values.Length; i++)
            {
                if (occupied[i]) array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Contains(item) && Remove(item.Key);
        }

        public int Count { get; private set; }

        public bool IsReadOnly { get { return false; } }

        #endregion

        public bool ContainsKey(TKey key)
        {
            return occupied[ordinal[key]];
        }

        public void Add(TKey key, TValue value)
        {
            var index = ordinal[key];
            if (occupied[index]) throw new ArgumentException("An element with the same key already exists.");
            values[index] = value;
            occupied[index] = true;
            Count++;
        }

        public bool Remove(TKey key)
        {
            var index = ordinal[key];
            if (!occupied[index]) return false;
            occupied[index] = false;
            Count--;
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var index = ordinal[key];
            if (!occupied[index])
            {
                value = default(TValue);
                return false;
            }
            value = values[index];
            return true;
        }

        public TValue this[TKey key]
        {
            get
            {
                var index = ordinal[key];
                if (!occupied[index]) throw new KeyNotFoundException();
                return values[index];
            }

            set
            {
                var index = ordinal[key];
                values[index] = value;
                if (occupied[index]) return;
                occupied[index] = true;
                Count++;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                var keyArray = new TKey[Count];
                var j = 0;
                for (var i = 0; i < keys.Length; i++)
                {
                    if (occupied[i])
                    {
                        keyArray[j++] = keys[i];
                    }
                }
                return keyArray;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                var valueArray = new TValue[Count];
                var j = 0;
                for (var i = 0; i < values.Length; i++)
                {
                    if (occupied[i])
                    {
                        valueArray[j++] = values[i];
                    }
                }
                return valueArray;
            }
        }

        private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private int currentIndex;
            private readonly EnumMap<TKey, TValue> map;

            public Enumerator(EnumMap<TKey, TValue> map)
                : this()
            {
                this.map = map;
                Reset();
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                while (currentIndex < map.Count)
                {
                    currentIndex++;
                    if (!map.occupied[currentIndex]) continue;
                    Current = new KeyValuePair<TKey, TValue>(keys[currentIndex], map.values[currentIndex]);
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                currentIndex = -1;
                Current = default(KeyValuePair<TKey, TValue>);
            }

            public KeyValuePair<TKey, TValue> Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
