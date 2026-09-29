using Microsoft.Win32.SafeHandles;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileOperationsTests
{
    [Collection(nameof(FileOperationsTestCollection))]
    public sealed class CanSeek
    {
        [Fact]
        public void RegularFile_ReturnsTrue()
        {
            // Arrange
            using (SafeFileHandle handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All))
            {
                // Act
                bool result = FileOperations.CanSeek(handle);

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public void NonSeekableHandle_ReturnsFalse()
        {
            // Arrange
            using (var pipe = new System.IO.Pipes.AnonymousPipeServerStream(System.IO.Pipes.PipeDirection.Out, System.IO.HandleInheritability.None))
            using (SafeFileHandle handle = new(pipe.SafePipeHandle.DangerousGetHandle(), ownsHandle: false))
            {
                // Act
                bool result = FileOperations.CanSeek(handle);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => FileOperations.CanSeek(null!));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void ClosedHandle_ThrowsArgumentException()
        {
            // Arrange
            SafeFileHandle handle = FileOperations.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            handle.Dispose();

            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => FileOperations.CanSeek(handle));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using (SafeFileHandle handle = new((nint)(-1), ownsHandle: false))
            {
                // Act
                ArgumentException exception = Assert.Throws<ArgumentException>(() => FileOperations.CanSeek(handle));

                // Assert
                Assert.Equal("file", exception.ParamName);
            }
        }
    }

}
