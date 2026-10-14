namespace Neme.Extensions.InteropServices.Tests;

public sealed class CollectionsMarshalExtensionsTest
{
    [Theory]
    [InlineData(new int[] { })]
    [InlineData(new int[] { 1, 2 })]
    [InlineData(new int[] { 1, 2, 3, 4 })]
    public void TestAsMemory(int[] values)
    {
        var memory = CollectionsMarshalExtensions.AsMemory(values.ToList());
        Assert.Equal(values, memory.ToArray());
    }

    [Fact]
    public void TestAsMemory_Null()
    {
        var memory = CollectionsMarshalExtensions.AsMemory<int>(null);
#pragma warning disable CA2265
        Assert.True(memory.Span == default);
#pragma warning restore CA2265
    }
}