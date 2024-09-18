namespace Neme.Extensions;

internal static class CharExtensions
{
    public static char Parse(ReadOnlySpan<char> s)
    {
        if (s.Length != 1)
            throw new FormatException("String must be exactly one character long.");

        return s[0];
    }

    public static bool TryParse(ReadOnlySpan<char> s, out char result)
    {
        if (s.Length != 1)
        {
            result = default;
            return false;
        }

        result = s[0];
        return true;
    }
}
