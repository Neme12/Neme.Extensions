using System.Text;
using System.Text.Unicode;

namespace Neme.Extensions.Text;

internal static class StringInfoExtensions
{
    public static int GetNextTextElementLength(ReadOnlySpan<byte> str) =>
        TextSegmentationUtility.GetLengthOfFirstUtf8ExtendedGraphemeCluster(str);

    public static int GetNextTextElementLength(ReadOnlySpan<Rune> str) =>
        TextSegmentationUtility.GetLengthOfFirstExtendedGraphemeCluster(str);
}
