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
        AssertParses<CustomParsable.Default>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);

        AssertParses<CustomParsable.Int>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.IntFoo>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.Int2>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.int2>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.Integer>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.NaN1>(new(new("foo", NumberStyles.Float | NumberStyles.AllowThousands)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.NaN2>(new(new("foo", NumberStyles.Float | NumberStyles.AllowThousands)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.NaNWrong1>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.NaNWrong2>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.NaNWrong3>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.NaNWrong4>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.NaNWrong5>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.NaNWrong6>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true);

#if NET7_0_OR_GREATER
        AssertParses<CustomParsable.ImplementingIBinaryInteger>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null, parseFromSpan: true);
        AssertParses<CustomParsable.ImplementingIFloatingPointIeee754>(new(new("foo", NumberStyles.Float | NumberStyles.AllowThousands)), "Some { foo }", null, parseFromSpan: true);
#endif
    }

    [Fact]
    public void Parse_DefaultedNumberStyles()
    {
        AssertParses<DefaultedCustomParsable.IntNumber>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, parseFromSpan: true, tryParse: false);
        AssertParses<DefaultedCustomParsable.Integer>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null, parseFromSpan: true, tryParse: false);
        AssertParses<DefaultedCustomParsable.Float>(new(new("foo", NumberStyles.Float)), "Some { foo }", null, parseFromSpan: true, tryParse: false);
    }

    private static class CustomParsable
    {
        public abstract record Base<TSelf>(string? Input, NumberStyles Style)
            where TSelf : Base<TSelf>, new()
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

        public sealed record Default(string? Input, NumberStyles Style) : Base<Default>(Input, Style)
        {
            public Default() : this(default, default)
            {
            }
        }

        public sealed record Int(string? Input, NumberStyles Style) : Base<Int>(Input, Style)
        {
            public Int() : this(default, default)
            {
            }
        }

        public sealed record IntFoo(string? Input, NumberStyles Style) : Base<IntFoo>(Input, Style)
        {
            public IntFoo() : this(default, default)
            {
            }
        }

        public sealed record Int2(string? Input, NumberStyles Style) : Base<Int2>(Input, Style)
        {
            public Int2() : this(default, default)
            {
            }
        }

        public sealed record int2(string? Input, NumberStyles Style) : Base<int2>(Input, Style)
        {
            public int2() : this(default, default)
            {
            }
        }

        public sealed record Integer(string? Input, NumberStyles Style) : Base<Integer>(Input, Style)
        {
            public Integer() : this(default, default)
            {
            }
        }

        public sealed record NaN1(string? Input, NumberStyles Style) : Base<NaN1>(Input, Style)
        {
            public NaN1() : this(default, default)
            {
            }

            public static readonly NaN1 NaN = default!;
        }

        public sealed record NaN2(string? Input, NumberStyles Style) : Base<NaN2>(Input, Style)
        {
            public NaN2() : this(default, default)
            {
            }

            public static NaN2 NaN => default!;
        }

        public sealed record NaNWrong1(string? Input, NumberStyles Style) : Base<NaNWrong1>(Input, Style)
        {
            public NaNWrong1() : this(default, default)
            {
            }

            public static int NaN => default;
        }

        public sealed record NaNWrong2(string? Input, NumberStyles Style) : Base<NaNWrong2>(Input, Style)
        {
            public NaNWrong2() : this(default, default)
            {
            }

            public static NaNWrong2 NaN { get; set; } = default!;
        }

        public sealed record NaNWrong3(string? Input, NumberStyles Style) : Base<NaNWrong3>(Input, Style)
        {
            public NaNWrong3() : this(default, default)
            {
            }

            public static NaNWrong3 NaN = default!;
        }

        public sealed record NaNWrong4(string? Input, NumberStyles Style) : Base<NaNWrong4>(Input, Style)
        {
            public NaNWrong4() : this(default, default)
            {
            }

#pragma warning disable CA1822 // Mark members as static
            public NaNWrong4 NaN => default!;
#pragma warning restore CA1822 // Mark members as static
        }

        public sealed record NaNWrong5(string? Input, NumberStyles Style) : Base<NaNWrong5>(Input, Style)
        {
            public NaNWrong5() : this(default, default)
            {
            }

            internal static NaNWrong5 NaN => default!;
        }

        public sealed record NaNWrong6(string? Input, NumberStyles Style) : Base<NaNWrong6>(Input, Style)
        {
            public NaNWrong6() : this(default, default)
            {
            }

            public static NaNWrong6 NaN() => default!;
        }

#if NET7_0_OR_GREATER
        public sealed record ImplementingIBinaryInteger(string? Input, NumberStyles Style) : Base<ImplementingIBinaryInteger>(Input, Style), IBinaryInteger<ImplementingIBinaryInteger>
        {
            public ImplementingIBinaryInteger() : this(default, default)
            {
            }

            static ImplementingIBinaryInteger INumberBase<ImplementingIBinaryInteger>.One => throw new NotImplementedException();

            static int INumberBase<ImplementingIBinaryInteger>.Radix => throw new NotImplementedException();

            static ImplementingIBinaryInteger INumberBase<ImplementingIBinaryInteger>.Zero => throw new NotImplementedException();

            static ImplementingIBinaryInteger IAdditiveIdentity<ImplementingIBinaryInteger, ImplementingIBinaryInteger>.AdditiveIdentity => throw new NotImplementedException();

            static ImplementingIBinaryInteger IMultiplicativeIdentity<ImplementingIBinaryInteger, ImplementingIBinaryInteger>.MultiplicativeIdentity => throw new NotImplementedException();

            static ImplementingIBinaryInteger INumberBase<ImplementingIBinaryInteger>.Abs(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsCanonical(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsComplexNumber(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsEvenInteger(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsFinite(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsImaginaryNumber(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsInfinity(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsInteger(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsNaN(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsNegative(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsNegativeInfinity(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsNormal(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsOddInteger(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsPositive(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsPositiveInfinity(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool IBinaryNumber<ImplementingIBinaryInteger>.IsPow2(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsRealNumber(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsSubnormal(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.IsZero(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IBinaryNumber<ImplementingIBinaryInteger>.Log2(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static ImplementingIBinaryInteger INumberBase<ImplementingIBinaryInteger>.MaxMagnitude(ImplementingIBinaryInteger x, ImplementingIBinaryInteger y) => throw new NotImplementedException();

            static ImplementingIBinaryInteger INumberBase<ImplementingIBinaryInteger>.MaxMagnitudeNumber(ImplementingIBinaryInteger x, ImplementingIBinaryInteger y) => throw new NotImplementedException();

            static ImplementingIBinaryInteger INumberBase<ImplementingIBinaryInteger>.MinMagnitude(ImplementingIBinaryInteger x, ImplementingIBinaryInteger y) => throw new NotImplementedException();

            static ImplementingIBinaryInteger INumberBase<ImplementingIBinaryInteger>.MinMagnitudeNumber(ImplementingIBinaryInteger x, ImplementingIBinaryInteger y) => throw new NotImplementedException();

            static ImplementingIBinaryInteger INumberBase<ImplementingIBinaryInteger>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

            static ImplementingIBinaryInteger INumberBase<ImplementingIBinaryInteger>.Parse(string s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

            static ImplementingIBinaryInteger ISpanParsable<ImplementingIBinaryInteger>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IParsable<ImplementingIBinaryInteger>.Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IBinaryInteger<ImplementingIBinaryInteger>.PopCount(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IBinaryInteger<ImplementingIBinaryInteger>.TrailingZeroCount(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.TryConvertFromChecked<TOther>(TOther value, out ImplementingIBinaryInteger result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.TryConvertFromSaturating<TOther>(TOther value, out ImplementingIBinaryInteger result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.TryConvertFromTruncating<TOther>(TOther value, out ImplementingIBinaryInteger result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.TryConvertToChecked<TOther>(ImplementingIBinaryInteger value, out TOther result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.TryConvertToSaturating<TOther>(ImplementingIBinaryInteger value, out TOther result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.TryConvertToTruncating<TOther>(ImplementingIBinaryInteger value, out TOther result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out ImplementingIBinaryInteger result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIBinaryInteger>.TryParse(string? s, NumberStyles style, IFormatProvider? provider, out ImplementingIBinaryInteger result) => throw new NotImplementedException();

            static bool ISpanParsable<ImplementingIBinaryInteger>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ImplementingIBinaryInteger result) => throw new NotImplementedException();

            static bool IParsable<ImplementingIBinaryInteger>.TryParse(string? s, IFormatProvider? provider, out ImplementingIBinaryInteger result) => throw new NotImplementedException();

            static bool IBinaryInteger<ImplementingIBinaryInteger>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static bool IBinaryInteger<ImplementingIBinaryInteger>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out ImplementingIBinaryInteger value) => throw new NotImplementedException();

            int IComparable.CompareTo(object? obj) => throw new NotImplementedException();

            int IComparable<ImplementingIBinaryInteger>.CompareTo(ImplementingIBinaryInteger? other) => throw new NotImplementedException();

            int IBinaryInteger<ImplementingIBinaryInteger>.GetByteCount() => throw new NotImplementedException();

            int IBinaryInteger<ImplementingIBinaryInteger>.GetShortestBitLength() => throw new NotImplementedException();

            string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
                ToString();

            bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotImplementedException();

            bool IBinaryInteger<ImplementingIBinaryInteger>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

            bool IBinaryInteger<ImplementingIBinaryInteger>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IUnaryPlusOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger>.operator +(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IAdditionOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, ImplementingIBinaryInteger>.operator +(ImplementingIBinaryInteger left, ImplementingIBinaryInteger right) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IUnaryNegationOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger>.operator -(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static ImplementingIBinaryInteger ISubtractionOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, ImplementingIBinaryInteger>.operator -(ImplementingIBinaryInteger left, ImplementingIBinaryInteger right) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IBitwiseOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, ImplementingIBinaryInteger>.operator ~(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IIncrementOperators<ImplementingIBinaryInteger>.operator ++(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IDecrementOperators<ImplementingIBinaryInteger>.operator --(ImplementingIBinaryInteger value) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IMultiplyOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, ImplementingIBinaryInteger>.operator *(ImplementingIBinaryInteger left, ImplementingIBinaryInteger right) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IDivisionOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, ImplementingIBinaryInteger>.operator /(ImplementingIBinaryInteger left, ImplementingIBinaryInteger right) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IModulusOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, ImplementingIBinaryInteger>.operator %(ImplementingIBinaryInteger left, ImplementingIBinaryInteger right) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IBitwiseOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, ImplementingIBinaryInteger>.operator &(ImplementingIBinaryInteger left, ImplementingIBinaryInteger right) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IBitwiseOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, ImplementingIBinaryInteger>.operator |(ImplementingIBinaryInteger left, ImplementingIBinaryInteger right) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IBitwiseOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, ImplementingIBinaryInteger>.operator ^(ImplementingIBinaryInteger left, ImplementingIBinaryInteger right) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IShiftOperators<ImplementingIBinaryInteger, int, ImplementingIBinaryInteger>.operator <<(ImplementingIBinaryInteger value, int shiftAmount) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IShiftOperators<ImplementingIBinaryInteger, int, ImplementingIBinaryInteger>.operator >>(ImplementingIBinaryInteger value, int shiftAmount) => throw new NotImplementedException();

            static bool IEqualityOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, bool>.operator ==(ImplementingIBinaryInteger? left, ImplementingIBinaryInteger? right) => throw new NotImplementedException();

            static bool IEqualityOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, bool>.operator !=(ImplementingIBinaryInteger? left, ImplementingIBinaryInteger? right) => throw new NotImplementedException();

            static bool IComparisonOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, bool>.operator <(ImplementingIBinaryInteger left, ImplementingIBinaryInteger right) => throw new NotImplementedException();

            static bool IComparisonOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, bool>.operator >(ImplementingIBinaryInteger left, ImplementingIBinaryInteger right) => throw new NotImplementedException();

            static bool IComparisonOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, bool>.operator <=(ImplementingIBinaryInteger left, ImplementingIBinaryInteger right) => throw new NotImplementedException();

            static bool IComparisonOperators<ImplementingIBinaryInteger, ImplementingIBinaryInteger, bool>.operator >=(ImplementingIBinaryInteger left, ImplementingIBinaryInteger right) => throw new NotImplementedException();

            static ImplementingIBinaryInteger IShiftOperators<ImplementingIBinaryInteger, int, ImplementingIBinaryInteger>.operator >>>(ImplementingIBinaryInteger value, int shiftAmount) => throw new NotImplementedException();
        }

        public sealed record ImplementingIFloatingPointIeee754(string? Input, NumberStyles Style) : Base<ImplementingIFloatingPointIeee754>(Input, Style), IFloatingPointIeee754<ImplementingIFloatingPointIeee754>
        {
            public ImplementingIFloatingPointIeee754() : this(default, default)
            {
            }

            static ImplementingIFloatingPointIeee754 IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.Epsilon => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.NaN => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.NegativeInfinity => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.NegativeZero => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.PositiveInfinity => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ISignedNumber<ImplementingIFloatingPointIeee754>.NegativeOne => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointConstants<ImplementingIFloatingPointIeee754>.E => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointConstants<ImplementingIFloatingPointIeee754>.Pi => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointConstants<ImplementingIFloatingPointIeee754>.Tau => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 INumberBase<ImplementingIFloatingPointIeee754>.One => throw new NotImplementedException();

            static int INumberBase<ImplementingIFloatingPointIeee754>.Radix => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 INumberBase<ImplementingIFloatingPointIeee754>.Zero => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IAdditiveIdentity<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754>.AdditiveIdentity => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IMultiplicativeIdentity<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754>.MultiplicativeIdentity => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 INumberBase<ImplementingIFloatingPointIeee754>.Abs(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.Acos(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IHyperbolicFunctions<ImplementingIFloatingPointIeee754>.Acosh(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.AcosPi(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.Asin(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IHyperbolicFunctions<ImplementingIFloatingPointIeee754>.Asinh(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.AsinPi(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.Atan(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.Atan2(ImplementingIFloatingPointIeee754 y, ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.Atan2Pi(ImplementingIFloatingPointIeee754 y, ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IHyperbolicFunctions<ImplementingIFloatingPointIeee754>.Atanh(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.AtanPi(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.BitDecrement(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.BitIncrement(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IRootFunctions<ImplementingIFloatingPointIeee754>.Cbrt(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.Cos(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IHyperbolicFunctions<ImplementingIFloatingPointIeee754>.Cosh(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.CosPi(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IExponentialFunctions<ImplementingIFloatingPointIeee754>.Exp(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IExponentialFunctions<ImplementingIFloatingPointIeee754>.Exp10(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IExponentialFunctions<ImplementingIFloatingPointIeee754>.Exp2(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.FusedMultiplyAdd(ImplementingIFloatingPointIeee754 left, ImplementingIFloatingPointIeee754 right, ImplementingIFloatingPointIeee754 addend) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IRootFunctions<ImplementingIFloatingPointIeee754>.Hypot(ImplementingIFloatingPointIeee754 x, ImplementingIFloatingPointIeee754 y) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.Ieee754Remainder(ImplementingIFloatingPointIeee754 left, ImplementingIFloatingPointIeee754 right) => throw new NotImplementedException();

            static int IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.ILogB(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsCanonical(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsComplexNumber(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsEvenInteger(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsFinite(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsImaginaryNumber(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsInfinity(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsInteger(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsNaN(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsNegative(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsNegativeInfinity(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsNormal(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsOddInteger(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsPositive(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsPositiveInfinity(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsRealNumber(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsSubnormal(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.IsZero(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ILogarithmicFunctions<ImplementingIFloatingPointIeee754>.Log(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ILogarithmicFunctions<ImplementingIFloatingPointIeee754>.Log(ImplementingIFloatingPointIeee754 x, ImplementingIFloatingPointIeee754 newBase) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ILogarithmicFunctions<ImplementingIFloatingPointIeee754>.Log10(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ILogarithmicFunctions<ImplementingIFloatingPointIeee754>.Log2(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 INumberBase<ImplementingIFloatingPointIeee754>.MaxMagnitude(ImplementingIFloatingPointIeee754 x, ImplementingIFloatingPointIeee754 y) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 INumberBase<ImplementingIFloatingPointIeee754>.MaxMagnitudeNumber(ImplementingIFloatingPointIeee754 x, ImplementingIFloatingPointIeee754 y) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 INumberBase<ImplementingIFloatingPointIeee754>.MinMagnitude(ImplementingIFloatingPointIeee754 x, ImplementingIFloatingPointIeee754 y) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 INumberBase<ImplementingIFloatingPointIeee754>.MinMagnitudeNumber(ImplementingIFloatingPointIeee754 x, ImplementingIFloatingPointIeee754 y) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 INumberBase<ImplementingIFloatingPointIeee754>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 INumberBase<ImplementingIFloatingPointIeee754>.Parse(string s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ISpanParsable<ImplementingIFloatingPointIeee754>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IParsable<ImplementingIFloatingPointIeee754>.Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IPowerFunctions<ImplementingIFloatingPointIeee754>.Pow(ImplementingIFloatingPointIeee754 x, ImplementingIFloatingPointIeee754 y) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IRootFunctions<ImplementingIFloatingPointIeee754>.RootN(ImplementingIFloatingPointIeee754 x, int n) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPoint<ImplementingIFloatingPointIeee754>.Round(ImplementingIFloatingPointIeee754 x, int digits, MidpointRounding mode) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IFloatingPointIeee754<ImplementingIFloatingPointIeee754>.ScaleB(ImplementingIFloatingPointIeee754 x, int n) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.Sin(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static (ImplementingIFloatingPointIeee754 Sin, ImplementingIFloatingPointIeee754 Cos) ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.SinCos(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static (ImplementingIFloatingPointIeee754 SinPi, ImplementingIFloatingPointIeee754 CosPi) ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.SinCosPi(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IHyperbolicFunctions<ImplementingIFloatingPointIeee754>.Sinh(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.SinPi(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IRootFunctions<ImplementingIFloatingPointIeee754>.Sqrt(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.Tan(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IHyperbolicFunctions<ImplementingIFloatingPointIeee754>.Tanh(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ITrigonometricFunctions<ImplementingIFloatingPointIeee754>.TanPi(ImplementingIFloatingPointIeee754 x) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.TryConvertFromChecked<TOther>(TOther value, out ImplementingIFloatingPointIeee754 result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.TryConvertFromSaturating<TOther>(TOther value, out ImplementingIFloatingPointIeee754 result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.TryConvertFromTruncating<TOther>(TOther value, out ImplementingIFloatingPointIeee754 result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.TryConvertToChecked<TOther>(ImplementingIFloatingPointIeee754 value, out TOther result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.TryConvertToSaturating<TOther>(ImplementingIFloatingPointIeee754 value, out TOther result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.TryConvertToTruncating<TOther>(ImplementingIFloatingPointIeee754 value, out TOther result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out ImplementingIFloatingPointIeee754 result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPointIeee754>.TryParse(string? s, NumberStyles style, IFormatProvider? provider, out ImplementingIFloatingPointIeee754 result) => throw new NotImplementedException();

            static bool ISpanParsable<ImplementingIFloatingPointIeee754>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ImplementingIFloatingPointIeee754 result) => throw new NotImplementedException();

            static bool IParsable<ImplementingIFloatingPointIeee754>.TryParse(string? s, IFormatProvider? provider, out ImplementingIFloatingPointIeee754 result) => throw new NotImplementedException();

            int IComparable.CompareTo(object? obj) => throw new NotImplementedException();

            int IComparable<ImplementingIFloatingPointIeee754>.CompareTo(ImplementingIFloatingPointIeee754? other) => throw new NotImplementedException();

            int IFloatingPoint<ImplementingIFloatingPointIeee754>.GetExponentByteCount() => throw new NotImplementedException();

            int IFloatingPoint<ImplementingIFloatingPointIeee754>.GetExponentShortestBitLength() => throw new NotImplementedException();

            int IFloatingPoint<ImplementingIFloatingPointIeee754>.GetSignificandBitLength() => throw new NotImplementedException();

            int IFloatingPoint<ImplementingIFloatingPointIeee754>.GetSignificandByteCount() => throw new NotImplementedException();

            string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
                ToString();

            bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotImplementedException();

            bool IFloatingPoint<ImplementingIFloatingPointIeee754>.TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

            bool IFloatingPoint<ImplementingIFloatingPointIeee754>.TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

            bool IFloatingPoint<ImplementingIFloatingPointIeee754>.TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

            bool IFloatingPoint<ImplementingIFloatingPointIeee754>.TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IUnaryPlusOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754>.operator +(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IAdditionOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754>.operator +(ImplementingIFloatingPointIeee754 left, ImplementingIFloatingPointIeee754 right) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IUnaryNegationOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754>.operator -(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 ISubtractionOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754>.operator -(ImplementingIFloatingPointIeee754 left, ImplementingIFloatingPointIeee754 right) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IIncrementOperators<ImplementingIFloatingPointIeee754>.operator ++(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IDecrementOperators<ImplementingIFloatingPointIeee754>.operator --(ImplementingIFloatingPointIeee754 value) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IMultiplyOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754>.operator *(ImplementingIFloatingPointIeee754 left, ImplementingIFloatingPointIeee754 right) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IDivisionOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754>.operator /(ImplementingIFloatingPointIeee754 left, ImplementingIFloatingPointIeee754 right) => throw new NotImplementedException();

            static ImplementingIFloatingPointIeee754 IModulusOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754>.operator %(ImplementingIFloatingPointIeee754 left, ImplementingIFloatingPointIeee754 right) => throw new NotImplementedException();

            static bool IEqualityOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754, bool>.operator ==(ImplementingIFloatingPointIeee754? left, ImplementingIFloatingPointIeee754? right) => throw new NotImplementedException();

            static bool IEqualityOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754, bool>.operator !=(ImplementingIFloatingPointIeee754? left, ImplementingIFloatingPointIeee754? right) => throw new NotImplementedException();

            static bool IComparisonOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754, bool>.operator <(ImplementingIFloatingPointIeee754 left, ImplementingIFloatingPointIeee754 right) => throw new NotImplementedException();

            static bool IComparisonOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754, bool>.operator >(ImplementingIFloatingPointIeee754 left, ImplementingIFloatingPointIeee754 right) => throw new NotImplementedException();

            static bool IComparisonOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754, bool>.operator <=(ImplementingIFloatingPointIeee754 left, ImplementingIFloatingPointIeee754 right) => throw new NotImplementedException();

            static bool IComparisonOperators<ImplementingIFloatingPointIeee754, ImplementingIFloatingPointIeee754, bool>.operator >=(ImplementingIFloatingPointIeee754 left, ImplementingIFloatingPointIeee754 right) => throw new NotImplementedException();
        }
#endif
    }

    private static class DefaultedCustomParsable
    {
        public sealed record IntNumber(string? Input, NumberStyles Style)
        {
            public static IntNumber NaN => default!;

            public static IntNumber Parse(string s, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null) =>
                new(s, style);

            public static IntNumber Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null) =>
                new(s.ToString(), style);
        }

        public sealed record Integer(string? Input, NumberStyles Style)
        {
            public static Integer NaN => default!;

            public static Integer Parse(string s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) =>
                new(s, style);

            public static Integer Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) =>
                new(s.ToString(), style);
        }

        public sealed record Float(string? Input, NumberStyles Style)
#if NET7_0_OR_GREATER
        : IBinaryInteger<Float>
#endif
        {
            public static Float Parse(string s, NumberStyles style = NumberStyles.Float, IFormatProvider? provider = null) =>
                new(s, style);

            public static Float Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Float, IFormatProvider? provider = null) =>
                new(s.ToString(), style);

#if NET7_0_OR_GREATER
        static Float INumberBase<Float>.One => throw new NotImplementedException();

        static int INumberBase<Float>.Radix => throw new NotImplementedException();

        static Float INumberBase<Float>.Zero => throw new NotImplementedException();

        static Float IAdditiveIdentity<Float, Float>.AdditiveIdentity => throw new NotImplementedException();

        static Float IMultiplicativeIdentity<Float, Float>.MultiplicativeIdentity => throw new NotImplementedException();

        static Float INumberBase<Float>.Abs(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsCanonical(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsComplexNumber(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsEvenInteger(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsFinite(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsImaginaryNumber(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsInfinity(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsInteger(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsNaN(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsNegative(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsNegativeInfinity(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsNormal(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsOddInteger(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsPositive(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsPositiveInfinity(Float value) => throw new NotImplementedException();

        static bool IBinaryNumber<Float>.IsPow2(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsRealNumber(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsSubnormal(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.IsZero(Float value) => throw new NotImplementedException();

        static Float IBinaryNumber<Float>.Log2(Float value) => throw new NotImplementedException();

        static Float INumberBase<Float>.MaxMagnitude(Float x, Float y) => throw new NotImplementedException();

        static Float INumberBase<Float>.MaxMagnitudeNumber(Float x, Float y) => throw new NotImplementedException();

        static Float INumberBase<Float>.MinMagnitude(Float x, Float y) => throw new NotImplementedException();

        static Float INumberBase<Float>.MinMagnitudeNumber(Float x, Float y) => throw new NotImplementedException();

        static Float INumberBase<Float>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

        static Float INumberBase<Float>.Parse(string s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

        static Float ISpanParsable<Float>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();

        static Float IParsable<Float>.Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();

        static Float IBinaryInteger<Float>.PopCount(Float value) => throw new NotImplementedException();

        static Float IBinaryInteger<Float>.TrailingZeroCount(Float value) => throw new NotImplementedException();

        static bool INumberBase<Float>.TryConvertFromChecked<TOther>(TOther value, out Float result) => throw new NotImplementedException();

        static bool INumberBase<Float>.TryConvertFromSaturating<TOther>(TOther value, out Float result) => throw new NotImplementedException();

        static bool INumberBase<Float>.TryConvertFromTruncating<TOther>(TOther value, out Float result) => throw new NotImplementedException();

        static bool INumberBase<Float>.TryConvertToChecked<TOther>(Float value, out TOther result) => throw new NotImplementedException();

        static bool INumberBase<Float>.TryConvertToSaturating<TOther>(Float value, out TOther result) => throw new NotImplementedException();

        static bool INumberBase<Float>.TryConvertToTruncating<TOther>(Float value, out TOther result) => throw new NotImplementedException();

        static bool INumberBase<Float>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Float result) => throw new NotImplementedException();

        static bool INumberBase<Float>.TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Float result) => throw new NotImplementedException();

        static bool ISpanParsable<Float>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Float result) => throw new NotImplementedException();

        static bool IParsable<Float>.TryParse(string? s, IFormatProvider? provider, out Float result) => throw new NotImplementedException();

        static bool IBinaryInteger<Float>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Float value) => throw new NotImplementedException();

        static bool IBinaryInteger<Float>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Float value) => throw new NotImplementedException();

        int IComparable.CompareTo(object? obj) => throw new NotImplementedException();

        int IComparable<Float>.CompareTo(Float? other) => throw new NotImplementedException();

        int IBinaryInteger<Float>.GetByteCount() => throw new NotImplementedException();

        int IBinaryInteger<Float>.GetShortestBitLength() => throw new NotImplementedException();

        string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
            ToString();

        bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotImplementedException();

        bool IBinaryInteger<Float>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        bool IBinaryInteger<Float>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

        static Float IUnaryPlusOperators<Float, Float>.operator +(Float value) => throw new NotImplementedException();

        static Float IAdditionOperators<Float, Float, Float>.operator +(Float left, Float right) => throw new NotImplementedException();

        static Float IUnaryNegationOperators<Float, Float>.operator -(Float value) => throw new NotImplementedException();

        static Float ISubtractionOperators<Float, Float, Float>.operator -(Float left, Float right) => throw new NotImplementedException();

        static Float IBitwiseOperators<Float, Float, Float>.operator ~(Float value) => throw new NotImplementedException();

        static Float IIncrementOperators<Float>.operator ++(Float value) => throw new NotImplementedException();

        static Float IDecrementOperators<Float>.operator --(Float value) => throw new NotImplementedException();

        static Float IMultiplyOperators<Float, Float, Float>.operator *(Float left, Float right) => throw new NotImplementedException();

        static Float IDivisionOperators<Float, Float, Float>.operator /(Float left, Float right) => throw new NotImplementedException();

        static Float IModulusOperators<Float, Float, Float>.operator %(Float left, Float right) => throw new NotImplementedException();

        static Float IBitwiseOperators<Float, Float, Float>.operator &(Float left, Float right) => throw new NotImplementedException();

        static Float IBitwiseOperators<Float, Float, Float>.operator |(Float left, Float right) => throw new NotImplementedException();

        static Float IBitwiseOperators<Float, Float, Float>.operator ^(Float left, Float right) => throw new NotImplementedException();

        static Float IShiftOperators<Float, int, Float>.operator <<(Float value, int shiftAmount) => throw new NotImplementedException();

        static Float IShiftOperators<Float, int, Float>.operator >>(Float value, int shiftAmount) => throw new NotImplementedException();

        static bool IEqualityOperators<Float, Float, bool>.operator ==(Float? left, Float? right) => throw new NotImplementedException();

        static bool IEqualityOperators<Float, Float, bool>.operator !=(Float? left, Float? right) => throw new NotImplementedException();

        static bool IComparisonOperators<Float, Float, bool>.operator <(Float left, Float right) => throw new NotImplementedException();

        static bool IComparisonOperators<Float, Float, bool>.operator >(Float left, Float right) => throw new NotImplementedException();

        static bool IComparisonOperators<Float, Float, bool>.operator <=(Float left, Float right) => throw new NotImplementedException();

        static bool IComparisonOperators<Float, Float, bool>.operator >=(Float left, Float right) => throw new NotImplementedException();

        static Float IShiftOperators<Float, int, Float>.operator >>>(Float value, int shiftAmount) => throw new NotImplementedException();
#endif
        }
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
