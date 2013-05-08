using ExtraConstraints;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EnumCollections
{
    public abstract class EnumSet<[EnumConstraint] T> : IFiniteSet<T>, IEquatable<EnumSet<T>>
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EnumSet<T>)obj);
        }

        public abstract override int GetHashCode();

        #region Static data

        protected static readonly T[] Value = EnumHelper<T>.Value;
        protected static readonly Dictionary<T, int> Ordinal = EnumHelper<T>.Ordinal;

        #endregion

        #region Static factory methods

        public static EnumSet<T> None()
        {
            if (Value.Length > 64)
            {
                return new LargeEnumSet<T>();
            }
            return new SmallEnumSet<T>();
        }

        public static EnumSet<T> All()
        {
            var result = None();
            result.Complement();
            return result;
        }

        public static EnumSet<T> Of(T t1)
        {
            var result = None();
            result.Add(t1);
            return result;
        }

        public static EnumSet<T> Of(T t1, T t2)
        {
            var result = None();
            result.Add(t1);
            result.Add(t2);
            return result;
        }

        // ReSharper disable MethodOverloadWithOptionalParameter
        public static EnumSet<T> Of(T t, params T[] rest)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            var result = None();
            result.Add(t);
            foreach (var v in rest)
            {
                result.Add(v);
            }
            return result;
        }

        public static EnumSet<T> Of(T t1, T t2, T t3)
        {
            var result = None();
            result.Add(t1);
            result.Add(t2);
            result.Add(t3);
            return result;
        }

        public static EnumSet<T> Of(T t1, T t2, T t3, T t4)
        {
            var result = None();
            result.Add(t1);
            result.Add(t2);
            result.Add(t3);
            result.Add(t4);
            return result;
        }

        public static EnumSet<T> Of(T t1, T t2, T t3, T t4, T t5)
        {
            var result = None();
            result.Add(t1);
            result.Add(t2);
            result.Add(t3);
            result.Add(t4);
            result.Add(t5);
            return result;
        }

        public static EnumSet<T> Of(T t1, T t2, T t3, T t4, T t5, T t6)
        {
            var result = None();
            result.Add(t1);
            result.Add(t2);
            result.Add(t3);
            result.Add(t4);
            result.Add(t5);
            result.Add(t6);
            return result;
        }

        public static EnumSet<T> Of(T t1, T t2, T t3, T t4, T t5, T t6, T t7)
        {
            var result = None();
            result.Add(t1);
            result.Add(t2);
            result.Add(t3);
            result.Add(t4);
            result.Add(t5);
            result.Add(t6);
            result.Add(t7);
            return result;
        }

        #endregion

        #region IFiniteSet<T> methods

        public abstract void Complement();

        #endregion

        #region ICollection<T> methods

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public abstract void Clear();

        public abstract bool Contains(T item);

        public abstract void CopyTo(T[] array, int arrayIndex);

        public abstract bool Remove(T item);

        public abstract int Count { get; }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable methods

        abstract public IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ISet<T> methods

        public abstract bool Add(T item);

        public abstract void UnionWith(IEnumerable<T> other);

        public abstract void IntersectWith(IEnumerable<T> other);

        public abstract void ExceptWith(IEnumerable<T> other);

        public abstract void SymmetricExceptWith(IEnumerable<T> other);

        public abstract bool IsSubsetOf(IEnumerable<T> other);

        public abstract bool IsSupersetOf(IEnumerable<T> other);

        public abstract bool IsProperSupersetOf(IEnumerable<T> other);

        public abstract bool IsProperSubsetOf(IEnumerable<T> other);

        public abstract bool Overlaps(IEnumerable<T> other);

        public abstract bool SetEquals(IEnumerable<T> other);

        #endregion

        #region IEquatable<T> methods

        public abstract bool Equals(EnumSet<T> other);

        #endregion

        public static bool operator ==(EnumSet<T> a, EnumSet<T> b)
        {
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(EnumSet<T> a, EnumSet<T> b)
        {
            return !(a == b);
        }

        public bool this[T t]
        {
            get { return Contains(t); }

            set
            {
                if (value) Add(t);
                else Remove(t);
            }
        }
    }
}
