using System;
using System.Collections.Generic;
using System.Linq;

namespace EnumCollections
{
    public class EnumSet<T> : EnumSetTypeConstrainer<Enum>, IEquatable<EnumSet<T>>
    {
        private static readonly T[] Value = Enum.GetValues(typeof(T)).Cast<T>().Distinct().ToArray();
        private static readonly IDictionary<T, int> Ordinal = new Dictionary<T, int>();

        static EnumSet()
        {
            for (var i = 0; i < Value.Length; i++)
                if (!Ordinal.ContainsKey(Value[i]))
                    Ordinal.Add(Value[i], i);
        }

        public bool Equals(EnumSet<T> other)
        {
            throw new NotImplementedException();
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
