using Microsoft.Win32.SafeHandles;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class SetLength
    {
        [Fact]
        public void ExtendingFile_UpdatesLengthAndZeroFillsNewBytes()
        {
            // Arrange
            byte[] initialContents = [1, 2, 3];
            byte[] expectedContents = [1, 2, 3, 0, 0];
            using var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(handle, initialContents);

            // Act
            FileIO.SetLength(handle, expectedContents.Length);

            // Assert
            Assert.Equal(expectedContents.Length, FileIO.GetLength(handle));
            using var stream = FileIO.CreateFileStream(handle, FileAccess.Read);
            stream.Position = 0;
            byte[] actualContents = new byte[expectedContents.Length];
            int bytesRead = stream.Read(actualContents, 0, actualContents.Length);
            Assert.Equal(expectedContents.Length, bytesRead);
            Assert.Equal(expectedContents, actualContents);
        }

        [Fact]
        public void TruncatingFile_ReducesLengthAndPreservesLeadingBytes()
        {
            // Arrange
            byte[] initialContents = [1, 2, 3, 4, 5];
            byte[] expectedContents = [1, 2];
            using var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(handle, initialContents);

            // Act
            FileIO.SetLength(handle, expectedContents.Length);

            // Assert
            Assert.Equal(expectedContents.Length, FileIO.GetLength(handle));
            using var stream = FileIO.CreateFileStream(handle, FileAccess.Read);
            stream.Position = 0;
            byte[] actualContents = new byte[expectedContents.Length];
            int bytesRead = stream.Read(actualContents, 0, actualContents.Length);
            Assert.Equal(expectedContents.Length, bytesRead);
            Assert.Equal(expectedContents, actualContents);
        }

        [Fact]
        public void NegativeLength_ThrowsArgumentExceptionWithoutChangingFileLength()
        {
            // Arrange
            byte[] initialContents = [1, 2, 3, 4];
            const long invalidLength = -1;
            using var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(handle, initialContents);

            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => FileIO.SetLength(handle, invalidLength));

            // Assert
            Assert.Equal("length", exception.ParamName);
            Assert.Equal(initialContents.Length, FileIO.GetLength(handle));
            using var stream = FileIO.CreateFileStream(handle, FileAccess.Read);
            stream.Position = 0;
            byte[] actualContents = new byte[initialContents.Length];
            int bytesRead = stream.Read(actualContents, 0, actualContents.Length);
            Assert.Equal(initialContents.Length, bytesRead);
            Assert.Equal(initialContents, actualContents);
        }

        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => FileIO.SetLength(null!, 0));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void ClosedHandle_ThrowsArgumentException()
        {
            // Arrange
            var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            handle.Dispose();

            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => FileIO.SetLength(handle, 0));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using var handle = new SafeFileHandle((nint)(-1), ownsHandle: false);

            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => FileIO.SetLength(handle, 0));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

    }
}
