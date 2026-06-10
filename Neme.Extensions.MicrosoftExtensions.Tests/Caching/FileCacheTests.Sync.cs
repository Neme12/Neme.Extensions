using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Neme.Extensions.Tests.Utilities;
using NodaTime;
using NodaTime.Testing;
using System.Runtime.InteropServices;

namespace Neme.Extensions.MicrosoftExtensions.Caching.Tests;

public sealed partial class FileCacheTests
{
    [Collection("FileCache")]
    public sealed class Sync : IDisposable
    {
        private const string MetadataExtension = ".metadata";

        private readonly string _testCacheDirectory;
        private readonly FakeClock _clock;
        private readonly ILogger<FileCache> _logger;

        public Sync()
        {
            _testCacheDirectory = Path.Combine(Path.GetTempPath(), $"FileCache_Tests_Sync_{Guid.NewGuid()}");
            _clock = new FakeClock(Instant.FromUtc(2026, 5, 20, 12, 0, 0));
            _logger = NullLogger<FileCache>.Instance;
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_testCacheDirectory, recursive: true);
            }
            catch
            {
                // Cleanup best effort
            }
        }

        private FileCache CreateFileCache(
            Duration? defaultExpiration = null,
            Duration? expirationScanFrequency = null,
            FileOptions? fileOptions = null)
        {
            var options = Options.Create(new FileCacheOptions
            {
                CacheDirectory = _testCacheDirectory,
                DefaultExpiration = defaultExpiration ?? Duration.FromHours(1),
                ExpirationScanFrequency = expirationScanFrequency ?? Duration.FromMinutes(10),
                DefaultSyncFileOptions = fileOptions ?? FileOptions.SequentialScan,
            });

            return new FileCache(options, _logger, _clock);
        }

        private static string GetMetadataPath(string filePath)
        {
            return filePath + MetadataExtension;
        }

        [Fact]
        public void Set_StoresDataSuccessfully()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Hello, World!"u8.ToArray();

            // Act
            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, FileCacheEntryOptions.Default);

            // Assert
            var filePath = cache.GetPath(key);
            Assert.NotNull(filePath);
            Assert.True(File.Exists(filePath));
            Assert.True(File.Exists(GetMetadataPath(filePath)));

            var content = File.ReadAllBytes(filePath);
            Assert.Equal(data, content);
        }

        [Fact]
        public void Get_ReturnsNullForNonExistentKey()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "non-existent-key";

            // Act
            using var result = cache.Get(key, FileCacheEntryReadOptions.Default);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Get_ReturnsFileHandleForExistingKey()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, FileCacheEntryOptions.Default);

            // Act
            using var result = cache.Get(key, FileCacheEntryReadOptions.Default);

            // Assert
            Assert.NotNull(result);
            using var stream = result.CreateFileStream();
            var content = new byte[data.Length];
#pragma warning disable CA2022 // TODO: Polyfill ReadExactly
            stream.Read(content);
