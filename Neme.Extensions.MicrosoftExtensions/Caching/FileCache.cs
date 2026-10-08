using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neme.Extensions.FileSystem;
using Neme.Extensions.FileSystem.Workflows;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using Neme.Extensions.Tasks;
using Neme.Extensions.Threading;
using NodaTime;
using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Neme.Extensions.MicrosoftExtensions.Caching;

/// <summary>
/// A file-based cache service that stores cached data on disk with time-based expiration.
/// Metadata is stored in a separate sidecar file next to the cached file using the <c>.metadata</c> extension.
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
/// <para><strong>Storage Layout:</strong> Each cached file stores its expiration metadata in a separate sidecar file
/// with the same path plus the <c>.metadata</c> extension.</para>
/// <para><strong>Sliding Expiration:</strong> When an entry has sliding expiration, each successful Get operation
/// automatically extends its lifetime by the configured duration.</para>
/// </remarks>
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

#if NET8_0_OR_GREATER
    private SearchValues<char> _invalidPathChars =
        SearchValues.Create(Path.GetInvalidPathChars());
#else
    private char[] _invalidPathChars =
        Path.GetInvalidPathChars();
#endif

    private const string MetadataExtension = ".metadata";

    private static readonly FileHandleRequest s_fileSyncReadOptions =
        FileHandleRequest.Open(FileSystemAccess.Read, FileShare.Read, flags: FileOptions.SequentialScan);

    private static readonly FileHandleRequest s_fileAsyncReadOptions =
        FileHandleRequest.Open(FileSystemAccess.Read, FileShare.Read, flags: FileOptions.SequentialScan | FileOptions.Asynchronous);

    private static readonly FileHandleRequest s_fileSyncWriteOptions =
        FileHandleRequest.Create(FileSystemAccess.ReadWriteDelete, FileShare.All, flags: FileOptions.SequentialScan);

    private static readonly FileHandleRequest s_fileAsyncWriteOptions =
        FileHandleRequest.Create(FileSystemAccess.ReadWriteDelete, FileShare.All, flags: FileOptions.SequentialScan | FileOptions.Asynchronous);

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

    private void ValidateKey(string key, [CallerArgumentExpression(nameof(key))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, paramName);

#if NET8_0_OR_GREATER
        if (key.AsSpan().IndexOfAny(_invalidPathChars) >= 0)
#else
        if (key.IndexOfAny(_invalidPathChars) >= 0)
#endif
        {
            throw new ArgumentException($"Cache keys must not contain invalid path characters.", paramName);
        }

        if (IsMetadataPath(key) || IsPartialPath(key))
            throw new ArgumentException($"Cache keys must not end with '{MetadataExtension}' or '{PartialFile.PartialExtension}'.", paramName);
    }

    /// <summary>
    /// Retrieves a cached file by key and returns a file handle for reading.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="options">Read-specific options. Use <see cref="FileCacheEntryReadOptions.Default"/> to apply global defaults.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="FileSession"/> handle with ownership transferred to the caller (you must dispose it),
    /// or <c>null</c> if the key doesn't exist or the entry has expired.</returns>
    /// <remarks>
    /// <para><strong>Ownership:</strong> The caller owns the returned <see cref="FileSession"/> and must dispose it.</para>
    /// <para><strong>Expired Entries:</strong> If the entry has expired, it is deleted from disk and <c>null</c> is returned.</para>
    /// <para><strong>Sliding Expiration:</strong> If the entry uses sliding expiration, this call automatically extends its lifetime.</para>
    /// <para><strong>vs GetPath:</strong> Use this method when you need a file stream. Use <see cref="GetPath"/> when you only need
    /// the file path (avoids opening a handle).</para>
    /// </remarks>
    [return: OwnershipTransfer]
    public FileSession? Get(
        string key,
        FileCacheEntryReadOptions options,
        CancellationToken cancellationToken = default)
    {
        var result = GetResultMaybeAsync<IAsyncState.Sync>(key, options, getFileSession: true, cancellationToken).GetAwaiter().GetCompletedResult();
        return result?.FileSession;
    }

    /// <summary>
    /// Asynchronously retrieves a cached file by key and returns a file handle for reading.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="options">Read-specific options. Use <see cref="FileCacheEntryReadOptions.Default"/> to apply global defaults.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="FileSession"/> handle with ownership transferred to the caller (you must dispose it),
    /// or <c>null</c> if the key doesn't exist or the entry has expired.</returns>
    /// <remarks>
    /// <para><strong>Ownership:</strong> The caller owns the returned <see cref="FileSession"/> and must dispose it.</para>
    /// <para><strong>Expired Entries:</strong> If the entry has expired, it is deleted from disk and <c>null</c> is returned.</para>
    /// <para><strong>Sliding Expiration:</strong> If the entry uses sliding expiration, this call automatically extends its lifetime.</para>
    /// <para><strong>vs GetPathAsync:</strong> Use this method when you need a file stream. Use <see cref="GetPathAsync"/> when you only need
    /// the file path (avoids opening a handle).</para>
    /// </remarks>
    [return: OwnershipTransfer]
    public async Task<FileSession?> GetAsync(
        string key,
        FileCacheEntryReadOptions options,
        CancellationToken cancellationToken = default)
    {
        var result = await GetResultMaybeAsync<IAsyncState.Async>(key, options, getFileSession: true, cancellationToken);
        return result?.FileSession;
    }

    /// <summary>
    /// Retrieves the file path of a cached entry by key without opening a file handle.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
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
        CancellationToken cancellationToken = default)
    {
        var result = GetResultMaybeAsync<IAsyncState.Sync>(key, null, getFileSession: false, cancellationToken).GetAwaiter().GetCompletedResult();
        return result?.FilePath;
    }

    /// <summary>
    /// Asynchronously retrieves the file path of a cached entry by key without opening a file handle.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
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
        CancellationToken cancellationToken = default)
    {
        var result = await GetResultMaybeAsync<IAsyncState.Async>(key, null, getFileSession: false, cancellationToken);
        return result?.FilePath;
    }

    [return: OwnershipTransfer]
    private async ValueTask<FilePathOrSession?> GetResultMaybeAsync<TAsync>(
        string key,
        FileCacheEntryReadOptions? options,
        bool getFileSession,
        CancellationToken cancellationToken)
        where TAsync : struct, IAsyncState
    {
        ValidateKey(key);

        await WaitForGlobalLockMaybeAsync<TAsync>(cancellationToken);

        using (await GetLock(key).WaitScopeMaybeAsync<TAsync>(cancellationToken))
        {
            var fileOptions = options?.FileOptions ?? DefaultFileOptions<TAsync>();

            return await GetCoreMaybeAsync<TAsync>(key, fileOptions, isGetOrCreate: false, getFileSession, cancellationToken);
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
        ArgumentNullException.ThrowIfNull(writeData);

        SetMaybeAsync<IAsyncState.Sync>(key, (stream, cancellationToken) =>
        {
            writeData(stream, cancellationToken);
            return Task.CompletedTask;
        }, options, cancellationToken).GetAwaiter().GetCompletedResult();
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
        ArgumentNullException.ThrowIfNull(writeData);

        await SetMaybeAsync<IAsyncState.Async>(key, writeData, options, cancellationToken);
    }

    private async ValueTask SetMaybeAsync<TAsync>(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> writeData,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken)
        where TAsync : struct, IAsyncState
    {
        ValidateKey(key);

        await WaitForGlobalLockMaybeAsync<TAsync>(cancellationToken);

        using (await GetLock(key).WaitScopeMaybeAsync<TAsync>(cancellationToken))
        {
            var resolvedOptions = GetResolvedEntryOptions<TAsync>(options);

            await SetCoreMaybeAsync<TAsync>(key, writeData, resolvedOptions, cancellationToken);
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
    /// <returns>A <see cref="FileSession"/> handle with ownership transferred to the caller (you must dispose it).</returns>
    /// <remarks>
    /// <para><strong>Atomicity:</strong> The check-and-create operation is atomic per key. If multiple threads call this simultaneously
    /// for the same key, only one will invoke the factory; others will wait and receive the newly created entry.</para>
    /// <para><strong>Expired Entries:</strong> If an entry exists but has expired, it is treated as non-existent:
    /// the factory is invoked and a fresh entry is created.</para>
    /// <para><strong>Ownership:</strong> The caller owns the returned <see cref="FileSession"/> and must dispose it.</para>
    /// <para><strong>vs Get + Set:</strong> This method is more efficient than checking Get and calling Set conditionally,
    /// as it performs the operation atomically under a single lock.</para>
    /// </remarks>
    [return: OwnershipTransfer]
    public FileSession GetOrCreate(
        string key,
        [Borrow] Action<Stream, CancellationToken> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var result = GetOrCreateResultMaybeAsync<IAsyncState.Sync>(key, (stream, cancellationToken) =>
        {
            factory(stream, cancellationToken);
            return Task.CompletedTask;
        }, options, getFileSession: true, cancellationToken).GetAwaiter().GetCompletedResult();
        return result.FileSession;
    }

    /// <summary>
    /// Asynchronously retrieves an existing cached file or creates it if it doesn't exist or has expired, returning a file handle.
    /// </summary>
    /// <param name="key">The cache key. Must not be null or empty.</param>
    /// <param name="factory">An async callback that creates the cache entry by writing to the stream. Only invoked if the entry doesn't exist or has expired.
    /// The stream is borrowed and must not be disposed by the callback.</param>
    /// <param name="options">Entry-specific options. Use <see cref="FileCacheEntryOptions.Default"/> to apply global defaults.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="FileSession"/> handle with ownership transferred to the caller (you must dispose it).</returns>
    /// <remarks>
    /// <para><strong>Atomicity:</strong> The check-and-create operation is atomic per key. If multiple threads call this simultaneously
    /// for the same key, only one will invoke the factory; others will wait and receive the newly created entry.</para>
    /// <para><strong>Expired Entries:</strong> If an entry exists but has expired, it is treated as non-existent:
    /// the factory is invoked and a fresh entry is created.</para>
    /// <para><strong>Ownership:</strong> The caller owns the returned <see cref="FileSession"/> and must dispose it.</para>
    /// <para><strong>vs GetAsync + SetAsync:</strong> This method is more efficient than checking GetAsync and calling SetAsync conditionally,
    /// as it performs the operation atomically under a single lock.</para>
    /// </remarks>
    [return: OwnershipTransfer]
    public async Task<FileSession> GetOrCreateAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var result = await GetOrCreateResultMaybeAsync<IAsyncState.Async>(key, factory, options, getFileSession: true, cancellationToken);
        return result.FileSession;
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
        ArgumentNullException.ThrowIfNull(factory);

        var result = GetOrCreateResultMaybeAsync<IAsyncState.Sync>(key, (stream, cancellationToken) =>
        {
            factory(stream, cancellationToken);
            return Task.CompletedTask;
        }, options, getFileSession: false, cancellationToken).GetAwaiter().GetCompletedResult();
        return result.FilePath;
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
        ArgumentNullException.ThrowIfNull(factory);

        var result = await GetOrCreateResultMaybeAsync<IAsyncState.Async>(key, factory, options, getFileSession: false, cancellationToken);
        return result.FilePath;
    }

    private async ValueTask<FilePathOrSession> GetOrCreateResultMaybeAsync<TAsync>(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> factory,
        FileCacheEntryOptions options,
        bool getFileSession,
        CancellationToken cancellationToken)
        where TAsync : struct, IAsyncState
    {
        ValidateKey(key);

        await WaitForGlobalLockMaybeAsync<TAsync>(cancellationToken);

        using (await GetLock(key).WaitScopeMaybeAsync<TAsync>(cancellationToken))
        {
            var resolvedOptions = GetResolvedEntryOptions<TAsync>(options);

            var cached = await GetCoreMaybeAsync<TAsync>(key, resolvedOptions.FileOptions, isGetOrCreate: true, getFileSession, cancellationToken);
            if (cached is not null)
                return cached.Value;

            await SetCoreMaybeAsync<TAsync>(key, factory, resolvedOptions, cancellationToken);
            return getFileSession
                ? FilePathOrSession.FromSession(FileSession.Open(GetFilePath(key), FileReadOptions<TAsync>() with { Flags = resolvedOptions.FileOptions }))
                : FilePathOrSession.FromPath(GetFilePath(key));
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
        RemoveMaybeAsync<IAsyncState.Sync>(key, cancellationToken).GetAwaiter().GetCompletedResult();
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
        await RemoveMaybeAsync<IAsyncState.Async>(key, cancellationToken);
    }

    private async ValueTask RemoveMaybeAsync<TAsync>(string key, CancellationToken cancellationToken)
        where TAsync : struct, IAsyncState
    {
        ValidateKey(key);

        var filePath = GetFilePath(key);

        await WaitForGlobalLockMaybeAsync<TAsync>(cancellationToken);

        using (await GetLock(key).WaitScopeMaybeAsync<TAsync>(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            DeleteFile(filePath);
            DeleteFile(GetMetadataPath(filePath));
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
        ClearMaybeAsync<IAsyncState.Sync>(cancellationToken).GetAwaiter().GetCompletedResult();
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
        await ClearMaybeAsync<IAsyncState.Async>(cancellationToken);
    }

    private async ValueTask ClearMaybeAsync<TAsync>(CancellationToken cancellationToken = default)
        where TAsync : struct, IAsyncState
    {
        using (await _globalLock.WaitScopeMaybeAsync<TAsync>(cancellationToken))
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

    private async Task WaitForGlobalLockMaybeAsync<TAsync>(CancellationToken cancellationToken)
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

    private FileOptions DefaultFileOptions<TAsync>()
        where TAsync : struct, IAsyncState
    {
        return TAsync.IsAsync ? _options.DefaultAsyncFileOptions : _options.DefaultSyncFileOptions;
    }

    private static FileHandleRequest FileReadOptions<TAsync>()
        where TAsync : struct, IAsyncState
    {
        return TAsync.IsAsync ? s_fileAsyncReadOptions : s_fileSyncReadOptions;
    }

    private static FileHandleRequest FileWriteOptions<TAsync>()
        where TAsync : struct, IAsyncState
    {
        return TAsync.IsAsync ? s_fileAsyncWriteOptions : s_fileSyncWriteOptions;
    }

    [return: OwnershipTransfer]
    private async Task<FilePathOrSession?> GetCoreMaybeAsync<TAsync>(
        string key,
        FileOptions options,
        bool isGetOrCreate,
        bool getFileHandle,
        CancellationToken cancellationToken)
        where TAsync : struct, IAsyncState
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
            DeleteFile(GetMetadataPath(filePath));
            return null;
        }

        return getFileHandle
            ? FilePathOrSession.FromSession(FileSession.Open(filePath, FileReadOptions<TAsync>() with { Flags = options }))
            : FilePathOrSession.FromPath(filePath);
    }

    [return: OwnershipTransfer]
    private async Task SetCoreMaybeAsync<TAsync>(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> writeData,
        ResolvedEntryOptions options,
        CancellationToken cancellationToken)
        where TAsync : struct, IAsyncState
    {
        var filePath = GetFilePath(key);

        var expiresAt = _clock.GetCurrentInstant().Plus(options.Expiration);

        var metadata = new FileCacheMetadata
        {
            ExpiresAt = expiresAt,
            SlidingExpiration = options.IsSlidingExpiration ? options.Expiration : null,
        };

        using (var file = OwnedOrBorrowed.Create(PartialFile.CreateFileStream(filePath, FileWriteOptions<TAsync>() with { Flags = options.FileOptions, CreationOptions = options.FileCreationOptions }, createDirectory: true)))
        {
            if (TAsync.IsAsync)
            {
                await writeData(file.Value.File, cancellationToken);
                await file.Value.File.FlushAsync(cancellationToken);
            }
            else
            {
                writeData(file.Value.File, cancellationToken).GetAwaiter().GetCompletedResult();
                file.Value.File.Flush();
            }

            await WriteMetadataAsync<TAsync>(file.Value.FinalPath, metadata, cancellationToken);

            file.Value.Commit(overwrite: true);

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

    private static string GetMetadataPath(string path) =>
        path + MetadataExtension;

    private static bool IsMetadataPath(string path) =>
        path.EndsWith(MetadataExtension, StringComparison.OrdinalIgnoreCase);

    private static bool IsPartialPath(string path) =>
        path.EndsWith(PartialFile.PartialExtension, StringComparison.OrdinalIgnoreCase);

    private ResolvedEntryOptions GetResolvedEntryOptions<TAsync>(FileCacheEntryOptions options)
        where TAsync : struct, IAsyncState
    {
        return new ResolvedEntryOptions
        {
            FileOptions = options.FileOptions ?? DefaultFileOptions<TAsync>(),
            FileCreationOptions = options.FileCreationOptions ?? _options.DefaultFileCreationOptions,
            Expiration = options.Expiration ?? _options.DefaultExpiration,
            IsSlidingExpiration = options.IsSlidingExpiration ?? _options.IsDefaultSlidingExpiration
        };
    }

    private static async Task<FileCacheMetadata?> ReadMetadataAsync<TAsync>(
        string filePath,
        CancellationToken cancellationToken)
        where TAsync : struct, IAsyncState
    {
        var metadataPath = filePath + MetadataExtension;

        if (!FileSession.TryOpen(metadataPath, FileReadOptions<TAsync>(), out var file, ignoreMissingDirectory: true))
            return null;

        using (file)
        {
            if (TAsync.IsAsync)
            {
                await using (var fileStream = file.CreateFileStream(FileAccess.Read))
                    return await JsonSerializer.DeserializeAsync(fileStream, FileCacheJsonSerializerContext.Default.FileCacheMetadata, cancellationToken);
            }
            else
            {
                using (var fileStream = file.CreateFileStream(FileAccess.Read))
                    return JsonSerializer.Deserialize(fileStream, FileCacheJsonSerializerContext.Default.FileCacheMetadata);
            }
        }
    }

    private static async Task WriteMetadataAsync<TAsync>(
        string filePath,
        FileCacheMetadata metadata,
        CancellationToken cancellationToken)
        where TAsync : struct, IAsyncState
    {
        var metadataPath = filePath + MetadataExtension;

        using (var file = PartialFile.CreateSessionFile(metadataPath, FileWriteOptions<TAsync>()))
        {
            if (TAsync.IsAsync)
            {
                await using (var fileStream = file.File.CreateFileStream(FileAccess.Write))
                    await JsonSerializer.SerializeAsync(fileStream, metadata, FileCacheJsonSerializerContext.Default.FileCacheMetadata, cancellationToken);
            }
            else
            {
                using (var fileStream = file.File.CreateFileStream(FileAccess.Write))
                    JsonSerializer.Serialize(fileStream, metadata, FileCacheJsonSerializerContext.Default.FileCacheMetadata);
            }

            file.Commit(overwrite: true);
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
        where TAsync : struct, IAsyncState
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
                if (IsMetadataPath(filePath) || IsPartialPath(filePath))
                    continue;

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
                            DeleteFile(GetMetadataPath(filePath));
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

    private readonly record struct FilePathOrSession
    {
        [Borrowed]
        private readonly object _object;

        private FilePathOrSession(string filePath)
        {
            _object = filePath;
        }

        private FilePathOrSession([Borrow] FileSession file)
        {
            _object = file;
        }

        public bool IsPath =>
            _object is string;

        public bool IsFileReference =>
            _object is FileSession;

        public string FilePath =>
            _object as string ?? throw new InvalidOperationException("Not a file path.");

        [Borrowed]
        public FileSession FileSession =>
            _object as FileSession ?? throw new InvalidOperationException("Not a FileReference.");

        public static FilePathOrSession FromPath(string filePath) =>
            new(filePath);

        public static FilePathOrSession FromSession([Borrow] FileSession file) =>
            new(file);
    }

    private readonly record struct ResolvedEntryOptions
    {
        public FileOptions FileOptions { get; init; }

        public FileCreationOptions FileCreationOptions { get; init; }

        public Duration Expiration { get; init; }

        public bool IsSlidingExpiration { get; init; }
    }
}
