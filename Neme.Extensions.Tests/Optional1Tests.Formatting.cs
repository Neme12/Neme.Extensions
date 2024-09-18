using Neme.Extensions.Tests.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Neme.Extensions.Tests;

public sealed partial class Optional1Tests
{
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
    public void Parse_Null()
    {
        AssertDoesNotParse<int>(null, null, null, parseFromSpan: false);
        AssertDoesNotParse<int>(null, null, CultureInfo.InvariantCulture, parseFromSpan: false);
        AssertDoesNotParse<int>(null, null, CultureInfo.GetCultureInfo("de"), parseFromSpan: false);
    }

    [Fact]
    public void Parse_None()
    {
        AssertParses<int>(default, "None", null, parseFromSpan: true);
        AssertParses<int>(default, "None", CultureInfo.InvariantCulture, parseFromSpan: true);
        AssertParses<int>(default, "None", CultureInfo.GetCultureInfo("de"), parseFromSpan: true);
    }

    [Fact]
    public void Parse_Some()
    {
        AssertParses<double>(new(42.2), $"Some {{ {42.2} }}", null);
        AssertParses<double>(new(42.2), "Some { 42.2 }", CultureInfo.InvariantCulture);
        AssertParses<double>(new(42.2), "Some { 42,2 }", CultureInfo.GetCultureInfo("de"));
    }

    [Fact]
    public void Parse_Version()
    {
        AssertParses<Version>(new(new(1, 2, 3, 4)), "Some { 1.2.3.4 }", null);
        AssertParses<Version>(new(new(1, 2, 3, 4)), "Some { 1.2.3.4 }", CultureInfo.InvariantCulture);
        AssertParses<Version>(new(new(1, 2, 3, 4)), "Some { 1.2.3.4 }", CultureInfo.GetCultureInfo("de"));
    }

    [Fact]
    public void Parse_Some_EmptyString()
    {
        AssertParses<string?>(new(""), "Some {  }", null, parseFromSpan: true);
        AssertParses<string?>(new(""), "Some {  }", CultureInfo.InvariantCulture, parseFromSpan: true);
        AssertParses<string?>(new(""), "Some {  }", CultureInfo.GetCultureInfo("de"), parseFromSpan: true);
    }

    [Fact]
    public void Parse_Some_String()
    {
        AssertParses<string>(new("hello"), "Some { hello }", null, parseFromSpan: true);
        AssertParses<string>(new("hello"), "Some { hello }", CultureInfo.InvariantCulture, parseFromSpan: true);
        AssertParses<string>(new("hello"), "Some { hello }", CultureInfo.GetCultureInfo("de"), parseFromSpan: true);
    }

    [Fact]
    public void Parse_Some_ReadOnlyMemory()
    {
        AssertParses<ReadOnlyMemory<char>>(new("hello".AsMemory()), "Some { hello }", null, parseFromSpan: true);
        AssertParses<ReadOnlyMemory<char>>(new("hello".AsMemory()), "Some { hello }", CultureInfo.InvariantCulture, parseFromSpan: true);
        AssertParses<ReadOnlyMemory<char>>(new("hello".AsMemory()), "Some { hello }", CultureInfo.GetCultureInfo("de"), parseFromSpan: true);
    }

    [Fact]
    public void Parse_Some_Memory()
    {
        AssertParses<Memory<char>>(new("hello".ToArray()), "Some { hello }", null, parseFromSpan: true);
        AssertParses<Memory<char>>(new("hello".ToArray()), "Some { hello }", CultureInfo.InvariantCulture, parseFromSpan: true);
        AssertParses<Memory<char>>(new("hello".ToArray()), "Some { hello }", CultureInfo.GetCultureInfo("de"), parseFromSpan: true);
    }

    [Fact]
    public void Parse_Some_Char()
    {
        AssertParses<char>(new('a'), "Some { a }", null, parseFromSpan: true);
        AssertParses<char>(new('a'), "Some { a }", CultureInfo.InvariantCulture, parseFromSpan: true);
        AssertParses<char>(new('a'), "Some { a }", CultureInfo.GetCultureInfo("de"), parseFromSpan: true);
        AssertDoesNotParse<char>("Some {  }", "", null, parseFromSpan: true);
        AssertDoesNotParse<char>("Some { ab }", "ab", null, parseFromSpan: true);
    }

#if NETCOREAPP3_0_OR_GREATER
    [Fact]
    public void Parse_Some_Rune()
    {
        AssertParses<Rune>(new(new(0x1f642)), "Some { 🙂 }", null, parseFromSpan: true);
        AssertParses<Rune>(new(new(0x1f642)), "Some { 🙂 }", CultureInfo.InvariantCulture, parseFromSpan: true);
        AssertParses<Rune>(new(new(0x1f642)), "Some { 🙂 }", CultureInfo.GetCultureInfo("de"), parseFromSpan: true);
        AssertDoesNotParse<Rune>("Some {  }", "", null, parseFromSpan: true);
        AssertDoesNotParse<Rune>("Some { ab }", "ab", null, parseFromSpan: true);
    }
#endif

    [Fact]
    public void Parse_Some_Null()
    {
        AssertParses<string?>(new(null), "Some { }", null, parseFromSpan: true);
        AssertParses<string?>(new(null), "Some { }", CultureInfo.InvariantCulture, parseFromSpan: true);
        AssertParses<string?>(new(null), "Some { }", CultureInfo.GetCultureInfo("de"), parseFromSpan: true);
    }

