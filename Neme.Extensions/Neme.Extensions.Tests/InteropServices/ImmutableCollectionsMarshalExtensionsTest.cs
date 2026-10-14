namespace Neme.Extensions.InteropServices.Tests;

public sealed class ImmutableCollectionsMarshalExtensionsTest
{
    [Theory]
    [InlineData(new int[] { })]
    [InlineData(new int[] { 1, 2 })]
    [InlineData(new int[] { 1, 2, 3, 4 })]
    public void TestAsSpan(int[] values)
    {
        var builder = ImmutableArray.CreateBuilder<int>();
        builder.AddRange(values);

        var span = ImmutableCollectionsMarshalExtensions.AsSpan(builder);
        Assert.Equal(values, span.ToArray());
    }

    [Fact]
    public void TestAsSpan_Null()
    {
        var span = ImmutableCollectionsMarshalExtensions.AsSpan<int>(null);
#pragma warning disable CA2265
        Assert.True(span == default);
#pragma warning restore CA2265
    }
}
