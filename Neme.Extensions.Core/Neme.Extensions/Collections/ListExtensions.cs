using Neme.Extensions.Contracts;

namespace Neme.Extensions.Collections;

public static class ListExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        Require.ArgumentNotNull(collection);
        Require.ArgumentNotNull(items);

        switch (collection)
        {
            case List<T> listGeneric:
                listGeneric.AddRange(items);
                break;

            case ImmutableArray<T>.Builder builder:
                builder.AddRange(items);
                break;

            default:
                foreach (var item in items)
                    collection.Add(item);
                break;
        }
    }

    public static void AddRange<T>(this ICollection<T> collection, ReadOnlySpan<T> items)
    {
        Require.ArgumentNotNull(collection);

        switch (collection)
        {
#if NET8_0_OR_GREATER
            case List<T> listGeneric:
                CollectionExtensions.AddRange(listGeneric, items);
                break;
#endif

            case ImmutableArray<T>.Builder builder:
                builder.AddRange(items);
                break;

            default:
                foreach (var item in items)
                    collection.Add(item);
                break;
        }
    }

    public static void InsertRange<T>(this IList<T> list, int index, IEnumerable<T> items)
    {
        Require.ArgumentNotNull(list);
        Require.ArgumentInRange(index, 0, list.Count);
        Require.ArgumentNotNull(items);

        switch (list)
        {
            case List<T> listGeneric:
                listGeneric.InsertRange(index, items);
                break;

            case ImmutableArray<T>.Builder builder:
                builder.InsertRange(index, items);
                break;

            default:
                foreach (var item in items)
                    list.Insert(index++, item);
                break;
        }
    }

    public static void InsertRange<T>(this IList<T> list, int index, ReadOnlySpan<T> items)
    {
        Require.ArgumentNotNull(list);
        Require.ArgumentInRange(index, 0, list.Count);

        switch (list)
        {
#if NET8_0_OR_GREATER
            case List<T> listGeneric:
                CollectionExtensions.InsertRange(listGeneric, index, items);
                break;
#endif

            default:
                foreach (var item in items)
                    list.Insert(index++, item);
                break;
        }
    }

    public static void RemoveRange<T>(this IList<T> list, int index, int count)
    {
        Require.ArgumentNotNull(list);
        Require.ArgumentInRange(index, 0, list.Count);
        Require.ArgumentInRange(count, 0, list.Count - index);

        switch (list)
        {
            case List<T> listGeneric:
                listGeneric.RemoveRange(index, count);
                break;

            case ImmutableArray<T>.Builder builder:
                builder.RemoveRange(index, count);
                break;

            default:
                for (int i = 0; i < count; ++i)
                    list.RemoveAt(index);
                break;
        }
    }
}
