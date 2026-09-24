using Microsoft.Win32.SafeHandles;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class GetAccess
    {
        [Fact]
        public void ReadOnlyHandle_ReturnsRead()
        {
            // Arrange
            using var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Read);

            // Act
            var result = FileIO.GetAccess(handle);

            // Assert
            Assert.Equal(FileAccess.Read, result);
        }

        [Fact]
        public void WriteOnlyHandle_ReturnsWrite()
        {
            // Arrange
            using var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Write);

            // Act
            var result = FileIO.GetAccess(handle);

            // Assert
            Assert.Equal(FileAccess.Write, result);
        }

        [Fact]
        public void ReadWriteHandle_ReturnsReadWrite()
        {
            // Arrange
            using var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite);

            // Act
            var result = FileIO.GetAccess(handle);

            // Assert
            Assert.Equal(FileAccess.ReadWrite, result);
        }

        [Fact]
        public void WriteHandleWithDelete_ReturnsWrite()
        {
            // Arrange
            using var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Write | FileSystemAccess.Delete);

            // Act
            var result = FileIO.GetAccess(handle);

            // Assert
            Assert.Equal(FileAccess.Write, result);
        }

        [Fact]
        public void ReadWriteHandleWithAdditionalFlags_ReturnsReadWrite()
        {
            // Arrange
            using var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite | FileSystemAccess.Delete | FileSystemAccess.ReadAttributes);

            // Act
            var result = FileIO.GetAccess(handle);

            // Assert
            Assert.Equal(FileAccess.ReadWrite, result);
        }

        [Fact]
        public void DeleteOnlyHandle_ReturnsNone()
        {
            // Arrange
            using var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Delete);

            // Act
            var result = FileIO.GetAccess(handle);

            // Assert
            Assert.Equal(FileAccess.None, result);
        }

        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileIO.GetAccess((SafeFileHandle)null!));
        }

        [Fact]
        public void ClosedHandle_ThrowsArgumentException()
        {
            // Arrange
            var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Read);
            handle.Dispose();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileIO.GetAccess(handle));
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using var handle = new SafeFileHandle((nint)(-1), ownsHandle: false);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileIO.GetAccess(handle));
        }
    }
}
