namespace Neme.Extensions.Linq;

public static partial class EnumerableExtensions
{
    public static int LastIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var result = LastOrDefaultIndexCore(source, predicate);
        if (result == -1)
            throw new InvalidOperationException("Sequence contains no matching element");

        return result;
    }

    public static int LastOrDefaultIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        return LastOrDefaultIndexCore(source, predicate);
    }

    private static int LastOrDefaultIndexCore<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source is IList<TSource> list)
        {
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (predicate(list[i]))
                    return i;
            }
        }
        else
        {
            int i = 0;

            using (var e = source.GetEnumerator())
            {
                for (; e.MoveNext(); ++i)
                {
                    var result = i;

                    if (predicate(e.Current))
                    {
                        for (; e.MoveNext(); ++i)
                        {
                            if (predicate(e.Current))
                                result = i;
                        }

                        return result;
                    }
                }
            }
        }

        return -1;
    }
}
