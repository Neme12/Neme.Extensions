namespace System.Runtime.InteropServices.Tests;

public sealed class ImmutableCollectionsMarshalPolyfillTest
{
    [Theory]
    [InlineData(new int[] { })]
    [InlineData(new int[] { 1, 2 })]
    public void TestAsImmutableArray(int[] values)
    {
        var immutableArray = ImmutableCollectionsMarshalPolyfill.AsImmutableArray(values);
        Assert.Equal(values, immutableArray);

        if (values.Length > 0)
        {
            values[0] = 0;
            Assert.Equal(values, immutableArray);
        }
    }

    [Fact]
    public void TestAsImmutableArray_Null()
    {
        var immutableArray = ImmutableCollectionsMarshalPolyfill.AsImmutableArray<int>(null);
        Assert.Equal(default, immutableArray);
    }

    [Theory]
    [InlineData(new int[] { })]
    [InlineData(new int[] { 1, 2 })]
    public void TestAsArray(int[] values)
    {
        var immutableArray = values.ToImmutableArray();
        var array = ImmutableCollectionsMarshalPolyfill.AsArray(immutableArray)!;
        Assert.Equal(values, array);

        if (array.Length > 0)
        {
            array[0] = 0;
            Assert.Equal(array, immutableArray);
        }
    }

    [Fact]
    public void TestAsArray_Null()
    {
        var array = ImmutableCollectionsMarshalPolyfill.AsArray<int>(default)!;
        Assert.Null(array);
    }
}
