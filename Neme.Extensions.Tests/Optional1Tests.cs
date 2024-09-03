using Neme.Extensions.Tests.Utilities;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Neme.Extensions.Tests;

public sealed class Optional1Tests
{
    private static readonly ConstructorInfo _serializationConstructor = typeof(Optional<int>)
        .GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.ExactBinding | BindingFlags.DeclaredOnly,
            binder: null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            modifiers: null)!;

    [Fact]
    public void DefaultHasNoValue()
    {
        var optional = default(Optional<int>);
        Assert.False(optional.HasValue);
        var e = Assert.Throws<InvalidOperationException>(() => optional.Value);
        Assert.Equal("Optional has no value.", e.Message);

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
    public void Serialization()
    {
        SerializeAndDeserialize<int>(default);
        SerializeAndDeserialize<int>(new(42));
        SerializeAndDeserialize<object?>(null);
    }

    [Fact]
    public void Serialization_Members_None()
    {
        var optional = default(Optional<int>);

#pragma warning disable SYSLIB0050 // Type or member is obsolete
        var serializationInfo = new SerializationInfo(typeof(Optional<int>), new FormatterConverter());
        ((ISerializable)optional).GetObjectData(serializationInfo, default);
#pragma warning restore SYSLIB0050 // Type or member is obsolete

        Assert.Equal(0, serializationInfo.MemberCount);

        var result = _serializationConstructor.Invoke([serializationInfo, default(StreamingContext)]);
        Assert.Equal(optional, result);
    }

    [Fact]
    public void Serialization_Members_Some()
    {
        var optional = new Optional<int>(42);

#pragma warning disable SYSLIB0050 // Type or member is obsolete
        var serializationInfo = new SerializationInfo(typeof(Optional<int>), new FormatterConverter());
        ((ISerializable)optional).GetObjectData(serializationInfo, default);
#pragma warning restore SYSLIB0050 // Type or member is obsolete

        Assert.Equal(1, serializationInfo.MemberCount);
        Assert.Equal(42, serializationInfo.GetInt32("Value"));

        var result = _serializationConstructor.Invoke([serializationInfo, default(StreamingContext)]);
        Assert.Equal(optional, result);
    }

    private static void SerializeAndDeserialize<T>(Optional<T> optional)
    {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable CA2300 // Do not use insecure deserializer BinaryFormatter
        var formatter = new BinaryFormatter();
        using var stream = new MemoryStream();
        formatter.Serialize(stream, optional);
        stream.Position = 0;

        var obj = formatter.Deserialize(stream);
        Assert.Equal(optional, obj);
#pragma warning restore CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning restore SYSLIB0011 // Type or member is obsolete
    }

#if NETCOREAPP2_0_OR_GREATER || NET471_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [Fact]
    public void ITupleImplementation_None()
    {
        ITuple optional = default(Optional<int>);
        Assert.Equal(0, optional.Length);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => optional[0]);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => optional[1]);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => optional[-1]);
    }

    [Fact]
    public void ITupleImplementation_Some()
    {
        ITuple optional = new Optional<int>(42);
        Assert.Equal(1, optional.Length);
        Assert.Equal(42, optional[0]);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => optional[1]);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => optional[-1]);
    }
