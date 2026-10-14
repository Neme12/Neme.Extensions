namespace Neme.Extensions.Contracts;

public sealed class AssertionFailedException : Exception
{
    public AssertionFailedException(string? message)
        : base(message)
    {
    }

    public AssertionFailedException(string? message, object? value)
        : base(message)
    {
        Value = value;
    }

    public object? Value { get; }
}
