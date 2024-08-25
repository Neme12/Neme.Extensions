using System.Collections;

namespace Neme.Extensions.Conversions;

public static class ComparerConversionExtensions
{
    public static IComparer AsNonGeneric<T>(this IComparer<T> comparer) =>
        comparer switch
        {
            NonGenericComparerWrapper<T> nonGenericComparerWrapper => nonGenericComparerWrapper.Comparer,
            IComparer nonGenericComparer => nonGenericComparer,
            _ => new GenericComparerWrapper<T>(comparer),
        };

    public static IComparer<T> AsGeneric<T>(this IComparer comparer) =>
        comparer switch
        {
            GenericComparerWrapper<T> genericComparerWrapper => genericComparerWrapper.Comparer,
            IComparer<T> genericComparer => genericComparer,
            _ => new NonGenericComparerWrapper<T>(comparer),
        };

    private sealed class NonGenericComparerWrapper<T>(IComparer comparer) : IComparer<T>
    {
        public IComparer Comparer => comparer;

        public int Compare(T? x, T? y) =>
            comparer.Compare(x, y);
    }

    private sealed class GenericComparerWrapper<T>(IComparer<T> comparer) : IComparer
    {
        public IComparer<T> Comparer => comparer;

        public int Compare(object? x, object? y) =>
            comparer.Compare((T)x!, (T)y!);
    }
}
