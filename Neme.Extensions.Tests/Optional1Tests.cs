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


    [Theory]
    [InlineData(42, 43, null)]
#pragma warning disable xUnit1010
    [InlineData(42f, 43f, float.NaN)]
#pragma warning restore xUnit1010
    public void Equality_StronglyTyped<T>(T value1, T value2, T? nan)
        where T : struct
    {
        Assert.True(new Optional<T>().Equals(default));
        Assert.True(new Optional<T>(value1).Equals(new Optional<T>(value1)));
        Assert.False(new Optional<T>().Equals(new Optional<T>(value1)));
        Assert.False(new Optional<T>(value1).Equals(new Optional<T>(value2)));

        if (nan is { } nanValue)
            Assert.True(new Optional<T>(nanValue).Equals(new Optional<T>(nanValue)));
    }

    [Fact]
    public void Equality_Custom()
    {
        Assert.True(new Optional<string>().Equals(default, StringComparer.OrdinalIgnoreCase));
        Assert.True(new Optional<string>("hello").Equals(new Optional<string>("HELLO"), StringComparer.OrdinalIgnoreCase));
        Assert.False(new Optional<string>().Equals(new Optional<string>("hello"), StringComparer.OrdinalIgnoreCase));
        Assert.False(new Optional<string>("hello").Equals(new Optional<string>("hello2"), StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void Equality_Override()
    {
        Assert.True(new Optional<int>().Equals((object)default(Optional<int>)));
        Assert.True(new Optional<int>(42).Equals((object)new Optional<int>(42)));
        Assert.False(new Optional<int>().Equals((object)new Optional<int>(42)));
        Assert.False(new Optional<int>(42).Equals((object)new Optional<int>(43)));

        Assert.False(new Optional<int>().Equals(new Optional<string>()));
        Assert.False(new Optional<int>(5).Equals(5));
        Assert.False(new Optional<int>().Equals(null));
    }

    [Theory]
    [InlineData(42, 43, null)]
#pragma warning disable xUnit1010
    [InlineData(42f, 43f, float.NaN)]
#pragma warning restore xUnit1010
    public void EqualityOperators<T>(T value1, T value2, T? nan)
        where T : struct
    {
        Assert.True(new Optional<T>() == default);
        Assert.True(new Optional<T>(value1) == new Optional<T>(value1));
        Assert.False(new Optional<T>() == new Optional<T>(value1));
        Assert.False(new Optional<T>(value1) == new Optional<T>(value2));

        if (nan is { } nanValue)
            Assert.False(new Optional<T>(nanValue) == new Optional<T>(nanValue));
    }

    [Fact]
    public void Equality_HashCode()
    {
        Assert.Equal(-1, new Optional<int>().GetHashCode());
        Assert.Equal(0, new Optional<string?>(null).GetHashCode());
#pragma warning disable CA1307 // Specify StringComparison for clarity
        Assert.Equal("hello".GetHashCode(), new Optional<string>("hello").GetHashCode());
#pragma warning restore CA1307 // Specify StringComparison for clarity
    }

    [Fact]
    public void Equality_HashCode_Custom()
    {
        Assert.Equal(-1, new Optional<string>().GetHashCode(StringComparer.OrdinalIgnoreCase));
        Assert.Equal(StringComparer.OrdinalIgnoreCase.GetHashCode("hello"), new Optional<string>("hello").GetHashCode(StringComparer.OrdinalIgnoreCase));
    }
}
