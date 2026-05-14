namespace Neme.Extensions;

public class ArgumentException2 : ArgumentException
{
    public ArgumentException2()
    {
    }

    public ArgumentException2(string? paramName, object? actualValue, string? message)
        : base(message, paramName)
    {
        ActualValue = actualValue;
    }

    public Optional<object?> ActualValue { get; }

    public override string Message
    {
        get
        {
            var message = base.Message;

            if (ActualValue.TryGetValue(out var value))
            {
                return value is null
                    ? $"{message}\nActual value was null."
                    : $"{message}\nActual value was '{value}'.";
            }

            return message;
        }
    }
}
