using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using System.Runtime.Versioning;

namespace Neme.Extensions.MicrosoftExtensions.Caching;

/// <summary>
/// Background hosted service that periodically cleans up expired file cache entries.
/// </summary>
[SupportedOSPlatform("windows6.0.6000")]
internal sealed partial class FileCacheCleanupService : IHostedService, IDisposable
{
    private readonly FileCache _fileCache;
    private readonly ILogger<FileCacheCleanupService> _logger;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _cleanupTask;

    public FileCacheCleanupService(
        FileCache fileCache,
        ILogger<FileCacheCleanupService> logger)
    {
        _fileCache = fileCache;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource is not null)
            throw new InvalidOperationException("Service is already running.");

        Log.FileCacheCleanupStarting(_logger, _fileCache.CacheDirectory, _fileCache.CleanupInterval);

        _cancellationTokenSource = new CancellationTokenSource();
        _cleanupTask = RunCleanupLoopAsync(_cancellationTokenSource.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource is null || _cleanupTask is null)
            throw new InvalidOperationException("Service is not running.");

        Log.FileCacheCleanupStopping(_logger);

        _cancellationTokenSource.Cancel();

        try
        {
            await _cleanupTask;
        }
        catch (OperationCanceledException)
        {
        }

        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
        _cleanupTask = null;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        _cleanupTask = null;
    }

    private async Task RunCleanupLoopAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(_fileCache.CleanupInterval.ToTimeSpan());

        while (await timer.WaitForNextTickAsync(cancellationToken))
            await _fileCache.CleanupExpiredFilesAsync();
    }

    private static partial class Log
    {
        [LoggerMessage(EventId = EventIds.FileCacheCleanupService.FileCacheCleanupStarting, EventName =  EventIds.FileCacheCleanupService.FileCacheCleanupStartingName, Level = LogLevel.Information, Message = "File cache cleanup starting. Directory: {Directory}, Cleanup interval: {Interval}")]
        public static partial void FileCacheCleanupStarting(ILogger logger, string directory, Duration interval);

        [LoggerMessage(EventId = EventIds.FileCacheCleanupService.FileCacheCleanupStopping, EventName = EventIds.FileCacheCleanupService.FileCacheCleanupStoppingName, Level = LogLevel.Information, Message = "File cache cleanup stopping")]
        public static partial void FileCacheCleanupStopping(ILogger logger);
    }
}
