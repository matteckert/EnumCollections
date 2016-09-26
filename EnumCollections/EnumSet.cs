using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EnumCollections
{
    public class EnumSet<T> : EnumSetTypeConstrainer<Enum>, ISet<T>
    {
        private static readonly T[] Value = Enum.GetValues(typeof(T)).Cast<T>().Distinct().ToArray();
        private static readonly IDictionary<T, int> Ordinal = new Dictionary<T, int>();
        //private static readonly ulong Mask = 0xFFFFFFFFFFFFFFFF >> (64 - Value.Length);

        static EnumSet()
        {
            for (var i = 0; i < Value.Length; i++)
                if (!Ordinal.ContainsKey(Value[i]))
                    Ordinal.Add(Value[i], i);
        }

        public int Count { get; private set; }

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

        public EnumSet()
        {
            Count = 0;
            IsReadOnly = false;
        }

        public EnumSet(IEnumerable<T> other) : this()
        {
            foreach (var e in other)
                Add(e);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _elements == EnumSetFrom(other)._elements;
        }

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
            _elements |= 1UL << Ordinal[item];
            var added = _elements != previous;
            if (added) Count++;
            return added;
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
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

            public void Reset()
            {
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }

    public abstract class EnumSetTypeConstrainer<TClass> where TClass : class
    {
        public static EnumSet<T> Of<T>() where T : struct, TClass
        {
            return new EnumSet<T>();
        }
    }
}
