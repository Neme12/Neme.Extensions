namespace Neme.Extensions;

internal static class SpanExtensions
{
    public static bool TryWrite(this Span<char> destination, string value, out int charsWritten)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        if (value.AsSpan().TryCopyTo(destination))
        {
            charsWritten = value.Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }
}
