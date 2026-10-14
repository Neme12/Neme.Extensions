using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.IO;
using Neme.Extensions.Tests.Utilities;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileOperationsTests
{
    [Collection(nameof(FileOperationsTestCollection))]
    public sealed class Delete
    {
        [Fact]
        public void WithOpenHandle_RemovesDirectoryEntry()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = FileHandleRequest.Open(FileSystemAccess.ReadWrite | FileSystemAccess.Delete, FileShare.All);

                using (var handle = FileOperations.OpenHandle(tempFile, options))
                {
                    // Act
                    FileOperations.Delete(handle);
                }

                // Assert
                Assert.False(File.Exists(tempFile));
                Assert.Throws<FileNotFoundException>(() =>
                    File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.All));
            }
            finally
            {
                File.DeleteIfExists(tempFile);
            }
        }

        [Fact]
        public void WithOpenHandle_LeavesHandleUsableUntilClosed()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = FileHandleRequest.Open(FileSystemAccess.ReadWrite | FileSystemAccess.Delete, FileShare.All);
                using var handle = FileOperations.OpenHandle(tempFile, options);

                // Act
                FileOperations.Delete(handle);

                // Assert
                using (var stream = handle.CreateFileStream(FileAccess.ReadWrite, bufferSize: 128))
                {
                    stream.WriteByte(123);
                    stream.Position = 0;
                    var result = stream.ReadByte();
                    Assert.Equal(123, result);
                }
            }
            finally
            {
                File.DeleteIfExists(tempFile);
            }
        }

        [Fact]
        public void WithoutDeleteAccess_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = FileHandleRequest.Open(FileSystemAccess.ReadWrite, FileShare.All);
                using var handle = FileOperations.OpenHandle(tempFile, options);

                // Act & Assert
                Assert.Throws<UnauthorizedAccessException>(() => FileOperations.Delete(handle));
                Assert.True(File.Exists(tempFile));
            }
            finally
            {
                File.DeleteIfExists(tempFile);
            }
        }
    }
}
