using Neme.Extensions.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Reflexy.Utilities;

public static class Diacritics
{
    public static bool ContainsDiacritics(NormalizedText normalizedText)
    {
        foreach (var rune in normalizedText.Text.EnumerateRunes())
        {
            if (Rune.GetUnicodeCategory(rune) == UnicodeCategory.NonSpacingMark)
                return true;
        }

        return false;
    }

    public static int GetDiacriticsCount(NormalizedText normalizedText)
    {
        int count = 0;

        foreach (var rune in normalizedText.Text.EnumerateRunes())
        {
            if (Rune.GetUnicodeCategory(rune) == UnicodeCategory.NonSpacingMark)
                ++count;
        }

        return count;
    }

    public static string GetTextWithoutDiacritics(NormalizedText normalizedText, bool renormalized)
    {
        var builder = new DefaultInterpolatedStringHandler(
            normalizedText.Text.Length, 0, null, stackalloc char[128]);

        var runeBuffer = (Span<char>)stackalloc char[2];

        foreach (var rune in normalizedText.Text.EnumerateRunes())
        {
            if (Rune.GetUnicodeCategory(rune) == UnicodeCategory.NonSpacingMark)
                continue;

            rune.EncodeToUtf16(runeBuffer);
            builder.AppendFormatted(runeBuffer[..rune.Utf16SequenceLength]);
        }

        var result = builder.ToStringAndClear();

        return renormalized
            ? result.Normalize(NormalizationForm.FormC)
            : result;
    }

    public static int GetTextPermutationsWithoutDiacriticsCount(NormalizedText normalizedText)
    {
        var count = GetDiacriticsCount(normalizedText);
        return (1 << count) - 1; // 2 ^ count - 1 (exclude the case when all diacritics are kept)
    }

    public static ImmutableArray<string> GetTextPermutationsWithoutDiacritics(
        NormalizedText normalizedText,
        bool renormalized)
    {
        var count = GetTextPermutationsWithoutDiacriticsCount(normalizedText);

        var builder = ImmutableArray.CreateBuilder<string>(count);
        builder.Count = count;
        Assert.True(TryGetTextPermutationsWithoutDiacritics(normalizedText, renormalized, ImmutableCollectionsMarshal.AsMemory(builder).Span, out var itemsWritten));
        Assert.True(itemsWritten == builder.Count);
        return builder.MoveToImmutable();
    }

    public static bool TryGetTextPermutationsWithoutDiacritics(
        NormalizedText normalizedText,
        bool renormalized,
        Span<string> destination,
        out int itemsWritten)
    {
        var accentMarkIndexes = (Span<int>)stackalloc int[32];

        int index = 0;
        var accentIndex = 0;

        foreach (var rune in normalizedText.Text.EnumerateRunes())
        {
            if (Rune.GetUnicodeCategory(rune) == UnicodeCategory.NonSpacingMark)
                accentMarkIndexes[accentIndex++] = index;

            ++index;
        }

        accentMarkIndexes = accentMarkIndexes[..accentIndex];

        var accentCount = accentIndex;
        var accentBitMask = 0u;

        var builder = new DefaultInterpolatedStringHandler(
            normalizedText.Text.Length, 0, null, stackalloc char[128]);

        var runeBuffer = (Span<char>)stackalloc char[2];

        if (accentCount > 31)
            throw new ArgumentException("Too many diacritics. The maximum supported number of diacritics is 31.", nameof(normalizedText));

        var permutationIndex = 0;

        while (accentBitMask < (1u << accentCount) - 1)
        {
            index = 0;
            accentIndex = 0;

            foreach (var rune in normalizedText.Text.EnumerateRunes())
            {
                bool contains;

#if NETCOREAPP3_0_OR_GREATER
                contains = accentMarkIndexes.Contains(index);
#else
                contains = accentMarkIndexes.IndexOf(index) >= 0;
#endif

                if (contains)
                {
                    if ((accentBitMask & (1u << accentIndex)) == 0)
                    {
                        ++accentIndex;
                        ++index;
                        continue;
                    }

                    ++accentIndex;
                }

                rune.EncodeToUtf16(runeBuffer);
                builder.AppendFormatted(runeBuffer[..rune.Utf16SequenceLength]);

                ++index;
            }

            var text = builder.ToStringAndClear();

            if (permutationIndex >= destination.Length)
            {
                itemsWritten = permutationIndex;
                return false;
            }

            destination[permutationIndex] = renormalized
                ? text.Normalize(NormalizationForm.FormC)
                : text;

            ++accentBitMask;
            ++permutationIndex;
        }

        itemsWritten = permutationIndex;
        return true;
    }

    public readonly record struct NormalizedText
    {
        private readonly string? _normalizedText;

        public NormalizedText(string text)
        {
            _normalizedText = text.Normalize(NormalizationForm.FormD);
        }

        public NormalizedText(ReadOnlySpan<char> text)
        {
#if NET10_0_OR_GREATER
            var length = text.GetNormalizedLength(NormalizationForm.FormD);

            _normalizedText = string.Create(length, text, (destination, source) =>
            {
                Assert.True(source.TryNormalize(destination, out var charsWritten, NormalizationForm.FormD));
                Assert.True(charsWritten == length);
            });
#else
            _normalizedText = text.ToString().Normalize(NormalizationForm.FormD);
#endif
        }

        public string Text =>
            _normalizedText ?? "";
    }
}