#endif

    [Fact]
    public void ToString_None()
    {
        var optional = default(Optional<int>);

        Assert.Equal("None", optional.ToString());
    }

    [Fact]
    public void ToString_Some()
    {
        var optional = new Optional<double>(42.2);

        Assert.Equal($"Some {{ {optional.Value} }}", optional.ToString());
    }

    [Fact]
    public void ToString_Some_Null()
    {
        var optional = new Optional<object?>(null);

        Assert.Equal("Some { }", optional.ToString());
    }

    [Fact]
    public void ToString_IFormattable_None()
    {
        var optional = default(Optional<int>);

        AssertToStringIFormattable(optional, "None", null);
        AssertToStringIFormattable(optional, "None", CultureInfo.InvariantCulture);
        AssertToStringIFormattable(optional, "None", CultureInfo.GetCultureInfo("de"));
    }

    [Fact]
    public void ToString_IFormattable_Some()
    {
        var optional = new Optional<double>(42.2);

        AssertToStringIFormattable(optional, $"Some {{ {optional.Value} }}", null);
        AssertToStringIFormattable(optional, "Some { 42.2 }", CultureInfo.InvariantCulture);
        AssertToStringIFormattable(optional, "Some { 42,2 }", CultureInfo.GetCultureInfo("de"));
    }

    [Fact]
    public void ToString_IFormattable_Some_Null()
    {
        var optional = new Optional<object?>(null);

        AssertToStringIFormattable(optional, "Some { }", null);
        AssertToStringIFormattable(optional, "Some { }", CultureInfo.InvariantCulture);
        AssertToStringIFormattable(optional, "Some { }", CultureInfo.GetCultureInfo("de"));
    }

    private static void AssertToStringIFormattable<T>(Optional<T> optional, string expected, CultureInfo? culture)
    {
        Assert.Equal(expected, optional.ToString(null, culture));
        Assert.Equal(expected, optional.ToString("", culture));
        AssertThrows.Argument_FormatStringNotSupported(() => optional.ToString("x", culture));
    }

#if NET6_0_OR_GREATER
    [Fact]
    public void ToString_ISpanFormattable_None()
    {
        var optional = default(Optional<int>);

        AssertToStringISpanFormattable(optional, "None", null);
        AssertToStringISpanFormattable(optional, "None", CultureInfo.InvariantCulture);
        AssertToStringISpanFormattable(optional, "None", CultureInfo.GetCultureInfo("de"));
    }

    [Fact]
    public void ToString_ISpanFormattable_Some()
    {
        var optional = new Optional<double>(42.2);

        AssertToStringISpanFormattable(optional, $"Some {{ {optional.Value} }}", null);
        AssertToStringISpanFormattable(optional, "Some { 42.2 }", CultureInfo.InvariantCulture);
        AssertToStringISpanFormattable(optional, "Some { 42,2 }", CultureInfo.GetCultureInfo("de"));
    }

    [Fact]
    public void ToString_ISpanFormattable_Some_Null()
    {
        var optional = new Optional<object?>(null);

        AssertToStringISpanFormattable(optional, "Some { }", null);
        AssertToStringISpanFormattable(optional, "Some { }", CultureInfo.InvariantCulture);
        AssertToStringISpanFormattable(optional, "Some { }", CultureInfo.GetCultureInfo("de"));
    }

    private static void AssertToStringISpanFormattable<T>(Optional<T> optional, string expected, CultureInfo? culture)
    {
        {
            Span<char> destination = stackalloc char[expected.Length];
            Assert.True(optional.TryFormat(destination, out var charsWritten, default, culture));
            Assert.Equal(expected, destination);
            Assert.Equal(expected.Length, charsWritten);
        }

        {
            Span<char> destination = stackalloc char[expected.Length + 5];
            Assert.True(optional.TryFormat(destination, out var charsWritten, default, culture));
            Assert.Equal(expected, destination[..expected.Length]);
            Assert.Equal(stackalloc char[5], destination[expected.Length..]);
            Assert.Equal(expected.Length, charsWritten);
        }

        if (expected.Length > 0)
        {
            Span<char> destination = stackalloc char[expected.Length - 1];
            Assert.False(optional.TryFormat(destination, out var charsWritten, default, culture));
            Assert.Equal(0, charsWritten);
        }

        AssertThrows.Argument_FormatStringNotSupported(() => optional.TryFormat(default, out _, "x", culture));
    }
