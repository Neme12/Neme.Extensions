using System.Text;

namespace Neme.Extensions.Text;

public static class GraphemeLinqExtensions
{
    public static Rune First(this Grapheme grapheme) =>
        grapheme.Runes[0];

    public static Rune FirstOrDefault(this Grapheme grapheme) =>
        grapheme.Runes[0];

    public static Rune Last(this Grapheme grapheme) =>
        grapheme.Runes[^1];

    public static Rune LastOrDefault(this Grapheme grapheme) =>
        grapheme.Runes[^1];
}
