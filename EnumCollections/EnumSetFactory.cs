using System;

namespace EnumCollections
{
    public abstract class EnumSetFactory<TClass> where TClass : class
    {
        public static EnumSet<T> Of<T>(params T[] list) where T : struct, TClass
        {
            if (Enum.GetValues(typeof(T)).Length > 64)
                return new ArrayEnumSet<T>(list);
            return new ScalarEnumSet<T>(list);
        }
    }
}