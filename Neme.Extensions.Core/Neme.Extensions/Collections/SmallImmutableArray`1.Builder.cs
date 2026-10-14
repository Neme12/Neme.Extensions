using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neme.Extensions.Contracts;
using Neme.Extensions.Linq;
using Roslyn.Utilities;

namespace Neme.Extensions.Collections;

public readonly partial struct SmallImmutableArray<T>
{
    [NonCopyable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Builder : IReadOnlyList<T>, IList<T>
    {
        internal T? _item0;
        internal T? _item1;
        internal ImmutableArray<T>.Builder? _items;
        internal int _count;

        internal Builder(int initialCapacity)
        {
            if (initialCapacity > InlineCapacity)
                _items = ImmutableArray.CreateBuilder<T>((int)BitOperationsPolyfill.RoundUpToPowerOf2((uint)initialCapacity));

            AssertInvariants();
        }

        public readonly int Count =>
            _count;

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                Require.ArgumentInRange(index, 0, Count - 1);

                AssertInvariants();

                if (_items is not null)
                {
                    return _items[index];
                }
                else
                {
                    Debug.AssertInRange(_count, 1, InlineCapacity);
                    Debug.AssertInRange(index, 0, InlineCapacity - 1);

                    return index switch
                    {
                        0 => _item0!,
                        _ => _item1!,
                    };
                }
            }
            set
            {
                Require.ArgumentInRange(index, 0, Count - 1);

                AssertInvariants();

                try
                {

                    if (_items is not null)
                    {
                        _items[index] = value;
                    }
                    else
                    {
                        Debug.AssertInRange(_count, 1, InlineCapacity);
                        Debug.AssertInRange(index, 0, InlineCapacity - 1);

                        switch (index)
                        {
                            case 0:
                                _item0 = value;
                                break;
                            default:
                                _item1 = value;
                                break;
                        }
                    }
                }
                finally
                {
                    AssertInvariants();
                }
            }
        }

        public int Capacity
        {
            readonly get
            {
                AssertInvariants();

                return _items is not null ? _items.Capacity : InlineCapacity;
            }
            set
            {
                Require.ArgumentGreaterThanOrEqual(value, InlineCapacity);
                Require.ArgumentGreaterThanOrEqual(value, _count);

                AssertInvariants();

                try
                {
                    if (value > InlineCapacity)
                    {
                        if (_items is null)
                            MoveInlineToBuilderCountIsLessThanMax(value);
                        else
                            _items.Capacity = value;
                    }
                    else
                    {
                        if (_items is not null)
                            MoveBuilderToInlineCountIsLessThanMax();
                    }
                }
                finally
                {
                    AssertInvariants();
                }
            }
        }

        public void EnsureCapacity(int capacity)
        {
            Require.ArgumentNotNegative(capacity);

            if (capacity <= _count)
                return;

            if (capacity > 2)
            {
                if (_items is null)
                    MoveInlineToBuilderCountIsLessThanMax(capacity);
                else
                    _items.EnsureCapacity(capacity);
            }
        }

        public void Add(T item)
        {
            AssertInvariants();

            try
            {
                if (_items is not null)
                {
                    _items.Add(item);
                }
                else
                {
                    Debug.AssertInRange(_count, 0, InlineCapacity);

                    switch (_count)
                    {
                        case 0:
                            _item0 = item;
                            break;
                        case 1:
                            _item1 = item;
                            break;
                        default:
                            MoveInlineToBuilderCountIsMax(4);
                            _items.Add(item);
                            break;
                    }
                }

                ++_count;
            }
            finally
            {
                AssertInvariants();
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            AssertInvariants();

            try
            {
                if (items.TryGetNonEnumeratedCount2(out var count) &&
                    _count + count is var newCount and > InlineCapacity)
                {
                    if (_items is null)
                        MoveInlineToBuilderCountIsLessThanMax(newCount);

                    _items.AddRange(items);
                    _count = _items.Count;
                }
                else
                {
                    foreach (var item in items)
                    {
                        Add(item);
                        ++_count;
                    }
                }
            }
            finally
            {
                AssertInvariants();
            }
        }

        public void Insert(int index, T item)
        {
            Require.ArgumentInRange(index, 0, Count);

            AssertInvariants();

            try
            {
                if (_items is not null)
                {
                    _items.Insert(index, item);
                }
                else
                {
                    Debug.AssertInRange(_count, 0, InlineCapacity);

                    switch (_count)
                    {
                        case 0:
                            _item0 = item;
                            break;
                        case 1:
                            if (index is 0)
                            {
                                _item1 = _item0;
                                _item0 = item;
                            }
                            else
                            {
                                _item1 = item;
                            }
                            break;
                        default:
                            MoveInlineToBuilderCountIsMax(4);
                            _items.Insert(index, item);
                            break;
                    }
                }

                ++_count;
            }
            finally
            {
                AssertInvariants();
            }
        }

        public void RemoveAt(int index)
        {
            Require.ArgumentInRange(index, 0, Count - 1);

            AssertInvariants();

            try
            {
                if (_items is not null)
                {
                    _items.RemoveAt(index);
                }
                else
                {
                    Debug.AssertInRange(_count, 0, InlineCapacity);

                    switch (_count)
                    {
                        case 0:
                            break;
                        case 1:
                            _item0 = default;
                            break;
                        default:
                            if (index is 0)
                            {
                                _item0 = _item1;
                                _item1 = default;
                            }
                            else
                            {
                                _item1 = default;
                            }
                            break;
                    }
                }

                --_count;
            }
            finally
            {
                AssertInvariants();
            }
        }

        public bool Remove(T item) =>
            Remove(item, null);

        public bool Remove(T item, EqualityComparer<T>? equalityComparer)
        {
            AssertInvariants();

            try
            {
                equalityComparer ??= EqualityComparer<T>.Default;

                if (_items is not null)
                {
                    if (_items.Remove(item))
                    {
                        --_count;
                        return true;
                    }
                }
                else
                {
                    Debug.AssertInRange(_count, 0, InlineCapacity);

                    if (_count > 0 && equalityComparer.Equals(_item0!, item))
                    {
                        _item0 = _item1;
                        _item1 = default;
                        --_count;
                        return true;
                    }

                    if (_count > 1 && equalityComparer.Equals(_item1!, item))
                    {
                        _item1 = default;
                        --_count;
                        return true;
                    }
                }

                return false;
            }
            finally
            {
                AssertInvariants();
            }
        }

        [MemberNotNull(nameof(_items))]
        private void MoveInlineToBuilderCountIsLessThanMax(int capacity)
        {
            Debug.AssertNull(_items);
            Debug.AssertInRange(_count, 0, InlineCapacity);

            _items = ImmutableArray.CreateBuilder<T>(capacity);

            if (_count > 0)
            {
                _items.Add(_item0!);
                _item0 = default;
            }

            if (_count > 1)
            {
                _items.Add(_item1!);
                _item1 = default;
            }
        }

        [MemberNotNull(nameof(_items))]
        private void MoveInlineToBuilderCountIsMax(int capacity)
        {
            Debug.AssertNull(_items);
            Debug.AssertEqual(_count, InlineCapacity);

            _items = ImmutableArray.CreateBuilder<T>(capacity);
            _items.Add(_item0!);
            _items.Add(_item1!);

            _item0 = default;
            _item1 = default;
        }

        private void MoveBuilderToInlineCountIsLessThanMax()
        {
            Debug.AssertNotNull(_items);
            Debug.AssertInRange(_count, 0, InlineCapacity);

            if (_count > 0)
                _item0 = _items[0];

            if (_count > 1)
                _item1 = _items[1];

            _items = null;
        }

        public void Clear()
        {
            AssertInvariants();

            try
            {
                _item0 = default;
                _item1 = default;
                _items?.Clear();
                _count = 0;
            }
            finally
            {
                AssertInvariants();
            }
        }

        public readonly bool Contains(T item) =>
            IndexOf(item) >= 0;

        public readonly bool Contains(T item, IEqualityComparer<T>? equalityComparer) =>
            IndexOf(item, equalityComparer) >= 0;

        public readonly int IndexOf(T item) =>
            IndexOf(item, null);

        public readonly int IndexOf(T item, IEqualityComparer<T>? equalityComparer)
        {
            AssertInvariants();

            equalityComparer ??= EqualityComparer<T>.Default;

            if (_items is not null)
            {
                return _items.IndexOf(item);
            }
            else
            {
                if (_count > 0 && equalityComparer.Equals(_item0!, item))
                    return 0;

                if (_count > 1 && equalityComparer.Equals(_item1!, item))
                    return 1;

                return -1;
            }
        }

#pragma warning disable CA1725 // Parameter names should match base declaration
        public readonly void CopyTo(T[] destination, int destinationIndex)
#pragma warning restore CA1725 // Parameter names should match base declaration
        {
            Require.ArgumentNotNull(destination);
            Require.ArgumentInRange(destinationIndex, 0, destination.Length - Count);

            AssertInvariants();

            if (_items is not null)
            {
                _items.CopyTo(destination, destinationIndex);
            }
            else
            {
                if (_count > 0)
                    destination[destinationIndex] = _item0!;

                if (_count > 1)
                    destination[destinationIndex + 1] = _item1!;
            }
        }

        public readonly SmallImmutableArray<T> ToImmutable()
        {
            AssertInvariants();

            if (_items is { } items)
            {
                return new(default, items.ToImmutable());
            }
            else
            {
                return InlineToImmutable();
            }
        }

        public SmallImmutableArray<T> MoveToImmutable()
        {
            AssertInvariants();

            try
            {
                if (_items is { } items)
                {
                    return new(default, items.MoveToImmutable());
                }
                else
                {
                    var result = InlineToImmutable();
                    this = default;
                    return result;
                }
            }
            finally
            {
                AssertInvariants();
            }
        }

        public SmallImmutableArray<T> DrainToImmutable()
        {
            AssertInvariants();

            try
            {
                if (_items is { } items)
                {
                    return new(default, items.DrainToImmutable());
                }
                else
                {
                    var result = InlineToImmutable();
                    this = default;
                    return result;
                }
            }
            finally
            {
                AssertInvariants();
            }
        }

        private readonly SmallImmutableArray<T> InlineToImmutable()
        {
            Debug.AssertNull(_items);
            Debug.AssertInRange(_count, 0, InlineCapacity);

            return _count switch
            {
                0 => default,
                1 => new(_item0!),
                _ => new(_item0!, _item1!),
            };
        }

        [Conditional("DEBUG")]
        private readonly void AssertInvariants()
        {
            if (_items is not null)
            {
                Debug.AssertGreaterThan(_count, InlineCapacity);
                Debug.AssertEqual(_count, _items.Count);
                Debug.AssertDefault(_item0);
                Debug.AssertDefault(_item1);
            }
            else
            {
                Debug.AssertInRange(_count, 0, InlineCapacity);

                if (_count < 2)
                    Debug.AssertDefault(_item1);

                if (_count < 1)
                    Debug.AssertDefault(_item0);
            }
        }

        readonly bool ICollection<T>.IsReadOnly =>
            false;

#pragma warning disable RS0042 // Do not copy value
        public readonly Enumerator GetEnumerator() =>
            new(this);

        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
            GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
#pragma warning restore RS0042 // Do not copy value

        [NonCopyable]
        [NonDefaultable]
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<T>
        {
            private readonly Builder _builder;
            private T? _current;
            private int _nextIndex;

            internal Enumerator(Builder builder)
            {
#pragma warning disable RS0042 // Do not copy value
                // We *do* copy the value, but only use readonly methods, so it's safe.
                Debug.AssertNotNull(builder);

                _builder = builder;
#pragma warning restore RS0042 // Do not copy value
            }

            public readonly T Current
                => _current!;

            readonly object? IEnumerator.Current =>
                Current;

            public bool MoveNext()
            {
                if (_nextIndex == 0)
                    Debug.AssertDefault(_current);

                Debug.AssertInRange(_nextIndex, 0, _builder.Count);

                if (_nextIndex == _builder.Count)
                    return false;

                _current = _builder[_nextIndex];
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
}
