using System;
using System.Collections;
using System.Collections.Generic;

namespace EnumCollections
{
    public abstract class EnumSet<T> : ISet<T>
    {
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        void ICollection<T>.Add(T item) => 
            Add(item);

        public bool IsReadOnly => false;

        public abstract void ExceptWith(IEnumerable<T> other);
        public abstract void IntersectWith(IEnumerable<T> other);
        public abstract bool IsProperSubsetOf(IEnumerable<T> other);
        public abstract bool IsProperSupersetOf(IEnumerable<T> other);
        public abstract bool IsSubsetOf(IEnumerable<T> other);
        public abstract bool IsSupersetOf(IEnumerable<T> other);
        public abstract bool Overlaps(IEnumerable<T> other);
        public abstract bool SetEquals(IEnumerable<T> other);
        public abstract void SymmetricExceptWith(IEnumerable<T> other);
        public abstract void UnionWith(IEnumerable<T> other);
        public abstract bool Add(T item);
        public abstract void Clear();
        public abstract bool Contains(T item);
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract bool Remove(T item);
        public abstract int Count { get; }

        internal static ulong CountBits(ulong v)
        {
            v = v - (v >> 1 & 0x5555555555555555UL);
            v = (v & 0x3333333333333333UL) + (v >> 2 & 0x3333333333333333UL);
            return (v + (v >> 4) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL >> 56;
        }

        internal static void ThrowIfNull(object argument, string name)
        {
            if (argument == null)
                throw new ArgumentNullException(name);
        }
    }

    public sealed class EnumSet : EnumSetFactory<Enum>
    {
        private EnumSet() { }
    }
}
