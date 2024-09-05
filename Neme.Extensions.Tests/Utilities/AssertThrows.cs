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
        Assert.Equal("Format string is not supported. The parameter must be null.", GetCoreMessage(e));
    }

    public static void Argument_ObjectMustBeOfType(string paramName, string typeName, Action testCode)
    {
        var e = Assert.Throws<ArgumentException>(paramName, testCode);
        Assert.Equal($"Object must be of type {typeName}.", GetCoreMessage(e));
    }

    private static string GetCoreMessage(ArgumentException exception)
    {
        const string testMessage = "<test message>";
        var testException = new ArgumentException(testMessage, exception.ParamName, exception.InnerException);

        var index = testException.Message.IndexOf(testMessage, StringComparison.Ordinal);
        var restLength = testException.Message.Length - testMessage.Length;

        return exception.Message[index..^restLength];
    }
}
