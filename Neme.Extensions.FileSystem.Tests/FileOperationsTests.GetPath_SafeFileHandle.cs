using Microsoft.Win32.SafeHandles;
using Neme.Extensions.IO;
using Neme.Extensions.Tests.Utilities;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileOperationsTests
{
    [Collection(nameof(FileOperationsTestCollection))]
    public sealed class GetPath_SafeFileHandle
    {
        [Fact]
        public void GetPath_WithValidFileHandle_ReturnsPath()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                using var fileStream = File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var handle = fileStream.SafeFileHandle;

                // Act
                var result = FileOperations.GetPath(handle);

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.DoesNotContain(@"\\?\", result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetPath_WithNullHandle_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileOperations.GetPath((SafeFileHandle)null!));
        }

        [Fact]
        public void GetPath_WithClosedHandle_ThrowsArgumentException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            SafeFileHandle handle;
            using (var fileStream = File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                handle = fileStream.SafeFileHandle;
            }

            try
            {
                // Act & Assert
                Assert.Throws<ArgumentException>(() => FileOperations.GetPath(handle));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetPath_WithInvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            var handle = new SafeFileHandle((nint)(-1), false);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileOperations.GetPath(handle));
        }

        [Fact]
        public void GetPath_ReturnsPathWithoutPrefix()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                using var fileStream = File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var handle = fileStream.SafeFileHandle;

                // Act
                var result = FileOperations.GetPath(handle);

                // Assert
                Assert.DoesNotContain(@"\\?\", result);
                Assert.NotEmpty(result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetPath_WithLongPath_ReturnsFullPath()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var longPath = Path.Combine(tempDir, Guid.NewGuid().ToString());

            try
            {
                Directory.CreateDirectory(longPath);
                var tempFile = Path.Combine(longPath, "test.txt");
                File.WriteAllText(tempFile, "test");

                using var fileStream = File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var handle = fileStream.SafeFileHandle;

                // Act
                var result = FileOperations.GetPath(handle);

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.DoesNotContain(@"\\?\", result);
            }
            finally
            {
                Directory.DeleteIfExists(longPath, recursive: true);
            }
        }
    }
}
