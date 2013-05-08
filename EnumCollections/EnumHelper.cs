using ExtraConstraints;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnumCollections
{
    internal static class EnumHelper<[EnumConstraint] T>
    {
        public static readonly T[] Value = ((T[])Enum.GetValues(typeof(T))).Distinct().ToArray();
        public static readonly Dictionary<T, int> Ordinal = new Dictionary<T, int>();

        static EnumHelper()
        {
            for (var i = 0; i < Value.Length; i++)
            {
                if (!Ordinal.ContainsKey(Value[i]))
                {
                    Ordinal.Add(Value[i], i);
                }
            }
        }
    }
}
