using System.Collections.Generic;

namespace EnumCollections
{
    public interface IFiniteSet<T> : ISet<T>
    {
        /// <summary>
        /// Modifies the current set so that it contains the complement of the elements in the set.
        /// </summary>
        void Complement();
    }
}
