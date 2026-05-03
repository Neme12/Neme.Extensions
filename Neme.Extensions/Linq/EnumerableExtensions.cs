namespace Neme.Extensions.Linq;

public static partial class EnumerableExtensions
{
    public static IEnumerable<(TSource value, int index)> WithIndex<TSource>(this IEnumerable<TSource> source) =>
        source.Select((value, index) => (value, index));
}
