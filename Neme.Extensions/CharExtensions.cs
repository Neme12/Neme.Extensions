using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Neme.Extensions;

internal static class CharExtensions
{
    public static char Parse(string s) =>
        char.Parse(s);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static char Parse(string s, IFormatProvider? provider) =>
#pragma warning disable CA1305 // Specify IFormatProvider
        Parse(s);
#pragma warning restore CA1305 // Specify IFormatProvider

    public static char Parse(ReadOnlySpan<char> s)
    {
        if (s.Length != 1)
            throw new FormatException("String must be exactly one character long.");
        
        return s[0];
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static char Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
#pragma warning disable CA1305 // Specify IFormatProvider
        Parse(s);
#pragma warning restore CA1305 // Specify IFormatProvider

    public static bool TryParse([NotNullWhen(true)] string? s, out char result) =>
        char.TryParse(s, out result);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out char result) =>
        TryParse(s, out result);

    public static bool TryParse(ReadOnlySpan<char> s, out char result)
    {
        if (s.Length != 1)
        {
            result = default;
            return false;
        }

        result = s[0];
        return true;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out char result) =>
        TryParse(s, out result);
}
