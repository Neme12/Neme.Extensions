namespace Neme.Extensions.Tests.Utilities;

internal static class AssertThrows
{
    public static void NotSupported_ReadOnlyCollection(Action testCode)
    {
        var exception = Assert.Throws<NotSupportedException>(testCode);
        Assert.Equal("Collection is read-only.", exception.Message);
    }
}
