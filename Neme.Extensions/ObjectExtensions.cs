using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

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

    extension<T>([NotNull] T? obj)
    {
        public T NotNull()
        {
            if (obj is null)
                throw new UnreachableException("Object must not be null.");

            return obj;
        }
    }

    extension<T>(T obj)
        where T : struct
    {
        public T NotDefault()
        {
            if (EqualityComparer<T>.Default.Equals(obj, default))
                throw new UnreachableException("Object must not be the default value.");

            return obj;
        }
    }
}
