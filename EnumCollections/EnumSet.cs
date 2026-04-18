using System;
using System.Collections;
using System.Collections.Generic;

namespace EnumCollections;

public static class EnumSet
{
    public static EnumSet<T> Of<T>(params IEnumerable<T> list) where T : struct, Enum => 
        new(list);
}
    
public class EnumSet<T>(params IEnumerable<T> items) : ISet<T> where T : struct, Enum
{
    private readonly ISet<T> _set = InitializeEnumSet(items);

    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();

    void ICollection<T>.Add(T item) => 
        Add(item);

    public bool IsReadOnly => 
        _set.IsReadOnly;

    public IEnumerator<T> GetEnumerator() => 
        _set.GetEnumerator();
        
    public void ExceptWith(IEnumerable<T> other) => 
        _set.ExceptWith(other);
        
    public void IntersectWith(IEnumerable<T> other) => 
        _set.IntersectWith(other);
        
    public bool IsProperSubsetOf(IEnumerable<T> other) => 
        _set.IsProperSubsetOf(other);
        
    public bool IsProperSupersetOf(IEnumerable<T> other) => 
        _set.IsProperSupersetOf(other);
        
    public bool IsSubsetOf(IEnumerable<T> other) => 
        _set.IsSubsetOf(other);

    public bool IsSupersetOf(IEnumerable<T> other) => 
        _set.IsSupersetOf(other);

    public bool Overlaps(IEnumerable<T> other) => 
        _set.Overlaps(other);

    public bool SetEquals(IEnumerable<T> other) => 
        _set.SetEquals(other);

    public void SymmetricExceptWith(IEnumerable<T> other) => 
        _set.SymmetricExceptWith(other);

    public void UnionWith(IEnumerable<T> other) =>
        _set.UnionWith(other);

    public bool Add(T item) => 
        _set.Add(item);

    public void Clear() => 
        _set.Clear();

    public bool Contains(T item) => 
        _set.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => 
        _set.CopyTo(array, arrayIndex);

    public bool Remove(T item) => 
        _set.Remove(item);

    public int Count => 
        _set.Count;
        
    private static ISet<T> InitializeEnumSet(IEnumerable<T> other)
    {
        return Enum.GetValues<T>().Length > 64 ? 
            new ArrayEnumSet<T>(other) :
            new ScalarEnumSet<T>(other);
    }
}
