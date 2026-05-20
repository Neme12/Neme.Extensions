using Microsoft.EntityFrameworkCore;
using Neme.Extensions.Linq;

namespace Neme.Extensions.EntityFrameworkCore;

public static class QueryableExtensions
{
    extension<TSource>(IQueryable<TSource> source)
    {
        public async Task<ImmutableArray<TSource>> ToImmutableArrayAsync(
            CancellationToken cancellationToken = default)
        {
            var builder = source.TryGetNonEnumeratedCount2(out var count)
                ? ImmutableArray.CreateBuilder<TSource>(count)
                : ImmutableArray.CreateBuilder<TSource>();

            await foreach (var item in source.AsAsyncEnumerable().WithCancellation(cancellationToken).ConfigureAwait(false))
                builder.Add(item);

            return builder.DrainToImmutable();
        }
    }
}
