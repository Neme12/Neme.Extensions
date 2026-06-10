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
    public sealed class Async : IDisposable
    {
        private const string MetadataExtension = ".metadata";

        private readonly string _testCacheDirectory;
        private readonly FakeClock _clock;
        private readonly ILogger<FileCache> _logger;

        public Async()
        {
            _testCacheDirectory = Path.Combine(Path.GetTempPath(), $"FileCache_Tests_Async_{Guid.NewGuid()}");
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
                DefaultAsyncFileOptions = fileOptions ?? (FileOptions.Asynchronous | FileOptions.SequentialScan),
            });

            return new FileCache(options, _logger, _clock);
        }

        private static string GetMetadataPath(string filePath)
        {
            return filePath + MetadataExtension;
        }

        [Fact]
        public async Task SetAsync_StoresDataSuccessfully()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Hello, World!"u8.ToArray();

            // Act
            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, FileCacheEntryOptions.Default);

            // Assert
            var filePath = await cache.GetPathAsync(key);
            Assert.NotNull(filePath);
            Assert.True(File.Exists(filePath));
            Assert.True(File.Exists(GetMetadataPath(filePath)));

            var content = await File.ReadAllBytesAsync(filePath);
            Assert.Equal(data, content);
        }

        [Fact]
        public async Task GetAsync_ReturnsNullForNonExistentKey()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "non-existent-key";

            // Act
            using var result = await cache.GetAsync(key, FileCacheEntryReadOptions.Default);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ReturnsFileHandleForExistingKey()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, FileCacheEntryOptions.Default);

            // Act
            using var result = await cache.GetAsync(key, FileCacheEntryReadOptions.Default);

            // Assert
            Assert.NotNull(result);
            using var stream = result.CreateFileStream();
            var content = new byte[data.Length];
#pragma warning disable CA2022 // TODO: Polyfill ReadExactly
            await stream.ReadAsync(content);
#pragma warning restore CA2022
            Assert.Equal(data, content);
        }

        [Fact]
        public async Task GetPathAsync_ReturnsNullForNonExistentKey()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "non-existent-key";

            // Act
            var result = await cache.GetPathAsync(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPathAsync_ReturnsPathForExistingKey()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, FileCacheEntryOptions.Default);

            // Act
            var result = await cache.GetPathAsync(key);

            // Assert
            Assert.NotNull(result);
            Assert.True(File.Exists(result));
            Assert.True(File.Exists(GetMetadataPath(result)));
        }

        [Fact]
        public async Task GetAsync_ReturnsNullForExpiredKey()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "expired-key";
            var data = "Test Data"u8.ToArray();

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, new FileCacheEntryOptions { Expiration = Duration.FromMinutes(30) });

            var filePath = await cache.GetPathAsync(key);
            Assert.NotNull(filePath);

            // Advance time past expiration
            _clock.Advance(Duration.FromMinutes(31));

            // Act
            using var result = await cache.GetAsync(key, FileCacheEntryReadOptions.Default);

            // Assert
            Assert.Null(result);
            Assert.False(File.Exists(filePath));
            Assert.False(File.Exists(GetMetadataPath(filePath)));
        }

        [Fact]
        public async Task GetAsync_WithSlidingExpiration_RefreshesExpiration()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "sliding-key";
            var data = "Test Data"u8.ToArray();

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, new FileCacheEntryOptions(Duration.FromMinutes(30), isSlidingExpiration: true));

            // Advance time but access before expiration
            _clock.Advance(Duration.FromMinutes(20));
            using var firstResult = await cache.GetAsync(key, FileCacheEntryReadOptions.Default);
            Assert.NotNull(firstResult);

            // Advance another 20 minutes (would be expired with absolute expiration)
            _clock.Advance(Duration.FromMinutes(20));

            // Act - should still be valid due to sliding expiration
            using var secondResult = await cache.GetAsync(key, FileCacheEntryReadOptions.Default);

            // Assert
            Assert.NotNull(secondResult);
        }

        [Fact]
        public async Task GetOrCreateAsync_CreatesEntryIfNotExists()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "new-key";
            var data = "Created Data"u8.ToArray();
            var factoryCalled = false;

            // Act
            using var result = await cache.GetOrCreateAsync(key, async (stream, ct) =>
            {
                factoryCalled = true;
                await stream.WriteAsync(data, ct);
            }, FileCacheEntryOptions.Default);

            // Assert
            Assert.True(factoryCalled);
            Assert.NotNull(result);

            using var stream = result.CreateFileStream();
            var content = new byte[data.Length];
