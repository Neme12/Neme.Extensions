using System.Text.Unicode;

namespace System.Globalization;

/// <summary>
/// This class defines behaviors specific to a writing system.
/// A writing system is the collection of scripts and orthographic rules
/// required to represent a language as text.
/// </summary>
public static class StringInfoPolyfill
{
    /// <summary>
    /// Returns the length of the first text element (extended grapheme cluster) that occurs in the input string.
    /// </summary>
    /// <remarks>
    /// A grapheme cluster is a sequence of one or more Unicode code points that should be treated as a single unit.
    /// </remarks>
    /// <param name="str">The input string to analyze.</param>
    /// <returns>The length (in chars) of the substring corresponding to the first text element within <paramref name="str"/>,
    /// or 0 if <paramref name="str"/> is empty.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
    public static int GetNextTextElementLength(string str)
    {
#if NET6_0_OR_GREATER
        return StringInfo.GetNextTextElementLength(str);
#else
        return GetNextTextElementLength(str, 0);
#endif
    }

    /// <summary>
    /// Returns the length of the first text element (extended grapheme cluster) that occurs in the input string
    /// starting at the specified index.
    /// </summary>
    /// <remarks>
    /// A grapheme cluster is a sequence of one or more Unicode code points that should be treated as a single unit.
    /// </remarks>
    /// <param name="str">The input string to analyze.</param>
    /// <param name="index">The char offset in <paramref name="str"/> at which to begin analysis.</param>
    /// <returns>The length (in chars) of the substring corresponding to the first text element within <paramref name="str"/> starting
    /// at index <paramref name="index"/>, or 0 if <paramref name="index"/> corresponds to the end of <paramref name="str"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative or beyond the end of <paramref name="str"/>.</exception>
    public static int GetNextTextElementLength(string str, int index)
    {
#if NET6_0_OR_GREATER
        return StringInfo.GetNextTextElementLength(str);
#else
        if (str is null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.str);
        }
        if ((uint)index > (uint)str.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
        }

        return GetNextTextElementLength(str.AsSpan(index));
#endif
    }

    /// <summary>
    /// Returns the length of the first text element (extended grapheme cluster) that occurs in the input span.
    /// </summary>
    /// <remarks>
    /// A grapheme cluster is a sequence of one or more Unicode code points that should be treated as a single unit.
    /// </remarks>
    /// <param name="str">The input span to analyze.</param>
    /// <returns>The length (in chars) of the substring corresponding to the first text element within <paramref name="str"/>,
    /// or 0 if <paramref name="str"/> is empty.</returns>
    public static int GetNextTextElementLength(ReadOnlySpan<char> str)
    {
#if NET6_0_OR_GREATER
        return StringInfo.GetNextTextElementLength(str);
#else
        return TextSegmentationUtility.GetLengthOfFirstUtf16ExtendedGraphemeCluster(str);
#endif
    }
}
