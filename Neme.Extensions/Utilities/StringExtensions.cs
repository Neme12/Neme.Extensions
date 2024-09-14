using System.Diagnostics.CodeAnalysis;

namespace Neme.Extensions.Utilities;

internal static class StringExtensions
{
    public static bool StartsWith(this string @string, string value, StringComparison comparisonType, [NotNullWhen(true)] out string? rest)
    {
        if (@string.StartsWith(value, comparisonType))
        {
            rest = @string[value.Length..];
            return true;
        }

        rest = default;
        return false;
    }
}
