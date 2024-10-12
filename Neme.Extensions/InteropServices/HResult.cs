using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using static Neme.Extensions.Utilities.FormatHelper;

namespace Neme.Extensions.InteropServices;

[DebuggerDisplay("{ToString(\"Xf\", null), nq}")]
internal readonly partial struct HResult : IEquatable<HResult>, IFormattable, IConvertible
#if NET6_0_OR_GREATER
    , ISpanFormattable
#endif
#if NET7_0_OR_GREATER
    , IEqualityOperators<HResult, HResult, bool>
#endif
{
    private const uint SMask = 0b10000000_00000000_00000000_00000000u;
    private const uint RMask = 0b01000000_00000000_00000000_00000000u;
    private const uint CMask = 0b00100000_00000000_00000000_00000000u;
    private const uint NMask = 0b00010000_00000000_00000000_00000000u;
    private const uint XMask = 0b00001000_00000000_00000000_00000000u;
    private const uint FacilityMask = 0b00000111_11111111_00000000_00000000u;
    private const uint CodeMask = 0b00000000_00000000_11111111_11111111;

    private const int SOffset = 31;
    private const int ROffset = 30;
    private const int COffset = 29;
    private const int NOffset = 28;
    private const int XOffset = 27;
    private const int FacilityOffset = 16;

    private readonly uint _value;

    public HResult(int value)
    {
        _value = unchecked((uint)value);
    }

    public HResult(uint value)
    {
        _value = value;
    }

    public HResult(bool isError, bool isCustomerDefined, Facility facility, ushort code)
    {
        if (((uint)facility & ~(FacilityMask >> FacilityOffset)) != 0)
            throw new ArgumentOutOfRangeException(nameof(facility));

        _value = Convert.ToUInt32(isError) << SOffset
            | Convert.ToUInt32(isCustomerDefined) << COffset
            | ((uint)facility << FacilityOffset)
            | code;
    }

    public HResult(bool s, bool r, bool c, bool n, bool x, Facility facility, ushort code)
    {
        if (((uint)facility & ~(FacilityMask >> FacilityOffset)) != 0)
            throw new ArgumentOutOfRangeException(nameof(facility));

        _value = Convert.ToUInt32(s) << SOffset
            | Convert.ToUInt32(r) << ROffset
            | Convert.ToUInt32(c) << COffset
            | Convert.ToUInt32(n) << NOffset
            | Convert.ToUInt32(x) << XOffset
            | (uint)facility << FacilityOffset
            | code;
    }

    public int Value =>
        unchecked((int)_value);

    public uint UValue =>
        _value;

    public bool SBit
    {
        get => (_value & SMask) != 0;
        init => _value = value
            ? _value | SMask
            : _value & ~SMask;
    }

    public bool RBit
    {
        get => (_value & RMask) != 0;
        init => _value = value
            ? _value | RMask
            : _value & ~RMask;
    }

    public bool CBit
    {
        get => (_value & CMask) != 0;
        init => _value = value
            ? _value | CMask
            : _value & ~CMask;
    }

    public bool NBit
    {
        get => (_value & NMask) != 0;
        init => _value = value
            ? _value | NMask
            : _value & ~NMask;
    }

    public bool XBit
    {
        get => (_value & XMask) != 0;
        init => _value = value
            ? _value | XMask
            : _value & ~XMask;
    }

    public bool IsError
    {
        get => SBit;
        init => SBit = value;
    }

    public bool IsCustomerDefined
    {
        get => CBit;
        init => CBit = value;
    }

    public Facility Facility
    {
        get => (Facility)((_value & FacilityMask) >> FacilityOffset);
        init => _value = (_value & ~FacilityMask) | ((uint)value << FacilityOffset);
    }

    public uint Code
    {
        get => _value & CodeMask;
        init => _value = (_value & ~CodeMask) | value;
    }

    public HResult Normalize() =>
        new(_value & (SMask | CMask | FacilityMask | CodeMask));

    public static (string name, string? description)? GetPredefinedConstant(HResult hResult)
    {
        return _constants.TryGetValue(unchecked((int)hResult._value), out var value)
            ? (value.name, value.description)
            : null;
    }

    public static HResult FromWin32(ushort errorCode) =>
        errorCode == 0 ? default : new(true, false, Facility.WIN32, errorCode);

    public override string ToString() =>
        ToString(null, null);

    enum NumberFormat
    {
        Signed,
        Unsigned,
        Binary,
        HexadecimalLower,
        HexadecimalUpper,
    }

    enum OutputFormat
    {
        Numeric,
        NumericWithName,
        NumericWithNameAndDescription,
    }

    private static (NumberFormat numberFormat, OutputFormat outputFormat) ParseFormat(ReadOnlySpan<char> format)
    {
        if (format.Length > 2)
            throw new FormatException();

        var numberFormat = format.Length > 0
            ? format[0] switch
            {
                's' or 'S' => NumberFormat.Signed,
                'u' or 'U' => NumberFormat.Unsigned,
                'b' or 'B' => NumberFormat.Binary,
                'x' => NumberFormat.HexadecimalLower,
                'X' => NumberFormat.HexadecimalUpper,
                _ => throw new FormatException(),
            }
            : NumberFormat.HexadecimalUpper;

        var outputFormat = format.Length > 1
            ? format[1] switch
            {
                'n' or 'N' => OutputFormat.NumericWithName,
                'f' or 'F' => OutputFormat.NumericWithNameAndDescription,
                _ => throw new FormatException(),
            }
            : OutputFormat.Numeric;

        return (numberFormat, outputFormat);
    }

#pragma warning disable CA1725 // Parameter names should match base declaration
    public string ToString(string? format = null, IFormatProvider? provider = null)
#pragma warning restore CA1725 // Parameter names should match base declaration
    {
        var (numberFormat, outputFormat) = ParseFormat(format.AsSpan());

        var str = numberFormat switch
        {
            NumberFormat.Signed => unchecked((int)_value).ToString("d", provider),
            NumberFormat.Unsigned => _value.ToString("d", provider),
            NumberFormat.Binary => Format(provider, $"0b{_value:b32}"),
            NumberFormat.HexadecimalLower => Format(provider, $"0x{_value:x8}"),
            NumberFormat.HexadecimalUpper => Format(provider, $"0x{_value:X8}"),
            _ => throw null!,
        };

        if (outputFormat >= OutputFormat.NumericWithName && _constants.TryGetValue(unchecked((int)_value), out var value))
        {
            str += outputFormat == OutputFormat.NumericWithNameAndDescription && value.description is not null
                ? $" ({value.name}: {value.description})"
                : $" ({value.name})";
        }

        return str;
    }

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        var (numberFormat, outputFormat) = ParseFormat(format);

        charsWritten = 0;

        int numberCharsWritten;
        var numberResult = numberFormat switch
        {
            NumberFormat.Signed => unchecked((int)_value).TryFormat(destination, out numberCharsWritten, "d", provider),
            NumberFormat.Unsigned => _value.TryFormat(destination, out numberCharsWritten, "d", provider),
            NumberFormat.Binary => destination.TryWrite(provider, $"0b{_value:b32}", out numberCharsWritten),
            NumberFormat.HexadecimalLower => destination.TryWrite(provider, $"0x{_value:x8}", out numberCharsWritten),
            NumberFormat.HexadecimalUpper => destination.TryWrite(provider, $"0x{_value:X8}", out numberCharsWritten),
            _ => throw null!,
        };

        charsWritten += numberCharsWritten;

        if (!numberResult)
            return false;

        if (outputFormat >= OutputFormat.NumericWithName && _constants.TryGetValue(unchecked((int)_value), out var value))
        {
            int restCharsWritten;
            var restResult = outputFormat == OutputFormat.NumericWithNameAndDescription && value.description is not null
                ? destination.TryWrite(provider, $" ({value.name}: {value.description})", out restCharsWritten)
                : destination.TryWrite(provider, $" ({value.name})", out restCharsWritten);

            charsWritten += restCharsWritten;

            if (!restResult)
                return false;
        }

        return true;
    }
