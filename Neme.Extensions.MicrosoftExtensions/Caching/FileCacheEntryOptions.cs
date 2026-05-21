using NodaTime;

namespace Neme.Extensions.MicrosoftExtensions.Caching;

public readonly record struct FileCacheEntryOptions
{
    public FileOptions? FileOptions { get; init; }

    public FileAttributes? FileAttributes { get; init; }

    public Duration? Expiration { get; init; }

    public Duration? SlidingExpiration { get; init; }

    public static FileCacheEntryOptions Default => new();
}