#pragma warning disable CA2022 // TODO: Polyfill ReadExactly
            await stream.ReadAsync(content);
#pragma warning restore CA2022
            Assert.Equal(data, content);
        }

        [Fact]
        public async Task GetOrCreateAsync_ReturnsExistingEntryIfExists()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "existing-key";
            var originalData = "Original Data"u8.ToArray();
            var newData = "New Data"u8.ToArray();

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(originalData, ct);
            }, FileCacheEntryOptions.Default);

            var factoryCalled = false;

            // Act
            using var result = await cache.GetOrCreateAsync(key, async (stream, ct) =>
            {
                factoryCalled = true;
                await stream.WriteAsync(newData, ct);
            }, FileCacheEntryOptions.Default);

            // Assert
            Assert.False(factoryCalled);
            Assert.NotNull(result);

            using var stream = result.CreateFileStream();
            var content = new byte[originalData.Length];
#pragma warning disable CA2022 // TODO: Polyfill ReadExactly
            await stream.ReadAsync(content);
#pragma warning restore CA2022
            Assert.Equal(originalData, content);
        }

        [Fact]
        public async Task GetOrCreatePathAsync_CreatesEntryIfNotExists()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "new-path-key";
            var data = "Created Data"u8.ToArray();
            var factoryCalled = false;

            // Act
            var result = await cache.GetOrCreatePathAsync(key, async (stream, ct) =>
            {
                factoryCalled = true;
                await stream.WriteAsync(data, ct);
            }, FileCacheEntryOptions.Default);

            // Assert
            Assert.True(factoryCalled);
            Assert.NotNull(result);
            Assert.True(File.Exists(result));
            Assert.True(File.Exists(GetMetadataPath(result)));

            var content = await File.ReadAllBytesAsync(result);
            Assert.Equal(data, content);
        }

        [Fact]
        public async Task GetOrCreatePathAsync_ReturnsExistingEntryIfExists()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "existing-path-key";
            var originalData = "Original Data"u8.ToArray();
            var newData = "New Data"u8.ToArray();

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(originalData, ct);
            }, FileCacheEntryOptions.Default);

            var factoryCalled = false;

            // Act
            var result = await cache.GetOrCreatePathAsync(key, async (stream, ct) =>
            {
                factoryCalled = true;
                await stream.WriteAsync(newData, ct);
            }, FileCacheEntryOptions.Default);

            // Assert
            Assert.False(factoryCalled);
            Assert.NotNull(result);
            Assert.True(File.Exists(GetMetadataPath(result)));

            var content = await File.ReadAllBytesAsync(result);
            Assert.Equal(originalData, content);
        }

        [Fact]
        public async Task RemoveAsync_DeletesExistingEntry()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "remove-key";
            var data = "Test Data"u8.ToArray();

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, FileCacheEntryOptions.Default);

            var filePath = await cache.GetPathAsync(key);
            Assert.NotNull(filePath);
            Assert.True(File.Exists(GetMetadataPath(filePath)));

            // Act
            await cache.RemoveAsync(key, CancellationToken.None);

            // Assert
            Assert.False(File.Exists(filePath));
            Assert.False(File.Exists(GetMetadataPath(filePath)));
            using var result = await cache.GetAsync(key, FileCacheEntryReadOptions.Default);
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveAsync_DoesNotThrowForNonExistentKey()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "non-existent-key";

            // Act & Assert
            await cache.RemoveAsync(key, CancellationToken.None);
        }

        [Fact]
        public async Task ClearAsync_RemovesAllEntries()
        {
            // Arrange
            using var cache = CreateFileCache();
            var keys = new[] { "key1", "key2", "key3" };
            var data = "Test Data"u8.ToArray();

            foreach (var key in keys)
            {
                await cache.SetAsync(key, async (stream, ct) =>
                {
                    await stream.WriteAsync(data, ct);
                }, FileCacheEntryOptions.Default);
            }

            // Verify all exist
            foreach (var key in keys)
            {
                var filePath = await cache.GetPathAsync(key);
                Assert.NotNull(filePath);
                Assert.True(File.Exists(GetMetadataPath(filePath)));
            }

            // Act
            await cache.ClearAsync(CancellationToken.None);

            // Assert
            foreach (var key in keys)
            {
                using var result = await cache.GetAsync(key, FileCacheEntryReadOptions.Default);
                Assert.Null(result);

                var filePath = Path.Combine(_testCacheDirectory, key);
                Assert.False(File.Exists(filePath));
                Assert.False(File.Exists(GetMetadataPath(filePath)));
            }
        }

        [Fact]
        public async Task CleanupExpiredFilesAsync_RemovesOnlyExpiredEntries()
        {
            // Arrange
            using var cache = CreateFileCache();
            var expiredKey = "expired-key";
            var validKey = "valid-key";
            var data = "Test Data"u8.ToArray();

            // Set expired entry
            await cache.SetAsync(expiredKey, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, new FileCacheEntryOptions { Expiration = Duration.FromMinutes(10) });

            // Set valid entry
            await cache.SetAsync(validKey, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, new FileCacheEntryOptions { Expiration = Duration.FromHours(2) });

            var expiredFilePath = Path.Combine(_testCacheDirectory, expiredKey);
            var validFilePath = Path.Combine(_testCacheDirectory, validKey);

            Assert.True(File.Exists(GetMetadataPath(expiredFilePath)));
            Assert.True(File.Exists(GetMetadataPath(validFilePath)));

            // Advance time to expire first entry
            _clock.Advance(Duration.FromMinutes(15));

            // Act
            await cache.CleanupExpiredFilesAsync();

            // Assert
            using var expiredResult = await cache.GetAsync(expiredKey, FileCacheEntryReadOptions.Default);
            Assert.Null(expiredResult);
            Assert.False(File.Exists(expiredFilePath));
            Assert.False(File.Exists(GetMetadataPath(expiredFilePath)));

            using var validResult = await cache.GetAsync(validKey, FileCacheEntryReadOptions.Default);
            Assert.NotNull(validResult);
            Assert.True(File.Exists(validFilePath));
            Assert.True(File.Exists(GetMetadataPath(validFilePath)));
        }

        [Fact]
        public async Task CleanupExpiredFilesAsync_DeletesFolderWhenAllFilesExpire()
        {
            // Arrange
            using var cache = CreateFileCache();
            var folder = "myfolder";
            var file1Key = $"{folder}/file1.txt";
            var file2Key = $"{folder}/file2.txt";
            var data = "Test Data"u8.ToArray();

            // Set first file with short expiration
            await cache.SetAsync(file1Key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, new FileCacheEntryOptions { Expiration = Duration.FromMinutes(10) });

            // Set second file with longer expiration
            await cache.SetAsync(file2Key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, new FileCacheEntryOptions { Expiration = Duration.FromMinutes(30) });

            var folderPath = Path.Combine(_testCacheDirectory, folder);
            var file1Path = Path.Combine(_testCacheDirectory, file1Key);
            var file2Path = Path.Combine(_testCacheDirectory, file2Key);

            // Verify both files and folder exist
            Assert.True(Directory.Exists(folderPath));
            Assert.True(File.Exists(file1Path));
            Assert.True(File.Exists(file2Path));
            Assert.True(File.Exists(GetMetadataPath(file1Path)));
            Assert.True(File.Exists(GetMetadataPath(file2Path)));

            // Advance time to expire first file only
            _clock.Advance(Duration.FromMinutes(15));

            // Act - cleanup should delete file1 but keep folder and file2
            await cache.CleanupExpiredFilesAsync();

            // Assert - file1 deleted, file2 and folder still exist
            Assert.False(File.Exists(file1Path));
            Assert.True(File.Exists(file2Path));
            Assert.False(File.Exists(GetMetadataPath(file1Path)));
            Assert.True(File.Exists(GetMetadataPath(file2Path)));
            Assert.True(Directory.Exists(folderPath));

            // Advance time to expire second file
            _clock.Advance(Duration.FromMinutes(20));

            // Act - cleanup should delete file2 and the folder
            await cache.CleanupExpiredFilesAsync();

            // Assert - both files and folder are deleted
            Assert.False(File.Exists(file1Path));
            Assert.False(File.Exists(file2Path));
            Assert.False(File.Exists(GetMetadataPath(file1Path)));
            Assert.False(File.Exists(GetMetadataPath(file2Path)));
            Assert.False(Directory.Exists(folderPath));
        }

        [Fact]
        public async Task SetAsync_ThrowsArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await cache.SetAsync(null!, (s, ct) => Task.CompletedTask, FileCacheEntryOptions.Default));
        }

        [Fact]
        public async Task SetAsync_ThrowsArgumentException_WhenKeyIsEmpty()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await cache.SetAsync(string.Empty, (s, ct) => Task.CompletedTask, FileCacheEntryOptions.Default));
        }

        [Fact]
        public async Task SetAsync_ThrowsArgumentNullException_WhenWriteDataIsNull()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await cache.SetAsync("key", null!, FileCacheEntryOptions.Default));
        }

        [Fact]
        public async Task GetAsync_ThrowsArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await cache.GetAsync(null!, FileCacheEntryReadOptions.Default));
        }

        [Fact]
        public async Task GetAsync_ThrowsArgumentException_WhenKeyIsEmpty()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await cache.GetAsync(string.Empty, FileCacheEntryReadOptions.Default));
        }

        [Fact]
        public async Task GetOrCreateAsync_ThrowsArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await cache.GetOrCreateAsync(null!, (s, ct) => Task.CompletedTask, FileCacheEntryOptions.Default));
        }

        [Fact]
        public async Task GetOrCreateAsync_ThrowsArgumentNullException_WhenFactoryIsNull()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await cache.GetOrCreateAsync("key", null!, FileCacheEntryOptions.Default));
        }

        [Fact]
        public async Task RemoveAsync_ThrowsArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            using var cache = CreateFileCache();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await cache.RemoveAsync(null!, CancellationToken.None));
        }

        [Fact]
        public async Task ConcurrentAccess_HandlesMultipleThreads()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "concurrent-key";
            var data = "Test Data"u8.ToArray();
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await cache.SetAsync(key, async (stream, ct) =>
                    {
                        await stream.WriteAsync(data, ct);
                    }, FileCacheEntryOptions.Default);
                }));
            }

            // Assert
            await Task.WhenAll(tasks);
        }

        [Fact]
        public async Task SetAsync_WithCustomExpiration_UsesSpecifiedDuration()
        {
            // Arrange
            using var cache = CreateFileCache(defaultExpiration: Duration.FromHours(1));
            var key = "custom-expiration-key";
            var data = "Test Data"u8.ToArray();
            var customExpiration = Duration.FromMinutes(5);

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, new FileCacheEntryOptions { Expiration = customExpiration });

            // Advance time within custom expiration
            _clock.Advance(Duration.FromMinutes(4));

            // Act
            using (var validResult = await cache.GetAsync(key, FileCacheEntryReadOptions.Default))
            {
                Assert.NotNull(validResult);
            }

            // Advance time past custom expiration
            _clock.Advance(Duration.FromMinutes(2));

            using (var expiredResult = await cache.GetAsync(key, FileCacheEntryReadOptions.Default))
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
        public async Task GetAsync_AppliesSpecifiedFileOptions()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, FileCacheEntryOptions.Default);

            // Act - Request with RandomAccess + Asynchronous file options
            using var result = await cache.GetAsync(key, new FileCacheEntryReadOptions(FileOptions.Asynchronous | FileOptions.RandomAccess));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(FileOptions.Asynchronous | FileOptions.RandomAccess, result.Options.Options);
        }

        [Fact]
        public async Task GetAsync_AppliesDefaultFileOptionsWhenNotSpecified()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, FileCacheEntryOptions.Default);

            // Act - Use default options
            using var result = await cache.GetAsync(key, FileCacheEntryReadOptions.Default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(FileOptions.Asynchronous | FileOptions.SequentialScan, result.Options.Options);
        }

        [Fact]
        public async Task GetAsync_FileHandleIsNotAsyncWithoutFlag()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, FileCacheEntryOptions.Default);

            // Act - Request without Asynchronous flag
            using var result = await cache.GetAsync(key, new FileCacheEntryReadOptions(FileOptions.SequentialScan));

            // Assert - Handle should NOT be async
            Assert.NotNull(result);
            Assert.False(result.Handle.IsAsync);
        }

        [Fact]
        public async Task GetAsync_FileHandleIsAsyncWithFlag()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, FileCacheEntryOptions.Default);

            // Act - Request with Asynchronous flag
            using var result = await cache.GetAsync(key, new FileCacheEntryReadOptions(FileOptions.Asynchronous | FileOptions.SequentialScan));

            // Assert - Handle should be async
            Assert.NotNull(result);
            Assert.True(result.Handle.IsAsync);
        }

        [Fact]
        public async Task SetAsync_AppliesSpecifiedFileAttributes()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            // Act - Create file with Temporary attribute
            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, new FileCacheEntryOptions 
            { 
                Expiration = Duration.FromHours(1),
                FileAttributes = FileAttributes.Temporary 
            });

            // Assert
            var filePath = await cache.GetPathAsync(key);
            Assert.NotNull(filePath);
            var attributes = File.GetAttributes(filePath);
            Assert.True(attributes.HasFlag(FileAttributes.Temporary));
        }

        [Fact]
        public async Task SetAsync_AppliesDefaultFileAttributesWhenNotSpecified()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            // Act - Create file without specifying attributes
            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, FileCacheEntryOptions.Default);

            // Assert - Should not have Temporary attribute (Windows adds Archive by default)
            var filePath = await cache.GetPathAsync(key);
            Assert.NotNull(filePath);
            var attributes = File.GetAttributes(filePath);
            Assert.False(attributes.HasFlag(FileAttributes.Temporary));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.True(attributes.HasFlag(FileAttributes.Archive));
        }

        [Fact]
        public async Task GetOrCreateAsync_AppliesSpecifiedFileOptions()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            // Act - Create with specific FileOptions
            using var result = await cache.GetOrCreateAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, new FileCacheEntryOptions 
            { 
                Expiration = Duration.FromHours(1),
                FileOptions = FileOptions.Asynchronous | FileOptions.RandomAccess 
            });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(FileOptions.Asynchronous | FileOptions.RandomAccess, result.Options.Options);
        }

        [Fact]
        public async Task GetAsync_AcceptsImplicitConversionFromFileCacheEntryOptions()
        {
            // Arrange
            using var cache = CreateFileCache();
            var key = "test-key";
            var data = "Test Data"u8.ToArray();

            await cache.SetAsync(key, async (stream, ct) =>
            {
                await stream.WriteAsync(data, ct);
            }, FileCacheEntryOptions.Default);

            // Act - Pass FileCacheEntryOptions directly (tests implicit conversion)
            var entryOptions = new FileCacheEntryOptions 
            { 
                FileOptions = FileOptions.Asynchronous | FileOptions.DeleteOnClose,
                Expiration = Duration.FromHours(1), // This should be ignored for Get
                FileAttributes = FileAttributes.Temporary // This should also be ignored
            };
            using var result = await cache.GetAsync(key, entryOptions);

            // Assert - Only FileOptions should be applied
            Assert.NotNull(result);
            Assert.Equal(FileOptions.Asynchronous | FileOptions.DeleteOnClose, result.Options.Options);
        }
    }
}
