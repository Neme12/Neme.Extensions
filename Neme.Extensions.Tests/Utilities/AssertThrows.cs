namespace Neme.Extensions.Tests.Utilities;

internal static class AssertThrows
{
    public static void NotSupported_ReadOnlyCollection(Action testCode)
    {
        var e = Assert.Throws<NotSupportedException>(testCode);
        Assert.Equal("Collection is read-only.", e.Message);
    }

    public static void Argument_FormatStringNotSupported(Action testCode)
    {
        var e = Assert.Throws<ArgumentException>("format", testCode);
        Assert.StartsWith("Format string is not supported. The parameter must be null.", e.Message, StringComparison.Ordinal);
    }

    public static void Argument_ObjectMustBeOfType(string paramName, string typeName, Action testCode)
    {
        var e = Assert.Throws<ArgumentException>(paramName, testCode);
        Assert.StartsWith($"Object must be of type {typeName}.", e.Message, StringComparison.Ordinal);
    }
}
