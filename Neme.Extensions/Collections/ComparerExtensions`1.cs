namespace Neme.Extensions.Collections;

internal static class ComparerExtensions<T>
{
    public static Comparer<T> CreateBy<TResult>(Func<T, TResult> selector, IComparer<TResult>? resultComparer = null)
    {
        if (selector is null)
            throw new ArgumentNullException(nameof(selector));

        return new SelectorComparer<TResult>(selector, resultComparer ?? Comparer<TResult>.Default);
    }

    private sealed class SelectorComparer<TResult>(Func<T, TResult> selector, IComparer<TResult> resultComparer) : Comparer<T>
    {
        public override int Compare(T? x, T? y)
        {
            if (x is null)
                return y is null ? 0 : -1;

            if (y is null)
                return 1;

            return resultComparer.Compare(selector(x), selector(y));
        }
    }
}
