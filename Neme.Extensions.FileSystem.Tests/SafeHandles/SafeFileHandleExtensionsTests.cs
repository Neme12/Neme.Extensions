using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.IO;
using IOPath = System.IO.Path;

namespace Neme.Extensions.FileSystem.Tests.SafeHandles;

public sealed class SafeFileHandleExtensionsTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class Path
    {
        [Fact]
        public void ValidFileHandle_ReturnsFullPath()
        {
            // Arrange
            var expected =  IOPath.GetFullPath($"{System.IO.Path.GetTempPath()}{nameof(SafeFileHandleExtensionsTests)}_{Guid.NewGuid():N}.tmp");
            string result;
            using (var tempFile = FileIO.OpenHandle(expected, FileOpenRequest.CreateNew(FileSystemAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete, FileOptions.DeleteOnClose, FileAttributes.Temporary)))
            {
                FileIO.WriteAllBytes(tempFile, "test"u8.ToArray());

                using (var handle = FileIO.OpenHandle(expected, FileOpenRequest.Open(FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete)))
                {
                    // Act
                    result = handle.Path;
                }
            }

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            var handle = (SafeFileHandle)null!;

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                _ = handle.Path;
            });

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void ClosedHandle_ThrowsArgumentException()
        {
            // Arrange
            var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Read);
            handle.Dispose();

            // Act
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                _ = handle.Path;
            });

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using (var handle = new SafeFileHandle((nint)(-1), ownsHandle: false))
            {
                // Act
                var exception = Assert.Throws<ArgumentException>(() =>
                {
                    _ = handle.Path;
                });

                // Assert
                Assert.Equal("file", exception.ParamName);
            }
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class Access
    {
        [Fact]
        public void ReadOnlyHandle_ReturnsRead()
        {
            // Arrange
            FileAccess result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Read))
                result = handle.Access;

            // Assert
            Assert.Equal(FileAccess.Read, result);
        }

        [Fact]
        public void WriteOnlyHandle_ReturnsWrite()
        {
            // Arrange
            FileAccess result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Write))
                result = handle.Access;

            // Assert
            Assert.Equal(FileAccess.Write, result);
        }

        [Fact]
        public void ReadWriteHandle_ReturnsReadWrite()
        {
            // Arrange
            FileAccess result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite))
                result = handle.Access;

            // Assert
            Assert.Equal(FileAccess.ReadWrite, result);
        }

        [Fact]
        public void DeleteOnlyHandle_ReturnsNone()
        {
            // Arrange
            FileAccess result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Delete))
                result = handle.Access;

            // Assert
            Assert.Equal(FileAccess.None, result);
        }

        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            var handle = (SafeFileHandle)null!;

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                _ = handle.Access;
            });

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void ClosedHandle_ThrowsArgumentException()
        {
            // Arrange
            var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Read);
            handle.Dispose();

            // Act
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                _ = handle.Access;
            });

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using (var handle = new SafeFileHandle((nint)(-1), ownsHandle: false))
            {
                // Act
                var exception = Assert.Throws<ArgumentException>(() =>
                {
                    _ = handle.Access;
                });

                // Assert
                Assert.Equal("file", exception.ParamName);
            }
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class CanRead
    {
        [Fact]
        public void ReadOnlyHandle_ReturnsTrue()
        {
            // Arrange
            bool result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Read))
                result = handle.CanRead;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void WriteOnlyHandle_ReturnsFalse()
        {
            // Arrange
            bool result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Write))
                result = handle.CanRead;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ReadWriteHandle_ReturnsTrue()
        {
            // Arrange
            bool result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite))
                result = handle.CanRead;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DeleteOnlyHandle_ReturnsFalse()
        {
            // Arrange
            bool result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Delete))
                result = handle.CanRead;

            // Assert
            Assert.False(result);
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class CanWrite
    {
        [Fact]
        public void ReadOnlyHandle_ReturnsFalse()
        {
            // Arrange
            bool result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Read))
                result = handle.CanWrite;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WriteOnlyHandle_ReturnsTrue()
        {
            // Arrange
            bool result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Write))
                result = handle.CanWrite;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ReadWriteHandle_ReturnsTrue()
        {
            // Arrange
            bool result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite))
                result = handle.CanWrite;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DeleteOnlyHandle_ReturnsFalse()
        {
            // Arrange
            bool result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.Delete))
                result = handle.CanWrite;

            // Assert
            Assert.False(result);
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class CanSeek
    {
        [Fact]
        public void RegularFileHandle_ReturnsTrue()
        {
            // Arrange
            bool result;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All))
                result = handle.CanSeek;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AnonymousPipeHandle_ReturnsFalse()
        {
            // Arrange
            bool result;
            using (var pipe = new System.IO.Pipes.AnonymousPipeServerStream(System.IO.Pipes.PipeDirection.Out, System.IO.HandleInheritability.None))
            using (var handle = new SafeFileHandle(pipe.SafePipeHandle.DangerousGetHandle(), ownsHandle: false))
            {
                // Act
                result = handle.CanSeek;
            }

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            var handle = (SafeFileHandle)null!;

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                _ = handle.CanSeek;
            });

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
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                _ = handle.CanSeek;
            });

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using (var handle = new SafeFileHandle((nint)(-1), ownsHandle: false))
            {
                // Act
                var exception = Assert.Throws<ArgumentException>(() =>
                {
                    _ = handle.CanSeek;
                });

                // Assert
                Assert.Equal("file", exception.ParamName);
            }
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class Position
    {
        [Fact]
        public void SettingPosition_UpdatesCurrentOffset()
        {
            // Arrange
            var contents = new byte[] { 10, 20, 30, 40 };
            const long expectedPosition = 2;
            long result;
            int nextByte;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All))
            {
                FileIO.WriteAllBytes(handle, contents);

                // Act
                handle.Position = expectedPosition;
                result = handle.Position;
                using (var stream = FileIO.CreateFileStream(handle, FileAccess.Read))
                    nextByte = stream.ReadByte();
            }

            // Assert
            Assert.Equal(expectedPosition, result);
            Assert.Equal(contents[(int)expectedPosition], nextByte);
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class Length
    {
        [Fact]
        public void ExtendingFile_UpdatesLengthAndZeroFillsNewBytes()
        {
            // Arrange
            var initialContents = new byte[] { 1, 2, 3 };
            var expectedContents = new byte[] { 1, 2, 3, 0, 0 };
            long result;
            byte[] actualContents;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All))
            {
                FileIO.WriteAllBytes(handle, initialContents);

                // Act
                handle.Length = expectedContents.Length;
                result = handle.Length;
                using (var stream = FileIO.CreateFileStream(handle, FileAccess.Read))
                {
                    stream.Position = 0;
                    actualContents = stream.ReadToEnd();
                }
            }

            // Assert
            Assert.Equal(expectedContents.LongLength, result);
            Assert.Equal(expectedContents, actualContents);
        }

        [Fact]
        public void TruncatingFile_ReducesLengthAndPreservesLeadingBytes()
        {
            // Arrange
            var initialContents = new byte[] { 1, 2, 3, 4, 5 };
            var expectedContents = new byte[] { 1, 2 };
            long result;
            byte[] actualContents;
            using (var handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All))
            {
                FileIO.WriteAllBytes(handle, initialContents);

                // Act
                handle.Length = expectedContents.Length;
                result = handle.Length;
                using (var stream = FileIO.CreateFileStream(handle, FileAccess.Read))
                {
                    stream.Position = 0;
                    actualContents = stream.ReadToEnd();
                }
            }

            // Assert
            Assert.Equal(expectedContents.LongLength, result);
            Assert.Equal(expectedContents, actualContents);
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class OpenedPath
    {
        [Fact]
        public void ValidFileHandle_ReturnsExpectedOpenedPath()
        {
            // Arrange
            var expected = $"{System.IO.Path.GetTempPath()}{nameof(SafeFileHandleExtensionsTests)}_{Guid.NewGuid():N}.tmp";
            string? result;
            using (var tempFile = FileIO.OpenHandle(expected, FileOpenRequest.CreateNew(FileSystemAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete, FileOptions.DeleteOnClose, FileAttributes.Temporary)))
            {
                FileIO.WriteAllBytes(tempFile, "test"u8.ToArray());

                using (var handle = FileIO.OpenHandle(expected, FileOpenRequest.Open(FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete)))
                {
                    // Act
                    result = handle.OpenedPath;
                }
            }

            // Assert
#if NET6_0_OR_GREATER
            Assert.Equal(expected, result);
#else
            Assert.Null(result);
#endif
        }
    }


}
