using Microsoft.Win32.SafeHandles;
using Neme.Extensions.IO;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileOperationsTests
{
    [Collection(nameof(FileOperationsTestCollection))]
    public sealed class GetAttributes
    {
        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            var handle = (SafeFileHandle)null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileOperations.GetAttributes(handle));
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using var handle = new SafeFileHandle((nint)(-1), ownsHandle: false);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileOperations.GetAttributes(handle));
        }

        [Fact]
        public void DirectoryHandle_ReturnsDirectoryAttribute()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            try
            {
                var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes, FileShare.All, FileHandleType.Directory);
                using var handle = FileOperations.OpenHandle(tempDirectory, options);

                // Act
                var attributes = FileOperations.GetAttributes(handle);

                // Assert
                Assert.Equal(FileHandleType.Directory, handle.Type);
                Assert.True(attributes.HasFlag(FileAttributes.Directory));
            }
            finally
            {
                Directory.DeleteIfExists(tempDirectory);
            }
        }
    }
}
