using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EnumCollections
{
    public sealed class ScalarEnumSet<T> : EnumSet<T>
    {
        private static readonly T[] Value = Enum.GetValues(typeof(T))
            .Cast<T>().Distinct().ToArray();
        private static readonly IDictionary<T, int> BitPosition = new Dictionary<T, int>();

        static ScalarEnumSet()
        {
            if (Value.Length > 64)
                throw new ArgumentException("Enum must have 64 constants or less");
            for (var i = 0; i < Value.Length; i++)
                if (!BitPosition.ContainsKey(Value[i]))
                    BitPosition.Add(Value[i], i);
        }

        public override int Count => (int) CountBits(_elements);

        private ulong _elements;

        internal ScalarEnumSet(IEnumerable<T> other)
        {
            foreach (var e in other)
                Add(e);
        }

        public override bool SetEquals(IEnumerable<T> other) =>
            _elements == EnumSetFrom(other)._elements;

        private static ScalarEnumSet<T> EnumSetFrom(IEnumerable<T> other)
        {
            ThrowIfNull(other, nameof(other));
            return other as ScalarEnumSet<T> ?? new ScalarEnumSet<T>(other);
        }

        public override bool Add(T item)
        {
            var previous = _elements;
            _elements |= 1UL << BitPosition[item];
            return _elements != previous;
        }

        public override bool Remove(T item)
        {
            var previous = _elements;
            _elements &= ~(1UL << BitPosition[item]);
            return _elements != previous;
        }

        public override bool Contains(T item) =>
            (_elements & 1UL << BitPosition[item]) != 0;

        public override void SymmetricExceptWith(IEnumerable<T> other) =>
            _elements ^= EnumSetFrom(other)._elements;

        public override void ExceptWith(IEnumerable<T> other) =>
            _elements &= ~EnumSetFrom(other)._elements;

        public override void IntersectWith(IEnumerable<T> other) =>
            _elements &= EnumSetFrom(other)._elements;

        private static bool IsSubset(ScalarEnumSet<T> a, ScalarEnumSet<T> b) =>
            (a._elements & ~b._elements) == 0;

        private static bool IsProperSubset(ScalarEnumSet<T> a, ScalarEnumSet<T> b) =>
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
            (_elements & EnumSetFrom(other)._elements) != 0;

        public override void UnionWith(IEnumerable<T> other) =>
            _elements |= EnumSetFrom(other)._elements;

        public override void Clear() =>
            _elements = 0;

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

        public override IEnumerator<T> GetEnumerator() =>
            new Enumerator(this);

        private class Enumerator : IEnumerator<T>
        {
            private readonly ScalarEnumSet<T> _enumSet;
            private int _currentBit;

            public Enumerator(ScalarEnumSet<T> enumSet)
            {
                _enumSet = enumSet;
            }

            public bool MoveNext()
            {
                if (_enumSet.Count == 0) return false;
                for (var i = _currentBit; i < Value.Length; i++)
                {
                    if ((_enumSet._elements & (1UL << i)) == 0) continue;
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