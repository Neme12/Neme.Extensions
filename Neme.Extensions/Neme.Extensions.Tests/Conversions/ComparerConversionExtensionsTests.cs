using System.Collections;

namespace Neme.Extensions.Conversions.Tests;

public sealed class ComparerConversionExtensionsTests
{
    [Fact]
    public void TestAsNonGeneric_Same()
    {
        var comparer = (IComparer<int>)Comparer<int>.Default;
        var asNonGeneric = comparer.AsNonGeneric();
        Assert.Same(comparer, asNonGeneric);
    }

    [Fact]
    public void TestAsNonGeneric_WrongType()
    {
        var comparer = new GenericComparer<int>();
        var asNonGeneric = comparer.AsNonGeneric();
        Assert.Throws<InvalidCastException>(() => asNonGeneric.Compare("foo", "bar"));
    }

    [Fact]
    public void TestAsNonGeneric_Wrapper()
    {
        var comparer = new GenericComparer<int>();
        var asNonGeneric = comparer.AsNonGeneric();
        Assert.NotSame(comparer, asNonGeneric);

        Assert.Equal(0, asNonGeneric.Compare(1, 1));
        Assert.Equal(-1, asNonGeneric.Compare(1, 2));
    }

    [Fact]
    public void TestAsNonGeneric_GenericWrapperUnwrapped()
    {
        var comparer = new NonGenericComparer();
        var asGeneric = comparer.AsGeneric<int>();
        var asNonGeneric = asGeneric.AsNonGeneric();
        Assert.Same(comparer, asNonGeneric);
    }

    [Fact]
    public void TestAsGeneric_Same()
    {
        var comparer = (IComparer)Comparer<int>.Default;
        var asGeneric = comparer.AsGeneric<int>();
        Assert.Same(comparer, asGeneric);
    }

    [Fact]
    public void TestAsGeneric_Wrapper()
    {
        var comparer = new NonGenericComparer();
        var asGeneric = comparer.AsGeneric<int>();
        Assert.NotSame(comparer, asGeneric);

        Assert.Equal(0, asGeneric.Compare(1, 1));
        Assert.Equal(-1, asGeneric.Compare(1, 2));
    }

    [Fact]
    public void TestAsGeneric_NonGenericWrapperUnwrapped()
    {
        var comparer = new GenericComparer<int>();
        var asNonGeneric = comparer.AsNonGeneric();
        var asGeneric = asNonGeneric.AsGeneric<int>();
        Assert.Same(comparer, asGeneric);
    }

    private sealed class NonGenericComparer : IComparer
    {
        public int Compare(object? x, object? y) =>
            Comparer<object>.Default.Compare(x!, y!);
    }

    private sealed class GenericComparer<T> : IComparer<T>
    {
        public int Compare(T? x, T? y) =>
            Comparer<T>.Default.Compare(x!, y!);
    }
}