#pragma warning restore CA2022
            Assert.Equal(data, content);
        }

        [Fact]
        public void GetPath_ReturnsNullForNonExistentKey()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "non-existent-key";

            // Act
            var result = cache.GetPath(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetPath_ReturnsPathForExistingKey()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, FileCacheEntryOptions.Default);

            // Act
            var result = cache.GetPath(key);

            // Assert
            Assert.NotNull(result);
            Assert.True(File.Exists(result));
            Assert.True(File.Exists(GetMetadataPath(result)));
        }

        [Fact]
        public void Get_ReturnsNullForExpiredKey()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "expired-key";
            var data = "Test Data"u8.ToArray();

            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, new FileCacheEntryOptions { Expiration = Duration.FromMinutes(30) });

            var filePath = cache.GetPath(key);
            Assert.NotNull(filePath);

            // Advance time past expiration
            _clock.Advance(Duration.FromMinutes(31));

            // Act
            using var result = cache.Get(key, FileCacheEntryReadOptions.Default);

            // Assert
            Assert.Null(result);
            Assert.False(File.Exists(filePath));
            Assert.False(File.Exists(GetMetadataPath(filePath)));
        }

        [Fact]
        public void Get_WithSlidingExpiration_RefreshesExpiration()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "sliding-key";
            var data = "Test Data"u8.ToArray();

            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, new FileCacheEntryOptions(Duration.FromMinutes(30), isSlidingExpiration: true));

            // Advance time but access before expiration
            _clock.Advance(Duration.FromMinutes(20));
            using var firstResult = cache.Get(key, FileCacheEntryReadOptions.Default);
            Assert.NotNull(firstResult);

            // Advance another 20 minutes (would be expired with absolute expiration)
            _clock.Advance(Duration.FromMinutes(20));

            // Act - should still be valid due to sliding expiration
            using var secondResult = cache.Get(key, FileCacheEntryReadOptions.Default);

            // Assert
            Assert.NotNull(secondResult);
        }

        [Fact]
        public void GetOrCreate_CreatesEntryIfNotExists()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "new-key";
            var data = "Created Data"u8.ToArray();
            var factoryCalled = false;

            // Act
            using var result = cache.GetOrCreate(key, (stream, ct) =>
            {
                factoryCalled = true;
                stream.Write(data);
            }, FileCacheEntryOptions.Default);

            // Assert
            Assert.True(factoryCalled);
            Assert.NotNull(result);

            using var stream = result.CreateFileStream();
            var content = new byte[data.Length];
#pragma warning disable CA2022 // TODO: Polyfill ReadExactly
            stream.Read(content);
#pragma warning restore CA2022
            Assert.Equal(data, content);
        }

        [Fact]
        public void GetOrCreate_ReturnsExistingEntryIfExists()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "existing-key";
            var originalData = "Original Data"u8.ToArray();
            var newData = "New Data"u8.ToArray();

            cache.Set(key, (stream, ct) =>
            {
                stream.Write(originalData);
            }, FileCacheEntryOptions.Default);

            var factoryCalled = false;

            // Act
            using var result = cache.GetOrCreate(key, (stream, ct) =>
            {
                factoryCalled = true;
                stream.Write(newData);
            }, FileCacheEntryOptions.Default);

            // Assert
            Assert.False(factoryCalled);
            Assert.NotNull(result);

            using var stream = result.CreateFileStream();
            var content = new byte[originalData.Length];
#pragma warning disable CA2022 // TODO: Polyfill ReadExactly
            stream.Read(content);
