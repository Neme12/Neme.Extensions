namespace Neme.Extensions.Tests;

public sealed class Optional1Tests
{
    [Fact]
    public void DefaultHasNoValue()
    {
        var optional = default(Optional<int>);
        Assert.False(optional.HasValue);
        Assert.Throws<InvalidOperationException>(() => optional.Value);

        Assert.Equal(0, optional.GetValueOrDefault());
        Assert.Equal(1, optional.GetValueOrDefault(1));

        Assert.False(optional.TryGetValue(out var value));
        Assert.Equal(0, value);

        optional.Deconstruct(out var hasValue, out value);
        Assert.False(hasValue);
        Assert.Equal(0, value);
    }

    [Fact]
    public void ConstructedHasValue()
    {
        var optional = new Optional<int>(42);
        Assert.True(optional.HasValue);
        Assert.Equal(42, optional.Value);

        Assert.Equal(42, optional.GetValueOrDefault());
        Assert.Equal(42, optional.GetValueOrDefault(1));

        Assert.True(optional.TryGetValue(out var value));
        Assert.Equal(42, value);

        optional.Deconstruct(out var hasValue, out value);
        Assert.True(hasValue);
        Assert.Equal(42, value);
    }

    [Fact]
    public void Equality_StronglyTyped()
    {
        Assert.True(new Optional<int>().Equals(default));
        Assert.True(new Optional<int>(42).Equals(new Optional<int>(42)));
        Assert.False(new Optional<int>().Equals(new Optional<int>(42)));
        Assert.False(new Optional<int>(42).Equals(new Optional<int>(43)));
    }

    [Fact]
    public void Equality_Override()
    {
        Assert.True(new Optional<int>().Equals((object)default(Optional<int>)));
        Assert.True(new Optional<int>(42).Equals((object)new Optional<int>(42)));
        Assert.False(new Optional<int>().Equals((object)new Optional<int>(42)));
        Assert.False(new Optional<int>(42).Equals((object)new Optional<int>(43)));

        Assert.False(new Optional<int>().Equals((object)new Optional<string>()));
        Assert.False(new Optional<int>(5).Equals((object)5));
        Assert.False(new Optional<int>().Equals(null));
    }
}
