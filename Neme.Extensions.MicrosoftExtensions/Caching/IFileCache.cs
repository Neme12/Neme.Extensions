using Neme.Extensions.FileSystem;
using Neme.Extensions.Ownership;

namespace Neme.Extensions.MicrosoftExtensions.Caching;

public interface IFileCache
{
    [return: OwnershipTransfer]
    Task<FsFile?> GetAsync(
        string key,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    Task<string?> GetPathAsync(
        string key,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    [return: OwnershipTransfer]
    Task<FsFile> GetOrCreateAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    Task<string> GetOrCreatePathAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> factory,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    Task SetAsync(
        string key,
        [Borrow] Func<Stream, CancellationToken, Task> writeData,
        FileCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken);

    Task ClearAsync(CancellationToken cancellationToken);
}
