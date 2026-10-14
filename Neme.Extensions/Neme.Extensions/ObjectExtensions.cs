using Neme.Extensions.Contracts;
using System.Globalization;

namespace Neme.Extensions;

public static class ObjectExtensions
{
    extension<T>(T obj)
        where T : notnull
    {
        public string? ToStringInvariant()
        {
            Require.ArgumentNotNull(obj);

            return obj is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.InvariantCulture)
                : obj.ToString();
        }
    }
}
