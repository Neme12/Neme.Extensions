using Neme.Extensions.Tests.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
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
        AssertDoesNotParse<int>(null, null, null, ParseMethods.FromString);
        AssertDoesNotParse<int>(null, null, CultureInfo.InvariantCulture, ParseMethods.FromString);
        AssertDoesNotParse<int>(null, null, CultureInfo.GetCultureInfo("de"), ParseMethods.FromString);
    }

    [Fact]
    public void Parse_None()
    {
        AssertParses<int>(default, "None", null);
        AssertParses<int>(default, "None", CultureInfo.InvariantCulture);
        AssertParses<int>(default, "None", CultureInfo.GetCultureInfo("de"));
    }

    [Fact]
    public void Parse_Some()
    {
        AssertParses<double>(new(42.2), $"Some {{ {42.2} }}", null, ParseMethodsForBuiltInTypes);
        AssertParses<double>(new(42.2), "Some { 42.2 }", CultureInfo.InvariantCulture, ParseMethodsForBuiltInTypes);
        AssertParses<double>(new(42.2), "Some { 42,2 }", CultureInfo.GetCultureInfo("de"), ParseMethodsForBuiltInTypes);
    }

    [Fact]
    public void Parse_Version()
    {
        AssertParses<Version>(new(new(1, 2, 3, 4)), "Some { 1.2.3.4 }", null, ParseMethodsForBuiltInTypes);
        AssertParses<Version>(new(new(1, 2, 3, 4)), "Some { 1.2.3.4 }", CultureInfo.InvariantCulture, ParseMethodsForBuiltInTypes);
        AssertParses<Version>(new(new(1, 2, 3, 4)), "Some { 1.2.3.4 }", CultureInfo.GetCultureInfo("de"), ParseMethodsForBuiltInTypes);
    }

    [Fact]
    public void Parse_Some_EmptyString()
    {
        AssertParses<string?>(new(""), "Some {  }", null);
        AssertParses<string?>(new(""), "Some {  }", CultureInfo.InvariantCulture);
        AssertParses<string?>(new(""), "Some {  }", CultureInfo.GetCultureInfo("de"));
    }

    [Fact]
    public void Parse_Some_String()
    {
        AssertParses<string>(new("hello"), "Some { hello }", null);
        AssertParses<string>(new("hello"), "Some { hello }", CultureInfo.InvariantCulture);
        AssertParses<string>(new("hello"), "Some { hello }", CultureInfo.GetCultureInfo("de"));
    }

    [Fact]
    public void Parse_Some_ReadOnlyMemory()
    {
        AssertParses<ReadOnlyMemory<char>>(new("hello".AsMemory()), "Some { hello }", null);
        AssertParses<ReadOnlyMemory<char>>(new("hello".AsMemory()), "Some { hello }", CultureInfo.InvariantCulture);
        AssertParses<ReadOnlyMemory<char>>(new("hello".AsMemory()), "Some { hello }", CultureInfo.GetCultureInfo("de"));
    }

    [Fact]
    public void Parse_Some_Memory()
    {
        AssertParses<Memory<char>>(new("hello".ToArray()), "Some { hello }", null);
        AssertParses<Memory<char>>(new("hello".ToArray()), "Some { hello }", CultureInfo.InvariantCulture);
        AssertParses<Memory<char>>(new("hello".ToArray()), "Some { hello }", CultureInfo.GetCultureInfo("de"));
    }

    [Fact]
    public void Parse_Some_Char()
    {
        AssertParses<char>(new('a'), "Some { a }", null);
        AssertParses<char>(new('a'), "Some { a }", CultureInfo.InvariantCulture);
        AssertParses<char>(new('a'), "Some { a }", CultureInfo.GetCultureInfo("de"));
        AssertDoesNotParse<char>("Some {  }", "", null);
        AssertDoesNotParse<char>("Some { ab }", "ab", null);
    }

#if NETCOREAPP3_0_OR_GREATER
    [Fact]
    public void Parse_Some_Rune()
    {
        AssertParses<Rune>(new(new(0x1f642)), "Some { 🙂 }", null);
        AssertParses<Rune>(new(new(0x1f642)), "Some { 🙂 }", CultureInfo.InvariantCulture);
        AssertParses<Rune>(new(new(0x1f642)), "Some { 🙂 }", CultureInfo.GetCultureInfo("de"));
        AssertDoesNotParse<Rune>("Some {  }", "", null);
        AssertDoesNotParse<Rune>("Some { ab }", "ab", null);
    }
