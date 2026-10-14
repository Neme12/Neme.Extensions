using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neme.Extensions.Buffers;
using Neme.Extensions.Collections;
using Neme.Extensions.Contracts;
using Neme.Extensions.InteropServices;
using Neme.Extensions.Linq;

namespace Neme.Extensions.Collections;

public static class SmallImmutableArray
{
    public static SmallImmutableArray<T> Create<T>(T item) =>
        new(item);

    public static SmallImmutableArray<T> Create<T>(T item1, T item2) =>
        new(item1, item2);

    public static SmallImmutableArray<T> Create<T>(T item1, T item2, T item3) =>
        new(ImmutableArray.Create(item1, item2, item3));

    public static SmallImmutableArray<T> Create<T>(T item1, T item2, T item3, T item4) =>
        new(ImmutableArray.Create(item1, item2, item3, item4));

    public static SmallImmutableArray<T> Create<T>(params T[] items)
    {
        Require.ArgumentNotNull(items);

        return new(items.AsSpan());
    }

    public static SmallImmutableArray<T> Create<T>(ImmutableArray<T> items)
    {
        Require.ArgumentNotDefault(items);

        return new(items);
    }

    public static SmallImmutableArray<T> Create<T>(ReadOnlySpan<T> items) =>
        new(items);

    public static SmallImmutableArray<T> Create<T>(Span<T> items) =>
        new(items);

    public static SmallImmutableArray<T> CreateRange<T>(IEnumerable<T> items)
    {
        Require.ArgumentNotNull(items);

        if (items is ISmallImmutableArray { Array: { } array })
            return new(default, ImmutableCollectionsMarshalPolyfill.AsImmutableArray((T[])array));

        if (items is IReadOnlyList<T> list)
            return new(list);

        var builder = CreateBuilder<T>(items.GetNonEnumeratedCount2() ?? 0);
        builder.AddRange(items);
        return builder.DrainToImmutable();
    }

    public static SmallImmutableArray<T>.Builder CreateBuilder<T>() =>
        new();

    public static SmallImmutableArray<T>.Builder CreateBuilder<T>(int initialCapacity)
    {
        Require.ArgumentNotNegative(initialCapacity);

        return new(initialCapacity);
    }

    public static TResult WithSpan<T, TResult>(
        this in SmallImmutableArray<T> array,
        ReadOnlySpanFunc<T, TResult> action)
        where T : unmanaged
    {
        Require.ArgumentNotNull(action);

        if (array._items is { IsDefault: false } items)
            return action(items.AsSpan());

        Debug.AssertInRange(array._length, 0, SmallImmutableArray<T>.InlineCapacity);

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return action(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in array._item1!), array._length));
#else
        unsafe
        {
            fixed (T* pointer = &array._item1)
                return action(new ReadOnlySpan<T>(pointer, array.Length));
        }
#endif
    }

    public static TResult WithSpan<T, TState, TResult>(
        this in SmallImmutableArray<T> array,
        TState state,
        ReadOnlySpanFunc<T, TState, TResult> action)
        where T : unmanaged
    {
        Require.ArgumentNotNull(action);

        if (array._items is { IsDefault: false } items)
            return action(items.AsSpan(), state);

        Debug.AssertInRange(array._length, 0, SmallImmutableArray<T>.InlineCapacity);

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return action(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in array._item1!), array._length), state);
#else
        unsafe
        {
            fixed (T* pointer = &array._item1)
                return action(new ReadOnlySpan<T>(pointer, array.Length), state);
        }
#endif
    }

    public static TResult WithSpan<T, TResult>(
        this in SmallImmutableArray<T>.Builder builder,
        ReadOnlySpanFunc<T, TResult> action)
        where T : unmanaged
    {
        Require.ArgumentNotNull(action);

        if (builder._items is { } items)
            return action(ImmutableCollectionsMarshalExtensions.AsSpan(items));

        Debug.AssertInRange(builder._count, 0, SmallImmutableArray<T>.InlineCapacity);

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return action(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in builder._item1!), builder._count));
#else
        unsafe
        {
            fixed (T* pointer = &builder._item1)
                return action(new ReadOnlySpan<T>(pointer, builder._count));
        }
#endif
    }

    public static TResult WithSpan<T, TState, TResult>(
        this in SmallImmutableArray<T>.Builder builder,
        TState state,
        ReadOnlySpanFunc<T, TState, TResult> action)
        where T : unmanaged
    {
        Require.ArgumentNotNull(action);

        if (builder._items is { } items)
            return action(ImmutableCollectionsMarshalExtensions.AsSpan(items), state);

        Debug.AssertInRange(builder._count, 0, SmallImmutableArray<T>.InlineCapacity);

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return action(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in builder._item1!), builder._count), state);
#else
        unsafe
        {
            fixed (T* pointer = &builder._item1)
                return action(new ReadOnlySpan<T>(pointer, builder._count), state);
        }
#endif
    }

    public static ref readonly T GetPinnableReference<T>(this in SmallImmutableArray<T> array)
    {
        if (array._items is { IsDefault: false } items)
            return ref ImmutableCollectionsMarshalPolyfill.AsArray(items)![0];

        Debug.AssertInRange(array._length, 0, SmallImmutableArray<T>.InlineCapacity);
        return ref array._item1!;
    }

    public static bool SequenceEqual<T>(this SmallImmutableArray<T> array, SmallImmutableArray<T> other)
        where T : IEquatable<T>?
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#pragma warning disable CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
        return array.AsSpan().SequenceEqual(other.AsSpan());
#pragma warning restore CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
#else
        return array.SequenceEqual((IEnumerable<T>)other);
#endif
    }

    public static int SequenceCompareTo<T>(this SmallImmutableArray<T> array, SmallImmutableArray<T> other)
        where T : IComparable<T>?
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#pragma warning disable CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
        return array.AsSpan().SequenceCompareTo(other.AsSpan());
#pragma warning restore CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
#else
        var arrayLength = array.Length;
        var otherLength = other.Length;

        int minLength = arrayLength;
        if (minLength > otherLength)
            minLength = otherLength;

        for (int i = 0; i < minLength; ++i)
        {
            T lookUp = other[i];
            int result = array[i]?.CompareTo(lookUp) ?? ((object?)lookUp is null ? 0 : -1);
            if (result != 0)
                return result;
        }

        return arrayLength.CompareTo(otherLength);
#endif
    }

    public static SmallImmutableArray<TSource> ToSmallImmutableArray<TSource>(this IEnumerable<TSource> items)
    {
        Require.ArgumentNotNull(items);

        return CreateRange(items);
    }

    public static ImmutableArray<TSource> ToSmallImmutableArray<TSource>(this ImmutableArray<TSource>.Builder builder)
    {
        Require.ArgumentNotNull(builder);

        return builder.ToImmutable();
    }
}
