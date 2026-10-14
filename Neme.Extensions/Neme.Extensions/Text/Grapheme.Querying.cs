using System.Globalization;
using System.Text;

namespace Neme.Extensions.Text;

public partial struct Grapheme
{
    public static bool IsWhiteSpace(Grapheme value) =>
        All(value, static rune => Rune.IsWhiteSpace(rune) || IsModifier(rune));

    public static bool IsSeparator(Grapheme value) =>
        All(value, static rune => Rune.IsSeparator(rune) || IsModifier(rune));

    public static bool IsLetter(Grapheme value) =>
        All(value, static rune => Rune.IsLetter(rune) || IsModifier(rune));

    public static bool IsLetterOrDigit(Grapheme value) =>
        All(value, static rune => Rune.IsLetterOrDigit(rune) || IsModifier(rune));

    public static bool IsDigit(Grapheme value) =>
        All(value, static rune => Rune.IsDigit(rune) || IsModifier(rune));

    public static bool IsNumber(Grapheme value) =>
        All(value, static rune => Rune.IsNumber(rune) || IsModifier(rune));

    public static bool IsUpper(Grapheme value) =>
        All(value, static rune => Rune.IsUpper(rune) || IsModifier(rune));

    public static bool IsLower(Grapheme value) =>
        All(value, static rune => Rune.IsLower(rune) || IsModifier(rune));

    private static bool All(Grapheme value, Func<Rune, bool> predicate)
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return value.AsSpan().All(predicate);
#else
        return value.All(predicate);
#endif
    }

    private static bool IsModifier(Rune rune) =>
        Rune.GetUnicodeCategory(rune) is
            UnicodeCategory.NonSpacingMark or
            UnicodeCategory.EnclosingMark or
            UnicodeCategory.SpacingCombiningMark or
            UnicodeCategory.Format;
}
