using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EnumCollections
{
    public class EnumSet<T> : ISet<T>
    {
        private static readonly T[] Value = Enum.GetValues(typeof(T))
            .Cast<T>().Distinct().ToArray();
        private static readonly IDictionary<T, int> BitPosition = new Dictionary<T, int>();

        static EnumSet()
        {
            if (Value.Length > 64)
                throw new ArgumentException("Enum must have 64 constants or less");
            for (var i = 0; i < Value.Length; i++)
                if (!BitPosition.ContainsKey(Value[i]))
                    BitPosition.Add(Value[i], i);
        }

        private static int CountBits(ulong v)
        {
            v = v - (v >> 1 & 0x5555555555555555UL);
            v = (v & 0x3333333333333333UL) + (v >> 2 & 0x3333333333333333UL);
            return (int)((v + (v >> 4) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL >> 56);
        }

        public int Count => CountBits(_elements);

        public bool IsReadOnly { get; }

        private ulong _elements;

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal EnumSet()
        {
            IsReadOnly = false;
        }

        internal EnumSet(IEnumerable<T> other) : this()
        {
            foreach (var e in other)
                Add(e);
        }

        public bool SetEquals(IEnumerable<T> other) => 
            _elements == EnumSetFrom(other)._elements;

        private static EnumSet<T> EnumSetFrom(IEnumerable<T> other)
        {
            ThrowIfNull(other, nameof(other));
            return other as EnumSet<T> ?? new EnumSet<T>(other);
        }

        private static void ThrowIfNull(object argument, string name)
        {
            if (argument == null)
                throw new ArgumentNullException(name);
        }

        public bool Add(T item)
        {
            var previous = _elements;
            _elements |= 1UL << BitPosition[item];
            return _elements != previous;
        }

        public bool Remove(T item)
        {
            var previous = _elements;
            _elements &= ~(1UL << BitPosition[item]);
            return _elements != previous;
        }

        void ICollection<T>.Add(T item) => 
            Add(item);

        public bool Contains(T item) => 
            (_elements & 1UL << BitPosition[item]) != 0;

        public void SymmetricExceptWith(IEnumerable<T> other) => 
            _elements ^= EnumSetFrom(other)._elements;

        public void ExceptWith(IEnumerable<T> other) =>
            _elements &= ~EnumSetFrom(other)._elements;

        public void IntersectWith(IEnumerable<T> other) => 
            _elements &= EnumSetFrom(other)._elements;

        private static bool IsSubset(EnumSet<T> a, EnumSet<T> b) => 
            (a._elements & ~b._elements) == 0;

        private static bool IsProperSubset(EnumSet<T> a, EnumSet<T> b) => 
            IsSubset(a, b) && a.Count < b.Count;

        public bool IsProperSubsetOf(IEnumerable<T> other) =>
            IsProperSubset(this, EnumSetFrom(other));

        public bool IsSubsetOf(IEnumerable<T> other) =>
            IsSubset(this, EnumSetFrom(other)); 

        public bool IsProperSupersetOf(IEnumerable<T> other) =>
            IsProperSubset(EnumSetFrom(other), this);

        public bool IsSupersetOf(IEnumerable<T> other) => 
            IsSubset(EnumSetFrom(other), this);

        public bool Overlaps(IEnumerable<T> other) => 
            (_elements & EnumSetFrom(other)._elements) != 0;

        public void UnionWith(IEnumerable<T> other) =>
            _elements |= EnumSetFrom(other)._elements;

        public void Clear() =>
            _elements = 0;

        public void CopyTo(T[] array, int index)
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
            private readonly EnumSet<T> _enumSet;
            private int _currentBit;

            public Enumerator(EnumSet<T> enumSet)
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

    public sealed class EnumSet : EnumSetTypeConstrainer<Enum>
    {
        private EnumSet() { }
    }

    public abstract class EnumSetTypeConstrainer<TClass> where TClass : class
    {
        public static EnumSet<T> Of<T>(params T[] list) where T : struct, TClass => 
            new EnumSet<T>(list);
    }
}