#endif

    [Fact]
    public void ImplicitConversion()
    {
        Optional<int> optional = 42;
        Assert.True(optional.HasValue);
        Assert.Equal(42, optional.Value);
    }

    [Fact]
    public void ExplicitConversion()
    {
        var optional = new Optional<int>(42);
        Assert.Equal(42, (int)optional);
    }

    [Fact]
    public void None()
    {
        Assert.Equal(default, Optional<int>.None);
    }

    [Theory]
    [InlineData(42, 43, null)]
#pragma warning disable xUnit1010
    [InlineData(42f, 43f, float.NaN)]
#pragma warning restore xUnit1010
    public void Equals_StronglyTyped<T>(T value1, T value2, T? nan)
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
    public void Equals_CustomComparer()
    {
        Assert.True(new Optional<string>().Equals(default, StringComparer.OrdinalIgnoreCase));
        Assert.True(new Optional<string>("a").Equals(new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));
        Assert.False(new Optional<string>().Equals(new Optional<string>("a"), StringComparer.OrdinalIgnoreCase));
        Assert.False(new Optional<string>("a").Equals(new Optional<string>(), StringComparer.OrdinalIgnoreCase));
        Assert.False(new Optional<string>("a").Equals(new Optional<string>("b"), StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void Equals_IStructuralEquatable()
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
    public void Equals_Override()
    {
        Assert.True(new Optional<int>().Equals((object)default(Optional<int>)));
        Assert.True(new Optional<int>(42).Equals((object)new Optional<int>(42)));
        Assert.False(new Optional<int>().Equals((object)new Optional<int>(42)));
        Assert.False(new Optional<int>(42).Equals((object)new Optional<int>()));
        Assert.False(new Optional<int>(42).Equals((object)new Optional<int>(43)));

        Assert.False(new Optional<int>().Equals(null));
        Assert.False(new Optional<int>().Equals(new Optional<string>()));
        Assert.False(new Optional<int>(5).Equals((object)5));
    }

    [Fact]
    public void CompareTo_StronglyTyped()
    {
        Assert.Equal(0, new Optional<int>().CompareTo(default));
        Assert.Equal(0, new Optional<int>(42).CompareTo(new Optional<int>(42)));
        Assert.Equal(-1, new Optional<int>().CompareTo(new Optional<int>(42)));
        Assert.Equal(1, new Optional<int>(42).CompareTo(new Optional<int>()));
        Assert.Equal(-1, new Optional<int>(42).CompareTo(new Optional<int>(43)));
        Assert.Equal(1, new Optional<int>(43).CompareTo(new Optional<int>(42)));
    }

    [Fact]
    public void CompareTo_CustomComparer()
    {
        Assert.Equal(0, new Optional<string>().CompareTo(default, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(0, new Optional<string>("a").CompareTo(new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, new Optional<string>().CompareTo(new Optional<string>("a"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, new Optional<string>("a").CompareTo(new Optional<string>(), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, new Optional<string>("a").CompareTo(new Optional<string>("B"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, new Optional<string>("b").CompareTo(new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void CompareTo_IStructuralComparable()
    {
        Assert.Equal(0, ((IStructuralComparable)new Optional<string>()).CompareTo(default(Optional<string>), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(0, ((IStructuralComparable)new Optional<string>("a")).CompareTo(new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, ((IStructuralComparable)new Optional<string>()).CompareTo(new Optional<string>("a"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, ((IStructuralComparable)new Optional<string>("a")).CompareTo(new Optional<string>(), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, ((IStructuralComparable)new Optional<string>("a")).CompareTo(new Optional<string>("B"), StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, ((IStructuralComparable)new Optional<string>("b")).CompareTo(new Optional<string>("A"), StringComparer.OrdinalIgnoreCase));

        Assert.Equal(1, ((IStructuralComparable)new Optional<string>()).CompareTo(null, StringComparer.OrdinalIgnoreCase));
        AssertThrows.Argument_ObjectMustBeOfType("other", "Neme.Extensions.Optional`1[System.String]", () => ((IStructuralComparable)new Optional<string>()).CompareTo(new Optional<int>(), StringComparer.OrdinalIgnoreCase));
        AssertThrows.Argument_ObjectMustBeOfType("other", "Neme.Extensions.Optional`1[System.String]", () => ((IStructuralComparable)new Optional<string>("a")).CompareTo("a", StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void CompareTo_IComparable()
    {
        Assert.Equal(0, ((IComparable)new Optional<int>()).CompareTo(default(Optional<int>)));
        Assert.Equal(0, ((IComparable)new Optional<int>(42)).CompareTo(new Optional<int>(42)));
        Assert.Equal(-1, ((IComparable)new Optional<int>()).CompareTo(new Optional<int>(42)));
        Assert.Equal(1, ((IComparable)new Optional<int>(42)).CompareTo(new Optional<int>()));
        Assert.Equal(-1, ((IComparable)new Optional<int>(42)).CompareTo(new Optional<int>(43)));
        Assert.Equal(1, ((IComparable)new Optional<int>(43)).CompareTo(new Optional<int>(42)));

        Assert.Equal(1, ((IComparable)new Optional<int>()).CompareTo(null));
        AssertThrows.Argument_ObjectMustBeOfType("obj", "Neme.Extensions.Optional`1[System.Int32]", () => ((IComparable)new Optional<int>()).CompareTo(new Optional<string>()));
        AssertThrows.Argument_ObjectMustBeOfType("obj", "Neme.Extensions.Optional`1[System.Int32]", () => ((IComparable)new Optional<int>(5)).CompareTo(5));
    }

    [Theory]
    [InlineData(42, 43, null)]
#pragma warning disable xUnit1010
    [InlineData(42f, 43f, float.NaN)]
#pragma warning restore xUnit1010
    public void EqualityOperator<T>(T value1, T value2, T? nan)
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
    public void InequalityOperator<T>(T value1, T value2, T? nan)
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
    public void EqualityOperator_CustomComparable()
    {
        Assert.True(new Optional<CustomComparable>() == default);
        Assert.True(new Optional<CustomComparable>(new(1)) == new Optional<CustomComparable>(new(2)));
        Assert.False(new Optional<CustomComparable>() == new Optional<CustomComparable>(new(1)));
        Assert.False(new Optional<CustomComparable>(new(1)) == new Optional<CustomComparable>());
        Assert.False(new Optional<CustomComparable>(new(2)) == new Optional<CustomComparable>(new(2)));
    }

    [Fact]
    public void InequalityOperator_CustomComparable()
    {
        Assert.False(new Optional<CustomComparable>() != default);
        Assert.False(new Optional<CustomComparable>(new(1)) != new Optional<CustomComparable>(new(2)));
        Assert.True(new Optional<CustomComparable>() != new Optional<CustomComparable>(new(1)));
        Assert.True(new Optional<CustomComparable>(new(1)) != new Optional<CustomComparable>());
        Assert.True(new Optional<CustomComparable>(new(-1)) != new Optional<CustomComparable>(new(1)));
    }

    [Fact]
    public void GetHashCode_Override()
    {
        Assert.Equal(-1, new Optional<int>().GetHashCode());
        Assert.Equal(0, new Optional<string?>(null).GetHashCode());
#pragma warning disable CA1307 // Specify StringComparison for clarity
        Assert.Equal("hello".GetHashCode(), new Optional<string>("hello").GetHashCode());
#pragma warning restore CA1307 // Specify StringComparison for clarity
    }

    [Fact]
    public void GetHashCode_CustomComparer()
    {
        Assert.Equal(-1, new Optional<string>().GetHashCode(StringComparer.OrdinalIgnoreCase));
        Assert.Equal(StringComparer.OrdinalIgnoreCase.GetHashCode("hello"), new Optional<string>("hello").GetHashCode(StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void GetHashCode_IStructuralEquatable()
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
