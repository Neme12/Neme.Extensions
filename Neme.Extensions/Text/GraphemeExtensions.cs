using Neme.Extensions.Contracts;
using System.Text;

namespace Neme.Extensions.Text;

public static class GraphemeExtensions
{
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public static ReadOnlySpan<Rune> AsSpan(this in Grapheme character)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        if (character._runes.Length == 0)
            return character._runes.UnsafeInlineAsSpan(1);

        return character._runes.AsSpan();
#pragma warning restore CS0618 // Type or member is obsolete
    }
#endif

    public static StringGraphemeEnumerator EnumerateGraphemes(this string str)
    {
        Require.ArgumentNotNull(str);

        return new(str);
    }

    public static SpanGraphemeEnumerator EnumerateGraphemes(this ReadOnlySpan<char> span) =>
        new(span);

    public static SpanGraphemeEnumerator EnumerateGraphemes(this Span<char> span) =>
        new(span);
}
