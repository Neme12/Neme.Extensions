using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neme.Extensions.FileSystem;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using Neme.Extensions.Threading;
using NodaTime;
using System.Collections.Concurrent;
using System.Runtime.Versioning;
using System.Text.Json;

namespace Neme.Extensions.MicrosoftExtensions.Caching;

/// <summary>
/// A file-based cache service that stores cached data on disk with time-based expiration.
/// Metadata is stored in NTFS Alternate Data Streams for efficiency.
/// Service <see cref="FileCacheCleanupService"/> cleans up expired entries in the background.
/// </summary>
[SupportedOSPlatform("windows6.0.6000")]
public sealed partial class FileCache : IFileCache, IDisposable
{
    private readonly FileCacheOptions _options;
    private readonly ILogger<FileCache> _logger;
    private readonly IClock _clock;
    private readonly string _cacheDirectory;
    private readonly SemaphoreSlim _globalLock = new(1, 1);
    private readonly ConcurrentDictionary<string, (SemaphoreSlim Lock, Instant LastUsed)> _locks = new();

    internal Duration CleanupInterval =>
        _options.ExpirationScanFrequency;

    internal string CacheDirectory =>
        _options.CacheDirectory;

    private const string MetadataStreamName = "metadata";

    private static readonly FsFileOptions s_fileReadOptions = new()
    {
        Mode = FileMode.Open,
        Access = FsFileAccess.Read,
        Share = FileShare.Read,
        Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
    };

    private static readonly FsFileOptions s_fileWriteOptions = new()
    {
        Mode = FileMode.Create,
        Access = FsFileAccess.ReadWrite | FsFileAccess.Delete,
        Share = FileShare.ReadWrite | FileShare.Delete,
        Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
    };

    public FileCache(
        IOptions<FileCacheOptions> optionsAccessor,
        ILogger<FileCache> logger,
        IClock clock)
    {
        _options = optionsAccessor.Value;
        _logger = logger;
        _clock = clock;
        _cacheDirectory = optionsAccessor.Value.CacheDirectory;

        Directory.CreateDirectory(_cacheDirectory);
    }

    [return: OwnershipTransfer]
    public async Task<FsFile?> GetAsync(
        string key,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        await WaitForGlobalLockAsync(cancellationToken);

        using (await GetLock(key).WaitScopeAsync(cancellationToken))
        {
            var fileOptions = options.FileOptions ?? _options.DefaultFileOptions;

            var result = await GetCoreAsync(key, fileOptions, isGetOrCreate: false, getFileHandle: true, cancellationToken);
            return result?.FsFile;
        }
    }

    public async Task<string?> GetPathAsync(
        string key,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        await WaitForGlobalLockAsync(cancellationToken);

        using (await GetLock(key).WaitScopeAsync(cancellationToken))
        {
            var fileOptions = options.FileOptions ?? _options.DefaultFileOptions;

            var result = await GetCoreAsync(key, fileOptions, isGetOrCreate: false, getFileHandle: false, cancellationToken);
            return result?.FilePath;
        }
    }

    public async Task SetAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> writeData,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(writeData);

        await WaitForGlobalLockAsync(cancellationToken);