    [Fact]
    public void Parse_InvalidStrings()
    {
        AssertDoesNotParse<int>("", null, null, parseFromSpan: true);
        AssertDoesNotParse<int>("42", null, null, parseFromSpan: true);
        AssertDoesNotParse<int>("{ }", null, null, parseFromSpan: true);
        AssertDoesNotParse<int>("{ 42 }", null, null, parseFromSpan: true);
        AssertDoesNotParse<int>("Some", null, null, parseFromSpan: true);
        AssertDoesNotParse<int>("Some {}", null, null, parseFromSpan: true);
        AssertDoesNotParse<int>("Some { x }", "x", null);
        AssertDoesNotParse<int>("SOME { 42 }", null, null, parseFromSpan: true);
    }

    [Fact]
    public void Parse_BultInTypesNumberStyles()
    {
        TestIsInteger<ushort>(null);
        TestIsInteger<short>(-1);
        TestIsInteger<uint>(null);
        TestIsInteger<int>(-1);
        TestIsInteger<ulong>(null);
        TestIsInteger<long>(-1);
#if NET7_0_OR_GREATER
        TestIsInteger<UInt128>(null);
        TestIsInteger<Int128>(-1);
#endif
#if NET5_0_OR_GREATER
        TestIsInteger<nuint>(null);
        TestIsInteger<nint>(-1);
#endif
        TestIsInteger<BigInteger>(BigInteger.MinusOne);

        TestIsFloatAndAllowThousands<float>(-1f, 42.2f, 10_000f);
        TestIsFloatAndAllowThousands<double>(-1d, 42.2d, 10_000d);
#if NET5_0_OR_GREATER
        TestIsFloatAndAllowThousands<Half>((Half)(-1f), (Half)42.2f, (Half)10_000f);
#endif

        TestIsNumber<decimal>(-1m, 42.2m, 10_000m);

        static void TestIsInteger<T>(T? negativeOne)
            where T : struct
        {
            // NumberStyles.AllowLeadingWhite and NumberStyles.AllowTrailingWhite are included.
            AssertParses<T>(new(default), "Some {  0  }", null);

            if (negativeOne is not null)
            {
                // NumberStyles.AllowLeadingSign is included
                AssertParses<T>(new(negativeOne.Value), "Some { -1 }", null);
            }

            // NumberStyles.AllowDecimalPoint is *not* included.
            AssertDoesNotParse<T>("Some { 42.2 }", "42.2", null);

            // NumberStyles.AllowThousands is *not* included.
            AssertDoesNotParse<T>("Some { 10,000 }", "10,000", null);

            // NumberStyles.AllowExponent is *not* included.
            AssertDoesNotParse<T>("Some { 4.22e+1 }", "4.22e+1", null);

            // NumberStyles.AllowParentheses is *not* included.
            AssertDoesNotParse<T>("Some { (42) }", "(42)", null);

            // NumberStyles.AllowHexSpecifier is *not* included.
            AssertDoesNotParse<T>("Some { f }", "f", null);
        }

        static void TestIsFloatAndAllowThousands<T>(T negativeOne, T value1, T value2)
            where T : struct
        {
            // NumberStyles.AllowLeadingWhite and NumberStyles.AllowTrailingWhite are included.
            AssertParses<T>(new(default), "Some {  0  }", null);

            // NumberStyles.AllowLeadingSign is included
            AssertParses<T>(new(negativeOne), "Some { -1 }", null);

            // NumberStyles.AllowDecimalPoint is included.
            AssertParses<T>(new(value1), "Some { 42.2 }", null);

            // NumberStyles.AllowThousands is included.
            AssertParses<T>(new(value2), "Some { 10,000 }", null);

            // NumberStyles.AllowExponent is included.
            AssertParses<T>(new(value1), "Some { 4.22e+1 }", null);

            // NumberStyles.AllowParentheses is *not* included.
            AssertDoesNotParse<T>("Some { (42) }", "(42)", null);

            // NumberStyles.AllowHexSpecifier is *not* included.
            AssertDoesNotParse<T>("Some { f }", "f", null);
        }

        static void TestIsNumber<T>(T negativeOne, T value1, T value2)
            where T : struct
        {
            // NumberStyles.AllowLeadingWhite and NumberStyles.AllowTrailingWhite are included.
            AssertParses<T>(new(default), "Some {  0  }", null);

            // NumberStyles.AllowLeadingSign is included
            AssertParses<T>(new(negativeOne), "Some { -1 }", null);

            // NumberStyles.AllowDecimalPoint is included.
            AssertParses<T>(new(value1), "Some { 42.2 }", null);

            // NumberStyles.AllowThousands is included.
            AssertParses<T>(new(value2), "Some { 10,000 }", null);

            // NumberStyles.AllowExponent is *not* included.
            AssertDoesNotParse<T>("Some { 4.22e+1 }", "4.22e+1", null);

            // NumberStyles.AllowParentheses is *not* included.
            AssertDoesNotParse<T>("Some { (42) }", "(42)", null);

            // NumberStyles.AllowHexSpecifier is *not* included.
            AssertDoesNotParse<T>("Some { f }", "f", null);
        }
    }

    [Fact]
    public void Parse_CustomParsableNumberStyles()
    {
        AssertParses<CustomParsable>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);

        AssertParses<Int>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<IntCustomParsable>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<Int2CustomParsable>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<intCustomParsable>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<IntegerCustomParsable>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsableContainingNaN1>(new(new("foo", NumberStyles.Float | NumberStyles.AllowThousands)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsableContainingNaN2>(new(new("foo", NumberStyles.Float | NumberStyles.AllowThousands)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsableContainingNaNWrong1>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsableContainingNaNWrong2>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsableContainingNaNWrong3>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsableContainingNaNWrong4>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsableContainingNaNWrong5>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsableContainingNaNWrong6>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);

#if NET7_0_OR_GREATER
        AssertParses<CustomParsableIBinaryInteger>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsableIFloatingPointIeee754>(new(new("foo", NumberStyles.Float | NumberStyles.AllowThousands)), "Some { foo }", null, parseFromSpan: true);
#endif
    }

    [Fact]
    public void Parse_DefaultedNumberStyles()
    {
        AssertParses<IntDefaultedCustomParsableNumber>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true, tryParse: false);
        AssertParses<DefaultedCustomParsableInteger>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null, parseFromSpan: true, tryParse: false);
        AssertParses<DefaultedCustomParsableFloat>(new(new("foo", NumberStyles.Float)), "Some { foo }", null, parseFromSpan: true, tryParse: false);
    }

