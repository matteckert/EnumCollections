using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EnumCollections
{
    public sealed class ArrayEnumSet<T> : EnumSet<T> where T : struct, Enum
    {
        private static readonly T[] Value = Enum.GetValues(typeof(T))
            .Cast<T>().Distinct().ToArray();

        private static readonly IDictionary<T, int> BitPosition = new Dictionary<T, int>();

        static ArrayEnumSet()
        {
            for (var i = 0; i < Value.Length; i++)
                if (!BitPosition.ContainsKey(Value[i]))
                    BitPosition.Add(Value[i], i);
        }

        public override int Count => 
            (int)_elements.Aggregate(0UL, (current, e) => current + CountBits(e));

        private readonly ulong[] _elements = new ulong[(Value.Length + 63) >> 6];

        public override IEnumerator<T> GetEnumerator() => 
            new Enumerator(this);

        internal ArrayEnumSet(IEnumerable<T> other)
        {
            foreach (var e in other)
                Add(e);
        }

        public override bool SetEquals(IEnumerable<T> other) =>
            !_elements.Where((t, i) => t != EnumSetFrom(other)._elements[i]).Any();

        private static ArrayEnumSet<T> EnumSetFrom(IEnumerable<T> other)
        {
            ThrowIfNull(other, nameof(other));
            return other as ArrayEnumSet<T> ?? new ArrayEnumSet<T>(other);
        }

        public override bool Add(T item)
        {
            var bucket = BitPosition[item] >> 6;
            var previous = _elements[bucket];
            _elements[bucket] |= 1UL << BitPosition[item];
            return _elements[bucket] != previous;
        }

        public override bool Remove(T item)
        {
            var bucket = GetBucket(item);
            var previous = _elements[bucket];
            _elements[bucket] &= ~(1UL << BitPosition[item]);
            return _elements[bucket] != previous;
        }

        private static int GetBucket(T item) =>
            BitPosition[item] >> 6;

        public override bool Contains(T item) =>
            (_elements[GetBucket(item)] & 1UL << BitPosition[item]) != 0;

        public override void SymmetricExceptWith(IEnumerable<T> other) =>
            ForOther(other, (i, a, b) => a[i] ^= b[i]);

        public void ForOther(IEnumerable<T> other, Action<int, ulong[], ulong[]> action)
        {
            var otherSet = EnumSetFrom(other);
            for (var i = 0; i < _elements.Length; i++)
                action(i, _elements, otherSet._elements);
        }

        public override void ExceptWith(IEnumerable<T> other) =>
            ForOther(other, (i, a, b) => a[i] &= ~b[i]);

        public override void IntersectWith(IEnumerable<T> other) =>
            ForOther(other, (i, a, b) => a[i] &= b[i]);

        private static bool IsSubset(ArrayEnumSet<T> a, ArrayEnumSet<T> b) =>
            !a._elements.Where((t, i) => (t & ~b._elements[i]) != 0).Any();

        private static bool IsProperSubset(ArrayEnumSet<T> a, ArrayEnumSet<T> b) =>
            IsSubset(a, b) && a.Count < b.Count;

        public override bool IsProperSubsetOf(IEnumerable<T> other) =>
            IsProperSubset(this, EnumSetFrom(other));

        public override bool IsSubsetOf(IEnumerable<T> other) =>
            IsSubset(this, EnumSetFrom(other));

        public override bool IsProperSupersetOf(IEnumerable<T> other) =>
            IsProperSubset(EnumSetFrom(other), this);

        public override bool IsSupersetOf(IEnumerable<T> other) =>
            IsSubset(EnumSetFrom(other), this);

        public override bool Overlaps(IEnumerable<T> other) =>
            _elements.Where((t, i) => (t & EnumSetFrom(other)._elements[i]) != 0).Any();

        public override void UnionWith(IEnumerable<T> other) =>
            ForOther(other, (i, a, b) => a[i] |= b[i]);

        public override void Clear()
        {
            for (var i = 0; i < _elements.Length; i++)
                _elements[i] = 0;
        }

        public override void CopyTo(T[] array, int index)
        {
            ThrowIfNull(array, nameof(array));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (Count > array.Length - index)
                throw new ArgumentException("Not enough space in " + nameof(array));
            foreach (var e in this)
                array[index++] = e;
        }

        private class Enumerator : IEnumerator<T>
        {
            private readonly ArrayEnumSet<T> _enumSet;
            private int _currentBit;

            public Enumerator(ArrayEnumSet<T> enumSet)
            {
                _enumSet = enumSet;
            }

            public bool MoveNext()
            {
                if (_enumSet.Count == 0) return false;
                for (var i = _currentBit; i < Value.Length; i++)
                {
                    if ((_enumSet._elements[GetBucket(Value[i])] & (1UL << i)) == 0) continue;
                    Current = Value[i];
                    _currentBit = i + 1;
                    return true;
                }
                return false;
            }

            public void Reset() { }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() { }
        }
    }
}