using System.Globalization;
using System.Reflection;

namespace Neme.Extensions.Reflection;

public static class PropertyInfoExtensions
{
    extension(PropertyInfo property)
    {
        public TValue GetValue<TValue>(object? obj) =>
            (TValue)property.GetValue(obj)!;

        public TValue GetValue<TValue>(object? obj, object?[]? index) =>
            (TValue)property.GetValue(obj, index)!;

        public TValue GetValue<TValue>(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? index, CultureInfo? culture) =>
            (TValue)property.GetValue(obj, invokeAttr, binder, index, culture)!;

        public TValue GetConstantValue<TValue>() =>
            (TValue)property.GetConstantValue()!;

        public TValue GetRawConstantValue<TValue>() =>
            (TValue)property.GetRawConstantValue()!;
    }
}
