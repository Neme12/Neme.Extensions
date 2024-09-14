using Neme.Extensions.Tests.Utilities;
using System.Globalization;
using System.Numerics;

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
        AssertDoesNotParse<int>(null!, null, null);
        AssertDoesNotParse<int>(null!, null, CultureInfo.InvariantCulture);
        AssertDoesNotParse<int>(null!, null, CultureInfo.GetCultureInfo("de"));
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
        AssertParses<string?>(new(""), "Some {  }", null);
        AssertParses<string?>(new(""), "Some {  }", CultureInfo.InvariantCulture);
        AssertParses<string?>(new(""), "Some {  }", CultureInfo.GetCultureInfo("de"));
    }

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
        AssertDoesNotParse<int>("Some { x }", "x", null);
        AssertDoesNotParse<int>("SOME { 42 }", null, null);
    }

    [Fact]
    public void Parse_TestNumberStyles()
    {
        TestIsInteger<ushort>(null);
        TestIsInteger<short>(-1);
        TestIsInteger<uint>(null);
        TestIsInteger<int>(-1);
        TestIsInteger<ulong>(null);
        TestIsInteger<long>(-1);
#if NET5_0_OR_GREATER
        TestIsInteger<nuint>(null);
        TestIsInteger<nint>(-1);
#endif
        TestIsInteger<BigInteger>(BigInteger.MinusOne);

        TestIsFloatAndAllowThousands<float>(-1f, 42.2f, 10_000f);
        TestIsFloatAndAllowThousands<double>(-1d, 42.2d, 10_000d);

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

    private static void AssertDoesNotParse<T>(string input, string? nestedInput, IFormatProvider? provider)
    {
        var parseSpan = ShouldParseSpan<T>();

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

            Assert.False(Optional<T>.TryParse(input, out var resultWithoutProvider1));
            Assert.Equal(default, resultWithoutProvider1);

            if (parseSpan)
            {
                Assert.False(Optional<T>.TryParse(input.AsSpan(), out var resultWithoutProvider2));
                Assert.Equal(default, resultWithoutProvider2);
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

        Assert.False(Optional<T>.TryParse(input, provider, out var resultWithProvider1));
        Assert.Equal(default, resultWithProvider1);

        if (parseSpan)
        {
            Assert.False(Optional<T>.TryParse(input.AsSpan(), provider, out var resultWithProvider2));
            Assert.Equal(default, resultWithProvider2);
        }
    }

    private static void AssertParses<T>(Optional<T> expected, string input, IFormatProvider? provider)
    {
        var parseSpan = ShouldParseSpan<T>();

        if (provider is null)
        {
#pragma warning disable CA1305 // Specify IFormatProvider
            Assert.Equal(expected, Optional<T>.Parse(input));

            if (parseSpan)
                Assert.Equal(expected, Optional<T>.Parse(input.AsSpan()));
#pragma warning restore CA1305 // Specify IFormatProvider

            Assert.True(Optional<T>.TryParse(input, out var resultWithoutProvider1));
            Assert.Equal(expected, resultWithoutProvider1);

            if (parseSpan)
            {
                Assert.True(Optional<T>.TryParse(input.AsSpan(), out var resultWithoutProvider2));
                Assert.Equal(expected, resultWithoutProvider2);
            }
        }

        Assert.Equal(expected, Optional<T>.Parse(input, provider));

        if (parseSpan)
            Assert.Equal(expected, Optional<T>.Parse(input.AsSpan(), provider));

        Assert.True(Optional<T>.TryParse(input, provider, out var resultWithProvider1));
        Assert.Equal(expected, resultWithProvider1);

        if (parseSpan)
        {
            Assert.True(Optional<T>.TryParse(input.AsSpan(), provider, out var resultWithProvider2));
            Assert.Equal(expected, resultWithProvider2);
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
}
