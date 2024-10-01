using System.Diagnostics.CodeAnalysis;

namespace Neme.Extensions.Collections;

internal static class EqualityComparerExtensions<T>
{
    public static EqualityComparer<T> CreateBy<TResult>(Func<T, TResult> selector, IEqualityComparer<TResult>? resultComparer = null)
    {
        if (selector is null)
            throw new ArgumentNullException(nameof(selector));

        return new SelectorEqualityComparer<TResult>(selector, resultComparer ?? EqualityComparer<TResult>.Default);
    }

    private sealed class SelectorEqualityComparer<TResult>(Func<T, TResult> selector, IEqualityComparer<TResult> resultComparer) : EqualityComparer<T>
    {
        public override bool Equals(T? x, T? y)
        {
            if (x is null)
                return y is null;

            if (y is null)
                return false;

            return resultComparer.Equals(selector(x), selector(y));
        }

        public override int GetHashCode([DisallowNull] T obj)
        {
            var result = selector(obj);
            if (result is null)
                return 0;

            return resultComparer.GetHashCode(result);
        }
    }
}