    private abstract record CustomParsableBase<TSelf>(string? Input, NumberStyles Style)
        where TSelf : CustomParsableBase<TSelf>, new()
    {
        public static TSelf Parse(string s, NumberStyles style, IFormatProvider? provider) =>
            new() { Input = s, Style = style };

        public static TSelf Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) =>
            new() { Input = s.ToString(), Style = style };

        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result)
        {
            if (s is null)
            {
                result = default;
                return false;
            }

            result = new() { Input = s, Style = style };
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result)
        {
            result = new() { Input = s.ToString(), Style = style };
            return true;
        }
    }

    private sealed record CustomParsable(string? Input, NumberStyles Style) : CustomParsableBase<CustomParsable>(Input, Style)
    {
        public CustomParsable() : this(default, default)
        {
        }
    }

    private sealed record Int(string? Input, NumberStyles Style) : CustomParsableBase<Int>(Input, Style)
    {
        public Int() : this(default, default)
        {
        }
    }

    private sealed record IntCustomParsable(string? Input, NumberStyles Style) : CustomParsableBase<IntCustomParsable>(Input, Style)
    {
        public IntCustomParsable() : this(default, default)
        {
        }
    }

    private sealed record Int2CustomParsable(string? Input, NumberStyles Style) : CustomParsableBase<Int2CustomParsable>(Input, Style)
    {
        public Int2CustomParsable() : this(default, default)
        {
        }
    }

    private sealed record intCustomParsable(string? Input, NumberStyles Style) : CustomParsableBase<intCustomParsable>(Input, Style)
    {
        public intCustomParsable() : this(default, default)
        {
        }
    }

    private sealed record IntegerCustomParsable(string? Input, NumberStyles Style) : CustomParsableBase<IntegerCustomParsable>(Input, Style)
    {
        public IntegerCustomParsable() : this(default, default)
        {
        }
    }

    private sealed record CustomParsableContainingNaN1(string? Input, NumberStyles Style) : CustomParsableBase<CustomParsableContainingNaN1>(Input, Style)
    {
        public CustomParsableContainingNaN1() : this(default, default)
        {
        }

        public static readonly CustomParsableContainingNaN1 NaN = default!;
    }

    private sealed record CustomParsableContainingNaN2(string? Input, NumberStyles Style) : CustomParsableBase<CustomParsableContainingNaN2>(Input, Style)
    {
        public CustomParsableContainingNaN2() : this(default, default)
        {
        }

        public static CustomParsableContainingNaN2 NaN => default!;
    }

    private sealed record CustomParsableContainingNaNWrong1(string? Input, NumberStyles Style) : CustomParsableBase<CustomParsableContainingNaNWrong1>(Input, Style)
    {
        public CustomParsableContainingNaNWrong1() : this(default, default)
        {
        }

        public static int NaN => default;
    }

    private sealed record CustomParsableContainingNaNWrong2(string? Input, NumberStyles Style) : CustomParsableBase<CustomParsableContainingNaNWrong2>(Input, Style)
    {
        public CustomParsableContainingNaNWrong2() : this(default, default)
        {
        }

        public static CustomParsableContainingNaNWrong2 NaN { get; set; } = default!;
    }

    private sealed record CustomParsableContainingNaNWrong3(string? Input, NumberStyles Style) : CustomParsableBase<CustomParsableContainingNaNWrong3>(Input, Style)
    {
        public CustomParsableContainingNaNWrong3() : this(default, default)
        {
        }

        public static CustomParsableContainingNaNWrong3 NaN = default!;
    }

    private sealed record CustomParsableContainingNaNWrong4(string? Input, NumberStyles Style) : CustomParsableBase<CustomParsableContainingNaNWrong4>(Input, Style)
    {
        public CustomParsableContainingNaNWrong4() : this(default, default)
        {
        }

        public CustomParsableContainingNaNWrong4 NaN => default!;
    }

    private sealed record CustomParsableContainingNaNWrong5(string? Input, NumberStyles Style) : CustomParsableBase<CustomParsableContainingNaNWrong5>(Input, Style)
    {
        public CustomParsableContainingNaNWrong5() : this(default, default)
        {
        }

        internal static CustomParsableContainingNaNWrong5 NaN => default!;
    }

    private sealed record CustomParsableContainingNaNWrong6(string? Input, NumberStyles Style) : CustomParsableBase<CustomParsableContainingNaNWrong6>(Input, Style)
    {
        public CustomParsableContainingNaNWrong6() : this(default, default)
        {
        }

        public static CustomParsableContainingNaNWrong6 NaN() => default!;
    }

