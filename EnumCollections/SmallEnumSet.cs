using System.Diagnostics;
using ExtraConstraints;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EnumCollections
{
    internal class SmallEnumSet<[EnumConstraint] T> : EnumSet<T>
    {
        // ReSharper disable StaticFieldInGenericType
        private static readonly ulong mask = 0xFFFFFFFFFFFFFFFF >> (64 - Value.Length);
        // ReSharper restore StaticFieldInGenericType

        private ulong elements;

        public override int GetHashCode()
        {
            return (int)(elements ^ 23);
        }

        public override void Complement()
        {
            elements = ~elements & mask;
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public override bool Add(T item)
        {
            var previous = elements;
            elements |= 1UL << Ordinal[item];
            return elements != previous;
        }

        private static SmallEnumSet<T> GetSmallEnumSetFrom(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            var otherSmallEnumSet = other as SmallEnumSet<T>;

            if (otherSmallEnumSet == null)
            {
                otherSmallEnumSet = new SmallEnumSet<T>();
                foreach (var v in other)
                {
                    otherSmallEnumSet.Add(v);
                }
            }
            return otherSmallEnumSet;
        }

        public override void UnionWith(IEnumerable<T> other)
        {
            var otherSmallEnumSet = GetSmallEnumSetFrom(other);

            elements |= otherSmallEnumSet.elements;
        }

        public override void IntersectWith(IEnumerable<T> other)
        {
            var otherSmallEnumSet = GetSmallEnumSetFrom(other);

            elements &= otherSmallEnumSet.elements;
        }

        public override void ExceptWith(IEnumerable<T> other)
        {
            var otherSmallEnumSet = GetSmallEnumSetFrom(other);

            elements &= ~otherSmallEnumSet.elements;
        }

        public override void SymmetricExceptWith(IEnumerable<T> other)
        {
            var otherSmallEnumSet = GetSmallEnumSetFrom(other);

            elements ^= otherSmallEnumSet.elements;
        }

        public override bool IsSubsetOf(IEnumerable<T> other)
        {
            var otherSmallEnumSet = GetSmallEnumSetFrom(other);

            return (elements & ~otherSmallEnumSet.elements) == 0;
        }

        public override bool IsProperSubsetOf(IEnumerable<T> other)
        {
            var otherSmallEnumSet = GetSmallEnumSetFrom(other);

            return (elements & ~otherSmallEnumSet.elements) == 0
                && Count < otherSmallEnumSet.Count;
        }

        public override bool IsSupersetOf(IEnumerable<T> other)
        {
            var otherSmallEnumSet = GetSmallEnumSetFrom(other);

            return (otherSmallEnumSet.elements & ~elements) == 0;
        }

        public override bool IsProperSupersetOf(IEnumerable<T> other)
        {
            var otherSmallEnumSet = GetSmallEnumSetFrom(other);

            return (otherSmallEnumSet.elements & ~elements) == 0
                   && Count > otherSmallEnumSet.Count;
        }

        public override bool Overlaps(IEnumerable<T> other)
        {
            var otherSmallEnumSet = GetSmallEnumSetFrom(other);

            return (elements & otherSmallEnumSet.elements) != 0;
        }

        public override bool SetEquals(IEnumerable<T> other)
        {
            var otherSmallEnumSet = GetSmallEnumSetFrom(other);

            return elements == otherSmallEnumSet.elements;
        }

        public override bool Equals(EnumSet<T> other)
        {
            var otherSmallEnumSet = GetSmallEnumSetFrom(other);

            return elements == otherSmallEnumSet.elements;
        }

        public override void Clear()
        {
            elements = 0;
        }

        public override bool Contains(T item)
        {
            return (elements & (1UL << Ordinal[item])) != 0;
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if (Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("Not enough space in destination array.");
            }

            foreach (var v in this)
            {
                array[arrayIndex++] = v;
            }
        }

        public override bool Remove(T item)
        {
            var previous = elements;
            elements &= ~(1UL << Ordinal[item]);
            return elements != previous;
        }

        public override int Count
        {
            get { return Bits.Count(elements); }
        }


        private struct Enumerator : IEnumerator<T>
        {
            private readonly SmallEnumSet<T> enumSet;
            private long unseen;
            private long lastReturned;

            public Enumerator(SmallEnumSet<T> enumSet)
                : this()
            {
                this.enumSet = enumSet;
                Reset();
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                if (unseen == 0) return false;

                lastReturned = unseen & -unseen;
                unseen -= lastReturned;

                Current = Value[Bits.TrailingZeroes((uint)lastReturned)];

                return true;
            }

            public void Reset()
            {
                unseen = (long)enumSet.elements;
                lastReturned = 0;
                Current = default(T);
            }

            public T Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
