using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neme.Extensions.FileSystem;
using Neme.Extensions.IO;
using Neme.Extensions.MicrosoftExtensions.InternalUtilities;
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
/// <remarks>
/// <para><strong>Thread Safety:</strong> This class is fully thread-safe. All operations are protected by per-key locks,
/// allowing concurrent access to different keys while serializing access to the same key.</para>
/// <para><strong>Default Options:</strong> When passing <see cref="FileCacheEntryOptions.Default"/> or default values,
/// the cache uses defaults from <see cref="FileCacheOptions"/> (configured during service registration).</para>
/// <para><strong>Expiration Behavior:</strong> Expired entries are removed during Get operations (returning null),
/// but may persist on disk until the background cleanup service runs. Use <see cref="Clear"/> or <see cref="ClearAsync"/>
/// to force immediate removal of all entries.</para>
/// <para><strong>Sliding Expiration:</strong> When an entry has sliding expiration, each successful Get operation
/// automatically extends its lifetime by the configured duration.</para>
/// </remarks>
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

    /// <summary>
    /// Retrieves a cached file by key and returns a file handle for reading.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="options">Entry-specific options. Use <see cref="FileCacheEntryOptions.Default"/> to apply global defaults.
    /// Only <see cref="FileCacheEntryOptions.FileOptions"/> is used; other properties are ignored during retrieval.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="FsFile"/> handle with ownership transferred to the caller (you must dispose it),
    /// or <c>null</c> if the key doesn't exist or the entry has expired.</returns>
    /// <remarks>
    /// <para><strong>Ownership:</strong> The caller owns the returned <see cref="FsFile"/> and must dispose it.</para>
    /// <para><strong>Expired Entries:</strong> If the entry has expired, it is deleted from disk and <c>null</c> is returned.</para>
    /// <para><strong>Sliding Expiration:</strong> If the entry uses sliding expiration, this call automatically extends its lifetime.</para>
    /// <para><strong>vs GetPath:</strong> Use this method when you need a file stream. Use <see cref="GetPath"/> when you only need
    /// the file path (avoids opening a handle).</para>
    /// </remarks>
    [return: OwnershipTransfer]
    public FsFile? Get(
        string key,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        WaitForGlobalLockAsync<IAsyncState.Sync>(cancellationToken).GetAwaiter().GetResult();

        using (GetLock(key).WaitScope(cancellationToken))
        {
            var fileOptions = options.FileOptions ?? _options.DefaultSyncFileOptions;

            var result = GetCoreAsync<IAsyncState.Sync>(key, fileOptions, isGetOrCreate: false, getFileHandle: true, cancellationToken).GetAwaiter().GetResult();
            return result?.FsFile;
        }
    }

    /// <summary>
    /// Asynchronously retrieves a cached file by key and returns a file handle for reading.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="options">Entry-specific options. Use <see cref="FileCacheEntryOptions.Default"/> to apply global defaults.
    /// Only <see cref="FileCacheEntryOptions.FileOptions"/> is used; other properties are ignored during retrieval.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="FsFile"/> handle with ownership transferred to the caller (you must dispose it),
    /// or <c>null</c> if the key doesn't exist or the entry has expired.</returns>
    /// <remarks>
    /// <para><strong>Ownership:</strong> The caller owns the returned <see cref="FsFile"/> and must dispose it.</para>
    /// <para><strong>Expired Entries:</strong> If the entry has expired, it is deleted from disk and <c>null</c> is returned.</para>
    /// <para><strong>Sliding Expiration:</strong> If the entry uses sliding expiration, this call automatically extends its lifetime.</para>
    /// <para><strong>vs GetPathAsync:</strong> Use this method when you need a file stream. Use <see cref="GetPathAsync"/> when you only need
    /// the file path (avoids opening a handle).</para>
    /// </remarks>
    [return: OwnershipTransfer]
    public async Task<FsFile?> GetAsync(
        string key,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        await WaitForGlobalLockAsync<IAsyncState.Async>(cancellationToken);

        using (await GetLock(key).WaitScopeAsync(cancellationToken))
        {
            var fileOptions = options.FileOptions ?? _options.DefaultAsyncFileOptions;

            var result = await GetCoreAsync<IAsyncState.Async>(key, fileOptions, isGetOrCreate: false, getFileHandle: true, cancellationToken);
            return result?.FsFile;
        }
    }

    /// <summary>
    /// Retrieves the file path of a cached entry by key without opening a file handle.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="options">Entry-specific options. Currently only used for consistency with other methods;
    /// since no file handle is opened, <see cref="FileCacheEntryOptions.FileOptions"/> has no effect.
    /// Metadata is always read using internal default options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The absolute file path to the cached file, or <c>null</c> if the key doesn't exist or the entry has expired.</returns>
    /// <remarks>
    /// <para><strong>Expired Entries:</strong> If the entry has expired, it is deleted from disk and <c>null</c> is returned.</para>
    /// <para><strong>Sliding Expiration:</strong> If the entry uses sliding expiration, this call automatically extends its lifetime.</para>
    /// <para><strong>vs Get:</strong> Use this method when you only need the file path and plan to open it yourself.
    /// This avoids allocating a file handle unnecessarily.</para>
    /// <para><strong>Warning:</strong> The returned path is valid at the time of the call, but the file could be deleted
    /// by cleanup operations or expiration before you access it. Consider using <see cref="Get"/> for guaranteed access.</para>
    /// </remarks>
    public string? GetPath(
        string key,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        WaitForGlobalLockAsync<IAsyncState.Sync>(cancellationToken).GetAwaiter().GetResult();

        using (GetLock(key).WaitScope(cancellationToken))
        {
            var fileOptions = options.FileOptions ?? _options.DefaultSyncFileOptions;

            var result = GetCoreAsync<IAsyncState.Sync>(key, fileOptions, isGetOrCreate: false, getFileHandle: false, cancellationToken).GetAwaiter().GetResult();
            return result?.FilePath;
        }
    }

    /// <summary>
    /// Asynchronously retrieves the file path of a cached entry by key without opening a file handle.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="options">Entry-specific options. Currently only used for consistency with other methods;
    /// since no file handle is opened, <see cref="FileCacheEntryOptions.FileOptions"/> has no effect.
    /// Metadata is always read using internal default options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The absolute file path to the cached file, or <c>null</c> if the key doesn't exist or the entry has expired.</returns>
    /// <remarks>
    /// <para><strong>Expired Entries:</strong> If the entry has expired, it is deleted from disk and <c>null</c> is returned.</para>
    /// <para><strong>Sliding Expiration:</strong> If the entry uses sliding expiration, this call automatically extends its lifetime.</para>
    /// <para><strong>vs GetAsync:</strong> Use this method when you only need the file path and plan to open it yourself.
    /// This avoids allocating a file handle unnecessarily.</para>
    /// <para><strong>Warning:</strong> The returned path is valid at the time of the call, but the file could be deleted
    /// by cleanup operations or expiration before you access it. Consider using <see cref="GetAsync"/> for guaranteed access.</para>
    /// </remarks>
    public async Task<string?> GetPathAsync(
        string key,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        await WaitForGlobalLockAsync<IAsyncState.Async>(cancellationToken);

        using (await GetLock(key).WaitScopeAsync(cancellationToken))
        {
            var fileOptions = options.FileOptions ?? _options.DefaultAsyncFileOptions;

            var result = await GetCoreAsync<IAsyncState.Async>(key, fileOptions, isGetOrCreate: false, getFileHandle: false, cancellationToken);
            return result?.FilePath;
        }
    }

    /// <summary>
    /// Stores or overwrites a cache entry with the specified key.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="writeData">A callback that writes data to the cache file stream. The stream is borrowed and must not be disposed by the callback.</param>
    /// <param name="options">Entry-specific options including expiration, file attributes, and file options.
    /// Use <see cref="FileCacheEntryOptions.Default"/> to apply global defaults from <see cref="FileCacheOptions"/>.
    /// If <see cref="FileCacheEntryOptions.Expiration"/> is null, uses <see cref="FileCacheOptions.DefaultExpiration"/>.
    /// If <see cref="FileCacheEntryOptions.IsSlidingExpiration"/> is null, uses <see cref="FileCacheOptions.IsDefaultSlidingExpiration"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// <para><strong>Overwrite Behavior:</strong> If an entry with the same key already exists, it is completely replaced.
    /// The old file is overwritten atomically.</para>
    /// <para><strong>Expiration:</strong> The expiration timer starts from the moment this method completes successfully.</para>
    /// <para><strong>Atomicity:</strong> Writes are performed to a temporary file and then finalized, ensuring that
    /// concurrent readers never see partially written data.</para>
    /// </remarks>
    public void Set(
        string key,
        [Borrow] Action<Stream, CancellationToken> writeData,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(writeData);

        WaitForGlobalLockAsync<IAsyncState.Sync>(cancellationToken).GetAwaiter().GetResult();

        using (GetLock(key).WaitScope(cancellationToken))
        {
            var resolvedOptions = GetResolvedEntryOptions<IAsyncState.Sync>(options);

            Func<Stream, CancellationToken, Task> writeDataFunc = (stream, cancellationToken) =>
            {
                writeData(stream, cancellationToken);
                return Task.CompletedTask;
            };

            SetCoreAsync<IAsyncState.Sync>(key, writeDataFunc, resolvedOptions, cancellationToken).GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// Asynchronously stores or overwrites a cache entry with the specified key.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="writeData">An async callback that writes data to the cache file stream. The stream is borrowed and must not be disposed by the callback.</param>
    /// <param name="options">Entry-specific options including expiration, file attributes, and file options.
    /// Use <see cref="FileCacheEntryOptions.Default"/> to apply global defaults from <see cref="FileCacheOptions"/>.
    /// If <see cref="FileCacheEntryOptions.Expiration"/> is null, uses <see cref="FileCacheOptions.DefaultExpiration"/>.
    /// If <see cref="FileCacheEntryOptions.IsSlidingExpiration"/> is null, uses <see cref="FileCacheOptions.IsDefaultSlidingExpiration"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// <para><strong>Overwrite Behavior:</strong> If an entry with the same key already exists, it is completely replaced.
    /// The old file is overwritten atomically.</para>
    /// <para><strong>Expiration:</strong> The expiration timer starts from the moment this method completes successfully.</para>
    /// <para><strong>Atomicity:</strong> Writes are performed to a temporary file and then finalized, ensuring that
    /// concurrent readers never see partially written data.</para>
    /// </remarks>
    public async Task SetAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> writeData,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(writeData);

        await WaitForGlobalLockAsync<IAsyncState.Async>(cancellationToken);

        using (await GetLock(key).WaitScopeAsync(cancellationToken))
        {
            var resolvedOptions = GetResolvedEntryOptions<IAsyncState.Async>(options);

            await SetCoreAsync<IAsyncState.Async>(key, writeData, resolvedOptions, cancellationToken);
        }
    }

    /// <summary>
    /// Retrieves an existing cached file or creates it if it doesn't exist or has expired, returning a file handle.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="factory">A callback that creates the cache entry by writing to the stream. Only invoked if the entry doesn't exist or has expired.
    /// The stream is borrowed and must not be disposed by the callback.</param>
    /// <param name="options">Entry-specific options. Use <see cref="FileCacheEntryOptions.Default"/> to apply global defaults.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="FsFile"/> handle with ownership transferred to the caller (you must dispose it).</returns>
    /// <remarks>
    /// <para><strong>Atomicity:</strong> The check-and-create operation is atomic per key. If multiple threads call this simultaneously
    /// for the same key, only one will invoke the factory; others will wait and receive the newly created entry.</para>
    /// <para><strong>Expired Entries:</strong> If an entry exists but has expired, it is treated as non-existent:
    /// the factory is invoked and a fresh entry is created.</para>
    /// <para><strong>Ownership:</strong> The caller owns the returned <see cref="FsFile"/> and must dispose it.</para>
    /// <para><strong>vs Get + Set:</strong> This method is more efficient than checking Get and calling Set conditionally,
    /// as it performs the operation atomically under a single lock.</para>
    /// </remarks>
    [return: OwnershipTransfer]
    public FsFile GetOrCreate(
        string key,
        [Borrow] Action<Stream, CancellationToken> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(factory);

        WaitForGlobalLockAsync<IAsyncState.Sync>(cancellationToken).GetAwaiter().GetResult();

        using (GetLock(key).WaitScope(cancellationToken))
        {
            var resolvedOptions = GetResolvedEntryOptions<IAsyncState.Sync>(options);

            var cached = GetCoreAsync<IAsyncState.Sync>(key, resolvedOptions.FileOptions, isGetOrCreate: true, getFileHandle: true, cancellationToken).GetAwaiter().GetResult();
            if (cached is not null)
                return cached.Value.FsFile;

            Func<Stream, CancellationToken, Task> factoryFunc = (stream, cancellationToken) =>
            {
                factory(stream, cancellationToken);
                return Task.CompletedTask;
            };

            SetCoreAsync<IAsyncState.Sync>(key, factoryFunc, resolvedOptions, cancellationToken).GetAwaiter().GetResult();
            return FileIO.Open(GetFilePath(key), s_fileReadOptions with { Options = resolvedOptions.FileOptions });
        }
    }

    /// <summary>
    /// Asynchronously retrieves an existing cached file or creates it if it doesn't exist or has expired, returning a file handle.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="factory">An async callback that creates the cache entry by writing to the stream. Only invoked if the entry doesn't exist or has expired.
    /// The stream is borrowed and must not be disposed by the callback.</param>
    /// <param name="options">Entry-specific options. Use <see cref="FileCacheEntryOptions.Default"/> to apply global defaults.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="FsFile"/> handle with ownership transferred to the caller (you must dispose it).</returns>
    /// <remarks>
    /// <para><strong>Atomicity:</strong> The check-and-create operation is atomic per key. If multiple threads call this simultaneously
    /// for the same key, only one will invoke the factory; others will wait and receive the newly created entry.</para>
    /// <para><strong>Expired Entries:</strong> If an entry exists but has expired, it is treated as non-existent:
    /// the factory is invoked and a fresh entry is created.</para>
    /// <para><strong>Ownership:</strong> The caller owns the returned <see cref="FsFile"/> and must dispose it.</para>
    /// <para><strong>vs GetAsync + SetAsync:</strong> This method is more efficient than checking GetAsync and calling SetAsync conditionally,
    /// as it performs the operation atomically under a single lock.</para>
    /// </remarks>
    [return: OwnershipTransfer]
    public async Task<FsFile> GetOrCreateAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(factory);

        await WaitForGlobalLockAsync<IAsyncState.Async>(cancellationToken);

        using (await GetLock(key).WaitScopeAsync(cancellationToken))
        {
            var resolvedOptions = GetResolvedEntryOptions<IAsyncState.Async>(options);

            var cached = await GetCoreAsync<IAsyncState.Async>(key, resolvedOptions.FileOptions, isGetOrCreate: true, getFileHandle: true, cancellationToken);
            if (cached is not null)
                return cached.Value.FsFile;

            await SetCoreAsync<IAsyncState.Async>(key, factory, resolvedOptions, cancellationToken);
            return FileIO.Open(GetFilePath(key), s_fileReadOptions with { Options = resolvedOptions.FileOptions });
        }
    }

    /// <summary>
    /// Retrieves the file path of an existing cached entry or creates it if it doesn't exist or has expired.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="factory">A callback that creates the cache entry by writing to the stream. Only invoked if the entry doesn't exist or has expired.
    /// The stream is borrowed and must not be disposed by the callback.</param>
    /// <param name="options">Entry-specific options. Use <see cref="FileCacheEntryOptions.Default"/> to apply global defaults.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The absolute file path to the cached file.</returns>
    /// <remarks>
    /// <para><strong>Atomicity:</strong> The check-and-create operation is atomic per key. If multiple threads call this simultaneously
    /// for the same key, only one will invoke the factory; others will wait and receive the path to the newly created entry.</para>
    /// <para><strong>Expired Entries:</strong> If an entry exists but has expired, it is treated as non-existent:
    /// the factory is invoked and a fresh entry is created.</para>
    /// <para><strong>vs GetOrCreate:</strong> Use this method when you only need the file path and plan to open it yourself.
    /// This avoids allocating a file handle unnecessarily.</para>
    /// </remarks>
    public string GetOrCreatePath(
        string key,
        [Borrow] Action<Stream, CancellationToken> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(factory);

        WaitForGlobalLockAsync<IAsyncState.Sync>(cancellationToken).GetAwaiter().GetResult();

        using (GetLock(key).WaitScope(cancellationToken))
        {
            var resolvedOptions = GetResolvedEntryOptions<IAsyncState.Sync>(options);

            var cached = GetCoreAsync<IAsyncState.Sync>(key, resolvedOptions.FileOptions, isGetOrCreate: true, getFileHandle: false, cancellationToken).GetAwaiter().GetResult();
            if (cached is not null)
                return cached.Value.FilePath;

            Func<Stream, CancellationToken, Task> factoryFunc = (stream, cancellationToken) =>
            {
                factory(stream, cancellationToken);
                return Task.CompletedTask;
            };

            SetCoreAsync<IAsyncState.Sync>(key, factoryFunc, resolvedOptions, cancellationToken).GetAwaiter().GetResult();
            return GetFilePath(key);
        }
    }

    /// <summary>
    /// Asynchronously retrieves the file path of an existing cached entry or creates it if it doesn't exist or has expired.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="factory">An async callback that creates the cache entry by writing to the stream. Only invoked if the entry doesn't exist or has expired.
    /// The stream is borrowed and must not be disposed by the callback.</param>
    /// <param name="options">Entry-specific options. Use <see cref="FileCacheEntryOptions.Default"/> to apply global defaults.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The absolute file path to the cached file.</returns>
    /// <remarks>
    /// <para><strong>Atomicity:</strong> The check-and-create operation is atomic per key. If multiple threads call this simultaneously
    /// for the same key, only one will invoke the factory; others will wait and receive the path to the newly created entry.</para>
    /// <para><strong>Expired Entries:</strong> If an entry exists but has expired, it is treated as non-existent:
    /// the factory is invoked and a fresh entry is created.</para>
    /// <para><strong>vs GetOrCreateAsync:</strong> Use this method when you only need the file path and plan to open it yourself.
    /// This avoids allocating a file handle unnecessarily.</para>
    /// </remarks>
    public async Task<string> GetOrCreatePathAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(factory);

        await WaitForGlobalLockAsync<IAsyncState.Async>(cancellationToken);

        using (await GetLock(key).WaitScopeAsync(cancellationToken))
        {
            var resolvedOptions = GetResolvedEntryOptions<IAsyncState.Async>(options);

            var cached = await GetCoreAsync<IAsyncState.Async>(key, resolvedOptions.FileOptions, isGetOrCreate: true, getFileHandle: false, cancellationToken);
            if (cached is not null)
                return cached.Value.FilePath;

            await SetCoreAsync<IAsyncState.Async>(key, factory, resolvedOptions, cancellationToken);
            return GetFilePath(key);
        }
    }

    /// <summary>
    /// Removes a cache entry by key, deleting the file from disk.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// <para><strong>Non-existent Keys:</strong> If the key doesn't exist, this method completes successfully without error.</para>
    /// <para><strong>File Deletion:</strong> The cache file and its metadata are immediately deleted from disk.</para>
    /// </remarks>
    public void Remove(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        var filePath = GetFilePath(key);

        WaitForGlobalLockAsync<IAsyncState.Sync>(cancellationToken).GetAwaiter().GetResult();

        using (GetLock(key).WaitScope(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            DeleteFile(filePath);
            Log.RemovedCacheKey(_logger, key);
        }
    }

    /// <summary>
    /// Asynchronously removes a cache entry by key, deleting the file from disk.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// <para><strong>Non-existent Keys:</strong> If the key doesn't exist, this method completes successfully without error.</para>
    /// <para><strong>File Deletion:</strong> The cache file and its metadata are immediately deleted from disk.</para>
    /// </remarks>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        var filePath = GetFilePath(key);

        await WaitForGlobalLockAsync<IAsyncState.Async>(cancellationToken);

        using (await GetLock(key).WaitScopeAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            DeleteFile(filePath);
            Log.RemovedCacheKey(_logger, key);
        }
    }

    /// <summary>
    /// Removes all cache entries, deleting all files and subdirectories from the cache directory.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// <para><strong>Global Lock:</strong> This operation acquires the global cache lock, blocking all other cache operations
    /// until the clear completes. Use with caution in high-concurrency scenarios.</para>
    /// <para><strong>Partial Failures:</strong> If deletion of individual files fails, errors are logged but the operation continues.
    /// The cache is left in a partially cleared state.</para>
    /// <para><strong>vs Background Cleanup:</strong> This method forces immediate removal of all entries, including non-expired ones.
    /// Background cleanup only removes expired entries.</para>
    /// </remarks>
    public void Clear(CancellationToken cancellationToken = default)
    {
        using (_globalLock.WaitScope(cancellationToken))
        {
            ClearCore(cancellationToken);
        }
    }

    /// <summary>
    /// Asynchronously removes all cache entries, deleting all files and subdirectories from the cache directory.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// <para><strong>Global Lock:</strong> This operation acquires the global cache lock, blocking all other cache operations
    /// until the clear completes. Use with caution in high-concurrency scenarios.</para>
    /// <para><strong>Partial Failures:</strong> If deletion of individual files fails, errors are logged but the operation continues.
    /// The cache is left in a partially cleared state.</para>
    /// <para><strong>vs Background Cleanup:</strong> This method forces immediate removal of all entries, including non-expired ones.
    /// Background cleanup only removes expired entries.</para>
    /// </remarks>
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        using (await _globalLock.WaitScopeAsync(cancellationToken))
        {
            ClearCore(cancellationToken);
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

    private async Task WaitForGlobalLockAsync<TAsync>(CancellationToken cancellationToken)
        where TAsync : IAsyncState
    {
        // Check if the global lock is currently held without acquiring it
        if (_globalLock.CurrentCount == 0)
        {
            // Lock is currently held, wait for it to be released
            if (TAsync.IsAsync)
                await _globalLock.WaitAsync(cancellationToken);
            else
                _globalLock.Wait(cancellationToken);

            _globalLock.Release();
        }
    }

    [return: OwnershipTransfer]
    private async Task<FilePathOrFsFile?> GetCoreAsync<TAsync>(
        string key,
        FileOptions options,
        bool isGetOrCreate,
        bool getFileHandle,
        CancellationToken cancellationToken)
        where TAsync : IAsyncState
    {
        var filePath = GetFilePath(key);

        var metadata = await ReadMetadataAsync<TAsync>(filePath, cancellationToken);
        if (metadata is null)
            return null;

        if (metadata.Value.SlidingExpiration.HasValue)
            metadata = await RefreshSlidingExpirationAsync<TAsync>(filePath, metadata.Value, cancellationToken);

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
    private async Task SetCoreAsync<TAsync>(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> writeData,
        ResolvedEntryOptions options,
        CancellationToken cancellationToken)
        where TAsync : IAsyncState
    {
        var filePath = GetFilePath(key);

        var expiresAt = _clock.GetCurrentInstant().Plus(options.Expiration);

        var metadata = new FileCacheMetadata
        {
            ExpiresAt = expiresAt,
            SlidingExpiration = options.IsSlidingExpiration ? options.Expiration : null,
        };

        using (var file = OwnedOrBorrowed.Create(PartialFileStream.Create(filePath, s_fileWriteOptions with { Options = options.FileOptions, Attributes = options.FileAttributes }, createDirectory: true)))
        {
            if (TAsync.IsAsync)
            {
                await writeData(file.Value.FileStream, cancellationToken);
                await file.Value.FileStream.FlushAsync(cancellationToken);
            }
            else
            {
                writeData(file.Value.FileStream, cancellationToken).GetAwaiter().GetResult();
                file.Value.FileStream.Flush();
            }

            await WriteMetadataAsync<TAsync>(file.Value.CurrentPath, metadata, cancellationToken);

            file.Value.FinalizeFile(overwrite: true);

            Log.CachedKey(_logger, key, expiresAt);
        }
    }

    public void ClearCore(CancellationToken cancellationToken)
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

    private string GetFilePath(string key)
    {
        return Path.Join(_cacheDirectory, key);
    }

    private ResolvedEntryOptions GetResolvedEntryOptions<TAsync>(FileCacheEntryOptions options)
        where TAsync : IAsyncState
    {
        return new ResolvedEntryOptions
        {
            FileOptions = options.FileOptions ?? (TAsync.IsAsync ? _options.DefaultAsyncFileOptions : _options.DefaultSyncFileOptions),
            FileAttributes = options.FileAttributes ?? _options.DefaultFileAttributes,
            Expiration = options.Expiration ?? _options.DefaultExpiration,
            IsSlidingExpiration = options.IsSlidingExpiration ?? _options.IsDefaultSlidingExpiration
        };
    }

    private static async Task<FileCacheMetadata?> ReadMetadataAsync<TAsync>(
        string filePath,
        CancellationToken cancellationToken)
        where TAsync : IAsyncState
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
        {
            if (TAsync.IsAsync)
            {
                await using (var fileStream = file.CreateFileStream())
                    return await JsonSerializer.DeserializeAsync(fileStream, FileCacheJsonSerializerContext.Default.FileCacheMetadata, cancellationToken);
            }
            else
            {
                using (var fileStream = file.CreateFileStream())
                    return JsonSerializer.Deserialize(fileStream, FileCacheJsonSerializerContext.Default.FileCacheMetadata);
            }
        }
    }

    private static async Task WriteMetadataAsync<TAsync>(
        string filePath,
        FileCacheMetadata metadata,
        CancellationToken cancellationToken)
        where TAsync : IAsyncState
    {
        var metadataPath = filePath + ":" + MetadataStreamName;

        using (var file = FileIO.Open(metadataPath, s_fileWriteOptions))
        {
            if (TAsync.IsAsync)
            {
                await using (var fileStream = file.CreateFileStream())
                    await JsonSerializer.SerializeAsync(fileStream, metadata, FileCacheJsonSerializerContext.Default.FileCacheMetadata, cancellationToken);
            }
            else
            {
                using (var fileStream = file.CreateFileStream())
                    JsonSerializer.Serialize(fileStream, metadata, FileCacheJsonSerializerContext.Default.FileCacheMetadata);
            }
        }
    }

    private static void DeleteFile(string filePath)
    {
        File.Delete(filePath);
    }

    private async Task<FileCacheMetadata> RefreshSlidingExpirationAsync<TAsync>(
        string filePath,
        FileCacheMetadata metadata,
        CancellationToken cancellationToken)
        where TAsync : IAsyncState
    {
        if (metadata.SlidingExpiration is null)
            return metadata;

        var newExpiration = _clock.GetCurrentInstant()
            .Plus(metadata.SlidingExpiration.Value);

        var updatedMetadata = metadata with { ExpiresAt = newExpiration };
        await WriteMetadataAsync<TAsync>(filePath, updatedMetadata, cancellationToken);

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
                        var metadata = await ReadMetadataAsync<IAsyncState.Async>(filePath, CancellationToken.None);
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

    private readonly record struct ResolvedEntryOptions
    {
        public FileOptions FileOptions { get; init; }

        public FileAttributes FileAttributes { get; init; }

        public Duration Expiration { get; init; }

        public bool IsSlidingExpiration { get; init; }
    }
}
