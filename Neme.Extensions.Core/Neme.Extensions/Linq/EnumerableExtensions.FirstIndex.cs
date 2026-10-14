namespace Neme.Extensions.Linq;

public static partial class EnumerableExtensions
{
    public static int FirstIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var result = FirstOrDefaultIndexCore(source, predicate);
        if (result == -1)
            throw new InvalidOperationException("Sequence contains no matching element");

        return result;
    }

    public static int FirstOrDefaultIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        return FirstOrDefaultIndexCore(source, predicate);
    }

    private static int FirstOrDefaultIndexCore<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        int i = 0;

        foreach (var element in source)
        {
            if (predicate(element))
                return i;

            ++i;
        }

        return -1;
    }
}
