using Neme.Extensions.FileSystem;
using Neme.Extensions.Ownership;

namespace Neme.Extensions.MicrosoftExtensions.Caching;

public interface IFileCache
{
    [return: OwnershipTransfer]
    OpenFile? Get(
        string key,
        FileCacheEntryReadOptions options,
        CancellationToken cancellationToken = default);

    [return: OwnershipTransfer]
    Task<OpenFile?> GetAsync(
        string key,
        FileCacheEntryReadOptions options,
        CancellationToken cancellationToken = default);

    string? GetPath(
        string key,
        CancellationToken cancellationToken = default);

    Task<string?> GetPathAsync(
        string key,
        CancellationToken cancellationToken = default);

    [return: OwnershipTransfer]
    OpenFile GetOrCreate(
        string key,
        [Borrow] Action<Stream, CancellationToken> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    [return: OwnershipTransfer]
    Task<OpenFile> GetOrCreateAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    string GetOrCreatePath(
        string key,
        [Borrow] Action<Stream, CancellationToken> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    Task<string> GetOrCreatePathAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    void Set(
        string key,
        [Borrow] Action<Stream, CancellationToken> writeData,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    Task SetAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> writeData,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    void Remove(string key, CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    void Clear(CancellationToken cancellationToken = default);

    Task ClearAsync(CancellationToken cancellationToken = default);
}
