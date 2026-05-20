using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Neme.Extensions.Contracts;
using System.Collections.Concurrent;

namespace Neme.Extensions.AspNetCore.Logging;

public sealed partial class ResponseLogger<T> : ILogger<T>, IAsyncDisposable
{
    private readonly HttpResponse _response;
    private readonly LogLevel _logLevel;
    private readonly ILogger<ResponseLogger<T>> _logger;
    private readonly ConcurrentQueue<Task>_pendingTasks = new();
    private readonly SemaphoreSlim _signal = new(0);
    private Task? _task = null;
    private bool _isCompletionRequested = false;
    private bool _disposed = false;

    public ResponseLogger(
        HttpResponse response,
        LogLevel logLevel,
        ILogger<ResponseLogger<T>> logger)
    {
        _response = response;
        _logLevel = logLevel;
        _logger = logger;
    }

#if NET8_0_OR_GREATER
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
#else
    public IDisposable BeginScope<TState>(TState state)
#endif
    {
        throw new NotSupportedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        Require.NotDisposed(_disposed, this);
        return logLevel != LogLevel.None && logLevel >= _logLevel;
    }

    public void Start(CancellationToken cancellationToken)
    {
        Require.NotDisposed(_disposed, this);

        if (_task is not null)
            throw new InvalidOperationException("Logger is already running.");

        _task = Task.Run(() => RunAsync(cancellationToken), cancellationToken)
            .ContinueWith(_ =>
            {
                _task = null;
                _isCompletionRequested = false;
            }, cancellationToken);
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested && !_isCompletionRequested)
            {
                await _signal.WaitAsync(cancellationToken).ConfigureAwait(false);

                if (_pendingTasks.TryDequeue(out var task))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await task.ConfigureAwait(false);
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
            Assert.True(_isCompletionRequested);

            // Finish processing any remaining tasks after completion is requested

            while (_pendingTasks.TryDequeue(out var task))
            {
                cancellationToken.ThrowIfCancellationRequested();

                await task.ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async Task CompleteAsync()
    {
        Require.NotDisposed(_disposed, this);

        if (_task is null)
            throw new InvalidOperationException("Logger is not running.");

        if (_isCompletionRequested)
            throw new InvalidOperationException("Completion has already been requested.");

        _isCompletionRequested = true;
        _signal.Release();
        await _task.ConfigureAwait(false);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Require.NotDisposed(_disposed, this);

        if (_task is null)
            throw new InvalidOperationException("Logger is not running.");
 
        if (_isCompletionRequested)
            throw new InvalidOperationException("Cannot log after completion has been requested.");

        var message = formatter(state, exception) + "\n";
        _pendingTasks.Enqueue(_response.WriteAsync(message));
        _signal.Release();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_task is not null)
        {
            if (_isCompletionRequested)
            {
                Logging.CompleteWasNotAwaited(_logger);
            }
            else
            {
                Logging.CompleteWasNotCalled(_logger);

                _isCompletionRequested = true;
                _signal.Release();
            }

            try
            {
                await _task.ConfigureAwait(false);
            }
            catch (Exception e)
            {
#if NET7_0_OR_GREATER
                Logging.ErrorDuringDisposal(_logger, e);
#else
                _logger.LogError(new EventId(EventIds.ResponseLogger.ErrorDuringDisposal, EventIds.ResponseLogger.ErrorDuringDisposalName), e, "Error during disposal: {Exception}", e);
#endif
            }
        }
    }

    private static partial class Logging
    {
#if NET7_0_OR_GREATER
#pragma warning disable SYSLIB1013
        [LoggerMessage(EventId = EventIds.ResponseLogger.ErrorDuringDisposal, EventName = EventIds.ResponseLogger.ErrorDuringDisposalName, Level = LogLevel.Error, Message = "Error during disposal: {Exception}")]
        public static partial void ErrorDuringDisposal(ILogger logger, Exception exception);
#pragma warning restore SYSLIB1013
#endif

        [LoggerMessage(EventId = EventIds.ResponseLogger.CompleteWasNotCalled, EventName = EventIds.ResponseLogger.CompleteWasNotCalledName, Level = LogLevel.Warning, Message = "CompleteAsync() was not called.")]
        public static partial void CompleteWasNotCalled(ILogger logger);

        [LoggerMessage(EventId = EventIds.ResponseLogger.CompleteWasNotAwaited, EventName = EventIds.ResponseLogger.CompleteWasNotAwaitedName, Level = LogLevel.Warning, Message = "CompleteAsync() was not awaited.")]
        public static partial void CompleteWasNotAwaited(ILogger logger);
    }
}
