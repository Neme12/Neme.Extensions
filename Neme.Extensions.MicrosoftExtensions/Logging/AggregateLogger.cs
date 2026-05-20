using Microsoft.Extensions.Logging;
using Neme.Extensions;

namespace Neme.Extensions.MicrosoftExtensions.Logging;

public sealed class AggregateLogger : ILogger
{
    private readonly ImmutableArray<ILogger> _loggers;

    public AggregateLogger(ImmutableArray<ILogger> loggers)
    {
        _loggers = loggers;
    }

    public AggregateLogger(IEnumerable<ILogger> loggers)
    {
        _loggers = loggers.ToImmutableArray();
    }

    public AggregateLogger(params ReadOnlySpan<ILogger> loggers)
    {
        _loggers = loggers.ToImmutableArray();
    }

#if NET8_0_OR_GREATER
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
#else
    public IDisposable BeginScope<TState>(TState state)
#endif
    {
        var scopes = ImmutableArray.CreateBuilder<IDisposable?>(_loggers.Length);

        foreach (var logger in _loggers)
            scopes.Add(logger.BeginScope(state));

        return new AggregateScope(scopes.MoveToImmutable());

    }
    public bool IsEnabled(LogLevel logLevel) =>
        _loggers.Any(logger => logger.IsEnabled(logLevel));

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        foreach (var logger in _loggers)
            logger.Log(logLevel, eventId, state, exception, formatter);
    }
}
