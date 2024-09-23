namespace Neme.Extensions;

public static class SpanExtensions
{
    public static bool TryWrite(this Span<char> destination, string value, out int charsWritten)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return TryWrite(destination, value.AsSpan(), out charsWritten);
    }

    public static bool TryWrite(this Span<char> destination, ReadOnlySpan<char> value, out int charsWritten)
    {
        if (value.TryCopyTo(destination))
        {
            charsWritten = value.Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }
}
