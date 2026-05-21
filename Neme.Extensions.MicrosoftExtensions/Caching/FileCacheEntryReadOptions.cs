namespace Neme.Extensions.MicrosoftExtensions.Caching;

/// <summary>
/// Configuration options for reading a cache entry in <see cref="FileCache"/>.
/// </summary>
/// <remarks>
/// <para><strong>Null Properties:</strong> When any property is <c>null</c>, the cache will use the corresponding
/// default value from <see cref="FileCacheOptions"/> configured during service registration.</para>
/// <para><strong>Default Instance:</strong> Use <see cref="Default"/> to use all global defaults.</para>
/// <para><strong>Read-Only Operations:</strong> These options only affect how the file handle is opened for reading.
/// They do not affect expiration checking or metadata reading, which always use internal defaults.</para>
/// </remarks>
public readonly record struct FileCacheEntryReadOptions
{
    /// <summary>
    /// Creates read options with specific file options.
    /// </summary>
    /// <param name="fileOptions">The <see cref="System.IO.FileOptions"/> flags to use when opening the cache file for reading.</param>
    /// <remarks>
    /// <para><strong>Use Case:</strong> Convenient when you only need to customize file options without using property initializer syntax.</para>
    /// <para><strong>Example:</strong> <c>new FileCacheEntryReadOptions(FileOptions.RandomAccess)</c> is equivalent to
    /// <c>new FileCacheEntryReadOptions { FileOptions = FileOptions.RandomAccess }</c>.</para>
    /// </remarks>
    public FileCacheEntryReadOptions(FileOptions fileOptions)
    {
        FileOptions = fileOptions;
    }

    /// <summary>
    /// File options to use when opening the cache file for reading.
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
    /// Gets a default instance where all properties are <c>null</c>, causing the cache to use global defaults from <see cref="FileCacheOptions"/>.
    /// </summary>
    /// <remarks>
    /// <para><strong>Equivalent:</strong> This is equivalent to <c>new FileCacheEntryReadOptions()</c> or <c>default(FileCacheEntryReadOptions)</c>.</para>
    /// </remarks>
    public static FileCacheEntryReadOptions Default => new();
}