        using (await GetLock(key).WaitScopeAsync(cancellationToken))
        {
            var fileOptions = options.FileOptions ?? _options.DefaultFileOptions;
            var fileAttributes = options.FileAttributes ?? _options.DefaultFileAttributes;

            await SetCoreAsync(key, writeData, options.Expiration, options.SlidingExpiration, fileOptions, fileAttributes, cancellationToken);
        }
    }

    [return: OwnershipTransfer]
    public async Task<FsFile> GetOrCreateAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(factory);

        await WaitForGlobalLockAsync(cancellationToken);

        using (await GetLock(key).WaitScopeAsync(cancellationToken))
        {
            var fileOptions = options.FileOptions ?? _options.DefaultFileOptions;
            var fileAttributes = options.FileAttributes ?? _options.DefaultFileAttributes;

            var cached = await GetCoreAsync(key, fileOptions, isGetOrCreate: true, getFileHandle: true, cancellationToken);
            if (cached is not null)
                return cached.Value.FsFile;

            await SetCoreAsync(key, factory, options.Expiration, options.SlidingExpiration, fileOptions, fileAttributes, cancellationToken);
            return FileIO.Open(GetFilePath(key), s_fileReadOptions with { Options = fileOptions });
        }
    }

    public async Task<string> GetOrCreatePathAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(factory);

        await WaitForGlobalLockAsync(cancellationToken);

        using (await GetLock(key).WaitScopeAsync(cancellationToken))
        {
            var fileOptions = options.FileOptions ?? _options.DefaultFileOptions;
            var fileAttributes = options.FileAttributes ?? _options.DefaultFileAttributes;

            var cached = await GetCoreAsync(key, fileOptions, isGetOrCreate: true, getFileHandle: false, cancellationToken);
            if (cached is not null)
                return cached.Value.FilePath;

            await SetCoreAsync(key, factory, options.Expiration, options.SlidingExpiration, fileOptions, fileAttributes, cancellationToken);
            return GetFilePath(key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        var filePath = GetFilePath(key);

        await WaitForGlobalLockAsync(cancellationToken);

        using (await GetLock(key).WaitScopeAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            DeleteFile(filePath);
            Log.RemovedCacheKey(_logger, key);
        }
    }

    private SemaphoreSlim GetLock(string key)
    {
        return _locks.AddOrUpdate(
            key,
            static (_, clock) => (new SemaphoreSlim(1, 1), clock.GetCurrentInstant()),
            static (_, existing, clock) => (existing.Lock, clock.GetCurrentInstant()),
            _clock).Lock;
    }

    private async Task WaitForGlobalLockAsync(CancellationToken cancellationToken)
    {
        // Check if the global lock is currently held without acquiring it
        if (_globalLock.CurrentCount == 0)
        {
            // Lock is currently held, wait for it to be released
            await _globalLock.WaitAsync(cancellationToken);
            _globalLock.Release();
        }
    }

    [return: OwnershipTransfer]
    private async Task<FilePathOrFsFile?> GetCoreAsync(
        string key,
        FileOptions options,
        bool isGetOrCreate,
        bool getFileHandle,
        CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(key);

        var metadata = await ReadMetadataAsync(filePath, cancellationToken);
        if (metadata is null)
            return null;

        if (metadata.Value.SlidingExpiration.HasValue)
            metadata = await RefreshSlidingExpirationAsync(filePath, metadata.Value, cancellationToken);

        var isExpired = _clock.GetCurrentInstant() > metadata.Value.ExpiresAt;
        if (isExpired && !isGetOrCreate)
        {
            DeleteFile(filePath);
            return null;
        }

        return getFileHandle
            ? FilePathOrFsFile.FromFsFile(FileIO.Open(filePath, s_fileReadOptions with { Options = options }))
            : FilePathOrFsFile.FromPath(filePath);
    }

    [return: OwnershipTransfer]
    private async Task SetCoreAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> writeData,
        Duration? expiration,
        Duration? slidingExpiration,
        FileOptions options,
        FileAttributes attributes,
        CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(key);

        var expirationDuration = slidingExpiration ?? expiration ?? _options.DefaultExpiration;
        var expiresAt = _clock.GetCurrentInstant().Plus(expirationDuration);

        var metadata = new FileCacheMetadata 
        { 
            ExpiresAt = expiresAt, 
            SlidingExpiration = slidingExpiration 
        };

        using (var file = OwnedOrBorrowed.Create(PartialFileStream.Create(filePath, s_fileWriteOptions with { Options = options, Attributes = attributes }, createDirectory: true)))
        {
            await writeData(file.Value.FileStream, cancellationToken);
            await file.Value.FileStream.FlushAsync(cancellationToken);

            await WriteMetadataAsync(file.Value.CurrentPath, metadata, cancellationToken);

            file.Value.FinalizeFile();

            Log.CachedKey(_logger, key, expiresAt);
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken)
    {
        using (await _globalLock.WaitScopeAsync(cancellationToken))
        {
            foreach (var file in Directory.EnumerateFiles(_cacheDirectory))
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
#if NET7_0_OR_GREATER
                    Log.FailedToDeleteCacheFile(_logger, e, file);
#else
                    _logger.LogWarning(new EventId(EventIds.FileCache.FailedToDeleteCacheFile, EventIds.FileCache.FailedToDeleteCacheFileName), e, "Failed to delete cache file: {File}. Error: {Exception}", file, e);
#endif
                }
            }

            foreach (var directory in Directory.EnumerateDirectories(_cacheDirectory))
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    Directory.Delete(directory, recursive: true);
                }
                catch (Exception e)
                {
#if NET7_0_OR_GREATER
                    Log.FailedToDeleteCacheFile(_logger, e, directory);
#else
                    _logger.LogWarning(new EventId(EventIds.FileCache.FailedToDeleteCacheFile, EventIds.FileCache.FailedToDeleteCacheFileName), e, "Failed to delete cache file: {File}. Error: {Exception}", directory, e);
#endif
                }
            }

            Log.CacheCleared(_logger);
        }
    }

    private string GetFilePath(string key)
    {
        return Path.Join(_cacheDirectory, key);
    }

    private static async Task<FileCacheMetadata?> ReadMetadataAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        var metadataPath = filePath + ":" + MetadataStreamName;

        FsFile file;

        try
        {
            file = FileIO.Open(metadataPath, s_fileReadOptions);
        }
        catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
        {
            return null;
        }

        using (file)
        await using (var fileStream = file.CreateFileStream())
        {
            return await JsonSerializer.DeserializeAsync(fileStream, FileCacheJsonSerializerContext.Default.FileCacheMetadata, cancellationToken);
        }
    }

    private static async Task WriteMetadataAsync(
        string filePath,
        FileCacheMetadata metadata,
        CancellationToken cancellationToken)
    {
        var metadataPath = filePath + ":" + MetadataStreamName;

        using (var file = FileIO.Open(metadataPath, s_fileWriteOptions))
        await using (var fileStream = file.CreateFileStream())
        {
            await JsonSerializer.SerializeAsync(fileStream, metadata, FileCacheJsonSerializerContext.Default.FileCacheMetadata, cancellationToken);
        }
    }

    private static void DeleteFile(string filePath)
    {
        File.Delete(filePath);
    }

    private async Task<FileCacheMetadata> RefreshSlidingExpirationAsync(
        string filePath,
        FileCacheMetadata metadata,
        CancellationToken cancellationToken)
    {
        if (metadata.SlidingExpiration is null)
            return metadata;

        var newExpiration = _clock.GetCurrentInstant()
            .Plus(metadata.SlidingExpiration.Value);

        var updatedMetadata = metadata with { ExpiresAt = newExpiration };
        await WriteMetadataAsync(filePath, updatedMetadata, cancellationToken);

        Log.RefreshedSlidingExpiration(_logger, filePath, newExpiration);
        return updatedMetadata;
    }

    internal async Task CleanupExpiredFilesAsync()
    {
        using (await _globalLock.WaitScopeAsync(CancellationToken.None))
        {
            var deletedCount = 0;

            foreach (var (filePath, isDirectory) in EnumerateAllFiles(_cacheDirectory))
            {
                try
                {
                    if (isDirectory)
                    {
                        Directory.DeleteIfEmpty(filePath);
                    }
                    else
                    {
                        var metadata = await ReadMetadataAsync(filePath, CancellationToken.None);
                        if (metadata is null)
                            Log.MetadataMissingForFile(_logger, filePath);

                        var isExpired = metadata.HasValue && _clock.GetCurrentInstant() > metadata.Value.ExpiresAt;
                        if (metadata is null || isExpired)
                        {
                            DeleteFile(filePath);
                            ++deletedCount;
                        }
                    }
                }
                catch (Exception e)
                {
#if NET7_0_OR_GREATER
                    Log.FailedToCleanupCacheFile(_logger, e, filePath);
#else
                    _logger.LogWarning(new EventId(EventIds.FileCache.FailedToCleanupCacheFile, EventIds.FileCache.FailedToCleanupCacheFileName), e, "Failed to cleanup cache file: {File}. Error: {Exception}", filePath, e);
#endif
                }
            }

            Log.CleanedUpExpiredEntries(_logger, deletedCount);

            // Cleanup unused locks
            CleanupUnusedLocks();
        }
    }

    private void CleanupUnusedLocks()
    {
        var cutoff = _clock.GetCurrentInstant() - Duration.FromMinutes(30);
        var removedCount = 0;

        foreach (var (key, value) in _locks)
        {
            // Only remove if:
            // 1. Not used recently
            // 2. Lock is available (CurrentCount == 1 means not held)
            if (value.LastUsed < cutoff && value.Lock.CurrentCount == 1)
            {
                if (_locks.TryRemove(key, out var removed))
                {
                    removed.Lock.Dispose();
                    ++removedCount;
                }
            }
        }

        if (removedCount > 0)
            Log.CleanedUpUnusedLocks(_logger, removedCount);
    }

    private static IEnumerable<(string filePath, bool isDirectory)> EnumerateAllFiles(string directory)
    {
        foreach (var file in Directory.EnumerateFiles(directory))
            yield return (file, false);

        foreach (var subdirectory in Directory.EnumerateDirectories(directory))
        {
            foreach (var (file, isDirectory) in EnumerateAllFiles(subdirectory))
                yield return (file, isDirectory);

            yield return (subdirectory, true);
        }
    }

    public void Dispose()
    {
        _globalLock.Dispose();

        foreach (var entry in _locks.Values)
        {
            entry.Lock.Dispose();
        }

        _locks.Clear();
    }

    private static partial class Log
    {
        [LoggerMessage(EventId = EventIds.FileCache.CachedKey, EventName = EventIds.FileCache.CachedKeyName, Level = LogLevel.Debug, Message = "Cached key: {Key}, expires at: {ExpiresAt}")]
        public static partial void CachedKey(ILogger logger, string key, Instant expiresAt);

        [LoggerMessage(EventId = EventIds.FileCache.RemovedCacheKey, EventName = EventIds.FileCache.RemovedCacheKeyName, Level = LogLevel.Debug, Message = "Removed cache key: {Key}")]
        public static partial void RemovedCacheKey(ILogger logger, string key);

#if NET7_0_OR_GREATER
#pragma warning disable SYSLIB1013
        [LoggerMessage(EventId = EventIds.FileCache.FailedToDeleteCacheFile, EventName = EventIds.FileCache.FailedToDeleteCacheFileName, Level = LogLevel.Warning, Message = "Failed to delete cache file: {File}. Error: {Exception}")]
        public static partial void FailedToDeleteCacheFile(ILogger logger, Exception exception, string file);
#pragma warning restore SYSLIB1013
#endif

        [LoggerMessage(EventId = EventIds.FileCache.CacheCleared, EventName = EventIds.FileCache.CacheClearedName, Level = LogLevel.Information, Message = "Cache cleared")]
        public static partial void CacheCleared(ILogger logger);

#if NET7_0_OR_GREATER
#pragma warning disable SYSLIB1013
        [LoggerMessage(EventId = EventIds.FileCache.FailedToCleanupCacheFile, EventName = EventIds.FileCache.FailedToCleanupCacheFileName, Level = LogLevel.Warning, Message = "Failed to cleanup cache file: {File}. Error: {Exception}")]
        public static partial void FailedToCleanupCacheFile(ILogger logger, Exception exception, string file);
#pragma warning restore SYSLIB1013
#endif

        [LoggerMessage(EventId = EventIds.FileCache.CleanedUpExpiredEntries, EventName = EventIds.FileCache.CleanedUpExpiredEntriesName, Level = LogLevel.Information, Message = "Cleaned up {Count} expired cache entries")]
        public static partial void CleanedUpExpiredEntries(ILogger logger, int count);

        [LoggerMessage(EventId = EventIds.FileCache.CleanedUpUnusedLocks, EventName = EventIds.FileCache.CleanedUpUnusedLocksName, Level = LogLevel.Information, Message = "Cleaned up {Count} unused cache locks")]
        public static partial void CleanedUpUnusedLocks(ILogger logger, int count);

        [LoggerMessage(EventId = EventIds.FileCache.RefreshedSlidingExpiration, EventName = EventIds.FileCache.RefreshedSlidingExpirationName, Level = LogLevel.Information, Message = "Refreshed sliding expiration for {File}, new expiration: {ExpiresAt}")]
        public static partial void RefreshedSlidingExpiration(ILogger logger, string file, Instant expiresAt);

        [LoggerMessage(EventId = EventIds.FileCache.MetadataMissingForFile, EventName = EventIds.FileCache.MetadataMissingForFileName, Level = LogLevel.Warning, Message = "Metadata missing for file {File}")]
        public static partial void MetadataMissingForFile(ILogger logger, string file);
    }

    private readonly record struct FilePathOrFsFile
    {
        [Borrowed]
        private readonly object _object;

        private FilePathOrFsFile(string filePath)
        {
            _object = filePath;
        }

        private FilePathOrFsFile([Borrow] FsFile file)
        {
            _object = file;
        }

        public bool IsPath =>
            _object is string;

        public bool IsFsFile =>
            _object is FsFile;

        public string FilePath =>
            _object as string ?? throw new InvalidOperationException("Not a file path.");

        [Borrowed]
        public FsFile FsFile =>
            _object as FsFile ?? throw new InvalidOperationException("Not a FsFile.");

        public static FilePathOrFsFile FromPath(string filePath) =>
            new(filePath);

        public static FilePathOrFsFile FromFsFile([Borrow] FsFile file) =>
            new(file);
    }
}