#pragma warning restore
            Assert.Equal(originalData, content);
        }

        [Fact]
        public void GetOrCreatePath_CreatesEntryIfNotExists()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "new-path-key";
            var data = "Created Data"u8.ToArray();
            var factoryCalled = false;

            // Act
            var result = cache.GetOrCreatePath(key, (stream, ct) =>
            {
                factoryCalled = true;
                stream.Write(data);
            }, FileCacheEntryOptions.Default);

            // Assert
            Assert.True(factoryCalled);
            Assert.NotNull(result);
            Assert.True(File.Exists(result));
            Assert.True(File.Exists(GetMetadataPath(result)));

            var content = File.ReadAllBytes(result);
            Assert.Equal(data, content);
        }

        [Fact]
        public void GetOrCreatePath_ReturnsExistingEntryIfExists()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "existing-path-key";
            var originalData = "Original Data"u8.ToArray();
            var newData = "New Data"u8.ToArray();

            cache.Set(key, (stream, ct) =>
            {
                stream.Write(originalData);
            }, FileCacheEntryOptions.Default);

            var factoryCalled = false;

            // Act
            var result = cache.GetOrCreatePath(key, (stream, ct) =>
            {
                factoryCalled = true;
                stream.Write(newData);
            }, FileCacheEntryOptions.Default);

            // Assert
            Assert.False(factoryCalled);
            Assert.NotNull(result);
            Assert.True(File.Exists(GetMetadataPath(result)));

            var content = File.ReadAllBytes(result);
            Assert.Equal(originalData, content);
        }

        [Fact]
        public void Remove_DeletesExistingEntry()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "remove-key";
            var data = "Test Data"u8.ToArray();

            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, FileCacheEntryOptions.Default);

            var filePath = cache.GetPath(key);
            Assert.NotNull(filePath);
            Assert.True(File.Exists(GetMetadataPath(filePath)));

            // Act
            cache.Remove(key, CancellationToken.None);

            // Assert
            Assert.False(File.Exists(filePath));
            Assert.False(File.Exists(GetMetadataPath(filePath)));
            using var result = cache.Get(key, FileCacheEntryReadOptions.Default);
            Assert.Null(result);
        }

        [Fact]
        public void Remove_DoesNotThrowForNonExistentKey()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "non-existent-key";

            // Act & Assert
            cache.Remove(key, CancellationToken.None);
        }

        [Fact]
        public void Clear_RemovesAllEntries()
        {
            // Arrange
            using var cache = CreateFileCache();
            var keys = new[] { "key1", "key2", "key3" };
            var data = "Test Data"u8.ToArray();

            foreach (var key in keys)
            {
                cache.Set(key, (stream, ct) =>
                {
                    stream.Write(data);
                }, FileCacheEntryOptions.Default);
            }

            // Verify all exist
            foreach (var key in keys)
            {
                var filePath = cache.GetPath(key);
                Assert.NotNull(filePath);
                Assert.True(File.Exists(GetMetadataPath(filePath)));
            }

            // Act
            cache.Clear(CancellationToken.None);

            // Assert
            foreach (var key in keys)
            {
                using var result = cache.Get(key, FileCacheEntryReadOptions.Default);
                Assert.Null(result);

                var filePath = Path.Combine(_testCacheDirectory, key);
                Assert.False(File.Exists(filePath));
                Assert.False(File.Exists(GetMetadataPath(filePath)));
            }
        }

        [Fact]
        public void Set_ThrowsArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                cache.Set(null!, (s, ct) => { }, FileCacheEntryOptions.Default));
        }

        [Fact]
        public void Set_ThrowsArgumentException_WhenKeyIsEmpty()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                cache.Set(string.Empty, (s, ct) => { }, FileCacheEntryOptions.Default));
        }

        [Fact]
        public void Set_ThrowsArgumentNullException_WhenWriteDataIsNull()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                cache.Set("key", null!, FileCacheEntryOptions.Default));
        }

        [Fact]
        public void Get_ThrowsArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                cache.Get(null!, FileCacheEntryReadOptions.Default));
        }

        [Fact]
        public void Get_ThrowsArgumentException_WhenKeyIsEmpty()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                cache.Get(string.Empty, FileCacheEntryReadOptions.Default));
        }

        [Fact]
        public void GetOrCreate_ThrowsArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                cache.GetOrCreate(null!, (s, ct) => { }, FileCacheEntryOptions.Default));
        }

        [Fact]
        public void GetOrCreate_ThrowsArgumentNullException_WhenFactoryIsNull()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                cache.GetOrCreate("key", null!, FileCacheEntryOptions.Default));
        }

        [Fact]
        public void Remove_ThrowsArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                cache.Remove(null!, CancellationToken.None));
        }

        [Fact]
        public void ConcurrentAccess_HandlesMultipleThreads()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "concurrent-key";
            var data = "Test Data"u8.ToArray();
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    cache.Set(key, (stream, ct) =>
                    {
                        stream.Write(data);
                    }, FileCacheEntryOptions.Default);
                }));
            }

            // Assert
#pragma warning disable xUnit1031
            Task.WaitAll(tasks.ToArray());
