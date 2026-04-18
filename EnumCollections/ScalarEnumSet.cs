using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace EnumCollections;

internal sealed class ScalarEnumSet<T> : ISet<T> where T : struct, Enum
{
    private static readonly T[] EnumValues = Enum.GetValues<T>().Distinct().ToArray();
    private static readonly FrozenDictionary<T, int> BitPosition;

    static ScalarEnumSet()
    {
        var bitPositionDictionary = new Dictionary<T, int>();
        if (EnumValues.Length > 64)
            throw new ArgumentException("Enum must have 64 constants or less");
        for (var i = 0; i < EnumValues.Length; i++)
            bitPositionDictionary.TryAdd(EnumValues[i], i);
        BitPosition = bitPositionDictionary.ToFrozenDictionary();
    }

    private ulong _elements;

    internal ScalarEnumSet(IEnumerable<T> other)
    {
        foreach (var e in other)
            Add(e);
    }
        
    public bool IsReadOnly => 
        false;
        
    public int Count => 
        (int) _elements.CountSetBits();

    public bool SetEquals(IEnumerable<T> other) =>
        _elements == EnumSetFrom(other)._elements;

    public bool Add(T item)
    {
        var previous = _elements;
        _elements |= 1UL << BitPosition[item];
        return _elements != previous;
    }
        
    void ICollection<T>.Add(T item) => 
        Add(item);

    public bool Remove(T item)
    {
        var previous = _elements;
        _elements &= ~(1UL << BitPosition[item]);
        return _elements != previous;
    }

    public bool Contains(T item) =>
        (_elements & 1UL << BitPosition[item]) != 0;

    public void SymmetricExceptWith(IEnumerable<T> other) =>
        _elements ^= EnumSetFrom(other)._elements;

    public void ExceptWith(IEnumerable<T> other) =>
        _elements &= ~EnumSetFrom(other)._elements;

    public void IntersectWith(IEnumerable<T> other) =>
        _elements &= EnumSetFrom(other)._elements;

    public bool IsProperSubsetOf(IEnumerable<T> other) =>
        IsProperSubset(this, EnumSetFrom(other));

    public bool IsSubsetOf(IEnumerable<T> other) =>
        IsSubset(this, EnumSetFrom(other));

    public bool IsProperSupersetOf(IEnumerable<T> other) =>
        IsProperSubset(EnumSetFrom(other), this);

    public bool IsSupersetOf(IEnumerable<T> other) =>
        IsSubset(EnumSetFrom(other), this);

    public bool Overlaps(IEnumerable<T> other) =>
        (_elements & EnumSetFrom(other)._elements) != 0;

    public void UnionWith(IEnumerable<T> other) =>
        _elements |= EnumSetFrom(other)._elements;
        
    public void Clear() =>
        _elements = 0;

    public void CopyTo(T[] array, int index)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(index);
            
        if (Count > array.Length - index)
            throw new ArgumentException("Not enough space in " + nameof(array));
            
        foreach (var e in this)
            array[index++] = e;
    }

    public IEnumerator<T> GetEnumerator() => 
        new Enumerator(this);
        
    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();

    private class Enumerator(ScalarEnumSet<T> enumSet) : IEnumerator<T>
    {
        private int _currentBit;

        public bool MoveNext()
        {
            if (enumSet.Count == 0) return false;
            for (var i = _currentBit; i < EnumValues.Length; i++)
            {
                if ((enumSet._elements & (1UL << i)) == 0) continue;
                Current = EnumValues[i];
                _currentBit = i + 1;
                return true;
            }
            return false;
        }

        public void Reset() { }

        public T Current { get; private set; }

        object IEnumerator.Current => 
            Current;

        public void Dispose() { }
    }
        
    private static bool IsSubset(ScalarEnumSet<T> a, ScalarEnumSet<T> b) =>
        (a._elements & ~b._elements) == 0;

    private static bool IsProperSubset(ScalarEnumSet<T> a, ScalarEnumSet<T> b) =>
        IsSubset(a, b) && a.Count < b.Count;
        
    private static ScalarEnumSet<T> EnumSetFrom(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return other as ScalarEnumSet<T> ?? new ScalarEnumSet<T>(other);
    }
}