using System.Reflection;

namespace Neme.Extensions.Reflection;

internal static class ParameterInfoExtensions
{
    public static Optional<object?> GetDefaultValue(this ParameterInfo parameter)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));

        return parameter.HasDefaultValue
            ? new(parameter.DefaultValue)
            : default;
    }

    public static Optional<T> GetDefaultValue<T>(this ParameterInfo parameter)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));

        return parameter.HasDefaultValue
            ? new((T)parameter.DefaultValue!)
            : default;
    }
}
