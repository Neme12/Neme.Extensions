using Neme.Extensions.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Neme.Extensions;

public static class ObjectExtensions
{
    extension<T>(T obj)
        where T : notnull
    {
        public string? ToStringInvariant()
        {
            return obj is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.InvariantCulture)
                : obj.ToString();
        }
    }

    extension<T>([NotNull] T? value)
    {
        public T NotNull([CallerArgumentExpression(nameof(value))] string? valueText = null)
        {
            if (value is null)
                throw new AssertionFailedException($"{nameof(NotNull)} failed for value: {valueText}", value);

            return value;
        }
    }

    extension<T>(T value) where T : struct
    {
        public T NotDefault([CallerArgumentExpression(nameof(value))] string? valueText = null)
        {
            if (EqualityComparer<T>.Default.Equals(value, default))
                throw new AssertionFailedException($"{nameof(NotDefault)} failed for value: {valueText}", value);

            return value;
        }
    }
}
