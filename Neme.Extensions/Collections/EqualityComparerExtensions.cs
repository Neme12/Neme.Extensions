using Neme.Extensions.Contracts;
using System.Collections;

namespace Neme.Extensions.Collections;

public static class EqualityComparerExtensions
{
    public static int GetHashCodeOrDefault(this IEqualityComparer equalityComparer, object? obj)
    {
        Require.ArgumentNotNull(equalityComparer);

        return obj is null ? 0 : equalityComparer.GetHashCode(obj);
    }
}
