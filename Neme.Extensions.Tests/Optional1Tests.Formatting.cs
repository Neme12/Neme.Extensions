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
        Assert.Equal(NumberStyles.Number, Optional<CustomParsable>.Parse("Some { foo }", null).Value.Style);

        Assert.Equal(NumberStyles.Integer, Optional<IntCustomParsable>.Parse("Some { foo }", null).Value.Style);
        Assert.Equal(NumberStyles.Float | NumberStyles.AllowThousands, Optional<CustomParsableContainingNaN1>.Parse("Some { foo }", null).Value.Style);
        Assert.Equal(NumberStyles.Float | NumberStyles.AllowThousands, Optional<CustomParsableContainingNaN2>.Parse("Some { foo }", null).Value.Style);

#if NET7_0_OR_GREATER
        Assert.Equal(NumberStyles.Integer, Optional<CustomParsableIBinaryInteger>.Parse("Some { foo }", null).Value.Style);
        Assert.Equal(NumberStyles.Float | NumberStyles.AllowThousands, Optional<CustomParsableIFloatingPointIeee754>.Parse("Some { foo }", null).Value.Style);
#endif
    }

    readonly record struct CustomParsable(string Input, NumberStyles Style)
    {
        public static CustomParsable Parse(string s, NumberStyles style, IFormatProvider? provider) =>
            new(s, style);
    }

    readonly record struct IntCustomParsable(string Input, NumberStyles Style)
    {
        public static IntCustomParsable Parse(string s, NumberStyles style, IFormatProvider? provider) =>
            new(s, style);
    }

    readonly record struct CustomParsableContainingNaN1(string Input, NumberStyles Style)
    {
        public static CustomParsableContainingNaN1 Parse(string s, NumberStyles style, IFormatProvider? provider) =>
            new(s, style);

        public const int NaN = 0;
    }

    readonly record struct CustomParsableContainingNaN2(string Input, NumberStyles Style)
    {
        public static CustomParsableContainingNaN2 Parse(string s, NumberStyles style, IFormatProvider? provider) =>
            new(s, style);

        public static int NaN => 0;
    }

#if NET7_0_OR_GREATER
    readonly record struct CustomParsableIBinaryInteger(string Input, NumberStyles Style) : IBinaryInteger<CustomParsableIBinaryInteger>
    {
        public static CustomParsableIBinaryInteger Parse(string s, NumberStyles style, IFormatProvider? provider) =>
            new(s, style);

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

        int IComparable<CustomParsableIBinaryInteger>.CompareTo(CustomParsableIBinaryInteger other) => throw new NotImplementedException();

        bool IEquatable<CustomParsableIBinaryInteger>.Equals(CustomParsableIBinaryInteger other) => throw new NotImplementedException();

        int IBinaryInteger<CustomParsableIBinaryInteger>.GetByteCount() => throw new NotImplementedException();

        int IBinaryInteger<CustomParsableIBinaryInteger>.GetShortestBitLength() => throw new NotImplementedException();

        string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();

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

        static bool IEqualityOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, bool>.operator ==(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static bool IEqualityOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, bool>.operator !=(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, bool>.operator <(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, bool>.operator >(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, bool>.operator <=(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIBinaryInteger, CustomParsableIBinaryInteger, bool>.operator >=(CustomParsableIBinaryInteger left, CustomParsableIBinaryInteger right) => throw new NotImplementedException();

        static CustomParsableIBinaryInteger IShiftOperators<CustomParsableIBinaryInteger, int, CustomParsableIBinaryInteger>.operator >>>(CustomParsableIBinaryInteger value, int shiftAmount) => throw new NotImplementedException();
    }

    readonly record struct CustomParsableIFloatingPointIeee754(string Input, NumberStyles Style) : IFloatingPointIeee754<CustomParsableIFloatingPointIeee754>
    {
        public static CustomParsableIFloatingPointIeee754 Parse(string s, NumberStyles style, IFormatProvider? provider) =>
            new(s, style);

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

        int IComparable<CustomParsableIFloatingPointIeee754>.CompareTo(CustomParsableIFloatingPointIeee754 other) => throw new NotImplementedException();

        bool IEquatable<CustomParsableIFloatingPointIeee754>.Equals(CustomParsableIFloatingPointIeee754 other) => throw new NotImplementedException();

        int IFloatingPoint<CustomParsableIFloatingPointIeee754>.GetExponentByteCount() => throw new NotImplementedException();

        int IFloatingPoint<CustomParsableIFloatingPointIeee754>.GetExponentShortestBitLength() => throw new NotImplementedException();

        int IFloatingPoint<CustomParsableIFloatingPointIeee754>.GetSignificandBitLength() => throw new NotImplementedException();

        int IFloatingPoint<CustomParsableIFloatingPointIeee754>.GetSignificandByteCount() => throw new NotImplementedException();

        string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();

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

        static bool IEqualityOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, bool>.operator ==(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static bool IEqualityOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, bool>.operator !=(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, bool>.operator <(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, bool>.operator >(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, bool>.operator <=(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();

        static bool IComparisonOperators<CustomParsableIFloatingPointIeee754, CustomParsableIFloatingPointIeee754, bool>.operator >=(CustomParsableIFloatingPointIeee754 left, CustomParsableIFloatingPointIeee754 right) => throw new NotImplementedException();
    }
#endif

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
