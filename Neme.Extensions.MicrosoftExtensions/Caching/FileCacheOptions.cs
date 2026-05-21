using NodaTime;

namespace Neme.Extensions.MicrosoftExtensions.Caching;

public sealed class FileCacheOptions
{
    public string CacheDirectory { get; set; } = null!;

    public Duration ExpirationScanFrequency { get; set; } =
        Duration.FromMinutes(10);

    public Duration DefaultExpiration { get; set; } =
        Duration.FromHours(1);

    public bool IsDefaultSlidingExpiration { get; set; }
        = false;

    public FileOptions DefaultSyncFileOptions { get; set; } =
        FileOptions.SequentialScan;

    public FileOptions DefaultAsyncFileOptions { get; set; } =
        FileOptions.Asynchronous | FileOptions.SequentialScan;

    public FileAttributes DefaultFileAttributes { get; set; } =
        FileAttributes.Normal;
}
