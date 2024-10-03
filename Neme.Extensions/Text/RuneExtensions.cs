using System.Buffers;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Neme.Extensions.Text;

#if NETCOREAPP3_0_OR_GREATER
internal static class RuneExtensions
{
    public static Rune Parse(string s)
    {
        if (s is null)
            throw new ArgumentNullException(nameof(s));

#pragma warning disable CA1305 // Specify IFormatProvider
        return Parse(s.AsSpan());
#pragma warning restore CA1305 // Specify IFormatProvider
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Rune Parse(string s, IFormatProvider? provider) =>
#pragma warning disable CA1305 // Specify IFormatProvider
        Parse(s);
#pragma warning restore CA1305 // Specify IFormatProvider

    public static Rune Parse(ReadOnlySpan<char> s)
    {
        if (Rune.DecodeFromUtf16(s, out var rune, out var charsConsumed) is not OperationStatus.Done || charsConsumed != s.Length)
            return ThrowHelper.ThrowFormat<Rune>(s.ToString());

        return rune;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Rune Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
#pragma warning disable CA1305 // Specify IFormatProvider
        Parse(s);
#pragma warning restore CA1305 // Specify IFormatProvider

    public static bool TryParse([NotNullWhen(true)] string? s, out Rune result) =>
        TryParse(s.AsSpan(), out result);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Rune result) =>
        TryParse(s, out result);

    public static bool TryParse(ReadOnlySpan<char> s, out Rune result) =>
        Rune.DecodeFromUtf16(s, out result, out var charsConsumed) is OperationStatus.Done && charsConsumed == s.Length;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Rune result) =>
        TryParse(s, out result);
}
#endif
