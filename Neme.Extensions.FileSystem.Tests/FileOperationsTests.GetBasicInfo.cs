using Microsoft.Win32.SafeHandles;
using NodaTime;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileOperationsTests
{
    [Collection(nameof(FileOperationsTestCollection))]
    public sealed class GetBasicInfo
    {
        [Fact]
        public void GetFileInfo_ValidHandle_ReturnsBasicFileInfo()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tempFile, [1, 2, 3, 4]);
                var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes, FileShare.All);
                using var handle = FileOperations.OpenHandle(tempFile, options);
                var now = SystemClock.Instance.GetCurrentInstant();
                var earliestExpectedTime = now - Duration.FromSeconds(5);

                // Act
                var result = FileOperations.GetBasicInfo(handle);

                // Assert
                Assert.Equal(4, result.Size);
                Assert.False(result.Attributes.HasFlag(FileAttributes.Directory));
                Assert.InRange(result.CreationTime!.Value, earliestExpectedTime, now);
                Assert.InRange(result.LastAccessTime, earliestExpectedTime, now);
                Assert.InRange(result.LastWriteTime, earliestExpectedTime, now);
                Assert.InRange(result.LastChangeTime, earliestExpectedTime, now);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetFileInfo_NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            var handle = (SafeFileHandle)null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileOperations.GetBasicInfo(handle));
        }

        [Fact]
        public void GetFileInfo_InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using var handle = new SafeFileHandle((nint)(-1), ownsHandle: false);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileOperations.GetBasicInfo(handle));
        }

        [Fact]
        public void GetFileInfo_DirectoryHandle_ReturnsDirectoryInfo()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            try
            {
                var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes, FileShare.All, 0, FileAttributes.Directory);
                using var handle = FileOperations.OpenHandle(tempDirectory, options);
                var now = SystemClock.Instance.GetCurrentInstant();
                var earliestExpectedTime = now - Duration.FromSeconds(5);

                // Act
                var result = FileOperations.GetBasicInfo(handle);

                // Assert
                Assert.True(result.Attributes.HasFlag(FileAttributes.Directory));
                Assert.InRange(result.CreationTime!.Value, earliestExpectedTime, now);
                Assert.InRange(result.LastAccessTime, earliestExpectedTime, now);
                Assert.InRange(result.LastWriteTime, earliestExpectedTime, now);
                Assert.InRange(result.LastChangeTime, earliestExpectedTime, now);
            }
            finally
            {
                Directory.Delete(tempDirectory);
            }
        }

    }
}
