namespace System.Runtime.InteropServices.Tests;

public sealed class CollectionsMarshalPolyfillTest
{
    [Theory]
    [InlineData(new int[] { })]
    [InlineData(new int[] { 1, 2 })]
    [InlineData(new int[] { 1, 2, 3, 4 })]
    public void TestAsSpan(int[] values)
    {
        var span = CollectionsMarshal.AsSpan(values.ToList());
        Assert.Equal(values, span.ToArray());
    }

    [Fact]
    public void TestAsSpan_Null()
    {
        var span = CollectionsMarshal.AsSpan<int>(null);
#pragma warning disable CA1508 // Avoid dead conditional code
#pragma warning disable CA2265 // Do not compare Span<T> to 'null' or 'default'
        Assert.True(span == default);
#pragma warning restore CA2265 // Do not compare Span<T> to 'null' or 'default'
#pragma warning restore CA1508 // Avoid dead conditional code
    }
}
