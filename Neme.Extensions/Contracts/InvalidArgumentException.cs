namespace Neme.Extensions.Contracts;

public sealed class InvalidArgumentException : ArgumentException2
{
    public InvalidArgumentException(string? paramName, object? actualValue, string? condition, string? message = null)
        : base(paramName, actualValue, message ?? $"Value must satisfy condition `{condition}`.")
    {
        Condition = condition;
    }

    public string? Condition { get; }
}
