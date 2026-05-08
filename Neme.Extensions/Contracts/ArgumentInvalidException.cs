namespace Neme.Extensions.Contracts;

public sealed class ArgumentInvalidException : ArgumentException2
{
    public ArgumentInvalidException(string? paramName, object? actualValue, string? condition)
        : base(paramName, actualValue, $"Value must satisfy condition `{condition}`.")
    {
        Condition = condition;
    }

    public string? Condition { get; }
}
