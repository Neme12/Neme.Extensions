using System.Collections;

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
        Assert.False(new Optional<T>(value1).Equals(new Optional<T>()));
        Assert.False(new Optional<T>(value1).Equals(new Optional<T>(value2)));

        if (nan is { } nanValue)
            Assert.True(new Optional<T>(nanValue).Equals(new Optional<T>(nanValue)));
    }

    [Fact]
    public void Equality_Custom()
    {
        Assert.True(new Optional<string>().Equals(default, StringComparer.OrdinalIgnoreCase));
        Assert.True(new Optional<string>("a").Equals(new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));
        Assert.False(new Optional<string>().Equals(new Optional<string>("a"), StringComparer.OrdinalIgnoreCase));
        Assert.False(new Optional<string>("a").Equals(new Optional<string>(), StringComparer.OrdinalIgnoreCase));
        Assert.False(new Optional<string>("a").Equals(new Optional<string>("b"), StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void Equality_IStructuralEquatable()
    {
        Assert.True(((IStructuralEquatable)new Optional<string>()).Equals(default(Optional<string>), StringComparer.OrdinalIgnoreCase));
        Assert.True(((IStructuralEquatable)new Optional<string>("a")).Equals(new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));
        Assert.False(((IStructuralEquatable)new Optional<string>()).Equals(new Optional<string>("a"), StringComparer.OrdinalIgnoreCase));
        Assert.False(((IStructuralEquatable)new Optional<string>("a")).Equals(new Optional<string>(), StringComparer.OrdinalIgnoreCase));
        Assert.False(((IStructuralEquatable)new Optional<string>("a")).Equals(new Optional<string>("b"), StringComparer.OrdinalIgnoreCase));

        Assert.False(((IStructuralEquatable)new Optional<string>()).Equals(null, StringComparer.OrdinalIgnoreCase));
        Assert.False(((IStructuralEquatable)new Optional<string>()).Equals(new Optional<int>(), StringComparer.OrdinalIgnoreCase));
        Assert.False(((IStructuralEquatable)new Optional<string>("a")).Equals("a", StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void Equality_Override()
    {
        Assert.True(new Optional<int>().Equals((object)default(Optional<int>)));
        Assert.True(new Optional<int>(42).Equals((object)new Optional<int>(42)));
        Assert.False(new Optional<int>().Equals((object)new Optional<int>(42)));
        Assert.False(new Optional<int>(42).Equals((object)new Optional<int>()));
        Assert.False(new Optional<int>(42).Equals((object)new Optional<int>(43)));

        Assert.False(new Optional<int>().Equals(null));
        Assert.False(new Optional<int>().Equals(new Optional<string>()));
        Assert.False(new Optional<int>(5).Equals(5));
    }

    [Fact]
    public void Comparisons_StronglyTyped()
    {
        Assert.Equal(0, new Optional<int>().CompareTo(default));
        Assert.Equal(0, new Optional<int>(42).CompareTo(new Optional<int>(42)));
        Assert.Equal(-1, new Optional<int>().CompareTo(new Optional<int>(42)));
        Assert.Equal(1, new Optional<int>(42).CompareTo(new Optional<int>()));
        Assert.Equal(-1, new Optional<int>(42).CompareTo(new Optional<int>(43)));
        Assert.Equal(1, new Optional<int>(43).CompareTo(new Optional<int>(42)));
    }

    [Fact]
    public void Comparisons_Custom()
    {
        Assert.Equal(0, new Optional<string>().CompareTo(default, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(0, new Optional<string>("a").CompareTo(new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, new Optional<string>().CompareTo(new Optional<string>("a"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, new Optional<string>("a").CompareTo(new Optional<string>(), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, new Optional<string>("a").CompareTo(new Optional<string>("B"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, new Optional<string>("b").CompareTo(new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void Comparisons_IStructuralComparable()
    {
        Assert.Equal(0, ((IStructuralComparable)new Optional<string>()).CompareTo(default(Optional<string>), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(0, ((IStructuralComparable)new Optional<string>("a")).CompareTo(new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, ((IStructuralComparable)new Optional<string>()).CompareTo(new Optional<string>("a"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, ((IStructuralComparable)new Optional<string>("a")).CompareTo(new Optional<string>(), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, ((IStructuralComparable)new Optional<string>("a")).CompareTo(new Optional<string>("B"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, ((IStructuralComparable)new Optional<string>("b")).CompareTo(new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));

        Assert.Equal(1, ((IStructuralComparable)new Optional<string>()).CompareTo(null, StringComparer.OrdinalIgnoreCase));
        var e1 = Assert.Throws<ArgumentException>("other", () => ((IStructuralComparable)new Optional<string>()).CompareTo(new Optional<int>(), StringComparer.OrdinalIgnoreCase));
        Assert.StartsWith("Object must be of type Neme.Extensions.Optional`1[System.String].", e1.Message, StringComparison.Ordinal);
        var e2 = Assert.Throws<ArgumentException>("other", () => ((IStructuralComparable)new Optional<string>("a")).CompareTo("a", StringComparer.OrdinalIgnoreCase));
        Assert.StartsWith("Object must be of type Neme.Extensions.Optional`1[System.String].", e2.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Comparisons_NonGeneric()
    {
        Assert.Equal(0, ((IComparable)new Optional<int>()).CompareTo(default(Optional<int>)));
        Assert.Equal(0, ((IComparable)new Optional<int>(42)).CompareTo(new Optional<int>(42)));
        Assert.Equal(-1, ((IComparable)new Optional<int>()).CompareTo(new Optional<int>(42)));
        Assert.Equal(1, ((IComparable)new Optional<int>(42)).CompareTo(new Optional<int>()));
        Assert.Equal(-1, ((IComparable)new Optional<int>(42)).CompareTo(new Optional<int>(43)));
        Assert.Equal(1, ((IComparable)new Optional<int>(43)).CompareTo(new Optional<int>(42)));

        Assert.Equal(1, ((IComparable)new Optional<int>()).CompareTo(null));
        var e1 = Assert.Throws<ArgumentException>("obj", () => ((IComparable)new Optional<int>()).CompareTo(new Optional<string>()));
        Assert.StartsWith("Object must be of type Neme.Extensions.Optional`1[System.Int32].", e1.Message, StringComparison.Ordinal);
        var e2 = Assert.Throws<ArgumentException>("obj", () => ((IComparable)new Optional<int>(5)).CompareTo(5));
        Assert.StartsWith("Object must be of type Neme.Extensions.Optional`1[System.Int32].", e2.Message, StringComparison.Ordinal);
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
        Assert.False(new Optional<T>(value1) == new Optional<T>());
        Assert.False(new Optional<T>(value1) == new Optional<T>(value2));

        if (nan is { } nanValue)
            Assert.False(new Optional<T>(nanValue) == new Optional<T>(nanValue));
    }

    [Theory]
    [InlineData(42, 43, null)]
#pragma warning disable xUnit1010
    [InlineData(42f, 43f, float.NaN)]
#pragma warning restore xUnit1010
    public void InequalityOperators<T>(T value1, T value2, T? nan)
        where T : struct
    {
        Assert.False(new Optional<T>() != default);
        Assert.False(new Optional<T>(value1) != new Optional<T>(value1));
        Assert.True(new Optional<T>() != new Optional<T>(value1));
        Assert.True(new Optional<T>(value1) != new Optional<T>());
        Assert.True(new Optional<T>(value1) != new Optional<T>(value2));

        if (nan is { } nanValue)
            Assert.True(new Optional<T>(nanValue) != new Optional<T>(nanValue));
    }

    [Fact]
    public void EqualityOperators_Custom()
    {
        Assert.True(new Optional<CustomComparable>() == default);
        Assert.True(new Optional<CustomComparable>(new(1)) == new Optional<CustomComparable>(new(2)));
        Assert.False(new Optional<CustomComparable>() == new Optional<CustomComparable>(new(1)));
        Assert.False(new Optional<CustomComparable>(new(1)) == new Optional<CustomComparable>());
        Assert.False(new Optional<CustomComparable>(new(2)) == new Optional<CustomComparable>(new(2)));
    }

    [Fact]
    public void InequalityOperators_Custom()
    {
        Assert.False(new Optional<CustomComparable>() != default);
        Assert.False(new Optional<CustomComparable>(new(1)) != new Optional<CustomComparable>(new(2)));
        Assert.True(new Optional<CustomComparable>() != new Optional<CustomComparable>(new(1)));
        Assert.True(new Optional<CustomComparable>(new(1)) != new Optional<CustomComparable>());
        Assert.True(new Optional<CustomComparable>(new(-1)) != new Optional<CustomComparable>(new(1)));
    }

    [Fact]
    public void HashCode()
    {
        Assert.Equal(-1, new Optional<int>().GetHashCode());
        Assert.Equal(0, new Optional<string?>(null).GetHashCode());
#pragma warning disable CA1307 // Specify StringComparison for clarity
        Assert.Equal("hello".GetHashCode(), new Optional<string>("hello").GetHashCode());
#pragma warning restore CA1307 // Specify StringComparison for clarity
    }

    [Fact]
    public void HashCode_Custom()
    {
        Assert.Equal(-1, new Optional<string>().GetHashCode(StringComparer.OrdinalIgnoreCase));
        Assert.Equal(StringComparer.OrdinalIgnoreCase.GetHashCode("hello"), new Optional<string>("hello").GetHashCode(StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void HashCode_IStructuralEquatable()
    {
        Assert.Equal(-1, ((IStructuralEquatable)new Optional<string>()).GetHashCode(StringComparer.OrdinalIgnoreCase));
        Assert.Equal(StringComparer.OrdinalIgnoreCase.GetHashCode("hello"), ((IStructuralEquatable)new Optional<string>("hello")).GetHashCode(StringComparer.OrdinalIgnoreCase));
    }

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    private readonly struct CustomComparable(int value)
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        public int Value => value;

        public static bool operator ==(CustomComparable a, CustomComparable b) =>
            a.Value == 1;

        public static bool operator !=(CustomComparable a, CustomComparable b) =>
            a.Value == -1;
    }
}
