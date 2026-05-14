using Neme.Extensions.Contracts;

namespace Neme.Extensions;

public static class ConvertExtensions
{
    public static object DefaultToType<TConvertible>(TConvertible value, Type targetType, IFormatProvider? provider)
        where TConvertible : IConvertible
    {
        Require.ArgumentNotNull(value);
        Require.ArgumentNotNull(targetType);

        if (targetType == value.GetType())
            return value;

        if (targetType == typeof(bool))
            return value.ToBoolean(provider);
        if (targetType == typeof(char))
            return value.ToChar(provider);
        if (targetType == typeof(byte))
            return value.ToByte(provider);
        if (targetType == typeof(sbyte))
            return value.ToSByte(provider);
        if (targetType == typeof(short))
            return value.ToInt16(provider);
        if (targetType == typeof(ushort))
            return value.ToUInt16(provider);
        if (targetType == typeof(int))
            return value.ToInt32(provider);
        if (targetType == typeof(uint))
            return value.ToUInt32(provider);
        if (targetType == typeof(long))
            return value.ToInt64(provider);
        if (targetType == typeof(ulong))
            return value.ToUInt64(provider);
        if (targetType == typeof(float))
            return value.ToSingle(provider);
        if (targetType == typeof(double))
            return value.ToDouble(provider);
        if (targetType == typeof(decimal))
            return value.ToDecimal(provider);
        if (targetType == typeof(DateTime))
            return value.ToDateTime(provider);
        if (targetType == typeof(string))
            return value.ToString(provider);
        if (targetType == typeof(object))
            return value;
        if (targetType == typeof(Enum))
            return (Enum)(object)value;

        throw new InvalidCastException();
    }
}
