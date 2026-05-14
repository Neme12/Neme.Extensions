using Neme.Extensions.Contracts;

namespace Neme.Extensions;

public static class SpanExtensions
{
    public static bool All<TSource>(this ReadOnlySpan<TSource> source, Func<TSource, bool> predicate)
    {
        Require.ArgumentNotNull(predicate);

        foreach (var item in source)
        {
            if (!predicate(item))
                return false;
        }

        return true;
    }

    public static bool Any<TSource>(this ReadOnlySpan<TSource> source, Func<TSource, bool> predicate)
    {
        Require.ArgumentNotNull(predicate);

        foreach (var item in source)
        {
            if (predicate(item))
                return true;
        }

        return false;
    }

    public static bool TryWrite(this Span<char> destination, string value, out int charsWritten)
    {
        Require.ArgumentNotNull(value);

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
