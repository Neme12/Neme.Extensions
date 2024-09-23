namespace Neme.Extensions.Tests.Utilities;

internal static class AssertPolyfill
{
    public static void Equal(ReadOnlySpan<char> expected, ReadOnlySpan<char> actual)
    {
#if NETCOREAPP2_1_OR_GREATER
        Assert.Equal(expected, actual);
#else
        if (!actual.SequenceEqual(expected))
            Assert.Equal(expected.ToString(), actual.ToString());
#endif
    }
}
