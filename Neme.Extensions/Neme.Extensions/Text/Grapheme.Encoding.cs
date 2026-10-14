using System.Globalization;

namespace Neme.Extensions.Text;

public partial struct Grapheme
{
    public int EncodeToUtf16(Span<char> destination)
    {
        var charsWritten = 0;

        foreach (var rune in Runes)
            charsWritten += rune.EncodeToUtf16(destination);

        return charsWritten;
    }

    public int EncodeToUtf8(Span<byte> destination)
    {
        var bytesWritten = 0;

        foreach (var rune in Runes)
            bytesWritten += rune.EncodeToUtf8(destination);

        return bytesWritten;
    }

    public bool TryEncodeToUtf16(Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;

        foreach (var rune in Runes)
        {
            if (!rune.TryEncodeToUtf16(destination, out var written))
                return false;

            charsWritten += written;
        }

        return true;
    }

    public bool TryEncodeToUtf8(Span<byte> destination, out int bytesWritten)
    {
        bytesWritten = 0;

        foreach (var rune in Runes)
        {
            if (!rune.TryEncodeToUtf8(destination, out var written))
                return false;

            bytesWritten += written;
        }

        return true;
    }

    public override string ToString()
    {
        var charCount = Length * 2;

        using (var builder = new SpanBuilder(
            charCount,
            CultureInfo.InvariantCulture,
            charCount <= 64 ? stackalloc char[charCount] : default))
        {
            foreach (var rune in Runes)
                builder.AppendFormatted(rune);

            return builder.ToString();
        }
    }

    string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
        ToString();

#if NET6_0_OR_GREATER
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) =>
        TryEncodeToUtf16(destination, out charsWritten);
#endif

#if NET8_0_OR_GREATER
    bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) =>
        TryEncodeToUtf8(utf8Destination, out bytesWritten);
#endif
}