#endif

    public bool Equals(HResult other) =>
        _value == other._value;

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is HResult other && Equals(other);

    public override int GetHashCode() =>
        _value.GetHashCode();

    TypeCode IConvertible.GetTypeCode() =>
        TypeCode.Object;

    bool IConvertible.ToBoolean(IFormatProvider? provider) =>
        ((IConvertible)_value).ToBoolean(provider);

    char IConvertible.ToChar(IFormatProvider? provider) =>
        ((IConvertible)_value).ToChar(provider);

    byte IConvertible.ToByte(IFormatProvider? provider) =>
        ((IConvertible)_value).ToByte(provider);

    sbyte IConvertible.ToSByte(IFormatProvider? provider) =>
        ((IConvertible)_value).ToSByte(provider);

    short IConvertible.ToInt16(IFormatProvider? provider) =>
        ((IConvertible)_value).ToInt16(provider);

    int IConvertible.ToInt32(IFormatProvider? provider) =>
        ((IConvertible)_value).ToInt32(provider);

    long IConvertible.ToInt64(IFormatProvider? provider) =>
        ((IConvertible)_value).ToInt64(provider);

    ushort IConvertible.ToUInt16(IFormatProvider? provider) =>
        ((IConvertible)_value).ToUInt16(provider);

    uint IConvertible.ToUInt32(IFormatProvider? provider) =>
        ((IConvertible)_value).ToUInt32(provider);

    ulong IConvertible.ToUInt64(IFormatProvider? provider) =>
        ((IConvertible)_value).ToUInt64(provider);

    float IConvertible.ToSingle(IFormatProvider? provider) =>
        ((IConvertible)_value).ToSingle(provider);

    decimal IConvertible.ToDecimal(IFormatProvider? provider) =>
        ((IConvertible)_value).ToDecimal(provider);

    double IConvertible.ToDouble(IFormatProvider? provider) =>
        ((IConvertible)_value).ToDouble(provider);

    string IConvertible.ToString(IFormatProvider? provider) =>
        ToString(null, provider);

    DateTime IConvertible.ToDateTime(IFormatProvider? provider) =>
        ((IConvertible)_value).ToDateTime(provider);

    object IConvertible.ToType(Type conversionType, IFormatProvider? provider) =>
        ((IConvertible)_value).ToType(conversionType, provider);

    public static bool operator ==(HResult left, HResult right) =>
        left.Equals(right);

    public static bool operator !=(HResult left, HResult right) =>
        !(left == right);

    public static implicit operator HResult(int value) =>
        new(value);

    public static implicit operator HResult(uint value) =>
        new(value);

    public static explicit operator int(HResult value) =>
        value.Value;

    public static explicit operator uint(HResult value) =>
        value.UValue;
}
