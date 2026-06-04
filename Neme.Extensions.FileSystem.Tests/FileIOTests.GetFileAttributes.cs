using Microsoft.Win32.SafeHandles;
using Neme.Extensions.IO;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class GetFileAttributes
    {
        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            var handle = (SafeFileHandle)null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileIO.GetFileAttributes(handle));
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using var handle = new SafeFileHandle((nint)(-1), ownsHandle: false);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileIO.GetFileAttributes(handle));
        }

        [Fact]
        public void DirectoryHandle_ReturnsDirectoryAttribute()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            try
            {
                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.ReadAttributes, FileShare.ReadWrite | FileShare.Delete)
                {
                    Attributes = FileAttributes.Directory
                };
                using var handle = FileIO.OpenHandle(tempDirectory, options);

                // Act
                var attributes = FileIO.GetFileAttributes(handle);

                // Assert
                Assert.True(attributes.HasFlag(FileAttributes.Directory));
            }
            finally
            {
                Directory.DeleteIfExists(tempDirectory);
            }
        }
    }
}
