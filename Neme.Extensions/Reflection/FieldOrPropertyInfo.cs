using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.Reflection;

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
internal abstract class FieldOrPropertyInfo : MemberInfo
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
{
    private FieldOrPropertyInfo()
    {
    }

    public Type Type
    {
        get
        {
            return (MemberInfo)this switch
            {
                FieldInfo field => field.FieldType,
                PropertyInfo property => property.PropertyType,
                _ => throw new InvalidOperationException(),
            };
        }
    }

    public bool CanRead =>
        (MemberInfo)this switch
        {
            FieldInfo field => true,
            PropertyInfo property => property.CanRead,
            _ => throw new InvalidOperationException(),
        };

    public bool CanWrite =>
        (MemberInfo)this switch
        {
            FieldInfo field => !field.IsLiteral,
            PropertyInfo property => property.CanWrite,
            _ => throw new InvalidOperationException(),
        };

    public bool IsLiteral =>
        (MemberInfo)this is FieldInfo { IsLiteral: true };

    public bool IsReadOnly =>
        (MemberInfo)this switch
        {
            FieldInfo field => field.IsLiteral || field.IsInitOnly,
            PropertyInfo property => !property.CanWrite,
            _ => throw new InvalidOperationException(),
        };

    public bool HasParameters =>
        (MemberInfo)this switch
        {
#pragma warning disable CA1508 // Avoid dead conditional code
            FieldInfo field => false,
            PropertyInfo property => property.GetIndexParameters().Length > 0,
#pragma warning restore CA1508 // Avoid dead conditional code
            _ => throw new InvalidOperationException(),
        };

    public object? GetValue(object? obj)
    {
        return (MemberInfo)this switch
        {
            FieldInfo field => field.GetValue(obj),
            PropertyInfo property => property.GetValue(obj),
            _ => throw new InvalidOperationException(),
        };
    }

    public void SetValue(object? obj, object? value)
    {
        switch ((MemberInfo)this)
        {
            case FieldInfo field:
                field.SetValue(obj, value);
                break;
            case PropertyInfo property:
                property.SetValue(obj, value);
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    public void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture)
    {
        switch ((MemberInfo)this)
        {
            case FieldInfo field:
                field.SetValue(obj, value, invokeAttr, binder, culture);
                break;
            case PropertyInfo property:
                property.SetValue(obj, value, invokeAttr, binder, null, culture);
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    public object? GetRawConstantValue()
    {
        return (MemberInfo)this switch
        {
            FieldInfo field => field.GetRawConstantValue(),
            PropertyInfo property => property.GetRawConstantValue(),
            _ => throw new InvalidOperationException(),
        };
    }

    [return: NotNullIfNotNull(nameof(fieldInfo))]
    public static implicit operator FieldOrPropertyInfo?(FieldInfo? fieldInfo) =>
        fieldInfo.AsFieldOrPropertyInfo();

    [return: NotNullIfNotNull(nameof(propertyInfo))]
    public static implicit operator FieldOrPropertyInfo?(PropertyInfo? propertyInfo) =>
        propertyInfo.AsFieldOrPropertyInfo();

    [StackTraceHidden]
    [return: NotNullIfNotNull(nameof(fieldOrPropertyInfo))]
    public static explicit operator FieldInfo?(FieldOrPropertyInfo? fieldOrPropertyInfo) =>
        (FieldInfo?)(MemberInfo?)fieldOrPropertyInfo;

    [StackTraceHidden]
    [return: NotNullIfNotNull(nameof(fieldOrPropertyInfo))]
    public static explicit operator PropertyInfo?(FieldOrPropertyInfo? fieldOrPropertyInfo) =>
        (PropertyInfo?)(MemberInfo?)fieldOrPropertyInfo;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(FieldOrPropertyInfo? left, FieldOrPropertyInfo? right)
    {
        // Test "right" first to allow branch elimination when inlined for null checks (== null)
        // so it can become a simple test
        if (right is null)
        {
            return left is null;
        }

        // Try fast reference equality and opposite null check prior to calling the slower virtual Equals
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        return left is not null && left.Equals(right);
    }

    public static bool operator !=(FieldOrPropertyInfo? left, FieldOrPropertyInfo? right) => !(left == right);
}

internal static class FieldOrPropertyInfoExtensions
{
    public static FieldOrPropertyInfo? AsFieldOrPropertyInfo(this MemberInfo? memberInfo) =>
        memberInfo is FieldInfo or PropertyInfo or null ? Unsafe.As<FieldOrPropertyInfo>(memberInfo) : null;

    [return: NotNullIfNotNull(nameof(memberInfo))]
    public static FieldOrPropertyInfo? ToFieldOrPropertyInfo(this MemberInfo? memberInfo) =>
        memberInfo is FieldInfo or PropertyInfo or null ? Unsafe.As<FieldOrPropertyInfo>(memberInfo) : throw new InvalidCastException();

    [return: NotNullIfNotNull(nameof(fieldInfo))]
    public static FieldOrPropertyInfo? AsFieldOrPropertyInfo(this FieldInfo? fieldInfo) =>
        Unsafe.As<FieldOrPropertyInfo>(fieldInfo);

    [return: NotNullIfNotNull(nameof(propertyInfo))]
    public static FieldOrPropertyInfo? AsFieldOrPropertyInfo(this PropertyInfo? propertyInfo) =>
        Unsafe.As<FieldOrPropertyInfo>(propertyInfo);
}
