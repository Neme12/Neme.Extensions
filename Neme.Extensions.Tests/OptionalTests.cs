namespace Neme.Extensions.Tests;

public sealed class OptionalTests
{
    [Fact]
    public void Some()
    {
        var optional = Optional.Some(42);
        Assert.True(optional.HasValue);
        Assert.Equal(42, optional.Value);
    }

    [Fact]
    public void FromNullable_Struct_WithValue()
    {
        var optional = Optional.FromNullable((int?)42);
        Assert.True(optional.HasValue);
        Assert.Equal(42, optional.Value);
    }

    [Fact]
    public void FromNullable_Struct_Null()
    {
        var optional = Optional.FromNullable((int?)null);
        Assert.False(optional.HasValue);
    }

    [Fact]
    public void FromNullable_Class_WithValue()
    {
        var optional = Optional.FromNullable("hello");
        Assert.True(optional.HasValue);
        Assert.Equal("hello", optional.Value);
    }

    [Fact]
    public void FromNullable_Class_Null()
    {
        var optional = Optional.FromNullable((string?)null);
        Assert.False(optional.HasValue);
    }

    [Fact]
    public void AsNullable_Struct_WithValue()
    {
        var optional = new Optional<int>(42);
        var nullable = optional.AsNullable();
        Assert.Equal(42, nullable);
    }

    [Fact]
    public void AsNullable_Struct_Null()
    {
        var optional = default(Optional<int>);
        var nullable = optional.AsNullable();
        Assert.Null(nullable);
    }

    [Fact]
    public void AsNullable_Class_WithValue()
    {
        var optional = new Optional<string>("hello");
        var nullable = optional.AsNullable();
        Assert.Equal("hello", nullable);
    }

    [Fact]
    public void AsNullable_Class_Null()
    {
        var optional = default(Optional<string>);
        var nullable = optional.AsNullable();
        Assert.Null(nullable);
    }
}
