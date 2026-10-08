using Microsoft.Win32.SafeHandles;
using Neme.Extensions.IO;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileOperationsTests
{
    [Collection(nameof(FileOperationsTestCollection))]
    public sealed class GetAccess
    {
        [Fact]
        public void ReadOnlyHandle_ReturnsRead()
        {
            // Arrange
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.Read);

            // Act
            var result = FileOperations.GetAccess(handle);

            // Assert
            Assert.Equal(FileAccess.Read, result);
        }

        [Fact]
        public void WriteOnlyHandle_ReturnsWrite()
        {
            // Arrange
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.Write);

            // Act
            var result = FileOperations.GetAccess(handle);

            // Assert
            Assert.Equal(FileAccess.Write, result);
        }

        [Fact]
        public void ReadWriteHandle_ReturnsReadWrite()
        {
            // Arrange
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadWrite);

            // Act
            var result = FileOperations.GetAccess(handle);

            // Assert
            Assert.Equal(FileAccess.ReadWrite, result);
        }

        [Fact]
        public void WriteHandleWithDelete_ReturnsWrite()
        {
            // Arrange
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.Write | FileSystemAccess.Delete);

            // Act
            var result = FileOperations.GetAccess(handle);

            // Assert
            Assert.Equal(FileAccess.Write, result);
        }

        [Fact]
        public void ReadWriteHandleWithAdditionalFlags_ReturnsReadWrite()
        {
            // Arrange
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadWrite | FileSystemAccess.Delete | FileSystemAccess.ReadAttributes);

            // Act
            var result = FileOperations.GetAccess(handle);

            // Assert
            Assert.Equal(FileAccess.ReadWrite, result);
        }

        [Fact]
        public void DeleteOnlyHandle_ReturnsNone()
        {
            // Arrange
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.Delete);

            // Act
            var result = FileOperations.GetAccess(handle);

            // Assert
            // macOS doesn't support opening a file with neither Read or Write access.
            var expected = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? FileAccess.Read : FileAccess.None;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ReadAttributesOnlyHandle_ReturnsNone()
        {
            // Arrange
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadAttributes);

            // Act
            var result = FileOperations.GetAccess(handle);

            // Assert
            // macOS doesn't support opening a file with neither Read or Write access.
            var expected = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? FileAccess.Read : FileAccess.None;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileOperations.GetAccess((SafeFileHandle)null!));
        }

        [Fact]
        public void ClosedHandle_ThrowsArgumentException()
        {
            // Arrange
            var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.Read);
            handle.Dispose();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileOperations.GetAccess(handle));
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using var handle = new SafeFileHandle((nint)(-1), ownsHandle: false);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileOperations.GetAccess(handle));
        }
    }
}
