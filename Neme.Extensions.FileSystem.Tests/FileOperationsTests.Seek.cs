using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.IO;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileOperationsTests
{
    [Collection(nameof(FileOperationsTestCollection))]
    public sealed class Seek
    {
        [Fact]
        public void FromBeginning_ReturnsUpdatedPosition()
        {
            // Arrange
            byte[] contents = [10, 20, 30, 40];
            const long expectedPosition = 2;
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(handle, contents);

            // Act
            long result = FileOperations.Seek(handle, expectedPosition, SeekOrigin.Begin);

            // Assert
            Assert.Equal(expectedPosition, result);
            using var stream = handle.CreateFileStream(FileAccess.Read);
            Assert.Equal(contents[(int)expectedPosition], stream.ReadByte());
        }

        [Fact]
        public void FromEnd_ReturnsUpdatedPosition()
        {
            // Arrange
            byte[] contents = [10, 20, 30, 40];
            const long offset = -2;
            const long expectedPosition = 2;
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(handle, contents);

            // Act
            long result = FileOperations.Seek(handle, offset, SeekOrigin.End);

            // Assert
            Assert.Equal(expectedPosition, result);
            using var stream = handle.CreateFileStream(FileAccess.Read);
            Assert.Equal(contents[(int)expectedPosition], stream.ReadByte());
        }

        [Fact]
        public void FromCurrent_ReturnsUpdatedPosition()
        {
            // Arrange
            byte[] contents = [10, 20, 30, 40];
            const long initialPosition = 1;
            const long offset = 2;
            const long expectedPosition = initialPosition + offset;
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(handle, contents);
            FileOperations.Seek(handle, initialPosition, SeekOrigin.Begin);

            // Act
            long result = FileOperations.Seek(handle, offset, SeekOrigin.Current);

            // Assert
            Assert.Equal(expectedPosition, result);
            using var stream = handle.CreateFileStream(FileAccess.Read);
            Assert.Equal(contents[(int)expectedPosition], stream.ReadByte());
        }

        [Fact]
        public void SeekingTwice_FromCurrent_UsesUpdatedPosition()
        {
            // Arrange
            byte[] contents = [10, 20, 30, 40, 50];
            const long firstPosition = 1;
            const long secondOffset = 2;
            const long expectedPosition = firstPosition + secondOffset;
            using var handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(handle, contents);

            // Act
            long firstResult = FileOperations.Seek(handle, firstPosition, SeekOrigin.Begin);
            long secondResult = FileOperations.Seek(handle, secondOffset, SeekOrigin.Current);

            // Assert
            Assert.Equal(firstPosition, firstResult);
            Assert.Equal(expectedPosition, secondResult);
            using var stream = handle.CreateFileStream(FileAccess.Read);
            Assert.Equal(contents[(int)expectedPosition], stream.ReadByte());
        }

        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => FileOperations.Seek(null!, 0, SeekOrigin.Begin));

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
            ArgumentException exception = Assert.Throws<ArgumentException>(() => FileOperations.Seek(handle, 0, SeekOrigin.Begin));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using var handle = new SafeFileHandle((nint)(-1), ownsHandle: false);

            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => FileOperations.Seek(handle, 0, SeekOrigin.Begin));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }
    }
}
