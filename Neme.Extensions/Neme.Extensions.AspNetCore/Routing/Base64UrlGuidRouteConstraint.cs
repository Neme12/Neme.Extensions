using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace Neme.Extensions.AspNetCore.Routing;

public sealed partial class Base64UrlGuidRouteConstraint : IRouteConstraint
{
    // lang=regex
    private const string RegexPattern = "^[A-Za-z0-9_-]{22}$";

#if NET7_0_OR_GREATER
    [GeneratedRegex(RegexPattern, RegexOptions.CultureInvariant)]
    internal static partial Regex Base64UrlGuidPattern();
#else
    private static readonly Regex _base64UrlRegex =
        new(RegexPattern, RegexOptions.CultureInvariant | RegexOptions.Compiled);

    internal static Regex Base64UrlGuidPattern() =>
        _base64UrlRegex;
#endif

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (values.TryGetValue(routeKey, out var value) && value is string stringValue)
            return Base64UrlGuidPattern().IsMatch(stringValue);

        return false;
    }
}
