using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Neme.Extensions.Reflection;

internal readonly struct FieldOrPropertyInfo
{
    private readonly MemberInfo _memberInfo;

    public FieldOrPropertyInfo(MemberInfo memberInfo)
    {
        if (!IsValidMember(memberInfo))
            throw new ArgumentException(null, nameof(memberInfo));

        _memberInfo = memberInfo;
    }

    public FieldOrPropertyInfo(FieldInfo fieldInfo)
    {
        _memberInfo = fieldInfo;
    }

    public FieldOrPropertyInfo(PropertyInfo propertyInfo)
    {
        if (!IsValidProperty(propertyInfo))
            throw new ArgumentException(null, nameof(propertyInfo));

        _memberInfo = propertyInfo;
    }

    public MemberTypes MemberType =>
        _memberInfo.MemberType;

    public string Name =>
        _memberInfo.Name;

    public Type? DeclaringType =>
        _memberInfo.DeclaringType;

    public Type? ReflectedType =>
        _memberInfo.ReflectedType;

    public Type Type
    {
        get
        {
            return _memberInfo switch
            {
                FieldInfo field => field.FieldType,
                PropertyInfo property => property.PropertyType,
                _ => throw new ObjectDisposedException(nameof(FieldOrPropertyInfo)),
            };
        }
    }

    public Module Module =>
        _memberInfo.Module;

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public bool HasSameMetadataDefinitionAs(MemberInfo other) =>
        _memberInfo.HasSameMetadataDefinitionAs(other);
#endif

    public bool IsDefined(Type attributeType, bool inherit) =>
        _memberInfo.IsDefined(attributeType, inherit);

    public object[] GetCustomAttributes(bool inherit) =>
        _memberInfo.GetCustomAttributes(inherit);

    public object[] GetCustomAttributes(Type attributeType, bool inherit) =>
        _memberInfo.GetCustomAttributes(attributeType, inherit);

    public IEnumerable<CustomAttributeData> CustomAttributes =>
        _memberInfo.CustomAttributes;

    public IList<CustomAttributeData> GetCustomAttributesData() =>
        _memberInfo.GetCustomAttributesData();

#if NETCOREAPP3_0_OR_GREATER
    public bool IsCollectible =>
        _memberInfo.IsCollectible;
#endif

    public int MetadataToken =>
        _memberInfo.MetadataToken;

    public bool IsLiteral =>
        _memberInfo is FieldInfo { IsLiteral: true };

    public bool IsReadOnly =>
        _memberInfo switch
        {
            FieldInfo field => field.IsLiteral || field.IsInitOnly,
            PropertyInfo property => !property.CanWrite,
            _ => throw new ObjectDisposedException(nameof(FieldOrPropertyInfo)),
        };

    public object? GetValue(object? obj)
    {
        return _memberInfo switch
        {
            FieldInfo field => field.GetValue(obj),
            PropertyInfo property => property.GetValue(obj),
            _ => throw new ObjectDisposedException(nameof(FieldOrPropertyInfo)),
        };
    }

    public void SetValue(object? obj, object? value)
    {
        switch (_memberInfo)
        {
            case FieldInfo field:
                field.SetValue(obj, value);
                break;
            case PropertyInfo property:
                property.SetValue(obj, value);
                break;
            default:
                throw new ObjectDisposedException(nameof(FieldOrPropertyInfo));
        }
    }

    public void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture)
    {
        switch (_memberInfo)
        {
            case FieldInfo field:
                field.SetValue(obj, value, invokeAttr, binder, culture);
                break;
            case PropertyInfo property:
                property.SetValue(obj, value, invokeAttr, binder, null, culture);
                break;
            default:
                throw new ObjectDisposedException(nameof(FieldOrPropertyInfo));
        }
    }

    public object? GetRawConstantValue()
    {
        return _memberInfo switch
        {
            FieldInfo field => field.GetRawConstantValue(),
            PropertyInfo property => property.GetRawConstantValue(),
            _ => throw new ObjectDisposedException(nameof(FieldOrPropertyInfo)),
        };
    }

    private static bool IsValidMember([NotNullWhen(true)] MemberInfo? memberInfo) =>
        memberInfo is FieldInfo || memberInfo is PropertyInfo property && IsValidProperty(property);

    private static bool IsValidProperty(PropertyInfo propertyInfo) =>
        propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0;

    public static FieldOrPropertyInfo? Get(MemberInfo? memberInfo) =>
        IsValidMember(memberInfo)
            ? new(memberInfo)
            : null;

    public static explicit operator FieldOrPropertyInfo(MemberInfo memberInfo) =>
        new(memberInfo);

    public static implicit operator FieldOrPropertyInfo(FieldInfo fieldInfo) =>
        new(fieldInfo);

    public static explicit operator FieldOrPropertyInfo(PropertyInfo propertyInfo) =>
        new(propertyInfo);

    public static implicit operator MemberInfo(FieldOrPropertyInfo fieldOrPropertyInfo) =>
        fieldOrPropertyInfo._memberInfo;

    public static explicit operator FieldInfo(FieldOrPropertyInfo fieldOrPropertyInfo) =>
        (FieldInfo)fieldOrPropertyInfo._memberInfo;

    public static explicit operator PropertyInfo(FieldOrPropertyInfo fieldOrPropertyInfo) =>
        (PropertyInfo)fieldOrPropertyInfo._memberInfo;
}
