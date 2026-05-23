using System.Runtime.CompilerServices;
using System.Text;

namespace System;

public static class StringPolyfill
{
    /// <summary>Maximum length allowed for a string.</summary>
    /// <remarks>Keep in sync with AllocateString in gchelpers.cpp.</remarks>
    internal const int MaxLength = 0x3FFFFFDF;

    public static string Ctor(ReadOnlySpan<char> value)
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return new string(value);
#else
        return value.ToString();
#endif
    }

    extension(string)
    {
#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER)
        internal static unsafe int strlen(byte* ptr) => SpanHelpers.IndexOfNullByte(ptr);
#endif

        /// <summary>Creates a new string by using the specified provider to control the formatting of the specified interpolated string.</summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="handler">The interpolated string.</param>
        /// <returns>The string that results for formatting the interpolated string using the specified format provider.</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static string Create(IFormatProvider? provider, [InterpolatedStringHandlerArgument(nameof(provider))] ref DefaultInterpolatedStringHandler handler) =>
            handler.ToStringAndClear();
#pragma warning restore IDE0060 // Remove unused parameter

        /// <summary>Creates a new string by using the specified provider to control the formatting of the specified interpolated string.</summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="initialBuffer">The initial buffer that may be used as temporary space as part of the formatting operation. The contents of this buffer may be overwritten.</param>
        /// <param name="handler">The interpolated string.</param>
        /// <returns>The string that results for formatting the interpolated string using the specified format provider.</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static string Create(IFormatProvider? provider, Span<char> initialBuffer, [InterpolatedStringHandlerArgument(nameof(provider), nameof(initialBuffer))] ref DefaultInterpolatedStringHandler handler) =>
            handler.ToStringAndClear();
#pragma warning restore IDE0060 // Remove unused parameter
    }

    /// <summary>Copies the contents of this string into the destination span.</summary>
    /// <param name="destination">The span into which to copy this string's contents.</param>
    /// <exception cref="ArgumentException">The destination span is shorter than the source string.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(this string @string, Span<char> destination)
    {
#if NET6_0_OR_GREATER
        @string.CopyTo(destination);
#else
        if (@string is null)
        {
            throw new ArgumentNullException(nameof(@string));
        }

        if ((uint)@string.Length <= (uint)destination.Length)
        {
            @string.AsSpan().CopyTo(destination);
        }
        else
        {
            ThrowHelper.ThrowArgumentException_DestinationTooShort();
        }
#endif
    }

    /// <summary>Copies the contents of this string into the destination span.</summary>
    /// <param name="destination">The span into which to copy this string's contents.</param>
    /// <returns>true if the data was copied; false if the destination was too short to fit the contents of the string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo(this string @string, Span<char> destination)
    {
#if NET6_0_OR_GREATER
        return @string.TryCopyTo(destination);
#else
        if (@string is null)
        {
            throw new ArgumentNullException(nameof(@string));
        }

        bool retVal = false;
        if ((uint)@string.Length <= (uint)destination.Length)
        {
            retVal = @string.AsSpan().TryCopyTo(destination);
        }
        return retVal;
#endif
    }

    /// <summary>
    /// Returns an enumeration of <see cref="Rune"/> from this string.
    /// </summary>
    /// <remarks>
    /// Invalid sequences will be represented in the enumeration by <see cref="Rune.ReplacementChar"/>.
    /// </remarks>
    public static StringRuneEnumerator EnumerateRunes(this string @string)
    {
#if NETCOREAPP3_0_OR_GREATER
        return @string.EnumerateRunes();
#else
        if (@string is null)
        {
            throw new ArgumentNullException(nameof(@string));
        }

        return new StringRuneEnumerator(@string);
#endif
    }

    public static bool StartsWith(this string @string, char value)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return @string.StartsWith(value);
#else
        if (@string is null)
        {
            throw new ArgumentNullException(nameof(@string));
        }

        return @string.Length != 0 && @string[0] == value;
#endif
    }

    public static bool EndsWith(this string @string, char value)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return @string.EndsWith(value);
#else
        if (@string is null)
        {
            throw new ArgumentNullException(nameof(@string));
        }

        int lastPos = @string.Length - 1;
        return (uint)lastPos < (uint)@string.Length && @string[lastPos] == value;
#endif
    }
}
