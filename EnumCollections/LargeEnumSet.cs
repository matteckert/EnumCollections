using ExtraConstraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EnumCollections
{
    internal class LargeEnumSet<[EnumConstraint] T> : EnumSet<T>
    {
        // ReSharper disable StaticFieldInGenericType
        private static readonly ulong mask = 0xFFFFFFFFFFFFFFFF >> (64 - (Value.Length % 64));
        // ReSharper restore StaticFieldInGenericType

        private readonly ulong[] elements = new ulong[(Value.Length + 63) >> 6];

        private int count;

        public override int GetHashCode()
        {
            unchecked
            {
                var running = elements.Aggregate<ulong, ulong>(0, (current, v) => current + v);
                return (int)(running ^ 23);
            }
        }

        public override void Complement()
        {
            for (var i = 0; i < elements.Length; i++)
            {
                elements[i] = ~elements[i];
            }

            elements[elements.Length - 1] = ~elements[elements.Length - 1] & mask;
        }

        public override bool Add(T item)
        {
            var bucket = Ordinal[item] >> 6;
            var previous = elements[bucket];
            elements[bucket] |= 1UL << Ordinal[item];
            var result = elements[bucket] != previous;
            if (result) count++;
            return result;
        }

        private static LargeEnumSet<T> GetLargeEnumSetFrom(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            var otherLargeEnumSet = other as LargeEnumSet<T>;

            if (otherLargeEnumSet == null)
            {
                otherLargeEnumSet = new LargeEnumSet<T>();
                foreach (var v in other)
                {
                    otherLargeEnumSet.Add(v);
                }
            }
            return otherLargeEnumSet;
        }

        private void RecalculateCount()
        {
            count = 0;
            foreach (var elementArray in elements)
            {
                count += Bits.Count(elementArray);
            }
        }

        public override void UnionWith(IEnumerable<T> other)
        {
            var otherLargeEnumSet = GetLargeEnumSetFrom(other);

            for (var i = 0; i < elements.Length; i++)
            {
                elements[i] |= otherLargeEnumSet.elements[i];
            }

            RecalculateCount();
        }

        public override void IntersectWith(IEnumerable<T> other)
        {
            var otherLargeEnumSet = GetLargeEnumSetFrom(other);

            for (var i = 0; i < elements.Length; i++)
            {
                elements[i] &= otherLargeEnumSet.elements[i];
            }

            RecalculateCount();
        }

        public override void ExceptWith(IEnumerable<T> other)
        {
            var otherLargeEnumSet = GetLargeEnumSetFrom(other);

            for (var i = 0; i < elements.Length; i++)
            {
                elements[i] &= ~otherLargeEnumSet.elements[i];
            }

            RecalculateCount();
        }

        public override void SymmetricExceptWith(IEnumerable<T> other)
        {
            var otherLargeEnumSet = GetLargeEnumSetFrom(other);

            for (var i = 0; i < elements.Length; i++)
            {
                elements[i] ^= otherLargeEnumSet.elements[i];
            }

            RecalculateCount();
        }

        public override bool IsSubsetOf(IEnumerable<T> other)
        {
            var otherLargeEnumSet = GetLargeEnumSetFrom(other);

            return !elements.Where((t, i) => (t & ~otherLargeEnumSet.elements[i]) != 0).Any();
        }

        public override bool IsProperSubsetOf(IEnumerable<T> other)
        {
            var otherLargeEnumSet = GetLargeEnumSetFrom(other);

            return !elements.Where((t, i) => (t & ~otherLargeEnumSet.elements[i]) != 0).Any() &&
                   count < otherLargeEnumSet.count;
        }

        public override bool IsSupersetOf(IEnumerable<T> other)
        {
            var otherLargeEnumSet = GetLargeEnumSetFrom(other);

            return !elements.Where((t, i) => (otherLargeEnumSet.elements[i] & ~t) != 0).Any();
        }

        public override bool IsProperSupersetOf(IEnumerable<T> other)
        {
            var otherLargeEnumSet = GetLargeEnumSetFrom(other);

            return !elements.Where((t, i) => (otherLargeEnumSet.elements[i] & ~t) != 0).Any() &&
                   count > otherLargeEnumSet.count;
        }

        public override bool Overlaps(IEnumerable<T> other)
        {
            var otherLargeEnumSet = GetLargeEnumSetFrom(other);

            return elements.Where((t, i) => (t | otherLargeEnumSet.elements[i]) != 0).Any();
        }

        public override bool SetEquals(IEnumerable<T> other)
        {
            var otherLargeEnumSet = GetLargeEnumSetFrom(other);

            return !elements.Where((t, i) => t != otherLargeEnumSet.elements[i]).Any();
        }

        public override bool Equals(EnumSet<T> other)
        {
            var otherLargeEnumSet = GetLargeEnumSetFrom(other);

            return !elements.Where((t, i) => t != otherLargeEnumSet.elements[i]).Any();
        }

        public override void Clear()
        {
            for (var i = 0; i < elements.Length; i++)
            {
                elements[i] = 0UL;
            }
            count = 0;
        }

        public override bool Contains(T item)
        {
            return (elements[Ordinal[item] >> 6] & (1UL << Ordinal[item])) != 0;
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
            var bucket = Ordinal[item] >> 6;
            var previous = elements[bucket];
            elements[bucket] &= ~(1UL << Ordinal[item]);
            var result = elements[bucket] != previous;
            if (result) count--;
            return result;
        }

        public override int Count
        {
            get { return count; }
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        private struct Enumerator : IEnumerator<T>
        {
            private readonly LargeEnumSet<T> enumSet;
            private long unseen;
            private long lastReturned;
            private long unseenIndex;
            private long lastReturnedIndex;

            public Enumerator(LargeEnumSet<T> enumSet)
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
                while (unseen == 0 && unseenIndex < enumSet.elements.Length - 1)
                {
                    unseen = (long)enumSet.elements[++unseenIndex];
                }

                if (unseen == 0) return false;

                lastReturned = unseen & -unseen;
                lastReturnedIndex = unseenIndex;
                unseen -= lastReturned;

                Current = Value[(lastReturnedIndex << 6)
                    + Bits.TrailingZeroes((uint)lastReturned)];

                return true;
            }

            public void Reset()
            {
                unseen = (long)enumSet.elements[0];
                lastReturnedIndex = 0;
                unseenIndex = 0;
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