#pragma warning restore xUnit1031
        }

        [Fact]
        public void Set_WithCustomExpiration_UsesSpecifiedDuration()
        {
            // Arrange
            using var cache = CreateFileCache(defaultExpiration: Duration.FromHours(1));
            var key = "custom-expiration-key";
            var data = "Test Data"u8.ToArray();
            var customExpiration = Duration.FromMinutes(5);

            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, new FileCacheEntryOptions { Expiration = customExpiration });

            // Advance time within custom expiration
            _clock.Advance(Duration.FromMinutes(4));

            // Act
            using (var validResult = cache.Get(key, FileCacheEntryReadOptions.Default))
            {
                Assert.NotNull(validResult);
            }

            // Advance time past custom expiration
            _clock.Advance(Duration.FromMinutes(2));

            using (var expiredResult = cache.Get(key, FileCacheEntryReadOptions.Default))
            {
                // Assert
                Assert.Null(expiredResult);
            }
        }

        [Fact]
        public void Dispose_ReleasesResources()
        {
            // Arrange
            var cache = CreateFileCache();

            // Act
            cache.Dispose();

            // Assert - no exception should be thrown
        }

        [Fact]
        public void Get_AppliesSpecifiedFileOptions()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, FileCacheEntryOptions.Default);

            // Act - Request with RandomAccess file options
            using var result = cache.Get(key, new FileCacheEntryReadOptions(FileOptions.RandomAccess));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(FileOptions.RandomAccess, result.Options.Options);
        }

        [Fact]
        public void Get_AppliesDefaultFileOptionsWhenNotSpecified()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, FileCacheEntryOptions.Default);

            // Act - Use default options
            using var result = cache.Get(key, FileCacheEntryReadOptions.Default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(FileOptions.SequentialScan, result.Options.Options);
        }

        [Fact]
        public void Get_FileHandleIsNotAsyncWithoutFlag()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, FileCacheEntryOptions.Default);

            // Act - Request without Asynchronous flag
            using var result = cache.Get(key, new FileCacheEntryReadOptions(FileOptions.SequentialScan));

            // Assert - Handle should NOT be async
            Assert.NotNull(result);
            Assert.False(result.Handle.IsAsync);
        }

        [Fact]
        public void Get_FileHandleIsAsyncWithFlag()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, FileCacheEntryOptions.Default);

            // Act - Request with Asynchronous flag
            using var result = cache.Get(key, new FileCacheEntryReadOptions(FileOptions.Asynchronous | FileOptions.SequentialScan));

            // Assert - Handle should be async
            Assert.NotNull(result);
            Assert.True(result.Handle.IsAsync);
        }

        [Fact]
        public void Set_AppliesSpecifiedFileAttributes()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            // Act - Create file with Temporary attribute
            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, new FileCacheEntryOptions 
            { 
                Expiration = Duration.FromHours(1),
                FileAttributes = FileAttributes.Hidden,
            });

            // Assert
            var filePath = cache.GetPath(key);
            Assert.NotNull(filePath);
            var attributes = File.GetAttributes(filePath);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Assert.True(attributes.HasFlag(FileAttributes.Hidden));
        }

        [Fact]
        public void Set_AppliesDefaultFileAttributesWhenNotSpecified()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            // Act - Create file without specifying attributes
            cache.Set(key, (stream, ct) =>
            {
                stream.Write(data);
            }, FileCacheEntryOptions.Default);

            // Assert - Should not have Temporary attribute (Windows adds Archive by default)
            var filePath = cache.GetPath(key);
            Assert.NotNull(filePath);
            var attributes = File.GetAttributes(filePath);
            Assert.False(attributes.HasFlag(FileAttributes.Temporary));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.True(attributes.HasFlag(FileAttributes.Archive));
        }

        [Fact]
        public void GetOrCreate_AppliesSpecifiedFileOptions()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            // Act - Create with specific FileOptions
            using var result = cache.GetOrCreate(key, (stream, ct) =>
            {
                stream.Write(data);
            }, new FileCacheEntryOptions 
            { 
                Expiration = Duration.FromHours(1),
                FileOptions = FileOptions.RandomAccess 
            });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(FileOptions.RandomAccess, result.Options.Options);
        }
    }
}
