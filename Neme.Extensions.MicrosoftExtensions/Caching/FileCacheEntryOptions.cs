using NodaTime;

namespace Neme.Extensions.MicrosoftExtensions.Caching;

/// <summary>
/// Configuration options for a specific cache entry in <see cref="FileCache"/>.
/// </summary>
/// <remarks>
/// <para><strong>Null Properties:</strong> When any property is <c>null</c>, the cache will use the corresponding
/// default value from <see cref="FileCacheOptions"/> configured during service registration.</para>
/// <para><strong>Default Instance:</strong> Use <see cref="Default"/> to create an entry that uses all global defaults.</para>
/// <para><strong>Constructors vs Initializers:</strong> Use constructors for simple expiration configuration.
/// Use property initializers when you also need to customize <see cref="FileOptions"/> or <see cref="FileAttributes"/>.</para>
/// </remarks>
public readonly record struct FileCacheEntryOptions
{
    /// <summary>
    /// Creates options with a specified expiration duration, using the global default to determine whether it's sliding or absolute.
    /// </summary>
    /// <param name="expiration">How long the cache entry should live before expiring.</param>
    /// <remarks>
    /// <para>This constructor sets <see cref="IsSlidingExpiration"/> to <c>null</c>, which means the expiration mode
    /// (sliding vs absolute) is determined by <see cref="FileCacheOptions.IsDefaultSlidingExpiration"/>.</para>
    /// <para><strong>Use this constructor when:</strong> You want to customize the expiration duration but inherit the
    /// sliding/absolute behavior from global configuration.</para>
    /// <para><strong>Use the other constructor when:</strong> You need to explicitly control whether this specific entry
    /// uses sliding or absolute expiration, regardless of the global default.</para>
    /// </remarks>
    public FileCacheEntryOptions(Duration expiration)
    {
        Expiration = expiration;
    }

    /// <summary>
    /// Creates options with an expiration duration and explicitly specifies whether it is sliding or absolute.
    /// </summary>
    /// <param name="expiration">How long the cache entry should live before expiring.</param>
    /// <param name="isSlidingExpiration">
    /// <c>true</c> for sliding expiration (timer resets on each access);
    /// <c>false</c> for absolute expiration (timer starts when created and never resets).
    /// </param>
    /// <remarks>
    /// <para><strong>Sliding Expiration:</strong> Each successful Get operation extends the entry's lifetime by the specified duration.
    /// Useful for data that should remain cached as long as it's being actively accessed.</para>
    /// <para><strong>Absolute Expiration:</strong> The entry expires after the specified duration, regardless of access patterns.
    /// Useful for time-sensitive data that must be refreshed periodically.</para>
    /// </remarks>
    public FileCacheEntryOptions(Duration expiration, bool isSlidingExpiration)
    {
        Expiration = expiration;
        IsSlidingExpiration = isSlidingExpiration;
    }

    /// <summary>
    /// File options to use when opening the cache file.
    /// </summary>
    /// <remarks>
    /// <para><strong>Null Behavior:</strong> When <c>null</c>, the cache uses <see cref="FileCacheOptions.DefaultSyncFileOptions"/>
    /// for synchronous methods or <see cref="FileCacheOptions.DefaultAsyncFileOptions"/> for async methods.</para>
    /// <para><strong>Typical Values:</strong> Combine <see cref="System.IO.FileOptions"/> flags like
    /// <see cref="System.IO.FileOptions.Asynchronous"/>, <see cref="System.IO.FileOptions.SequentialScan"/>,
    /// or <see cref="System.IO.FileOptions.RandomAccess"/> based on your access pattern.</para>
    /// </remarks>
    public FileOptions? FileOptions { get; init; }

    /// <summary>
    /// File attributes to apply to the cache file.
    /// </summary>
    /// <remarks>
    /// <para><strong>Null Behavior:</strong> When <c>null</c>, the cache uses <see cref="FileCacheOptions.DefaultFileAttributes"/>.</para>
    /// <para><strong>Common Values:</strong> <see cref="System.IO.FileAttributes.Normal"/> (default),
    /// <see cref="System.IO.FileAttributes.Temporary"/> (hint for OS to keep in memory),
    /// or <see cref="System.IO.FileAttributes.Hidden"/> (hide from directory listings).</para>
    /// </remarks>
    public FileAttributes? FileAttributes { get; init; }

    /// <summary>
    /// The duration before the cache entry expires.
    /// </summary>
    /// <remarks>
    /// <para><strong>Null Behavior:</strong> When <c>null</c>, the cache uses <see cref="FileCacheOptions.DefaultExpiration"/>.</para>
    /// <para><strong>Expiration Mode:</strong> Whether this is sliding or absolute expiration depends on <see cref="IsSlidingExpiration"/>.
    /// If <see cref="IsSlidingExpiration"/> is <c>null</c>, the mode is determined by <see cref="FileCacheOptions.IsDefaultSlidingExpiration"/>.</para>
    /// <para><strong>Timer Start:</strong> For absolute expiration, the timer starts when the entry is created.
    /// For sliding expiration, the timer resets on each successful Get operation.</para>
    /// </remarks>
    public Duration? Expiration { get; init; }

    /// <summary>
    /// Specifies whether <see cref="Expiration"/> represents sliding or absolute expiration.
    /// </summary>
    /// <remarks>
    /// <para><strong>Null Behavior:</strong> When <c>null</c>, the cache uses <see cref="FileCacheOptions.IsDefaultSlidingExpiration"/>.</para>
    /// <para><strong>True (Sliding):</strong> Each successful Get operation resets the expiration timer.
    /// The entry remains cached as long as it's accessed within the expiration window.</para>
    /// <para><strong>False (Absolute):</strong> The entry expires after the specified duration from creation,
    /// regardless of how often it's accessed.</para>
    /// </remarks>
    public bool? IsSlidingExpiration { get; init; }

    /// <summary>
    /// Gets a default instance where all properties are <c>null</c>, causing the cache to use global defaults from <see cref="FileCacheOptions"/>.
    /// </summary>
    /// <remarks>
    /// <para><strong>Use Case:</strong> Pass this when you want the cache entry to inherit all configuration from
    /// <see cref="FileCacheOptions"/> without any per-entry customization.</para>
    /// <para><strong>Equivalent:</strong> This is equivalent to <c>new FileCacheEntryOptions()</c> or <c>default(FileCacheEntryOptions)</c>.</para>
    /// </remarks>
    public static FileCacheEntryOptions Default => new();
}
