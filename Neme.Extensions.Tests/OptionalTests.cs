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

    [Fact]
    public void GetHasValueRef()
    {
        var optional = default(Optional<int>);
        ref readonly var hasValue = ref Optional.GetHasValueRef(in optional);
        Assert.False(hasValue);

        optional = new(42);
        Assert.True(hasValue);
    }

    [Fact]
    public void GetValueRefOrDefaultRef()
    {
        var optional = default(Optional<int>);
        ref readonly var value = ref Optional.GetValueRefOrDefaultRef(in optional);
        Assert.Equal(0, value);

        optional = new(42);
        Assert.Equal(42, value);
    }

    [Fact]
    public void Equals1()
    {
        Assert.True(Optional.Equals(new Optional<int>(), default));
        Assert.True(Optional.Equals(new Optional<int>(42), new Optional<int>(42)));
        Assert.False(Optional.Equals(new Optional<int>(), new Optional<int>(42)));
        Assert.False(Optional.Equals(new Optional<int>(42), new Optional<int>()));
        Assert.False(Optional.Equals(new Optional<int>(42), new Optional<int>(43)));
    }

    [Fact]
    public void Equals_CustomComparer()
    {
        Assert.True(Optional.Equals(new Optional<string>(), default, StringComparer.OrdinalIgnoreCase));
        Assert.True(Optional.Equals(new Optional<string>("a"), new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));
        Assert.False(Optional.Equals(new Optional<string>(), new Optional<string>("a"), StringComparer.OrdinalIgnoreCase));
        Assert.False(Optional.Equals(new Optional<string>("a"), new Optional<string>(), StringComparer.OrdinalIgnoreCase));
        Assert.False(Optional.Equals(new Optional<string>("a"), new Optional<string>("b"), StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void Compare()
    {
        Assert.Equal(0, Optional.Compare(new Optional<int>(), default));
        Assert.Equal(0, Optional.Compare(new Optional<int>(42), new Optional<int>(42)));
        Assert.Equal(-1, Optional.Compare(new Optional<int>(), new Optional<int>(42)));
        Assert.Equal(1, Optional.Compare(new Optional<int>(42), new Optional<int>()));
        Assert.Equal(-1, Optional.Compare(new Optional<int>(42), new Optional<int>(43)));
        Assert.Equal(1, Optional.Compare(new Optional<int>(43), new Optional<int>(42)));
    }

    [Fact]
    public void Compare_CustomComparer()
    {
        Assert.Equal(0, Optional.Compare(new Optional<string>(), default, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(0, Optional.Compare(new Optional<string>("a"), new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, Optional.Compare(new Optional<string>(), new Optional<string>("a"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, Optional.Compare(new Optional<string>("a"), new Optional<string>(), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, Optional.Compare(new Optional<string>("a"), new Optional<string>("B"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, Optional.Compare(new Optional<string>("b"), new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void GetUnderlyingType()
    {
        Assert.Throws<ArgumentNullException>("optionalType", () => Optional.GetUnderlyingType(null!));
        Assert.Null(Optional.GetUnderlyingType(typeof(int)));
        Assert.Null(Optional.GetUnderlyingType(typeof(List<string>)));
        Assert.Null(Optional.GetUnderlyingType(typeof(Optional<>)));
        Assert.Equal(typeof(int), Optional.GetUnderlyingType(typeof(Optional<int>)));
        Assert.Equal(typeof(List<string>), Optional.GetUnderlyingType(typeof(Optional<List<string>>)));
    }
}
