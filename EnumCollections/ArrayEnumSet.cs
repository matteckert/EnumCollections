using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace EnumCollections;

internal sealed class ArrayEnumSet<T> : ISet<T> where T : struct, Enum
{
    private static readonly T[] EnumValues = Enum.GetValues<T>().Distinct().ToArray();
    private static readonly FrozenDictionary<T, int> BitPosition;
    
    private readonly ulong[] _elements = new ulong[(EnumValues.Length + 63) >> 6];
    
    static ArrayEnumSet()
    {
        var bitPositionDictionary = new Dictionary<T, int>();
        for (var i = 0; i < EnumValues.Length; i++)
            bitPositionDictionary.TryAdd(EnumValues[i], i);
        BitPosition = bitPositionDictionary.ToFrozenDictionary();
    }
    
    internal ArrayEnumSet(IEnumerable<T> other)
    {
        foreach (var e in other)
            Add(e);
    }
    
    public bool IsReadOnly => 
        false;

    public int Count => 
        (int)_elements.Aggregate(0UL, (current, e) => current + e.CountSetBits());

    public bool SetEquals(IEnumerable<T> other) =>
        !_elements.Where((t, i) => t != EnumSetFrom(other)._elements[i]).Any();

    public bool Add(T item)
    {
        var bucket = BitPosition[item] >> 6;
        var previous = _elements[bucket];
        _elements[bucket] |= 1UL << BitPosition[item];
        return _elements[bucket] != previous;
    }
    
    void ICollection<T>.Add(T item) => 
        Add(item);

    public bool Remove(T item)
    {
        var bucket = GetBucket(item);
        var previous = _elements[bucket];
        _elements[bucket] &= ~(1UL << BitPosition[item]);
        return _elements[bucket] != previous;
    }

    public bool Contains(T item) => 
        (_elements[GetBucket(item)] & 1UL << BitPosition[item]) != 0;

    public void SymmetricExceptWith(IEnumerable<T> other) => 
        ForOther(other, (i, a, b) => a[i] ^= b[i]);

    public void ExceptWith(IEnumerable<T> other) =>
        ForOther(other, (i, a, b) => a[i] &= ~b[i]);

    public void IntersectWith(IEnumerable<T> other) =>
        ForOther(other, (i, a, b) => a[i] &= b[i]);

    public bool IsProperSubsetOf(IEnumerable<T> other) =>
        IsProperSubset(this, EnumSetFrom(other));

    public bool IsSubsetOf(IEnumerable<T> other) =>
        IsSubset(this, EnumSetFrom(other));

    public bool IsProperSupersetOf(IEnumerable<T> other) =>
        IsProperSubset(EnumSetFrom(other), this);

    public bool IsSupersetOf(IEnumerable<T> other) =>
        IsSubset(EnumSetFrom(other), this);

    public bool Overlaps(IEnumerable<T> other) =>
        _elements.Where((t, i) => (t & EnumSetFrom(other)._elements[i]) != 0).Any();

    public void UnionWith(IEnumerable<T> other) =>
        ForOther(other, (i, a, b) => a[i] |= b[i]);

    public void Clear() => 
        Array.Clear(_elements, 0, _elements.Length);

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

    private class Enumerator(ArrayEnumSet<T> enumSet) : IEnumerator<T>
    {
        private int _currentBit;

        public bool MoveNext()
        {
            if (enumSet.Count == 0) return false;
            for (var i = _currentBit; i < EnumValues.Length; i++)
            {
                if ((enumSet._elements[GetBucket(EnumValues[i])] & (1UL << i)) == 0) continue;
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
    
    private static bool IsSubset(ArrayEnumSet<T> a, ArrayEnumSet<T> b) =>
        !a._elements.Where((t, i) => (t & ~b._elements[i]) != 0).Any();

    private static bool IsProperSubset(ArrayEnumSet<T> a, ArrayEnumSet<T> b) =>
        IsSubset(a, b) && a.Count < b.Count;
    
    private static int GetBucket(T item) => 
        BitPosition[item] >> 6;
    
    private static ArrayEnumSet<T> EnumSetFrom(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return other as ArrayEnumSet<T> ?? new ArrayEnumSet<T>(other);
    }
    
    private void ForOther(IEnumerable<T> other, Action<int, ulong[], ulong[]> action)
    {
        var otherSet = EnumSetFrom(other);
        for (var i = 0; i < _elements.Length; i++)
            action(i, _elements, otherSet._elements);
    }
}
