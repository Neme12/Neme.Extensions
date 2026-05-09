using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.Collections.Generic;

public static class CollectionExtensionsPolyfill
{
    public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return CollectionExtensions.GetValueOrDefault(dictionary, key);
#else
        return dictionary.GetValueOrDefault(key, default!);
#endif
    }

    public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return CollectionExtensions.GetValueOrDefault(dictionary, key, defaultValue);
#else
        if (dictionary is null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
        }

        return dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;
#endif
    }

    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return CollectionExtensions.TryAdd(dictionary, key, value);
#else
        if (dictionary is null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
        }

        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
            return true;
        }

        return false;
#endif
    }

    public static bool Remove<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        [MaybeNullWhen(false)] out TValue value)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return CollectionExtensions.Remove(dictionary, key, out value);
#else
        if (dictionary is null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
        }

        if (dictionary.TryGetValue(key, out value))
        {
            dictionary.Remove(key);
            return true;
        }

        value = default;
        return false;
#endif
    }

    public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
    {
#if NET7_0_OR_GREATER
        return CollectionExtensions.AsReadOnly(list);
#else
        return new ReadOnlyCollection<T>(list);
#endif
    }

    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
#if NET7_0_OR_GREATER
        return CollectionExtensions.AsReadOnly(dictionary);
#else
        return new ReadOnlyDictionary<TKey, TValue>(dictionary);
#endif
    }

    public static void AddRange<T>(this List<T> list, ReadOnlySpan<T> source)
    {
#if NET8_0_OR_GREATER
        CollectionExtensions.AddRange(list, source);
#else
        if (list is null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.list);
        }

        foreach (var item in source)
            list.Add(item);
#endif
    }

    public static void InsertRange<T>(this List<T> list, int index, ReadOnlySpan<T> source)
    {
#if NET8_0_OR_GREATER
        CollectionExtensions.InsertRange(list, index, source);
#else
        if (list is null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.list);
        }

        if ((uint)index > (uint)list.Count)
        {
            ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
        }

        foreach (var item in source)
            list.Insert(index++, item);
#endif
    }

    public static void CopyTo<T>(this List<T> list, Span<T> destination)
    {
#if NET8_0_OR_GREATER
        CollectionExtensions.CopyTo(list, destination);
#else
        if (list is null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.list);
        }

        CollectionsMarshal.AsSpan(list).CopyTo(destination);
#endif
    }
}
