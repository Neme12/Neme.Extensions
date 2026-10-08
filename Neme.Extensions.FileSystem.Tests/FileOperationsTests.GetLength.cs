using Microsoft.Win32.SafeHandles;
using Neme.Extensions.IO;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileOperationsTests
{
    [Collection(nameof(FileOperationsTestCollection))]
    public sealed class GetLength
    {
        [Fact]
        public void NonEmptyFile_ReturnsFileLength()
        {
            // Arrange
            byte[] contents = [1, 2, 3, 4, 5];
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(handle, contents);

            // Act
            long result = FileOperations.GetLength(handle);

            // Assert
            Assert.Equal(contents.LongLength, result);
        }

        [Fact]
        public void EmptyFile_ReturnsZero()
        {
            // Arrange
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);

            // Act
            long result = FileOperations.GetLength(handle);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void SeekedHandle_ReturnsTotalLength()
        {
            // Arrange
            byte[] contents = [10, 20, 30, 40];
            const long expectedLength = 4;
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(handle, contents);
            FileOperations.Seek(handle, 2, SeekOrigin.Begin);

            // Act
            long result = FileOperations.GetLength(handle);

            // Assert
            Assert.Equal(expectedLength, result);
        }

        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => FileOperations.GetLength(null!));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void ClosedHandle_ThrowsArgumentException()
        {
            // Arrange
            var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            handle.Dispose();

            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => FileOperations.GetLength(handle));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using var handle = new SafeFileHandle((nint)(-1), ownsHandle: false);

            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => FileOperations.GetLength(handle));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

    }
}
