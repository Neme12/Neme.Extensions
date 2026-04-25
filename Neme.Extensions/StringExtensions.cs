using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Neme.Extensions;

public static class StringExtensions
{
    extension(string @string)
    {
        public bool StartsWith(ReadOnlySpan<char> value, StringComparison comparisonType, [NotNullWhen(true)] out string? rest)
        {
            if (@string.AsSpan().StartsWith(value, comparisonType))
            {
                rest = @string[value.Length..];
                return true;
            }

            rest = default;
            return false;
        }

        public bool EndsWith(ReadOnlySpan<char> value, StringComparison comparisonType, [NotNullWhen(true)] out string? rest)
        {
            if (@string.AsSpan().EndsWith(value, comparisonType))
            {
                rest = @string[..^value.Length];
                return true;
            }

            rest = default;
            return false;
        }

        public string RemoveFromStart(ReadOnlySpan<char> value, StringComparison comparisonType)
        {
            if (@string.AsSpan().StartsWith(value, comparisonType))
                return @string[value.Length..];

            return @string;
        }

        public string RemoveFromEnd(ReadOnlySpan<char> value, StringComparison comparisonType)
        {
            if (@string.AsSpan().EndsWith(value, comparisonType))
                return @string[..^value.Length];

            return @string;
        }

        public static string JoinWithLastSeparator(string? separator, string? lastSeparator, IEnumerable<string?> values)
        {
            if (!values.TryGetCountWithoutEnumerating(out var count))
            {
                var valuesList = values.ToList();
                count = valuesList.Count;
                values = valuesList;
            }

            switch (count)
            {
                case 0:
                    return "";
                case 1:
                    {
                        using var enumerator = values.GetEnumerator();
                        enumerator.MoveNext();
                        return enumerator.Current ?? "";
                    }
                case >= 2:
                    {
                        var lastPart = string.Join(lastSeparator, values.Skip(count - 2));
                        if (count == 2)
                            return lastPart;

                        return string.Join(separator, values.Take(count - 2).Append(lastPart));
                    }
                default:
                    throw new UnreachableException();
            }
        }
    }
}
