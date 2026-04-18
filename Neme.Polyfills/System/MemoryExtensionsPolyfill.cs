using System.Runtime.CompilerServices;
using System.Text;

namespace System;

public static class MemoryExtensionsPolyfill
{
#if !NETCOREAPP3_0_OR_GREATER
    public static SpanRuneEnumerator EnumerateRunes(this ReadOnlySpan<char> span)
    {
        return new SpanRuneEnumerator(span);
    }
#endif

#if !NETCOREAPP3_0_OR_GREATER
    public static SpanRuneEnumerator EnumerateRunes(this Span<char> span)
    {
        return new SpanRuneEnumerator(span);
    }
#endif

#if !NET9_0_OR_GREATER
    /// <summary>
    /// Determines whether the specified value appears at the start of the span.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to compare.</param>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <returns><see langword="true" /> if <paramref name="value" /> matches the beginning of <paramref name="span" />; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>? =>
        span.Length != 0 && (span[0]?.Equals(value) ?? (object?)value is null);
#endif

#if !NET10_0_OR_GREATER
    /// <summary>
    /// Determines whether the specified value appears at the start of the span.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to compare.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements, or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.</param>
    /// <returns><see langword="true" /> if <paramref name="value" /> matches the beginning of <paramref name="span" />; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null) =>
        span.Length != 0 &&
        (comparer is null ? EqualityComparer<T>.Default.Equals(span[0], value) : comparer.Equals(span[0], value));
#endif

#if !NET9_0_OR_GREATER
    /// <summary>
    /// Determines whether the specified value appears at the end of the span.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to compare.</param>
    /// <typeparam name="T">The type of the elements in the span.</typeparam>
    /// <returns><see langword="true" /> if <paramref name="value" /> matches the end of <paramref name="span" />; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>? =>
        span.Length != 0 && (span[^1]?.Equals(value) ?? (object?)value is null);
#endif

#if !NET10_0_OR_GREATER
    /// <summary>
    /// Determines whether the specified value appears at the end of the span.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the span.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to compare.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements, or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.</param>
    /// <returns><see langword="true" /> if <paramref name="value" /> matches the end of <paramref name="span" />; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null) =>
        span.Length != 0 &&
        (comparer is null ? EqualityComparer<T>.Default.Equals(span[^1], value) : comparer.Equals(span[^1], value));
#endif
}