#endif

    [Fact]
    public void Parse_Some_Null()
    {
        AssertParses<string?>(new(null), "Some { }", null);
        AssertParses<string?>(new(null), "Some { }", CultureInfo.InvariantCulture);
        AssertParses<string?>(new(null), "Some { }", CultureInfo.GetCultureInfo("de"));
    }

    [Fact]
    public void Parse_InvalidStrings()
    {
        AssertDoesNotParse<int>("", null, null);
        AssertDoesNotParse<int>("42", null, null);
        AssertDoesNotParse<int>("{ }", null, null);
        AssertDoesNotParse<int>("{ 42 }", null, null);
        AssertDoesNotParse<int>("Some", null, null);
        AssertDoesNotParse<int>("Some {}", null, null);
        AssertDoesNotParse<int>("Some { x }", "x", null, ParseMethodsForBuiltInTypes);
        AssertDoesNotParse<int>("SOME { 42 }", null, null);
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

        // Not .NET 7 because the implementation is broken in .NET 7.
#if NET8_0_OR_GREATER
        TestIsFloatAndAllowThousandsComplex();
#endif

        static void TestIsInteger<T>(T? negativeOne)
            where T : struct
        {
            // NumberStyles.AllowLeadingWhite and NumberStyles.AllowTrailingWhite are included.
            AssertParses<T>(new(default), "Some {  0  }", null, ParseMethodsForBuiltInTypes);

            if (negativeOne is not null)
            {
                // NumberStyles.AllowLeadingSign is included
                AssertParses<T>(new(negativeOne.Value), "Some { -1 }", null, ParseMethodsForBuiltInTypes);
            }

            // NumberStyles.AllowDecimalPoint is *not* included.
            AssertDoesNotParse<T>("Some { 42.2 }", "42.2", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowThousands is *not* included.
            AssertDoesNotParse<T>("Some { 10,000 }", "10,000", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowExponent is *not* included.
            AssertDoesNotParse<T>("Some { 4.22e+1 }", "4.22e+1", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowParentheses is *not* included.
            AssertDoesNotParse<T>("Some { (42) }", "(42)", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowHexSpecifier is *not* included.
            AssertDoesNotParse<T>("Some { f }", "f", null, ParseMethodsForBuiltInTypes);
        }

        static void TestIsFloatAndAllowThousands<T>(T negativeOne, T value1, T value2)
            where T : struct
        {
            // NumberStyles.AllowLeadingWhite and NumberStyles.AllowTrailingWhite are included.
            AssertParses<T>(new(default), "Some {  0  }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowLeadingSign is included
            AssertParses<T>(new(negativeOne), "Some { -1 }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowDecimalPoint is included.
            AssertParses<T>(new(value1), "Some { 42.2 }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowThousands is included.
            AssertParses<T>(new(value2), "Some { 10,000 }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowExponent is included.
            AssertParses<T>(new(value1), "Some { 4.22e+1 }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowParentheses is *not* included.
            AssertDoesNotParse<T>("Some { (42) }", "(42)", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowHexSpecifier is *not* included.
            AssertDoesNotParse<T>("Some { f }", "f", null, ParseMethodsForBuiltInTypes);
        }

#if NET8_0_OR_GREATER
        static void TestIsFloatAndAllowThousandsComplex()
        {
            // NumberStyles.AllowLeadingWhite and NumberStyles.AllowTrailingWhite are included.
            // No trailing space around the whole number as that is broken.
            AssertParses<Complex>(new(default), "Some { <  0 ;  0 > }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowLeadingSign is included
            AssertParses<Complex>(new(new(-1, 0)), "Some { <-1; 0> }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowDecimalPoint is included.
            AssertParses<Complex>(new(new(42.2d, 1)), "Some { <42.2; 1> }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowThousands is included.
            AssertParses<Complex>(new(new(10_000d, 2)), "Some { <10,000; 2> }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowExponent is included.
            AssertParses<Complex>(new(new(42.2d, 1)), "Some { <4.22e+1; 1> }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowParentheses is *not* included.
            AssertDoesNotParse<Complex>("Some { (<4.22e+1; 1>) }", "(<4.22e+1; 1>)", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowHexSpecifier is *not* included.
            AssertDoesNotParse<Complex>("Some { f }", "f", null, ParseMethodsForBuiltInTypes);
        }
#endif

        static void TestIsNumber<T>(T negativeOne, T value1, T value2)
            where T : struct
        {
            // NumberStyles.AllowLeadingWhite and NumberStyles.AllowTrailingWhite are included.
            AssertParses<T>(new(default), "Some {  0  }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowLeadingSign is included
            AssertParses<T>(new(negativeOne), "Some { -1 }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowDecimalPoint is included.
            AssertParses<T>(new(value1), "Some { 42.2 }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowThousands is included.
            AssertParses<T>(new(value2), "Some { 10,000 }", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowExponent is *not* included.
            AssertDoesNotParse<T>("Some { 4.22e+1 }", "4.22e+1", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowParentheses is *not* included.
            AssertDoesNotParse<T>("Some { (42) }", "(42)", null, ParseMethodsForBuiltInTypes);

            // NumberStyles.AllowHexSpecifier is *not* included.
            AssertDoesNotParse<T>("Some { f }", "f", null, ParseMethodsForBuiltInTypes);
        }
    }

    [Fact]
    public void Parse_CustomParsable()
    {
#if NET7_0_OR_GREATER
        AssertParses<CustomParsable.Interface>(new(new(ParseImplementationMethodKind.Interface, "foo")), "Some { foo }", null);
#endif
        AssertParses<CustomParsable.Provider>(new(new(ParseImplementationMethodKind.Provider, "foo")), "Some { foo }", null);
        AssertParses<CustomParsable.NumberStylesProvider>(new(new(ParseImplementationMethodKind.NumberStylesProvider, "foo")), "Some { foo }", null);
        AssertParses<CustomParsable.Plain>(new(new(ParseImplementationMethodKind.Plain, "foo")), "Some { foo }", null);

        AssertNoParseMethod<CustomParsable.None>("Some { foo }", null);
    }

    [Fact]
    public void Parse_CustomParsableNumberStyles()
    {
        AssertParses<CustomParsableNumberStyles.Default>(new(new("foo", NumberStyles.Number)), "Some { foo }", null);

        AssertParses<CustomParsableNumberStyles.Int>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.IntFoo>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.Int2>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.int2>(new(new("foo", NumberStyles.Number)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.Integer>(new(new("foo", NumberStyles.Number)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.NaN1>(new(new("foo", NumberStyles.Float | NumberStyles.AllowThousands)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.NaN2>(new(new("foo", NumberStyles.Float | NumberStyles.AllowThousands)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.NaNWrong1>(new(new("foo", NumberStyles.Number)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.NaNWrong2>(new(new("foo", NumberStyles.Number)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.NaNWrong3>(new(new("foo", NumberStyles.Number)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.NaNWrong4>(new(new("foo", NumberStyles.Number)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.NaNWrong5>(new(new("foo", NumberStyles.Number)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.NaNWrong6>(new(new("foo", NumberStyles.Number)), "Some { foo }", null);

#if NET7_0_OR_GREATER
        AssertParses<CustomParsableNumberStyles.ImplementingIBinaryInteger>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.ImplementingIFloatingPoint>(new(new("foo", NumberStyles.Number)), "Some { foo }", null);
        AssertParses<CustomParsableNumberStyles.ImplementingIFloatingPointIeee754>(new(new("foo", NumberStyles.Float | NumberStyles.AllowThousands)), "Some { foo }", null);
#endif
    }

    [Fact]
    public void Parse_CustomParsableDefaultedNumberStyles()
    {
        // Test Parse and TryParse separately to make sure the default value on TryParse is from the TryParse method by default.
        AssertParses<CustomParsableDefaultedNumberStyles.IntNumberParse>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, ParseMethods.Parse);
        AssertParses<CustomParsableDefaultedNumberStyles.IntNumberTryParse>(new(new("foo", NumberStyles.Number)), "Some { foo }", null, ParseMethods.TryParse);
        AssertParses<CustomParsableDefaultedNumberStyles.IntegerParse>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null, ParseMethods.Parse);
        AssertParses<CustomParsableDefaultedNumberStyles.IntegerTryParse>(new(new("foo", NumberStyles.Integer)), "Some { foo }", null, ParseMethods.TryParse);
        AssertParses<CustomParsableDefaultedNumberStyles.FloatParse>(new(new("foo", NumberStyles.Float)), "Some { foo }", null, ParseMethods.Parse);
        AssertParses<CustomParsableDefaultedNumberStyles.FloatTryParse>(new(new("foo", NumberStyles.Float)), "Some { foo }", null, ParseMethods.TryParse);

        // When there's both Parse and TryParse and the default value is different, it is picked from TryParse when using TryParse.
        // It is always picked from each specific used method, except that in case of using string methods, the equivalent span one
        // is called when present. That's why in cases of string we only test the StringOnly variant.
        AssertParses<CustomParsableDefaultedNumberStyles.DifferentBetweenParseAndTryParseStringOnly>(new(new("foo", NumberStyles.AllowCurrencySymbol)), "Some { foo }", null, ParseMethods.ParseFromString);
        AssertParses<CustomParsableDefaultedNumberStyles.DifferentBetweenParseAndTryParse>(new(new("foo", NumberStyles.AllowParentheses)), "Some { foo }", null, ParseMethods.ParseFromSpan);
        AssertParses<CustomParsableDefaultedNumberStyles.DifferentBetweenParseAndTryParseStringOnly>(new(new("foo", NumberStyles.AllowThousands)), "Some { foo }", null, ParseMethods.TryParseFromString);
        AssertParses<CustomParsableDefaultedNumberStyles.DifferentBetweenParseAndTryParse>(new(new("foo", NumberStyles.AllowDecimalPoint)), "Some { foo }", null, ParseMethods.TryParseFromSpan);

        // When there's no default value on TryParse, it is copied from Parse.
        AssertParses<CustomParsableDefaultedNumberStyles.TryParseInheritedFromParse>(new(new("foo", NumberStyles.AllowLeadingSign)), "Some { foo }", null);
    }

    public enum ParseImplementationMethodKind
    {
        Interface,
        Plain,
        Provider,
        NumberStylesProvider,
    }

    private static class CustomParsable
    {
#if NET7_0_OR_GREATER
        public sealed record Interface(ParseImplementationMethodKind Method, string? Input) : ISpanParsable<Interface>
        {
            static Interface IParsable<Interface>.Parse(string s, IFormatProvider? provider)
            {
                return new(ParseImplementationMethodKind.Interface, s);
            }

            public static Interface Parse(string s)
            {
                return new(ParseImplementationMethodKind.Plain, s);
            }

            public static Interface Parse(string s, IFormatProvider? provider)
            {
                return new(ParseImplementationMethodKind.Provider, s);
            }

            public static Interface Parse(string s, NumberStyles style, IFormatProvider? provider)
            {
                return new(ParseImplementationMethodKind.NumberStylesProvider, s);
            }

            static Interface ISpanParsable<Interface>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
            {
                return new(ParseImplementationMethodKind.Interface, s.ToString());
            }

            public static Interface Parse(ReadOnlySpan<char> s)
            {
                return new(ParseImplementationMethodKind.Plain, s.ToString());
            }

            public static Interface Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
            {
                return new(ParseImplementationMethodKind.Provider, s.ToString());
            }

            public static Interface Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
            {
                return new(ParseImplementationMethodKind.NumberStylesProvider, s.ToString());
            }

            static bool IParsable<Interface>.TryParse(string? s, IFormatProvider? provider, out Interface result)
            {
                result = new(ParseImplementationMethodKind.Interface, s);
                return true;
            }

            public static bool TryParse(string? s, out Interface result)
            {
                result = new(ParseImplementationMethodKind.Plain, s);
                return true;
            }

            public static bool TryParse(string? s, IFormatProvider? provider, out Interface result)
            {
                result = new(ParseImplementationMethodKind.Provider, s);
                return true;
            }

            public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Interface result)
            {
                result = new(ParseImplementationMethodKind.NumberStylesProvider, s);
                return true;
            }

            static bool ISpanParsable<Interface>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Interface result)
            {
                result = new(ParseImplementationMethodKind.Interface, s.ToString());
                return true;
            }

            public static bool TryParse(ReadOnlySpan<char> s, out Interface result)
            {
                result = new(ParseImplementationMethodKind.Plain, s.ToString());
                return true;
            }

            public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Interface result)
            {
                result = new(ParseImplementationMethodKind.Provider, s.ToString());
                return true;
            }

            public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Interface result)
            {
                result = new(ParseImplementationMethodKind.NumberStylesProvider, s.ToString());
                return true;
            }
        }
#endif

        public sealed record Provider(ParseImplementationMethodKind Method, string? Input)
        {
            public static Provider Parse(string s)
            {
                return new(ParseImplementationMethodKind.Plain, s);
            }

            public static Provider Parse(string s, IFormatProvider? provider)
            {
                return new(ParseImplementationMethodKind.Provider, s);
            }

            public static Provider Parse(string s, NumberStyles style, IFormatProvider? provider)
            {
                return new(ParseImplementationMethodKind.NumberStylesProvider, s);
            }

            public static Provider Parse(ReadOnlySpan<char> s)
            {
                return new(ParseImplementationMethodKind.Plain, s.ToString());
            }

            public static Provider Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
            {
                return new(ParseImplementationMethodKind.Provider, s.ToString());
            }

            public static Provider Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
            {
                return new(ParseImplementationMethodKind.NumberStylesProvider, s.ToString());
            }

            public static bool TryParse(string? s, out Provider result)
            {
                result = new(ParseImplementationMethodKind.Plain, s);
                return true;
            }

            public static bool TryParse(string? s, IFormatProvider? provider, out Provider result)
            {
                result = new(ParseImplementationMethodKind.Provider, s);
                return true;
            }

            public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Provider result)
            {
                result = new(ParseImplementationMethodKind.NumberStylesProvider, s);
                return true;
            }

            public static bool TryParse(ReadOnlySpan<char> s, out Provider result)
            {
                result = new(ParseImplementationMethodKind.Plain, s.ToString());
                return true;
            }

            public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Provider result)
            {
                result = new(ParseImplementationMethodKind.Provider, s.ToString());
                return true;
            }

            public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Provider result)
            {
                result = new(ParseImplementationMethodKind.NumberStylesProvider, s.ToString());
                return true;
            }
        }

        public sealed record NumberStylesProvider(ParseImplementationMethodKind Method, string? Input)
        {
            public static NumberStylesProvider Parse(string s)
            {
                return new(ParseImplementationMethodKind.Plain, s);
            }

            public static NumberStylesProvider Parse(string s, NumberStyles style, IFormatProvider? provider)
            {
                return new(ParseImplementationMethodKind.NumberStylesProvider, s);
            }

            public static NumberStylesProvider Parse(ReadOnlySpan<char> s)
            {
                return new(ParseImplementationMethodKind.Plain, s.ToString());
            }

            public static NumberStylesProvider Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
            {
                return new(ParseImplementationMethodKind.NumberStylesProvider, s.ToString());
            }

            public static bool TryParse(string? s, out NumberStylesProvider result)
            {
                result = new(ParseImplementationMethodKind.Plain, s);
                return true;
            }

            public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out NumberStylesProvider result)
            {
                result = new(ParseImplementationMethodKind.NumberStylesProvider, s);
                return true;
            }

            public static bool TryParse(ReadOnlySpan<char> s, out NumberStylesProvider result)
            {
                result = new(ParseImplementationMethodKind.Plain, s.ToString());
                return true;
            }

            public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out NumberStylesProvider result)
            {
                result = new(ParseImplementationMethodKind.NumberStylesProvider, s.ToString());
                return true;
            }
        }

        public sealed record Plain(ParseImplementationMethodKind Method, string? Input)
        {
            public static Plain Parse(string s)
            {
                return new(ParseImplementationMethodKind.Plain, s);
            }

            public static Plain Parse(ReadOnlySpan<char> s)
            {
                return new(ParseImplementationMethodKind.Plain, s.ToString());
            }

            public static bool TryParse(string? s, out Plain result)
            {
                result = new(ParseImplementationMethodKind.Plain, s);
                return true;
            }

            public static bool TryParse(ReadOnlySpan<char> s, out Plain result)
            {
                result = new(ParseImplementationMethodKind.Plain, s.ToString());
                return true;
            }
        }

        public sealed record None
        {
        }
    }

    private static class CustomParsableNumberStyles
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

            static ImplementingIBinaryInteger ISpanParsable<ImplementingIBinaryInteger>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
                Parse(s, NumberStyles.Integer, provider);

            static ImplementingIBinaryInteger IParsable<ImplementingIBinaryInteger>.Parse(string s, IFormatProvider? provider) =>
                Parse(s, NumberStyles.Integer, provider);

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

            static bool ISpanParsable<ImplementingIBinaryInteger>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out ImplementingIBinaryInteger result) =>
                TryParse(s, NumberStyles.Integer, provider, out result);

            static bool IParsable<ImplementingIBinaryInteger>.TryParse(string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ImplementingIBinaryInteger result) =>
                TryParse(s, NumberStyles.Integer, provider, out result);

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

        public sealed record ImplementingIFloatingPoint(string? Input, NumberStyles Style) : Base<ImplementingIFloatingPoint>(Input, Style), IFloatingPoint<ImplementingIFloatingPoint>
        {
            public ImplementingIFloatingPoint() : this(default, default)
            {
            }

            static ImplementingIFloatingPoint IFloatingPointConstants<ImplementingIFloatingPoint>.E => throw new NotImplementedException();

            static ImplementingIFloatingPoint IFloatingPointConstants<ImplementingIFloatingPoint>.Pi => throw new NotImplementedException();

            static ImplementingIFloatingPoint IFloatingPointConstants<ImplementingIFloatingPoint>.Tau => throw new NotImplementedException();

            static ImplementingIFloatingPoint ISignedNumber<ImplementingIFloatingPoint>.NegativeOne => throw new NotImplementedException();

            static ImplementingIFloatingPoint INumberBase<ImplementingIFloatingPoint>.One => throw new NotImplementedException();

            static int INumberBase<ImplementingIFloatingPoint>.Radix => throw new NotImplementedException();

            static ImplementingIFloatingPoint INumberBase<ImplementingIFloatingPoint>.Zero => throw new NotImplementedException();

            static ImplementingIFloatingPoint IAdditiveIdentity<ImplementingIFloatingPoint, ImplementingIFloatingPoint>.AdditiveIdentity => throw new NotImplementedException();

            static ImplementingIFloatingPoint IMultiplicativeIdentity<ImplementingIFloatingPoint, ImplementingIFloatingPoint>.MultiplicativeIdentity => throw new NotImplementedException();

            static ImplementingIFloatingPoint INumberBase<ImplementingIFloatingPoint>.Abs(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsCanonical(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsComplexNumber(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsEvenInteger(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsFinite(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsImaginaryNumber(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsInfinity(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsInteger(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsNaN(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsNegative(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsNegativeInfinity(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsNormal(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsOddInteger(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsPositive(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsPositiveInfinity(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsRealNumber(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsSubnormal(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.IsZero(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static ImplementingIFloatingPoint INumberBase<ImplementingIFloatingPoint>.MaxMagnitude(ImplementingIFloatingPoint x, ImplementingIFloatingPoint y) => throw new NotImplementedException();

            static ImplementingIFloatingPoint INumberBase<ImplementingIFloatingPoint>.MaxMagnitudeNumber(ImplementingIFloatingPoint x, ImplementingIFloatingPoint y) => throw new NotImplementedException();

            static ImplementingIFloatingPoint INumberBase<ImplementingIFloatingPoint>.MinMagnitude(ImplementingIFloatingPoint x, ImplementingIFloatingPoint y) => throw new NotImplementedException();

            static ImplementingIFloatingPoint INumberBase<ImplementingIFloatingPoint>.MinMagnitudeNumber(ImplementingIFloatingPoint x, ImplementingIFloatingPoint y) => throw new NotImplementedException();

            static ImplementingIFloatingPoint INumberBase<ImplementingIFloatingPoint>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

            static ImplementingIFloatingPoint INumberBase<ImplementingIFloatingPoint>.Parse(string s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

            static ImplementingIFloatingPoint ISpanParsable<ImplementingIFloatingPoint>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
                Parse(s, NumberStyles.Number, provider);

            static ImplementingIFloatingPoint IParsable<ImplementingIFloatingPoint>.Parse(string s, IFormatProvider? provider) =>
                Parse(s, NumberStyles.Number, provider);

            static ImplementingIFloatingPoint IFloatingPoint<ImplementingIFloatingPoint>.Round(ImplementingIFloatingPoint x, int digits, MidpointRounding mode) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.TryConvertFromChecked<TOther>(TOther value, out ImplementingIFloatingPoint result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.TryConvertFromSaturating<TOther>(TOther value, out ImplementingIFloatingPoint result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.TryConvertFromTruncating<TOther>(TOther value, out ImplementingIFloatingPoint result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.TryConvertToChecked<TOther>(ImplementingIFloatingPoint value, out TOther result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.TryConvertToSaturating<TOther>(ImplementingIFloatingPoint value, out TOther result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.TryConvertToTruncating<TOther>(ImplementingIFloatingPoint value, out TOther result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out ImplementingIFloatingPoint result) => throw new NotImplementedException();

            static bool INumberBase<ImplementingIFloatingPoint>.TryParse(string? s, NumberStyles style, IFormatProvider? provider, out ImplementingIFloatingPoint result) => throw new NotImplementedException();

            static bool ISpanParsable<ImplementingIFloatingPoint>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out ImplementingIFloatingPoint result) =>
                TryParse(s, NumberStyles.Number, provider, out result);

            static bool IParsable<ImplementingIFloatingPoint>.TryParse(string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ImplementingIFloatingPoint result) =>
                TryParse(s, NumberStyles.Number, provider, out result);

            int IComparable.CompareTo(object? obj) => throw new NotImplementedException();

            int IComparable<ImplementingIFloatingPoint>.CompareTo(ImplementingIFloatingPoint? other) => throw new NotImplementedException();

            int IFloatingPoint<ImplementingIFloatingPoint>.GetExponentByteCount() => throw new NotImplementedException();

            int IFloatingPoint<ImplementingIFloatingPoint>.GetExponentShortestBitLength() => throw new NotImplementedException();

            int IFloatingPoint<ImplementingIFloatingPoint>.GetSignificandBitLength() => throw new NotImplementedException();

            int IFloatingPoint<ImplementingIFloatingPoint>.GetSignificandByteCount() => throw new NotImplementedException();

            string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
                ToString();

            bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotImplementedException();

            bool IFloatingPoint<ImplementingIFloatingPoint>.TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

            bool IFloatingPoint<ImplementingIFloatingPoint>.TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

            bool IFloatingPoint<ImplementingIFloatingPoint>.TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

            bool IFloatingPoint<ImplementingIFloatingPoint>.TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

            static ImplementingIFloatingPoint IUnaryPlusOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint>.operator +(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static ImplementingIFloatingPoint IAdditionOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint, ImplementingIFloatingPoint>.operator +(ImplementingIFloatingPoint left, ImplementingIFloatingPoint right) => throw new NotImplementedException();

            static ImplementingIFloatingPoint IUnaryNegationOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint>.operator -(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static ImplementingIFloatingPoint ISubtractionOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint, ImplementingIFloatingPoint>.operator -(ImplementingIFloatingPoint left, ImplementingIFloatingPoint right) => throw new NotImplementedException();

            static ImplementingIFloatingPoint IIncrementOperators<ImplementingIFloatingPoint>.operator ++(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static ImplementingIFloatingPoint IDecrementOperators<ImplementingIFloatingPoint>.operator --(ImplementingIFloatingPoint value) => throw new NotImplementedException();

            static ImplementingIFloatingPoint IMultiplyOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint, ImplementingIFloatingPoint>.operator *(ImplementingIFloatingPoint left, ImplementingIFloatingPoint right) => throw new NotImplementedException();

            static ImplementingIFloatingPoint IDivisionOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint, ImplementingIFloatingPoint>.operator /(ImplementingIFloatingPoint left, ImplementingIFloatingPoint right) => throw new NotImplementedException();

            static ImplementingIFloatingPoint IModulusOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint, ImplementingIFloatingPoint>.operator %(ImplementingIFloatingPoint left, ImplementingIFloatingPoint right) => throw new NotImplementedException();

            static bool IEqualityOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint, bool>.operator ==(ImplementingIFloatingPoint? left, ImplementingIFloatingPoint? right) => throw new NotImplementedException();

            static bool IEqualityOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint, bool>.operator !=(ImplementingIFloatingPoint? left, ImplementingIFloatingPoint? right) => throw new NotImplementedException();

            static bool IComparisonOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint, bool>.operator <(ImplementingIFloatingPoint left, ImplementingIFloatingPoint right) => throw new NotImplementedException();

            static bool IComparisonOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint, bool>.operator >(ImplementingIFloatingPoint left, ImplementingIFloatingPoint right) => throw new NotImplementedException();

            static bool IComparisonOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint, bool>.operator <=(ImplementingIFloatingPoint left, ImplementingIFloatingPoint right) => throw new NotImplementedException();

            static bool IComparisonOperators<ImplementingIFloatingPoint, ImplementingIFloatingPoint, bool>.operator >=(ImplementingIFloatingPoint left, ImplementingIFloatingPoint right) => throw new NotImplementedException();
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

            static ImplementingIFloatingPointIeee754 ISpanParsable<ImplementingIFloatingPointIeee754>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
                Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider);

            static ImplementingIFloatingPointIeee754 IParsable<ImplementingIFloatingPointIeee754>.Parse(string s, IFormatProvider? provider) =>
                Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider);

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

            static bool ISpanParsable<ImplementingIFloatingPointIeee754>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out ImplementingIFloatingPointIeee754 result) =>
                TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

            static bool IParsable<ImplementingIFloatingPointIeee754>.TryParse(string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ImplementingIFloatingPointIeee754 result) =>
                TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

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

    private static class CustomParsableDefaultedNumberStyles
    {
        public abstract record Base<TSelf>(string? Input, NumberStyles Style)
            where TSelf : Base<TSelf>, new()
        {
            protected static bool TryParseCore([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result)
            {
                if (s is null)
                {
                    result = default;
                    return false;
                }

                result = new() { Input = s, Style = style };
                return true;
            }

            protected static bool TryParseCore(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result)
            {
                result = new() { Input = s.ToString(), Style = style };
                return true;
            }
        }

        public sealed record IntNumberParse(string? Input, NumberStyles Style) : Base<IntNumberParse>(Input, Style)
        {
            public IntNumberParse() : this(default, default)
            {
            }

            public static IntNumberParse NaN => default!;

            public static IntNumberParse Parse(string s, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null) =>
                new(s, style);

            public static IntNumberParse Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null) =>
                new(s.ToString(), style);
        }

        public sealed record IntNumberTryParse(string? Input, NumberStyles Style) : Base<IntNumberTryParse>(Input, Style)
        {
            public IntNumberTryParse() : this(default, default)
            {
            }

            public static IntNumberTryParse NaN => default!;

            public static bool TryParse([NotNullWhen(true)] string? s, [Optional][DefaultParameterValue(NumberStyles.Number)] NumberStyles style, [Optional][DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out IntNumberTryParse result) =>
                TryParseCore(s, style, provider, out result);

            public static bool TryParse(ReadOnlySpan<char> s, [Optional][DefaultParameterValue(NumberStyles.Number)] NumberStyles style, [Optional][DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out IntNumberTryParse result) =>
                TryParseCore(s, style, provider, out result);
        }

        public sealed record IntegerParse(string? Input, NumberStyles Style) : Base<IntegerParse>(Input, Style)
        {
            public IntegerParse() : this(default, default)
            {
            }

            public static IntegerParse NaN => default!;

            public static IntegerParse Parse(string s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) =>
                new(s, style);

            public static IntegerParse Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) =>
                new(s.ToString(), style);
        }

        public sealed record IntegerTryParse(string? Input, NumberStyles Style) : Base<IntegerTryParse>(Input, Style)
        {
            public IntegerTryParse() : this(default, default)
            {
            }

            public static IntegerTryParse NaN => default!;

            public static bool TryParse([NotNullWhen(true)] string? s, [Optional][DefaultParameterValue(NumberStyles.Integer)] NumberStyles style, [Optional][DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out IntegerTryParse result) =>
                TryParseCore(s, style, provider, out result);

            public static bool TryParse(ReadOnlySpan<char> s, [Optional][DefaultParameterValue(NumberStyles.Integer)] NumberStyles style, [Optional][DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out IntegerTryParse result) =>
                TryParseCore(s, style, provider, out result);
        }

        public abstract record FloatBase<TSelf>(string? Input, NumberStyles Style) : Base<TSelf>(Input, Style)
#if NET7_0_OR_GREATER
            , IBinaryInteger<TSelf>
#endif
            where TSelf : FloatBase<TSelf>, new()
        {
#if NET7_0_OR_GREATER
            static TSelf INumberBase<TSelf>.One => throw new NotImplementedException();

            static int INumberBase<TSelf>.Radix => throw new NotImplementedException();

            static TSelf INumberBase<TSelf>.Zero => throw new NotImplementedException();

            static TSelf IAdditiveIdentity<TSelf, TSelf>.AdditiveIdentity => throw new NotImplementedException();

            static TSelf IMultiplicativeIdentity<TSelf, TSelf>.MultiplicativeIdentity => throw new NotImplementedException();

            static TSelf INumberBase<TSelf>.Abs(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsCanonical(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsComplexNumber(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsEvenInteger(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsFinite(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsImaginaryNumber(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsInfinity(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsInteger(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsNaN(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsNegative(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsNegativeInfinity(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsNormal(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsOddInteger(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsPositive(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsPositiveInfinity(TSelf value) => throw new NotImplementedException();

            static bool IBinaryNumber<TSelf>.IsPow2(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsRealNumber(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsSubnormal(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.IsZero(TSelf value) => throw new NotImplementedException();

            static TSelf IBinaryNumber<TSelf>.Log2(TSelf value) => throw new NotImplementedException();

            static TSelf INumberBase<TSelf>.MaxMagnitude(TSelf x, TSelf y) => throw new NotImplementedException();

            static TSelf INumberBase<TSelf>.MaxMagnitudeNumber(TSelf x, TSelf y) => throw new NotImplementedException();

            static TSelf INumberBase<TSelf>.MinMagnitude(TSelf x, TSelf y) => throw new NotImplementedException();

            static TSelf INumberBase<TSelf>.MinMagnitudeNumber(TSelf x, TSelf y) => throw new NotImplementedException();

            static TSelf INumberBase<TSelf>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

            static TSelf INumberBase<TSelf>.Parse(string s, NumberStyles style, IFormatProvider? provider) => throw new NotImplementedException();

            static TSelf ISpanParsable<TSelf>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();

            static TSelf IParsable<TSelf>.Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();

            static TSelf IBinaryInteger<TSelf>.PopCount(TSelf value) => throw new NotImplementedException();

            static TSelf IBinaryInteger<TSelf>.TrailingZeroCount(TSelf value) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.TryConvertFromChecked<TOther>(TOther value, out TSelf result) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.TryConvertFromSaturating<TOther>(TOther value, out TSelf result) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.TryConvertFromTruncating<TOther>(TOther value, out TSelf result) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.TryConvertToChecked<TOther>(TSelf value, out TOther result) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.TryConvertToSaturating<TOther>(TSelf value, out TOther result) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.TryConvertToTruncating<TOther>(TSelf value, out TOther result) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out TSelf result) => throw new NotImplementedException();

            static bool INumberBase<TSelf>.TryParse(string? s, NumberStyles style, IFormatProvider? provider, out TSelf result) => throw new NotImplementedException();

            static bool ISpanParsable<TSelf>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out TSelf result) => throw new NotImplementedException();

            static bool IParsable<TSelf>.TryParse(string? s, IFormatProvider? provider, out TSelf result) => throw new NotImplementedException();

            static bool IBinaryInteger<TSelf>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out TSelf value) => throw new NotImplementedException();

            static bool IBinaryInteger<TSelf>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out TSelf value) => throw new NotImplementedException();

            int IComparable.CompareTo(object? obj) => throw new NotImplementedException();

            int IComparable<TSelf>.CompareTo(TSelf? other) => throw new NotImplementedException();

#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
            bool IEquatable<TSelf>.Equals(TSelf? other) => throw new NotImplementedException();
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations

            int IBinaryInteger<TSelf>.GetByteCount() => throw new NotImplementedException();

            int IBinaryInteger<TSelf>.GetShortestBitLength() => throw new NotImplementedException();

            string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
                ToString();

            bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotImplementedException();

            bool IBinaryInteger<TSelf>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

            bool IBinaryInteger<TSelf>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

            static TSelf IUnaryPlusOperators<TSelf, TSelf>.operator +(TSelf value) => throw new NotImplementedException();

            static TSelf IAdditionOperators<TSelf, TSelf, TSelf>.operator +(TSelf left, TSelf right) => throw new NotImplementedException();

            static TSelf IUnaryNegationOperators<TSelf, TSelf>.operator -(TSelf value) => throw new NotImplementedException();

            static TSelf ISubtractionOperators<TSelf, TSelf, TSelf>.operator -(TSelf left, TSelf right) => throw new NotImplementedException();

            static TSelf IBitwiseOperators<TSelf, TSelf, TSelf>.operator ~(TSelf value) => throw new NotImplementedException();

            static TSelf IIncrementOperators<TSelf>.operator ++(TSelf value) => throw new NotImplementedException();

            static TSelf IDecrementOperators<TSelf>.operator --(TSelf value) => throw new NotImplementedException();

            static TSelf IMultiplyOperators<TSelf, TSelf, TSelf>.operator *(TSelf left, TSelf right) => throw new NotImplementedException();

            static TSelf IDivisionOperators<TSelf, TSelf, TSelf>.operator /(TSelf left, TSelf right) => throw new NotImplementedException();

            static TSelf IModulusOperators<TSelf, TSelf, TSelf>.operator %(TSelf left, TSelf right) => throw new NotImplementedException();

            static TSelf IBitwiseOperators<TSelf, TSelf, TSelf>.operator &(TSelf left, TSelf right) => throw new NotImplementedException();

            static TSelf IBitwiseOperators<TSelf, TSelf, TSelf>.operator |(TSelf left, TSelf right) => throw new NotImplementedException();

            static TSelf IBitwiseOperators<TSelf, TSelf, TSelf>.operator ^(TSelf left, TSelf right) => throw new NotImplementedException();

            static TSelf IShiftOperators<TSelf, int, TSelf>.operator <<(TSelf value, int shiftAmount) => throw new NotImplementedException();

            static TSelf IShiftOperators<TSelf, int, TSelf>.operator >>(TSelf value, int shiftAmount) => throw new NotImplementedException();

            static bool IEqualityOperators<TSelf, TSelf, bool>.operator ==(TSelf? left, TSelf? right) => throw new NotImplementedException();

            static bool IEqualityOperators<TSelf, TSelf, bool>.operator !=(TSelf? left, TSelf? right) => throw new NotImplementedException();

            static bool IComparisonOperators<TSelf, TSelf, bool>.operator <(TSelf left, TSelf right) => throw new NotImplementedException();

            static bool IComparisonOperators<TSelf, TSelf, bool>.operator >(TSelf left, TSelf right) => throw new NotImplementedException();

            static bool IComparisonOperators<TSelf, TSelf, bool>.operator <=(TSelf left, TSelf right) => throw new NotImplementedException();

            static bool IComparisonOperators<TSelf, TSelf, bool>.operator >=(TSelf left, TSelf right) => throw new NotImplementedException();

            static TSelf IShiftOperators<TSelf, int, TSelf>.operator >>>(TSelf value, int shiftAmount) => throw new NotImplementedException();
#endif
        }

        public sealed record FloatParse(string? Input, NumberStyles Style) : FloatBase<FloatParse>(Input, Style)
#if NET7_0_OR_GREATER
            , ISpanParsable<FloatParse>
#endif
        {
            public FloatParse() : this(default, default)
            {
            }

            public static FloatParse Parse(string s, NumberStyles style = NumberStyles.Float, IFormatProvider? provider = null) =>
                new(s, style);

            public static FloatParse Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Float, IFormatProvider? provider = null) =>
                new(s.ToString(), style);

#if NET7_0_OR_GREATER
            static FloatParse ISpanParsable<FloatParse>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
                Parse(s, provider: provider);

            static FloatParse IParsable<FloatParse>.Parse(string s, IFormatProvider? provider) =>
                Parse(s, provider: provider);
#endif
        }

        public sealed record FloatTryParse(string? Input, NumberStyles Style) : FloatBase<FloatTryParse>(Input, Style)
#if NET7_0_OR_GREATER
            , ISpanParsable<FloatTryParse>
#endif
        {
            public FloatTryParse() : this(default, default)
            {
            }

            public static bool TryParse([NotNullWhen(true)] string? s, [Optional][DefaultParameterValue(NumberStyles.Float)] NumberStyles style, [Optional][DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out FloatTryParse result) =>
                TryParseCore(s, style, provider, out result);

            public static bool TryParse(ReadOnlySpan<char> s, [Optional][DefaultParameterValue(NumberStyles.Float)] NumberStyles style, [Optional][DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out FloatTryParse result) =>
                TryParseCore(s, style, provider, out result);

#if NET7_0_OR_GREATER
            static bool ISpanParsable<FloatTryParse>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out FloatTryParse result) =>
                TryParse(s, provider: provider, result: out result);

            static bool IParsable<FloatTryParse>.TryParse(string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out FloatTryParse result) =>
                TryParse(s, provider: provider, result: out result);
#endif
        }

        public sealed record DifferentBetweenParseAndTryParseStringOnly(string? Input, NumberStyles Style) : Base<DifferentBetweenParseAndTryParseStringOnly>(Input, Style)
        {
            public DifferentBetweenParseAndTryParseStringOnly() : this(default, default)
            {
            }

            public static DifferentBetweenParseAndTryParseStringOnly Parse(string s, NumberStyles style = NumberStyles.AllowCurrencySymbol, IFormatProvider? provider = null) =>
                new(s, style);

            public static bool TryParse([NotNullWhen(true)] string? s, [Optional][DefaultParameterValue(NumberStyles.AllowThousands)] NumberStyles style, [Optional][DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out DifferentBetweenParseAndTryParseStringOnly result) =>
                TryParseCore(s, style, provider, out result);
        }

        public sealed record DifferentBetweenParseAndTryParse(string? Input, NumberStyles Style) : Base<DifferentBetweenParseAndTryParse>(Input, Style)
        {
            public DifferentBetweenParseAndTryParse() : this(default, default)
            {
            }

            public static DifferentBetweenParseAndTryParse Parse(string s, NumberStyles style = NumberStyles.AllowCurrencySymbol, IFormatProvider? provider = null) =>
                new(s, style);

            public static DifferentBetweenParseAndTryParse Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.AllowParentheses, IFormatProvider? provider = null) =>
                new(s.ToString(), style);

            public static bool TryParse([NotNullWhen(true)] string? s, [Optional][DefaultParameterValue(NumberStyles.AllowThousands)] NumberStyles style, [Optional][DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out DifferentBetweenParseAndTryParse result) =>
                TryParseCore(s, style, provider, out result);

            public static bool TryParse(ReadOnlySpan<char> s, [Optional][DefaultParameterValue(NumberStyles.AllowDecimalPoint)] NumberStyles style, [Optional][DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out DifferentBetweenParseAndTryParse result) =>
                TryParseCore(s, style, provider, out result);
        }

        public sealed record TryParseInheritedFromParse(string? Input, NumberStyles Style) : Base<TryParseInheritedFromParse>(Input, Style)
        {
            public TryParseInheritedFromParse() : this(default, default)
            {
            }

            public static TryParseInheritedFromParse Parse(string s, NumberStyles style = NumberStyles.AllowLeadingSign, IFormatProvider? provider = null) =>
                new(s, style);

            public static TryParseInheritedFromParse Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.AllowLeadingSign, IFormatProvider? provider = null) =>
                new(s.ToString(), style);

            public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out TryParseInheritedFromParse result) =>
                TryParseCore(s, style, provider, out result);

            public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out TryParseInheritedFromParse result) =>
                TryParseCore(s, style, provider, out result);
        }
    }

    [Flags]
    private enum ParseMethods
    {
        None = 0,
        ParseFromString = 1 << 0,
        ParseFromSpan = 1 << 1,
        TryParseFromString = 1 << 2,
        TryParseFromSpan = 1 << 3,
        Parse = ParseFromString | ParseFromSpan,
        TryParse = TryParseFromString | TryParseFromSpan,
        FromString = ParseFromString | TryParseFromString,
        FromSpan = ParseFromSpan | TryParseFromSpan,
        All = -1,
    }

    private static void AssertNoParseMethod<T>(string input, IFormatProvider? provider, ParseMethods parseMethods = ParseMethods.All)
    {
        if (provider is null)
        {
#pragma warning disable CA1305 // Specify IFormatProvider
            if (parseMethods.HasFlag(ParseMethods.ParseFromString))
            {
                AssertThrowsNoParseMethod(nameof(Optional<int>.Parse), () => Optional<T>.Parse(input));
            }

            if (parseMethods.HasFlag(ParseMethods.ParseFromSpan))
            {
                AssertThrowsNoParseMethod(nameof(Optional<int>.Parse), () => Optional<T>.Parse(input.AsSpan()));
            }
#pragma warning restore CA1305 // Specify IFormatProvider

            if (parseMethods.HasFlag(ParseMethods.TryParseFromString))
            {
#pragma warning disable CA1806 // Do not ignore method results
                AssertThrowsNoParseMethod(nameof(Optional<int>.TryParse), () => Optional<T>.TryParse(input, out _));
#pragma warning restore CA1806 // Do not ignore method results
            }

            if (parseMethods.HasFlag(ParseMethods.TryParseFromSpan))
            {
#pragma warning disable CA1806 // Do not ignore method results
                AssertThrowsNoParseMethod(nameof(Optional<int>.TryParse), () => Optional<T>.TryParse(input.AsSpan(), out _));
#pragma warning restore CA1806 // Do not ignore method results
            }
        }

        if (parseMethods.HasFlag(ParseMethods.ParseFromString))
        {
            AssertThrowsNoParseMethod(nameof(Optional<int>.Parse), () => Optional<T>.Parse(input, provider));
        }

        if (parseMethods.HasFlag(ParseMethods.ParseFromSpan))
        {
            AssertThrowsNoParseMethod(nameof(Optional<int>.Parse), () => Optional<T>.Parse(input.AsSpan(), provider));
        }

        if (parseMethods.HasFlag(ParseMethods.TryParseFromString))
        {
            AssertThrowsNoParseMethod(nameof(Optional<int>.TryParse), () => Optional<T>.TryParse(input, provider, out _));
        }

        if (parseMethods.HasFlag(ParseMethods.TryParseFromSpan))
        {
            AssertThrowsNoParseMethod(nameof(Optional<int>.TryParse), () => Optional<T>.TryParse(input.AsSpan(), provider, out _));
        }

        static void AssertThrowsNoParseMethod(string methodName, Action testCode)
        {
            var e = Assert.Throws<NotSupportedException>(testCode);
            Assert.Equal($"Type {typeof(T)} has no appropriate {methodName} method.", e.Message);
        }
    }

    private static void AssertDoesNotParse<T>(string? input, string? nestedInput, IFormatProvider? provider, ParseMethods parseMethods = ParseMethods.All)
    {
        if (provider is null)
        {
#pragma warning disable CA1305 // Specify IFormatProvider
            if (parseMethods.HasFlag(ParseMethods.ParseFromString))
            {
                if (input is null)
                {
                    Assert.Throws<ArgumentNullException>("s", () => Optional<T>.Parse(input!));
                }
                else
                {
                    AssertThrowsFormat<T>(input, nestedInput, () => Optional<T>.Parse(input));
                }
            }

            if (parseMethods.HasFlag(ParseMethods.ParseFromSpan))
            {
                AssertThrowsFormat<T>(input ?? "", nestedInput, () => Optional<T>.Parse(input.AsSpan()));
            }
#pragma warning restore CA1305 // Specify IFormatProvider

            if (parseMethods.HasFlag(ParseMethods.TryParseFromString))
            {
                Assert.False(Optional<T>.TryParse(input, out var resultWithoutProvider1));
                Assert.Equal(default, resultWithoutProvider1);
            }

            if (parseMethods.HasFlag(ParseMethods.TryParseFromSpan))
            {
                Assert.False(Optional<T>.TryParse(input.AsSpan(), out var resultWithoutProvider2));
                Assert.Equal(default, resultWithoutProvider2);
            }
        }

        if (parseMethods.HasFlag(ParseMethods.ParseFromString))
        {
            if (input is null)
            {
                Assert.Throws<ArgumentNullException>("s", () => Optional<T>.Parse(input!, provider));
            }
            else
            {
                AssertThrowsFormat<T>(input, nestedInput, () => Optional<T>.Parse(input, provider));
            }
        }

        if (parseMethods.HasFlag(ParseMethods.ParseFromSpan))
        {
            AssertThrowsFormat<T>(input ?? "", nestedInput, () => Optional<T>.Parse(input.AsSpan(), provider));
        }

        if (parseMethods.HasFlag(ParseMethods.TryParseFromString))
        {
            Assert.False(Optional<T>.TryParse(input, provider, out var resultWithProvider1));
            Assert.Equal(default, resultWithProvider1);
        }

        if (parseMethods.HasFlag(ParseMethods.TryParseFromSpan))
        {
            Assert.False(Optional<T>.TryParse(input.AsSpan(), provider, out var resultWithProvider2));
            Assert.Equal(default, resultWithProvider2);
        }

        static void AssertThrowsFormat<TNestedException>(string input, string? nestedInput, Action testCode)
        {
            if (typeof(T) == typeof(Complex))
                Assert.Throws<OverflowException>(testCode);
            else
                AssertThrows.Format(input, nestedInput, testCode);
        }
    }

    private static void AssertParses<T>(Optional<T> expected, string input, IFormatProvider? provider, ParseMethods parseMethods = ParseMethods.All)
    {
        var comparer = new CustomComparer<T>();

        if (provider is null)
        {
#pragma warning disable CA1305 // Specify IFormatProvider
            if (parseMethods.HasFlag(ParseMethods.ParseFromString))
            {
                Assert.Equal(expected, Optional<T>.Parse(input), comparer);
            }

            if (parseMethods.HasFlag(ParseMethods.ParseFromSpan))
            {
                Assert.Equal(expected, Optional<T>.Parse(input.AsSpan()), comparer);
            }
#pragma warning restore CA1305 // Specify IFormatProvider

            if (parseMethods.HasFlag(ParseMethods.TryParseFromString))
            {
                Assert.True(Optional<T>.TryParse(input, out var resultWithoutProvider1));
                Assert.Equal(expected, resultWithoutProvider1, comparer);
            }

            if (parseMethods.HasFlag(ParseMethods.TryParseFromSpan))
            {
                Assert.True(Optional<T>.TryParse(input.AsSpan(), out var resultWithoutProvider2));
                Assert.Equal(expected, resultWithoutProvider2, comparer);
            }
        }

        if (parseMethods.HasFlag(ParseMethods.ParseFromString))
        {
            Assert.Equal(expected, Optional<T>.Parse(input, provider), comparer);
        }

        if (parseMethods.HasFlag(ParseMethods.ParseFromSpan))
        {
            Assert.Equal(expected, Optional<T>.Parse(input.AsSpan(), provider), comparer);
        }

        if (parseMethods.HasFlag(ParseMethods.TryParseFromString))
        {
            Assert.True(Optional<T>.TryParse(input, provider, out var resultWithProvider1));
            Assert.Equal(expected, resultWithProvider1, comparer);
        }

        if (parseMethods.HasFlag(ParseMethods.TryParseFromSpan))
        {
            Assert.True(Optional<T>.TryParse(input.AsSpan(), provider, out var resultWithProvider2));
            Assert.Equal(expected, resultWithProvider2, comparer);
        }
    }

    private static ParseMethods ParseMethodsForBuiltInTypes
    {
        get
        {
            return IsFromSpanSupportedOnBuiltInTypes() ? ParseMethods.All : ParseMethods.FromString;

            static bool IsFromSpanSupportedOnBuiltInTypes() =>
#if NETCOREAPP2_1_OR_GREATER
                true;
#else
                false;
#endif
}
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
