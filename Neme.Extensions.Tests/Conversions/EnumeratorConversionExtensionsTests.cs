using System.Collections;

namespace Neme.Extensions.Conversions.Tests;

public sealed class EnumeratorConversionExtensionsTests
{
    [Fact]
    public void TestAsNonGeneric()
    {
        var enumerator = (IEnumerator<int>)new List<int> { 1, 2, 3 }.GetEnumerator();
        var asNonGeneric = enumerator.AsNonGeneric();
        Assert.Same(enumerator, asNonGeneric);
    }

    [Fact]
    public void TestAsGeneric_AlreadyGeneric()
    {
        var enumerator = (IEnumerator)new List<int> { 1, 2, 3 }.GetEnumerator();
        var asGeneric = enumerator.AsGeneric<int>();
        Assert.Same(enumerator, asGeneric);
    }

    [Fact]
    public void TestAsGeneric_Custom()
    {
        var enumerator = new CustomNonGenericEnumerator().AsGeneric<int>();
        Enumerate();
        enumerator.Reset();
        Enumerate();
        enumerator.Dispose();
        Enumerate();

        var enumerator2 = enumerator.AsGeneric<int>();
        Assert.Same(enumerator, enumerator2);

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
        var enumerator = new CustomNonGenericEnumerator().AsGeneric<string>();
        Assert.True(enumerator.MoveNext());
        Assert.Throws<InvalidCastException>(() => enumerator.Current);
    }

    private sealed class CustomNonGenericEnumerator : IEnumerator
    {
        private readonly int[] _array = [1, 2];
        private int _nextPosition = 0;

        public object Current =>
            _array[_nextPosition - 1];

        public bool MoveNext()
        {
            if (_nextPosition >= _array.Length)
                return false;

            ++_nextPosition;
            return true;
        }

        public void Reset()
        {
            _nextPosition = 0;
        }
    }
}
