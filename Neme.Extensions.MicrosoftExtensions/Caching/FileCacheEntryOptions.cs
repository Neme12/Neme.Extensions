using NodaTime;

namespace Neme.Extensions.MicrosoftExtensions.Caching;

public readonly record struct FileCacheEntryOptions
{
    public FileCacheEntryOptions(Duration expiration)
    {
        Expiration = expiration;
    }

    public FileCacheEntryOptions(Duration expiration, bool isSlidingExpiration)
    {
        Expiration = expiration;
        IsSlidingExpiration = isSlidingExpiration;
    }

    public FileOptions? FileOptions { get; init; }

    public FileAttributes? FileAttributes { get; init; }

    public Duration? Expiration { get; init; }

    public bool? IsSlidingExpiration { get; init; }

    public static FileCacheEntryOptions Default => new();
}