#if NET7_0_OR_GREATER
    private sealed record CustomParsableIBinaryInteger(string? Input, NumberStyles Style) : CustomParsableBase<CustomParsableIBinaryInteger>(Input, Style), IBinaryInteger<CustomParsableIBinaryInteger>
    {
        public CustomParsableIBinaryInteger() : this(default, default)
        {
        }

        static CustomParsableIBinaryInteger INumberBase<CustomParsableIBinaryInteger>.One => throw new NotImplementedException();

        static int INumberBase<CustomParsableIBinaryInteger>.Radix => throw new NotImplementedException();

        static CustomParsableIBinaryInteger INumberBase<CustomParsableIBinaryInteger>.Zero => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IAdditiveIdentity<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.AdditiveIdentity => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IMultiplicativeIdentity<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.MultiplicativeIdentity => throw new NotImplementedException();

        static CustomParsableIBinaryInteger INumberBase<CustomParsableIBinaryInteger>.Abs(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsCanonical(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsComplexNumber(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsEvenInteger(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsFinite(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsImaginaryNumber(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsInfinity(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsInteger(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsNaN(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsNegative(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsNegativeInfinity(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsNormal(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsOddInteger(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsPositive(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsPositiveInfinity(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool IBinaryNumber<CustomParsableIBinaryInteger>.IsPow2(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsRealNumber(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsSubnormal(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.IsZero(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IBinaryNumber<CustomParsableIBinaryInteger>.Log2(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger INumberBase<CustomParsableIBinaryInteger>.MaxMagnitude(CustomParsableIBinaryInteger x, CustomParsableIBinaryInteger y) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger INumberBase<CustomParsableIBinaryInteger>.MaxMagnitudeNumber(CustomParsableIBinaryInteger x, CustomParsableIBinaryInteger y) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger INumberBase<CustomParsableIBinaryInteger>.MinMagnitude(CustomParsableIBinaryInteger x, CustomParsableIBinaryInteger y) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger INumberBase<CustomParsableIBinaryInteger>.MinMagnitudeNumber(CustomParsableIBinaryInteger x, CustomParsableIBinaryInteger y) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger INumberBase<CustomParsableIBinaryInteger>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger INumberBase<CustomParsableIBinaryInteger>.Parse(string s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger ISpanParsable<CustomParsableIBinaryInteger>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IParsable<CustomParsableIBinaryInteger>.Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IBinaryInteger<CustomParsableIBinaryInteger>.PopCount(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IBinaryInteger<CustomParsableIBinaryInteger>.TrailingZeroCount(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.TryConvertFromChecked<TOther>(TOther value, out CustomParsableIBinaryInteger result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.TryConvertFromSaturating<TOther>(TOther value, out CustomParsableIBinaryInteger result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.TryConvertFromTruncating<TOther>(TOther value, out CustomParsableIBinaryInteger result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.TryConvertToChecked<TOther>(CustomParsableIBinaryInteger value, out TOther result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.TryConvertToSaturating<TOther>(CustomParsableIBinaryInteger value, out TOther result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.TryConvertToTruncating<TOther>(CustomParsableIBinaryInteger value, out TOther result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out CustomParsableIBinaryInteger result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIBinaryInteger>.TryParse(string? s, NumberStyles style, IFormatProvider? provider, out CustomParsableIBinaryInteger result) => throw new NotImplementedException();

        static bool ISpanParsable<CustomParsableIBinaryInteger>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CustomParsableIBinaryInteger result) => throw new NotImplementedException();

        static bool IParsable<CustomParsableIBinaryInteger>.TryParse(string? s, IFormatProvider? provider, out CustomParsableIBinaryInteger result) => throw new NotImplementedException();

        static bool IBinaryInteger<CustomParsableIBinaryInteger>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static bool IBinaryInteger<CustomParsableIBinaryInteger>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        int IComparable.CompareTo(object? obj) => throw new NotImplementedException();

        int IComparable<CustomParsableIBinaryInteger>.CompareTo(CustomParsableIBinaryInteger? other) => throw new NotImplementedException();

        int IBinaryInteger<CustomParsableIBinaryInteger>.GetByteCount() => throw new NotImplementedException();

        int IBinaryInteger<CustomParsableIBinaryInteger>.GetShortestBitLength() => throw new NotImplementedException();

        string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
            ToString();

        bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotImplementedException();

        bool IBinaryInteger<CustomParsableIBinaryInteger>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        bool IBinaryInteger<CustomParsableIBinaryInteger>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IUnaryPlusOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.operator +(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IAdditionOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.operator +(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IUnaryNegationOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.operator -(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger ISubtractionOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.operator -(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IBitwiseOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.operator ~(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IIncrementOperators<CustomParsableIBinaryInteger>.operator ++(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IDecrementOperators<CustomParsableIBinaryInteger>.operator --(CustomParsableIBinaryInteger value) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IMultiplyOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.operator *(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IDivisionOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.operator /(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IModulusOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.operator %(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IBitwiseOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.operator &(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IBitwiseOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.operator |(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IBitwiseOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, CustomParsableIBinaryInteger>.operator ^(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IShiftOperators<CustomParsableIBinaryInteger, int, CustomParsableIBinaryInteger>.operator <<(CustomParsableIBinaryInteger value, int shiftAmount) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IShiftOperators<CustomParsableIBinaryInteger, int, CustomParsableIBinaryInteger>.operator >>(CustomParsableIBinaryInteger value, int shiftAmount) => throw new NotImplementedException();

        static bool IEqualityOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, bool>.operator ==(CustomParsableIBinaryInteger? left, CustomParsableIBinaryInteger? right) => throw new NotImplementedException();

        static bool IEqualityOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, bool>.operator !=(CustomParsableIBinaryInteger? left, CustomParsableIBinaryInteger? right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, bool>.operator <(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, bool>.operator >(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, bool>.operator <=(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, bool>.operator >=(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IShiftOperators<CustomParsableIBinaryInteger, int, CustomParsableIBinaryInteger>.operator >>>(CustomParsableIBinaryInteger value, int shiftAmount) => throw new NotImplementedException();
    }

    private sealed record CustomParsableIFloatingPointIeee754(string? Input, NumberStyles Style) : CustomParsableBase<CustomParsableIFloatingPointIeee754>(Input, Style), IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>
    {
        public CustomParsableIFloatingPointIeee754() : this(default, default)
        {
        }

        static CustomParsableIFloatingPointIeee754 IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.Epsilon => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.NaN => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.NegativeInfinity => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.NegativeZero => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.PositiveInfinity => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ISignedNumber<CustomParsableIFloatingPointIeee754>.NegativeOne => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointConstants<CustomParsableIFloatingPointIeee754>.E => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointConstants<CustomParsableIFloatingPointIeee754>.Pi => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointConstants<CustomParsableIFloatingPointIeee754>.Tau => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 INumberBase<CustomParsableIFloatingPointIeee754>.One => throw new NotImplementedException();

        static int INumberBase<CustomParsableIFloatingPointIeee754>.Radix => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 INumberBase<CustomParsableIFloatingPointIeee754>.Zero => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IAdditiveIdentity<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754>.AdditiveIdentity => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IMultiplicativeIdentity<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754>.MultiplicativeIdentity => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 INumberBase<CustomParsableIFloatingPointIeee754>.Abs(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.Acos(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IHyperbolicFunctions<CustomParsableIFloatingPointIeee754>.Acosh(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.AcosPi(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.Asin(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IHyperbolicFunctions<CustomParsableIFloatingPointIeee754>.Asinh(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.AsinPi(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.Atan(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.Atan2(CustomParsableIFloatingPointIeee754 y, CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.Atan2Pi(CustomParsableIFloatingPointIeee754 y, CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IHyperbolicFunctions<CustomParsableIFloatingPointIeee754>.Atanh(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.AtanPi(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.BitDecrement(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.BitIncrement(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IRootFunctions<CustomParsableIFloatingPointIeee754>.Cbrt(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.Cos(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IHyperbolicFunctions<CustomParsableIFloatingPointIeee754>.Cosh(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.CosPi(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IExponentialFunctions<CustomParsableIFloatingPointIeee754>.Exp(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IExponentialFunctions<CustomParsableIFloatingPointIeee754>.Exp10(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IExponentialFunctions<CustomParsableIFloatingPointIeee754>.Exp2(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.FusedMultiplyAdd(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right, CustomParsableIFloatingPointIeee754 addend) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IRootFunctions<CustomParsableIFloatingPointIeee754>.Hypot(CustomParsableIFloatingPointIeee754 x, CustomParsableIFloatingPointIeee754 y) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.Ieee754Remainder(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static int IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.ILogB(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsCanonical(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsComplexNumber(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsEvenInteger(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsFinite(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsImaginaryNumber(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsInfinity(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsInteger(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsNaN(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsNegative(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsNegativeInfinity(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsNormal(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsOddInteger(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsPositive(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsPositiveInfinity(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsRealNumber(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsSubnormal(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.IsZero(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ILogarithmicFunctions<CustomParsableIFloatingPointIeee754>.Log(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ILogarithmicFunctions<CustomParsableIFloatingPointIeee754>.Log(CustomParsableIFloatingPointIeee754 x, CustomParsableIFloatingPointIeee754 newBase) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ILogarithmicFunctions<CustomParsableIFloatingPointIeee754>.Log10(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ILogarithmicFunctions<CustomParsableIFloatingPointIeee754>.Log2(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 INumberBase<CustomParsableIFloatingPointIeee754>.MaxMagnitude(CustomParsableIFloatingPointIeee754 x, CustomParsableIFloatingPointIeee754 y) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 INumberBase<CustomParsableIFloatingPointIeee754>.MaxMagnitudeNumber(CustomParsableIFloatingPointIeee754 x, CustomParsableIFloatingPointIeee754 y) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 INumberBase<CustomParsableIFloatingPointIeee754>.MinMagnitude(CustomParsableIFloatingPointIeee754 x, CustomParsableIFloatingPointIeee754 y) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 INumberBase<CustomParsableIFloatingPointIeee754>.MinMagnitudeNumber(CustomParsableIFloatingPointIeee754 x, CustomParsableIFloatingPointIeee754 y) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 INumberBase<CustomParsableIFloatingPointIeee754>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 INumberBase<CustomParsableIFloatingPointIeee754>.Parse(string s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ISpanParsable<CustomParsableIFloatingPointIeee754>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IParsable<CustomParsableIFloatingPointIeee754>.Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IPowerFunctions<CustomParsableIFloatingPointIeee754>.Pow(CustomParsableIFloatingPointIeee754 x, CustomParsableIFloatingPointIeee754 y) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IRootFunctions<CustomParsableIFloatingPointIeee754>.RootN(CustomParsableIFloatingPointIeee754 x, int n) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPoint<CustomParsableIFloatingPointIeee754>.Round(CustomParsableIFloatingPointIeee754 x, int digits, MidpointRounding mode) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>.ScaleB(CustomParsableIFloatingPointIeee754 x, int n) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.Sin(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static (CustomParsableIFloatingPointIeee754 Sin, CustomParsableIFloatingPointIeee754 Cos) ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.SinCos(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static (CustomParsableIFloatingPointIeee754 SinPi, CustomParsableIFloatingPointIeee754 CosPi) ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.SinCosPi(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IHyperbolicFunctions<CustomParsableIFloatingPointIeee754>.Sinh(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.SinPi(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IRootFunctions<CustomParsableIFloatingPointIeee754>.Sqrt(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.Tan(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IHyperbolicFunctions<CustomParsableIFloatingPointIeee754>.Tanh(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ITrigonometricFunctions<CustomParsableIFloatingPointIeee754>.TanPi(CustomParsableIFloatingPointIeee754 x) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.TryConvertFromChecked<TOther>(TOther value, out CustomParsableIFloatingPointIeee754 result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.TryConvertFromSaturating<TOther>(TOther value, out CustomParsableIFloatingPointIeee754 result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.TryConvertFromTruncating<TOther>(TOther value, out CustomParsableIFloatingPointIeee754 result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.TryConvertToChecked<TOther>(CustomParsableIFloatingPointIeee754 value, out TOther result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.TryConvertToSaturating<TOther>(CustomParsableIFloatingPointIeee754 value, out TOther result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.TryConvertToTruncating<TOther>(CustomParsableIFloatingPointIeee754 value, out TOther result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out CustomParsableIFloatingPointIeee754 result) => throw new NotImplementedException();

        static bool INumberBase<CustomParsableIFloatingPointIeee754>.TryParse(string? s, NumberStyles style, IFormatProvider? provider, out CustomParsableIFloatingPointIeee754 result) => throw new NotImplementedException();

        static bool ISpanParsable<CustomParsableIFloatingPointIeee754>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CustomParsableIFloatingPointIeee754 result) => throw new NotImplementedException();

        static bool IParsable<CustomParsableIFloatingPointIeee754>.TryParse(string? s, IFormatProvider? provider, out CustomParsableIFloatingPointIeee754 result) => throw new NotImplementedException();

        int IComparable.CompareTo(object? obj) => throw new NotImplementedException();

        int IComparable<CustomParsableIFloatingPointIeee754>.CompareTo(CustomParsableIFloatingPointIeee754? other) => throw new NotImplementedException();

        int IFloatingPoint<CustomParsableIFloatingPointIeee754>.GetExponentByteCount() => throw new NotImplementedException();

        int IFloatingPoint<CustomParsableIFloatingPointIeee754>.GetExponentShortestBitLength() => throw new NotImplementedException();

        int IFloatingPoint<CustomParsableIFloatingPointIeee754>.GetSignificandBitLength() => throw new NotImplementedException();

        int IFloatingPoint<CustomParsableIFloatingPointIeee754>.GetSignificandByteCount() => throw new NotImplementedException();

        string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
            ToString();

        bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotImplementedException();

        bool IFloatingPoint<CustomParsableIFloatingPointIeee754>.TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        bool IFloatingPoint<CustomParsableIFloatingPointIeee754>.TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        bool IFloatingPoint<CustomParsableIFloatingPointIeee754>.TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        bool IFloatingPoint<CustomParsableIFloatingPointIeee754>.TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IUnaryPlusOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754>.operator +(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IAdditionOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754>.operator +(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IUnaryNegationOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754>.operator -(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 ISubtractionOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754>.operator -(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IIncrementOperators<CustomParsableIFloatingPointIeee754>.operator ++(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IDecrementOperators<CustomParsableIFloatingPointIeee754>.operator --(CustomParsableIFloatingPointIeee754 value) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IMultiplyOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754>.operator *(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IDivisionOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754>.operator /(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static CustomParsableIFloatingPointIeee754 IModulusOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754>.operator %(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static bool IEqualityOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, bool>.operator ==(CustomParsableIFloatingPointIeee754? left, CustomParsableIFloatingPointIeee754? right) => throw new NotImplementedException();

        static bool IEqualityOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, bool>.operator !=(CustomParsableIFloatingPointIeee754? left, CustomParsableIFloatingPointIeee754? right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, bool>.operator <(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, bool>.operator >(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, bool>.operator <=(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, bool>.operator >=(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();
    }
#endif

    private sealed record IntDefaultedCustomParsableNumber(string? Input, NumberStyles Style)
    {
        public static IntDefaultedCustomParsableNumber NaN => default!;

        public static IntDefaultedCustomParsableNumber Parse(string s, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null) =>
            new(s, style);

        public static IntDefaultedCustomParsableNumber Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null) =>
            new(s.ToString(), style);
    }

    private sealed record DefaultedCustomParsableInteger(string? Input, NumberStyles Style)
    {
        public static DefaultedCustomParsableInteger NaN => default!;

        public static DefaultedCustomParsableInteger Parse(string s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) =>
            new(s, style);

        public static DefaultedCustomParsableInteger Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) =>
            new(s.ToString(), style);
    }

    private sealed record DefaultedCustomParsableFloat(string? Input, NumberStyles Style)
#if NET7_0_OR_GREATER
        : IBinaryInteger<DefaultedCustomParsableFloat>
#endif
    {
        public static DefaultedCustomParsableFloat Parse(string s, NumberStyles style = NumberStyles.Float, IFormatProvider? provider = null) =>
            new(s, style);

        public static DefaultedCustomParsableFloat Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Float, IFormatProvider? provider = null) =>
            new(s.ToString(), style);

#if NET7_0_OR_GREATER
        static DefaultedCustomParsableFloat INumberBase<DefaultedCustomParsableFloat>.One => throw new NotImplementedException();

        static int INumberBase<DefaultedCustomParsableFloat>.Radix => throw new NotImplementedException();

        static DefaultedCustomParsableFloat INumberBase<DefaultedCustomParsableFloat>.Zero => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IAdditiveIdentity<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.AdditiveIdentity => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IMultiplicativeIdentity<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.MultiplicativeIdentity => throw new NotImplementedException();

        static DefaultedCustomParsableFloat INumberBase<DefaultedCustomParsableFloat>.Abs(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsCanonical(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsComplexNumber(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsEvenInteger(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsFinite(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsImaginaryNumber(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsInfinity(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsInteger(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsNaN(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsNegative(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsNegativeInfinity(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsNormal(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsOddInteger(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsPositive(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsPositiveInfinity(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool IBinaryNumber<DefaultedCustomParsableFloat>.IsPow2(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsRealNumber(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsSubnormal(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.IsZero(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IBinaryNumber<DefaultedCustomParsableFloat>.Log2(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat INumberBase<DefaultedCustomParsableFloat>.MaxMagnitude(DefaultedCustomParsableFloat x, DefaultedCustomParsableFloat y) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat INumberBase<DefaultedCustomParsableFloat>.MaxMagnitudeNumber(DefaultedCustomParsableFloat x, DefaultedCustomParsableFloat y) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat INumberBase<DefaultedCustomParsableFloat>.MinMagnitude(DefaultedCustomParsableFloat x, DefaultedCustomParsableFloat y) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat INumberBase<DefaultedCustomParsableFloat>.MinMagnitudeNumber(DefaultedCustomParsableFloat x, DefaultedCustomParsableFloat y) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat INumberBase<DefaultedCustomParsableFloat>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat INumberBase<DefaultedCustomParsableFloat>.Parse(string s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat ISpanParsable<DefaultedCustomParsableFloat>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IParsable<DefaultedCustomParsableFloat>.Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IBinaryInteger<DefaultedCustomParsableFloat>.PopCount(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IBinaryInteger<DefaultedCustomParsableFloat>.TrailingZeroCount(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.TryConvertFromChecked<TOther>(TOther value, out DefaultedCustomParsableFloat result) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.TryConvertFromSaturating<TOther>(TOther value, out DefaultedCustomParsableFloat result) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.TryConvertFromTruncating<TOther>(TOther value, out DefaultedCustomParsableFloat result) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.TryConvertToChecked<TOther>(DefaultedCustomParsableFloat value, out TOther result) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.TryConvertToSaturating<TOther>(DefaultedCustomParsableFloat value, out TOther result) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.TryConvertToTruncating<TOther>(DefaultedCustomParsableFloat value, out TOther result) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out DefaultedCustomParsableFloat result) => throw new NotImplementedException();

        static bool INumberBase<DefaultedCustomParsableFloat>.TryParse(string? s, NumberStyles style, IFormatProvider? provider, out DefaultedCustomParsableFloat result) => throw new NotImplementedException();

        static bool ISpanParsable<DefaultedCustomParsableFloat>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out DefaultedCustomParsableFloat result) => throw new NotImplementedException();

        static bool IParsable<DefaultedCustomParsableFloat>.TryParse(string? s, IFormatProvider? provider, out DefaultedCustomParsableFloat result) => throw new NotImplementedException();

        static bool IBinaryInteger<DefaultedCustomParsableFloat>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static bool IBinaryInteger<DefaultedCustomParsableFloat>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        int IComparable.CompareTo(object? obj) => throw new NotImplementedException();

        int IComparable<DefaultedCustomParsableFloat>.CompareTo(DefaultedCustomParsableFloat? other) => throw new NotImplementedException();

        int IBinaryInteger<DefaultedCustomParsableFloat>.GetByteCount() => throw new NotImplementedException();

        int IBinaryInteger<DefaultedCustomParsableFloat>.GetShortestBitLength() => throw new NotImplementedException();

        string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
            ToString();

        bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotImplementedException();

        bool IBinaryInteger<DefaultedCustomParsableFloat>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        bool IBinaryInteger<DefaultedCustomParsableFloat>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IUnaryPlusOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.operator +(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IAdditionOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.operator +(DefaultedCustomParsableFloat left, DefaultedCustomParsableFloat right) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IUnaryNegationOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.operator -(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat ISubtractionOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.operator -(DefaultedCustomParsableFloat left, DefaultedCustomParsableFloat right) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IBitwiseOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.operator ~(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IIncrementOperators<DefaultedCustomParsableFloat>.operator ++(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IDecrementOperators<DefaultedCustomParsableFloat>.operator --(DefaultedCustomParsableFloat value) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IMultiplyOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.operator *(DefaultedCustomParsableFloat left, DefaultedCustomParsableFloat right) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IDivisionOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.operator /(DefaultedCustomParsableFloat left, DefaultedCustomParsableFloat right) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IModulusOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.operator %(DefaultedCustomParsableFloat left, DefaultedCustomParsableFloat right) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IBitwiseOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.operator &(DefaultedCustomParsableFloat left, DefaultedCustomParsableFloat right) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IBitwiseOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.operator |(DefaultedCustomParsableFloat left, DefaultedCustomParsableFloat right) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IBitwiseOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, DefaultedCustomParsableFloat>.operator ^(DefaultedCustomParsableFloat left, DefaultedCustomParsableFloat right) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IShiftOperators<DefaultedCustomParsableFloat, int, DefaultedCustomParsableFloat>.operator <<(DefaultedCustomParsableFloat value, int shiftAmount) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IShiftOperators<DefaultedCustomParsableFloat, int, DefaultedCustomParsableFloat>.operator >>(DefaultedCustomParsableFloat value, int shiftAmount) => throw new NotImplementedException();

        static bool IEqualityOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, bool>.operator ==(DefaultedCustomParsableFloat? left, DefaultedCustomParsableFloat? right) => throw new NotImplementedException();

        static bool IEqualityOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, bool>.operator !=(DefaultedCustomParsableFloat? left, DefaultedCustomParsableFloat? right) => throw new NotImplementedException();

        static bool IComparisonOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, bool>.operator <(DefaultedCustomParsableFloat left, DefaultedCustomParsableFloat right) => throw new NotImplementedException();

        static bool IComparisonOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, bool>.operator >(DefaultedCustomParsableFloat left, DefaultedCustomParsableFloat right) => throw new NotImplementedException();

        static bool IComparisonOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, bool>.operator <=(DefaultedCustomParsableFloat left, DefaultedCustomParsableFloat right) => throw new NotImplementedException();

        static bool IComparisonOperators<DefaultedCustomParsableFloat, DefaultedCustomParsableFloat, bool>.operator >=(DefaultedCustomParsableFloat left, DefaultedCustomParsableFloat right) => throw new NotImplementedException();

        static DefaultedCustomParsableFloat IShiftOperators<DefaultedCustomParsableFloat, int, DefaultedCustomParsableFloat>.operator >>>(DefaultedCustomParsableFloat value, int shiftAmount) => throw new NotImplementedException();
#endif
    }

    private static void AssertDoesNotParse<T>(string? input, string? nestedInput, IFormatProvider? provider, bool? parseFromSpan = null, bool tryParse = true)
    {
        var parseSpan = parseFromSpan ?? ShouldParseSpan<T>();

        if (provider is null)
        {
#pragma warning disable CA1305 // Specify IFormatProvider
            if (input is null)
            {
                Assert.Throws<ArgumentNullException>("s", () => Optional<T>.Parse(input!));
            }
            else
            {
                AssertThrows.Format(input, nestedInput, () => Optional<T>.Parse(input));

                if (parseSpan)
                    AssertThrows.Format(input, nestedInput, () => Optional<T>.Parse(input.AsSpan()));
            }

            if (tryParse)
            {
                Assert.False(Optional<T>.TryParse(input, out var resultWithoutProvider1));
                Assert.Equal(default, resultWithoutProvider1);

                if (parseSpan)
                {
                    Assert.False(Optional<T>.TryParse(input.AsSpan(), out var resultWithoutProvider2));
                    Assert.Equal(default, resultWithoutProvider2);
                }
            }
#pragma warning restore CA1305 // Specify IFormatProvider
        }

        if (input is null)
        {
            Assert.Throws<ArgumentNullException>("s", () => Optional<T>.Parse(input!, provider));
        }
        else
        {
            AssertThrows.Format(input, nestedInput, () => Optional<T>.Parse(input, provider));

            if (parseSpan)
                AssertThrows.Format(input, nestedInput, () => Optional<T>.Parse(input.AsSpan(), provider));
        }

        if (tryParse)
        {
            Assert.False(Optional<T>.TryParse(input, provider, out var resultWithProvider1));
            Assert.Equal(default, resultWithProvider1);

            if (parseSpan)
            {
                Assert.False(Optional<T>.TryParse(input.AsSpan(), provider, out var resultWithProvider2));
                Assert.Equal(default, resultWithProvider2);
            }
        }
    }

    private static void AssertParses<T>(Optional<T> expected, string input, IFormatProvider? provider, bool? parseFromSpan = null, bool tryParse = true)
    {
        var parseSpan = parseFromSpan ?? ShouldParseSpan<T>();
        var comparer = new CustomComparer<T>();

        if (provider is null)
        {
#pragma warning disable CA1305 // Specify IFormatProvider
            Assert.Equal(expected, Optional<T>.Parse(input), comparer);

            if (parseSpan)
                Assert.Equal(expected, Optional<T>.Parse(input.AsSpan()), comparer);
#pragma warning restore CA1305 // Specify IFormatProvider

            if (tryParse)
            {
                Assert.True(Optional<T>.TryParse(input, out var resultWithoutProvider1));
                Assert.Equal(expected, resultWithoutProvider1, comparer);

                if (parseSpan)
                {
                    Assert.True(Optional<T>.TryParse(input.AsSpan(), out var resultWithoutProvider2));
                    Assert.Equal(expected, resultWithoutProvider2, comparer);
                }
            }
        }

        Assert.Equal(expected, Optional<T>.Parse(input, provider), comparer);

        if (parseSpan)
            Assert.Equal(expected, Optional<T>.Parse(input.AsSpan(), provider), comparer);

        if (tryParse)
        {
            Assert.True(Optional<T>.TryParse(input, provider, out var resultWithProvider1));
            Assert.Equal(expected, resultWithProvider1, comparer);

            if (parseSpan)
            {
                Assert.True(Optional<T>.TryParse(input.AsSpan(), provider, out var resultWithProvider2));
                Assert.Equal(expected, resultWithProvider2, comparer);
            }
        }
    }

    private static bool ShouldParseSpan<T>()
    {
#if NETCOREAPP2_1_OR_GREATER
        return true;
#else
        return false;
#endif
    }

    private sealed class CustomComparer<T> : IEqualityComparer<Optional<T>>
    {
        private static readonly IEqualityComparer<T> _valueComparer =
            typeof(T) == typeof(ReadOnlyMemory<char>)
                ? (IEqualityComparer<T>)(object)new ReadOnlyMemoryComparer() :
            typeof(T) == typeof(Memory<char>)
                ? (IEqualityComparer<T>)(object)new MemoryComparer() :
            EqualityComparer<T>.Default;

        public bool Equals(Optional<T> x, Optional<T> y) =>
            Optional.Equals(x, y, _valueComparer);

        public int GetHashCode([DisallowNull] Optional<T> obj) =>
            obj.GetHashCode(_valueComparer);

        private sealed class ReadOnlyMemoryComparer : IEqualityComparer<ReadOnlyMemory<char>>
        {
            public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y) =>
                x.Span.Equals(y.Span, StringComparison.Ordinal);

            public int GetHashCode([DisallowNull] ReadOnlyMemory<char> obj)
            {
#pragma warning disable CA1307 // Specify StringComparison for clarity
                return obj.Span.ToString().GetHashCode();
#pragma warning restore CA1307 // Specify StringComparison for clarity
            }
        }

        private sealed class MemoryComparer : IEqualityComparer<Memory<char>>
        {
            public bool Equals(Memory<char> x, Memory<char> y) =>
                ((ReadOnlySpan<char>)x.Span).Equals((ReadOnlySpan<char>)y.Span, StringComparison.Ordinal);

            public int GetHashCode([DisallowNull] Memory<char> obj)
            {
#pragma warning disable CA1307 // Specify StringComparison for clarity
                return obj.Span.ToString().GetHashCode();
#pragma warning restore CA1307 // Specify StringComparison for clarity
            }
        }
    }
}
