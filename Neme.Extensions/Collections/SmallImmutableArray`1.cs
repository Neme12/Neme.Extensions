using Neme.Extensions.Contracts;
using Roslyn.Utilities;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Collections;

internal interface ISmallImmutableArray
{
    public Array? Array { get; }
}

[StructLayout(LayoutKind.Sequential)]
public readonly partial struct SmallImmutableArray<T> :
    IReadOnlyList<T>,
    IList<T>,
    IList,
    IEquatable<SmallImmutableArray<T>>,
    IStructuralEquatable,
    IStructuralComparable,
    ISmallImmutableArray,
    IFormattable
{
    internal readonly T? _item1;
    internal readonly T? _item2;
    internal readonly int _length;
    internal readonly ImmutableArray<T> _items;

    internal const int InlineCapacity = 2;

    internal SmallImmutableArray(T item1)
    {
        _item1 = item1;
        _length = 1;

        AssertInvariants();
    }

    internal SmallImmutableArray(T item1, T item2)
    {
        _item1 = item1;
        _item2 = item2;
        _length = 2;

        AssertInvariants();
    }

    internal SmallImmutableArray(ImmutableArray<T> items)
    {
        Debug.AssertNotDefault(items);

        switch (items)
        {
            case []:
                break;
            case [var item1]:
                _item1 = item1;
                _length = 1;
                break;
            case [var item1, var item2]:
                _item1 = item1;
                _item2 = item2;
                _length = 2;
                break;
            default:
                _items = items;
                _length = items.Length;
                break;
        }

        AssertInvariants();
    }

    internal SmallImmutableArray(ReadOnlySpan<T> items)
    {
        switch (items)
        {
            case []:
                break;
            case [var item1]:
                _item1 = item1;
                _length = 1;
                break;
            case [var item1, var item2]:
                _item1 = item1;
                _item2 = item2;
                _length = 2;
                break;
            default:
                _items = items.ToImmutableArray();
                _length = items.Length;
                break;
        }

        AssertInvariants();
    }

    internal SmallImmutableArray(IReadOnlyList<T> items)
    {
        switch (items)
        {
            case []:
                break;
            case [var item1]:
                _item1 = item1;
                _length = 1;
                break;
            case [var item1, var item2]:
                _item1 = item1;
                _item2 = item2;
                _length = 2;
                break;
            default:
                _items = items.ToImmutableArray();
                _length = items.Count;
                break;
        }

        AssertInvariants();
    }

    internal SmallImmutableArray(bool _, ImmutableArray<T> items)
    {
        Debug.AssertNotEmpty(items);
        Debug.AssertGreaterThan(items.Length, InlineCapacity);

        _items = items;
        _length = items.Length;

        AssertInvariants();
    }

    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Require.ArgumentInRange(index, 0, Length - 1);

            AssertInvariants();

            if (!_items.IsDefault)
            {
                return _items[index];
            }
            else
            {
                Debug.AssertInRange(_length, 1, InlineCapacity);
                Debug.AssertInRange(index, 0, InlineCapacity - 1);

                return index switch
                {
                    0 => _item1!,
                    _ => _item2!,
                };
            }
        }
    }

    public int Length =>
        _length;

    public bool Contains(T item) =>
        IndexOf(item) >= 0;

    public bool Contains(T item, IEqualityComparer<T>? equalityComparer) =>
        IndexOf(item, equalityComparer) >= 0;

    public int IndexOf(T item) =>
        IndexOf(item, null);

    public int IndexOf(T item, IEqualityComparer<T>? equalityComparer)
    {
        AssertInvariants();

        equalityComparer ??= EqualityComparer<T>.Default;

        if (!_items.IsDefault)
        {
            return _items.IndexOf(item);
        }
        else
        {
            if (_length > 0 && equalityComparer.Equals(_item1!, item))
                return 0;

            if (_length > 0 && equalityComparer.Equals(_item2!, item))
                return 1;

            return -1;
        }
    }

#pragma warning disable CA1725 // Parameter names should match base declaration
    public void CopyTo(T[] destination, int destinationIndex)
#pragma warning restore CA1725 // Parameter names should match base declaration
    {
        Require.ArgumentNotNull(destination);
        Require.ArgumentInRange(destinationIndex, 0, destination.Length - Length);

        AssertInvariants();

        if (!_items.IsDefault)
        {
            _items.CopyTo(destination, destinationIndex);
        }
        else
        {
            if (_length > 0)
                destination[destinationIndex] = _item1!;

            if (_length > 1)
                destination[destinationIndex + 1] = _item2!;
        }
    }

    void ICollection.CopyTo(Array destination, int destinationIndex)
    {
        Require.ArgumentNotNull(destination);
        Require.ArgumentInRange(destinationIndex, 0, destination.Length - Length);

        AssertInvariants();

        if (!_items.IsDefault)
        {
            ((ICollection)_items).CopyTo(destination, destinationIndex);
        }
        else
        {
            if (_length > 0)
                destination.SetValue(_item1, destinationIndex);

            if (_length > 1)
                destination.SetValue(_item2, destinationIndex + 1);
        }
    }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [UnscopedRef]
    public ReadOnlySpan<T> AsSpan()
    {
        if (_items is { } items)
            return items.AsSpan();

        Debug.AssertInRange(_length, 0, InlineCapacity);
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _item1!), _length);
    }

    [UnscopedRef]
    public ReadOnlySpan<T> UnsafeInlineAsSpan(int length)
    {
        Require.ArgumentInRange(Length, 0, InlineCapacity);
        Require.ArgumentInRange(length, 0, InlineCapacity);

        Debug.AssertDefault(_items);
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _item1!), length);
    }
#endif

    [UnscopedRef]
    public ref readonly T ItemRef(int index)
    {
        Require.ArgumentInRange(index, 0, _length - 1);

        if (_items is { } items)
            return ref items.ItemRef(index);

        Debug.AssertInRange(_length, 1, InlineCapacity);
        Debug.AssertInRange(index, 0, InlineCapacity - 1);

        switch (index)
        {
            case 0:
                return ref _item1;
            default:
                return ref _item2;
        }
    }

    [Conditional("DEBUG")]
    private void AssertInvariants()
    {
        if (!_items.IsDefault)
        {
            Debug.AssertGreaterThan(_length, InlineCapacity);
            Debug.AssertEqual(_length, _items.Length);
            Debug.AssertDefault(_item1);
            Debug.AssertDefault(_item2);
        }
        else
        {
            Debug.AssertInRange(_length, 0, InlineCapacity);

            if (_length < 2)
                Debug.AssertDefault(_item2);

            if (_length < 1)
                Debug.AssertDefault(_item1);
        }
    }

    private static bool IsCompatibleObject(object? value)
    {
        // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
        // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
        return value is T || default(T) is null && value is null;
    }

    bool IList.Contains(object? value)
    {
        if (IsCompatibleObject(value))
            return Contains((T)value!);

        return false;
    }

    int IList.IndexOf(object? value)
    {
        if (IsCompatibleObject(value))
            return IndexOf((T)value!);

        return -1;
    }

    public bool Equals(SmallImmutableArray<T> other) =>
        Equals(other, null);

    public bool Equals(SmallImmutableArray<T> other, IEqualityComparer<T>? equalityComparer)
    {
        AssertInvariants();
     
        equalityComparer ??= EqualityComparer<T>.Default;

        if (_length != other._length)
            return false;

        if (!_items.IsDefault)
        {
            Debug.AssertNotDefault(other._items);
            return _items.SequenceEqual(other._items, equalityComparer);
        }
        else
        {
            if (_length > 0 && !equalityComparer.Equals(_item1!, other._item1!))
                return false;

            if (_length > 1 && !equalityComparer.Equals(_item2!, other._item2!))
                return false;

            return true;
        }
    }

    bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
    {
        Require.ArgumentValid(other, other is null or ISmallImmutableArray);
        Require.ArgumentNotNull(comparer);

        if (other is null)
            return false;

        var list = (IList)other;
        var length = _length;

        if (length != list.Count)
            return false;

        for (int i = 0; i < length; ++i)
        {
            if (!comparer.Equals(this[i], list[i]))
                return false;
        }

        return true;
    }

    public bool CompareTo(SmallImmutableArray<T> other, IEqualityComparer<T>? equalityComparer)
    {
        AssertInvariants();

        equalityComparer ??= EqualityComparer<T>.Default;

        if (_length != other._length)
            return false;

        if (!_items.IsDefault)
        {
            Debug.AssertNotDefault(other._items);
            return _items.SequenceEqual(other._items, equalityComparer);
        }
        else
        {
            if (_length > 0 && !equalityComparer.Equals(_item1!, other._item1!))
                return false;

            if (_length > 1 && !equalityComparer.Equals(_item2!, other._item2!))
                return false;

            return true;
        }
    }

    int IStructuralComparable.CompareTo(object? other, IComparer comparer)
    {
        Require.ArgumentValid(other, other is null or ISmallImmutableArray);
        Require.ArgumentNotNull(comparer);

        if (other is null)
            return 1;

        var list = (IList)other;
        var length = _length;

        var difference = length - list.Count;
        if (difference != 0)
            return difference;

        for (int i = 0; i < length; ++i)
        {
            var result = comparer.Compare(this[i], list[i]);
            if (result != 0)
                return result;
        }

        return 0;
    }

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
        Require.ArgumentNotNull(comparer);

        var hashCode = 0;

        foreach (var item in this)
            hashCode = HashCode.Combine(hashCode, comparer.GetHashCodeOrDefault(item));

        return hashCode;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is SmallImmutableArray<T> other && Equals(other);

    public override string ToString() =>
        ToString(CultureInfo.CurrentCulture);

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        ToString(formatProvider);

    private string ToString(IFormatProvider? formatProvider)
    {
        var handler = new DefaultInterpolatedStringHandler(
            literalLength: 22 + _length * 2,
            formattedCount: _length,
            formatProvider);

        handler.AppendLiteral($"SmallImmutableArray<{typeof(T).Name}> [");

        int index = 0;

        foreach (var item in this)
        {
            handler.AppendFormatted(item);
            if (index < _length - 1)
                handler.AppendLiteral(", ");

            ++index;
        }

        handler.AppendLiteral("]");

        return handler.ToStringAndClear();
    }

    int IReadOnlyCollection<T>.Count =>
        Length;

    int ICollection<T>.Count =>
        Length;

    bool ICollection<T>.IsReadOnly =>
        true;

    bool IList.IsFixedSize =>
        true;

    bool IList.IsReadOnly =>
        true;

    int ICollection.Count =>
        Length;

    bool ICollection.IsSynchronized =>
        true;

    object ICollection.SyncRoot =>
        throw new NotSupportedException();

    T IList<T>.this[int index]
    {
        get => this[index];
        set => throw new NotSupportedException();
    }

    object? IList.this[int index]
    {
        get => this[index];
        set => throw new NotImplementedException();
    }

    public Enumerator GetEnumerator() =>
        new(this);

#pragma warning disable RS0042 // Do not copy value
    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();
#pragma warning restore RS0042 // Do not copy value

    void IList<T>.Insert(int index, T item) =>
        throw new NotSupportedException();

    void IList<T>.RemoveAt(int index) =>
        throw new NotSupportedException();

    void ICollection<T>.Add(T item) =>
        throw new NotSupportedException();

    void ICollection<T>.Clear() =>
        throw new NotSupportedException();

    bool ICollection<T>.Remove(T item) =>
        throw new NotSupportedException();

    int IList.Add(object? value) =>
        throw new NotSupportedException();

    void IList.Clear() =>
        throw new NotSupportedException();
    
    void IList.Insert(int index, object? value) =>
        throw new NotSupportedException();

    void IList.Remove(object? value) =>
        throw new NotSupportedException();

    void IList.RemoveAt(int index) =>
        throw new NotSupportedException();

    readonly Array? ISmallImmutableArray.Array =>
        _items.IsDefault ? null : ImmutableCollectionsMarshalPolyfill.AsArray(_items);

    public static readonly SmallImmutableArray<T> Empty;

    [NonDefaultable]
    [StructLayout(LayoutKind.Auto)]
    public struct Enumerator : IEnumerator<T>
    {
        private readonly SmallImmutableArray<T> _array;
        private T? _current;
        private int _nextIndex;

        internal Enumerator(SmallImmutableArray<T> array)
        {
            _array = array;
        }

        public readonly T Current
            => _current!;

        readonly object? IEnumerator.Current =>
            Current;

        public bool MoveNext()
        {
            if (_nextIndex == 0)
                Debug.AssertDefault(_current);

            Debug.AssertInRange(_nextIndex, 0, _array.Length);

            if (_nextIndex >= _array.Length)
                return false;

            _current = _array[_nextIndex];
            ++_nextIndex;
            return true;
        }

        public void Reset()
        {
            _current = default;
            _nextIndex = 0;
        }

#pragma warning disable IDE0251 // Make member 'readonly'
        void IDisposable.Dispose()
#pragma warning restore IDE0251 // Make member 'readonly'
        {
        }
    }
}
