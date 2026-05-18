using System.Globalization;
using System.Reflection;

namespace Neme.Extensions.Reflection;

public static class PropertyInfoExtensions
{
    extension(PropertyInfo property)
    {
        public object? GetValue<TValue>(object? obj) =>
            (TValue)property.GetValue(obj)!;

        public object? GetValue<TValue>(object? obj, object?[]? index) =>
            (TValue)property.GetValue(obj, index)!;

        public object? GetValue<TValue>(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? index, CultureInfo? culture) =>
            (TValue)property.GetValue(obj, invokeAttr, binder, index, culture)!;
    }
}
