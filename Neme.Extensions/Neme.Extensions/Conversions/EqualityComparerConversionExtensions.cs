using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Neme.Extensions.Conversions;

public static class EqualityComparerConversionExtensions
{
    public static IEqualityComparer AsNonGeneric<T>(this IEqualityComparer<T> equalityComparer) =>
        equalityComparer switch
        {
            NonGenericEqualityComparerWrapper<T> nonGenericEqualityComparerWrapper => nonGenericEqualityComparerWrapper.EqualityComparer,
            IEqualityComparer nonGenericEqualityComparer => nonGenericEqualityComparer,
            _ => new GenericEqualityComparerWrapper<T>(equalityComparer),
        };

    public static IEqualityComparer<T> AsGeneric<T>(this IEqualityComparer equalityComparer) =>
        equalityComparer switch
        {
            GenericEqualityComparerWrapper<T> genericEqualityComparerWrapper => genericEqualityComparerWrapper.EqualityComparer,
            IEqualityComparer<T> genericEqualityComparer => genericEqualityComparer,
            _ => new NonGenericEqualityComparerWrapper<T>(equalityComparer),
        };

    private sealed class NonGenericEqualityComparerWrapper<T>(IEqualityComparer equalityComparer) : IEqualityComparer<T>
    {
        public IEqualityComparer EqualityComparer => equalityComparer;

        public bool Equals(T? x, T? y) =>
            equalityComparer.Equals(x, y);

#pragma warning disable CS8767
        public int GetHashCode([DisallowNull] T obj) =>
#pragma warning restore CS8767
            equalityComparer.GetHashCode(obj);
    }

    private sealed class GenericEqualityComparerWrapper<T>(IEqualityComparer<T> equalityComparer) : IEqualityComparer
    {
        public IEqualityComparer<T> EqualityComparer => equalityComparer;

        public new bool Equals(object? x, object? y) =>
            equalityComparer.Equals((T)x!, (T)y!);

        public int GetHashCode(object obj) =>
            equalityComparer.GetHashCode((T)obj);
    }
}
