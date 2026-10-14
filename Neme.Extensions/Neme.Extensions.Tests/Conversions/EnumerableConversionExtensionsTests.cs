using System.Collections;

namespace Neme.Extensions.Conversions.Tests;

public sealed class EnumerableConversionExtensionsTests
{
    [Fact]
    public void TestAsNonGeneric()
    {
        var enumerable = (IEnumerable<int>)new List<int> { 1, 2, 3 };
        var asNonGeneric = enumerable.AsNonGeneric();
        Assert.Same(enumerable, asNonGeneric);
    }

    [Fact]
    public void TestAsNonGeneric_NonGenericWrapper()
    {
        var enumerable = new CustomNonGenericEnumerable();
        var asGeneric = enumerable.AsGeneric<int>();
        Assert.Same(enumerable, asGeneric.AsNonGeneric());
    }

    [Fact]
    public void TestAsGeneric_AlreadyGeneric()
    {
        var enumerable = (IEnumerable)new List<int> { 1, 2, 3 };
        var asGeneric = enumerable.AsGeneric<int>();
        Assert.Same(enumerable, asGeneric);
    }

    [Fact]
    public void TestAsGeneric_Custom()
    {
        var enumerable = new CustomNonGenericEnumerable().AsGeneric<int>();
        var enumerator = enumerable.GetEnumerator();
        Enumerate();
        enumerator.Reset();
        Enumerate();
        enumerator.Dispose();
        Enumerate();

        var enumerable2 = enumerable.AsGeneric<int>();
        Assert.Same(enumerable, enumerable2);

        void Enumerate()
        {
            Assert.True(enumerator.MoveNext());
            Assert.Equal(1, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current);

            Assert.False(enumerator.MoveNext());
        }
    }

    [Fact]
    public void TestAsGeneric_WrongType()
    {
        var enumerable = new CustomNonGenericEnumerable().AsGeneric<string>();
        var enumerator = enumerable.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.Throws<InvalidCastException>(() => enumerator.Current);
    }

    private sealed class CustomNonGenericEnumerable : IEnumerable
    {
        public IEnumerator GetEnumerator() => new EnumeratorConversionExtensionsTests.CustomNonGenericEnumerator();
    }
}
