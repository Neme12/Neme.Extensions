using System.Collections;
using Neme.Extensions.Contracts;

namespace Neme.Extensions.Linq;

public static partial class EnumerableExtensions
{
    public static IEnumerable<(TSource value, int index)> WithIndex<TSource>(
        this IEnumerable<TSource> source)
    {
        return source.Select((value, index) => (value, index));
    }

    public static int? GetNonEnumeratedCount2<TSource>(this IEnumerable<TSource> source)
    {
        if (source.TryGetNonEnumeratedCount2(out var count))
            return count;

        return null;
    }

    public static bool TryGetNonEnumeratedCount2<TSource>(this IEnumerable<TSource> source, out int count)
    {
        Require.ArgumentNotNull(source);

        switch (source)
        {
            case IReadOnlyCollection<TSource> collection:
                count = collection.Count;
                return true;
#if !NET6_0_OR_GREATER
                case ICollection<TSource> collection:
                    count = collection.Count;
                    return true;
                case ICollection collection:
                    count = collection.Count;
                    return true;
#endif
            default:
#if NET6_0_OR_GREATER
                return Enumerable.TryGetNonEnumeratedCount(source, out count);
#else
                count = default;
                return false;
#endif
        }
    }

    public static Optional<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source)
    {
        Require.ArgumentNotNull(source);

        if (source is IList<TSource> list)
        {
            if (list.Count > 0)
                return list[0];
        }
        else
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                    return enumerator.Current;
            }
        }

        return default;
    }

    public static Optional<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        Require.ArgumentNotNull(source);
        Require.ArgumentNotNull(predicate);

        foreach (TSource element in source)
        {
            if (predicate(element))
                return element;
        }

        return default;
    }

    public static Optional<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source)
    {
        Require.ArgumentNotNull(source);

        if (source is IList<TSource> list)
        {
            var count = list.Count;
            if (count is > 0)
                return list[count - 1];

            return default;
        }
        else
        {
            Optional<TSource> result = default;

            foreach (var item in source)
                result = item;

            return result;
        }
    }


    public static Optional<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        Require.ArgumentNotNull(source);
        Require.ArgumentNotNull(predicate);

        if (source is IList<TSource> list)
        {
            for (int i = list.Count - 1; i >= 0; --i)
            {
                var item = list[i];
                if (predicate(item))
                    return item;
            }

            return default;
        }
        else
        {
            Optional<TSource> result = default;

            foreach (var item in source)
            {
                if (predicate(item))
                    result = item;
            }

            return result;
        }
    }

    public static Optional<TSource> ElementAtOrNone<TSource>(this IEnumerable<TSource> source, int index)
    {
        Require.ArgumentNotNull(source);

        if (source is IList<TSource> list)
        {
            return index >= 0 && index < list.Count
                ? list[index]
                : Optional<TSource>.None;
        }

        if (index >= 0)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (index == 0)
                        return enumerator.Current;

                    --index;
                }
            }
        }

        return default;
    }
}
