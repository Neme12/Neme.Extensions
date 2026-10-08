namespace Neme.Extensions.Contracts;

public sealed class InvalidValueException : Exception
{
    private readonly string? _additionalMessage;

    public InvalidValueException(string valueSource, object? actualValue, string? message, string? additionalMessage = null)
        : base(message)
    {
        ValueSource = valueSource;
        ActualValue = actualValue;
        _additionalMessage = additionalMessage;
    }

    public string? ValueSource { get; }

    public Optional<object?> ActualValue { get; }

    public override string Message
    {
        get
        {
            var message = base.Message;

            if (_additionalMessage is not null)
                message += $"\n{_additionalMessage}";

            if (ValueSource is not null)
                message += $"\nValue source: '{FormatValue(ValueSource)}'";

            if (ActualValue.TryGetValue(out var actualValue))
                message += $"\nActual value was '{FormatValue(actualValue)}'.";

            return message;
        }
    }


    private static string FormatValue(object? value) =>
        value is null ? "null" : $"'{value}'";
}
