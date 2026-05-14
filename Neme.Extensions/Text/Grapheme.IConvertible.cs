using Neme.Extensions.Contracts;
using System.Text;

namespace Neme.Extensions.Text;

public partial struct Grapheme
{
    private Rune ToRune()
    {
        return Runes switch
        {
            [var rune] => rune,
            _ => throw new InvalidCastException(),
        };
    }

    TypeCode IConvertible.GetTypeCode() =>
        TypeCode.Object;

    bool IConvertible.ToBoolean(IFormatProvider? provider) =>
        throw new InvalidCastException();

    byte IConvertible.ToByte(IFormatProvider? provider) =>
        Convert.ToByte(ToRuneUnsignedValue());

    sbyte IConvertible.ToSByte(IFormatProvider? provider) =>
        Convert.ToSByte(ToRuneValue());

    char IConvertible.ToChar(IFormatProvider? provider) =>
        Convert.ToChar(ToRuneUnsignedValue());

    short IConvertible.ToInt16(IFormatProvider? provider) =>
        Convert.ToInt16(ToRuneValue());

    ushort IConvertible.ToUInt16(IFormatProvider? provider) =>
        Convert.ToUInt16(ToRuneUnsignedValue());

    int IConvertible.ToInt32(IFormatProvider? provider) =>
        ToRuneValue();

    uint IConvertible.ToUInt32(IFormatProvider? provider) =>
        ToRuneUnsignedValue();

    long IConvertible.ToInt64(IFormatProvider? provider) =>
        ToRuneValue();

    ulong IConvertible.ToUInt64(IFormatProvider? provider) =>
        ToRuneUnsignedValue();

#if NET7_0_OR_GREATER
    private Int128 ToInt128() =>
        ToRuneValue();

    private UInt128 ToUInt128() =>
        ToRuneUnsignedValue();
#endif

    float IConvertible.ToSingle(IFormatProvider? provider) =>
        throw new InvalidCastException();

    double IConvertible.ToDouble(IFormatProvider? provider) =>
        throw new InvalidCastException();

    decimal IConvertible.ToDecimal(IFormatProvider? provider) =>
        throw new InvalidCastException();

    DateTime IConvertible.ToDateTime(IFormatProvider? provider) =>
        throw new InvalidCastException();

    string IConvertible.ToString(IFormatProvider? provider) =>
        ToString();

    object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
    {
        Require.ArgumentNotNull(conversionType);

        if (conversionType == typeof(Rune))
            return ToRune();
#if NET7_0_OR_GREATER
        if (conversionType == typeof(Int128))
            return ToInt128();
        if (conversionType == typeof(UInt128))
            return ToUInt128();
#endif

        return ConvertExtensions.DefaultToType(this, conversionType, provider);
    }

    private int ToRuneValue() =>
        ToRune().Value;

    private uint ToRuneUnsignedValue() =>
        unchecked((uint)ToRune().Value);
}
