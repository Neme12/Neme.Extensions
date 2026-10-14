using Neme.Extensions;
using Neme.Extensions.Contracts;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Neme.Extensions;

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class OptionalTypeConverter : TypeConverter
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
{
    private readonly TypeConverter _converter;

    public OptionalTypeConverter(Type optionalType)
    {
        Require.ArgumentNotNull(optionalType);

        var converterType = typeof(Converter<>).MakeGenericType(Optional.GetUnderlyingType(optionalType)!);
        _converter = (TypeConverter)Activator.CreateInstance(converterType)!;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        _converter.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) =>
        _converter.CanConvertTo(context, destinationType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
        _converter.ConvertFrom(context, culture, value);

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) =>
        _converter.ConvertTo(context, culture, value, destinationType);

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    private sealed class Converter<T> : TypeConverter
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        private static readonly TypeConverter s_underlyingTypeConverter =
            TypeDescriptor.GetConverter(typeof(T));

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType is null)
                return false;

            if (Optional.GetUnderlyingType(sourceType) is { } sourceUnderlyingType &&
                typeof(T).IsAssignableFrom(sourceUnderlyingType))
            {
                return true;
            }

            //if (typeof(T).IsAssignableFrom(sourceType))
            //    return true;

            //var nullableUnderlyingType = Nullable.GetUnderlyingType(sourceType);
            //if (nullableUnderlyingType is not null && typeof(T).IsAssignableFrom(nullableUnderlyingType))
            //    return true;

            //if (s_underlyingTypeConverter.CanConvertFrom(context, sourceType))
            //    return true;

            //if (nullableUnderlyingType is not null && s_underlyingTypeConverter.CanConvertFrom(context, nullableUnderlyingType))
            //    return true;

            return false;
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            if (destinationType is null)
                return false;

            if (Optional.GetUnderlyingType(destinationType) is { } destinationUnderlyingType &&
                destinationUnderlyingType.IsAssignableFrom(typeof(T)))
            {
                return true;
            }

            //if (destinationType.IsAssignableFrom(typeof(T)))
            //    return true;

            //if (s_underlyingTypeConverter.CanConvertTo(context, destinationType))
            //    return true;

            //var nullableUnderlyingType = Nullable.GetUnderlyingType(destinationType);
            //if (nullableUnderlyingType is not null && s_underlyingTypeConverter.CanConvertTo(context, nullableUnderlyingType))
            //    return true;

            return false;
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is null)
                return Optional<T>.None;

            var type = value.GetType();
            if (Optional.GetUnderlyingType(type) is { } underlyingType)
            {
                Debug.Assert(typeof(T).IsAssignableFrom(underlyingType));

                var deconstructMethod = type.GetMethod(
                    nameof(Optional<T>.Deconstruct),
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER

                    0,
#endif
                    BindingFlags.Public | BindingFlags.Instance,
                    [typeof(bool), underlyingType]);

                Debug.AssertNotNull(deconstructMethod);
                Debug.Assert(!deconstructMethod!.ContainsGenericParameters);
                Debug.AssertEqual(deconstructMethod.ReturnType, typeof(void));

                var parameters = new object?[] { default(bool), ActivatorExtensions.CreateDefaultValue(underlyingType) };
                deconstructMethod.Invoke(value, parameters);

                var (hasValue, innerValue) = ((bool)parameters[0]!, (T)parameters[1]!);
                return hasValue ? new Optional<T>(innerValue) : default;
            }
            else throw new NotSupportedException();
            //else if (value is T underlyingValue)
            //{
            //    return Optional.Create(underlyingValue);
            //}
            //else
            //{
            //    Debug.Assert(s_underlyingTypeConverter.CanConvertFrom(context, value.GetType()));
            //    underlyingValue = (T)s_underlyingTypeConverter.ConvertFrom(context, culture, value)!;
            //    return Optional.Create(underlyingValue);
            //}
        }

        public override object? ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            Require.ArgumentNotNull(value);
            Require.ArgumentTypeEqual(value, typeof(Optional<T>));

            var optional = (Optional<T>)value!;
            if (optional == default)
                return null;

            if (destinationType.IsAssignableFrom(typeof(T)))
            {
                return optional.Value;
            }
            else
            {
                if (s_underlyingTypeConverter.CanConvertTo(context, destinationType))
                {
                    return s_underlyingTypeConverter.ConvertTo(context, culture, optional.Value, destinationType);
                }
                else
                {
                    var nullableUnderlyingType = Nullable.GetUnderlyingType(destinationType);
                    Debug.AssertNotNull(nullableUnderlyingType);
                    return s_underlyingTypeConverter.ConvertTo(context, culture, optional.Value, nullableUnderlyingType);
                }
            }

            throw new ArgumentOutOfRangeException(nameof(destinationType));
        }
    }
}
