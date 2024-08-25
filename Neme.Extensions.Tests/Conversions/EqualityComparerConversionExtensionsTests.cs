using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Neme.Extensions.Conversions.Tests;

public sealed class EqualityComparerConversionExtensionsTests
{
    [Fact]
    public void TestAsNonGeneric_Same()
    {
        var equalityComparer = (IEqualityComparer<int>)EqualityComparer<int>.Default;
        var asNonGeneric = equalityComparer.AsNonGeneric();
        Assert.Same(equalityComparer, asNonGeneric);
    }

    [Fact]
    public void TestAsNonGeneric_WrongType()
    {
        var equalityComparer = new GenericEqualityComparer<int>();
        var asNonGeneric = equalityComparer.AsNonGeneric();
        Assert.Throws<InvalidCastException>(() => asNonGeneric.Equals("foo", "bar"));
    }

    [Fact]
    public void TestAsNonGeneric_Wrapper()
    {
        var equalityComparer = new GenericEqualityComparer<int>();
        var asNonGeneric = equalityComparer.AsNonGeneric();
        Assert.NotSame(equalityComparer, asNonGeneric);

        Assert.True(asNonGeneric.Equals(1, 1));
        Assert.False(asNonGeneric.Equals(1, 2));
        Assert.Equal(EqualityComparer<int>.Default.GetHashCode(42), asNonGeneric.GetHashCode(42));
    }

    [Fact]
    public void TestAsNonGeneric_GenericWrapperUnwrapped()
    {
        var equalityComparer = new NonGenericEqualityComparer();
        var asGeneric = equalityComparer.AsGeneric<int>();
        var asNonGeneric = asGeneric.AsNonGeneric();
        Assert.Same(equalityComparer, asNonGeneric);
    }

    [Fact]
    public void TestAsGeneric_Same()
    {
        var equalityComparer = (IEqualityComparer)EqualityComparer<int>.Default;
        var asGeneric = equalityComparer.AsGeneric<int>();
        Assert.Same(equalityComparer, asGeneric);
    }

    [Fact]
    public void TestAsGeneric_Wrapper()
    {
        var equalityComparer = new NonGenericEqualityComparer();
        var asGeneric = equalityComparer.AsGeneric<int>();
        Assert.NotSame(equalityComparer, asGeneric);

        Assert.True(asGeneric.Equals(1, 1));
        Assert.False(asGeneric.Equals(1, 2));
        Assert.Equal(EqualityComparer<int>.Default.GetHashCode(42), asGeneric.GetHashCode(42));
    }

    [Fact]
    public void TestAsGeneric_NonGenericWrapperUnwrapped()
    {
        var equalityComparer = new GenericEqualityComparer<int>();
        var asNonGeneric = equalityComparer.AsNonGeneric();
        var asGeneric = asNonGeneric.AsGeneric<int>();
        Assert.Same(equalityComparer, asGeneric);
    }

    private sealed class NonGenericEqualityComparer : IEqualityComparer
    {
        public new bool Equals(object? x, object? y) =>
            EqualityComparer<object>.Default.Equals(x!, y!);

        public int GetHashCode(object obj) =>
            EqualityComparer<object>.Default.GetHashCode(obj);
    }

    private sealed class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y) =>
            EqualityComparer<T>.Default.Equals(x!, y!);

#pragma warning disable CS8767
        public int GetHashCode([DisallowNull] T obj) =>
#pragma warning restore CS8767
            EqualityComparer<T>.Default.GetHashCode(obj);
    }
}
