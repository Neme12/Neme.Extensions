using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.SafeHandles;

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
            var expected = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{nameof(SafeFileHandleExtensionsTests)}_{Guid.NewGuid():N}.tmp"));
            File.WriteAllText(expected, "test");

            try
            {
                using (SafeFileHandle handle = FileIO.OpenHandle(expected, FileOpenRequest.Open(FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete)))
                {
                    // Act
                    var result = handle.Path;

                    // Assert
                    Assert.Equal(expected, result);
                }
            }
            finally
            {
                if (File.Exists(expected))
                {
                    File.Delete(expected);
                }
            }
        }

        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            SafeFileHandle handle = null!;

            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
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
            SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.Read);
            handle.Dispose();

            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
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
            using (SafeFileHandle handle = new((nint)(-1), ownsHandle: false))
            {
                // Act
                ArgumentException exception = Assert.Throws<ArgumentException>(() =>
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
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.Read))
            {
                // Act
                FileAccess result = handle.Access;

                // Assert
                Assert.Equal(FileAccess.Read, result);
            }
        }

        [Fact]
        public void WriteOnlyHandle_ReturnsWrite()
        {
            // Arrange
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.Write))
            {
                // Act
                FileAccess result = handle.Access;

                // Assert
                Assert.Equal(FileAccess.Write, result);
            }
        }

        [Fact]
        public void ReadWriteHandle_ReturnsReadWrite()
        {
            // Arrange
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite))
            {
                // Act
                FileAccess result = handle.Access;

                // Assert
                Assert.Equal(FileAccess.ReadWrite, result);
            }
        }

        [Fact]
        public void DeleteOnlyHandle_ReturnsNone()
        {
            // Arrange
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.Delete))
            {
                // Act
                FileAccess result = handle.Access;

                // Assert
                Assert.Equal(FileAccess.None, result);
            }
        }

        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            SafeFileHandle handle = null!;

            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
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
            SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.Read);
            handle.Dispose();

            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
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
            using (SafeFileHandle handle = new((nint)(-1), ownsHandle: false))
            {
                // Act
                ArgumentException exception = Assert.Throws<ArgumentException>(() =>
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
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.Read))
            {
                // Act
                bool result = handle.CanRead;

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public void WriteOnlyHandle_ReturnsFalse()
        {
            // Arrange
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.Write))
            {
                // Act
                bool result = handle.CanRead;

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public void ReadWriteHandle_ReturnsTrue()
        {
            // Arrange
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite))
            {
                // Act
                bool result = handle.CanRead;

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public void DeleteOnlyHandle_ReturnsFalse()
        {
            // Arrange
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.Delete))
            {
                // Act
                bool result = handle.CanRead;

                // Assert
                Assert.False(result);
            }
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class CanWrite
    {
        [Fact]
        public void ReadOnlyHandle_ReturnsFalse()
        {
            // Arrange
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.Read))
            {
                // Act
                bool result = handle.CanWrite;

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public void WriteOnlyHandle_ReturnsTrue()
        {
            // Arrange
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.Write))
            {
                // Act
                bool result = handle.CanWrite;

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public void ReadWriteHandle_ReturnsTrue()
        {
            // Arrange
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite))
            {
                // Act
                bool result = handle.CanWrite;

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public void DeleteOnlyHandle_ReturnsFalse()
        {
            // Arrange
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.Delete))
            {
                // Act
                bool result = handle.CanWrite;

                // Assert
                Assert.False(result);
            }
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class CanSeek
    {
        [Fact]
        public void RegularFileHandle_ReturnsTrue()
        {
            // Arrange
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All))
            {
                // Act
                bool result = handle.CanSeek;

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public void AnonymousPipeHandle_ReturnsFalse()
        {
            // Arrange
            using (var pipe = new System.IO.Pipes.AnonymousPipeServerStream(System.IO.Pipes.PipeDirection.Out, System.IO.HandleInheritability.None))
            using (SafeFileHandle handle = new(pipe.SafePipeHandle.DangerousGetHandle(), ownsHandle: false))
            {
                // Act
                bool result = handle.CanSeek;

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            SafeFileHandle handle = null!;

            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
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
            SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            handle.Dispose();

            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
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
            using (SafeFileHandle handle = new((nint)(-1), ownsHandle: false))
            {
                // Act
                ArgumentException exception = Assert.Throws<ArgumentException>(() =>
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
            byte[] contents = [10, 20, 30, 40];
            const long expectedPosition = 2;
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All))
            {
                FileIO.WriteAllBytes(handle, contents);

                // Act
                handle.Position = expectedPosition;
                long result = handle.Position;

                // Assert
                Assert.Equal(expectedPosition, result);
                using (FileStream stream = FileIO.CreateFileStream(handle, FileAccess.Read))
                {
                    Assert.Equal(contents[(int)expectedPosition], stream.ReadByte());
                }
            }
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class Length
    {
        [Fact]
        public void ExtendingFile_UpdatesLengthAndZeroFillsNewBytes()
        {
            // Arrange
            byte[] initialContents = [1, 2, 3];
            byte[] expectedContents = [1, 2, 3, 0, 0];
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All))
            {
                FileIO.WriteAllBytes(handle, initialContents);

                // Act
                handle.Length = expectedContents.Length;
                long result = handle.Length;

                // Assert
                Assert.Equal(expectedContents.LongLength, result);
                using (FileStream stream = FileIO.CreateFileStream(handle, FileAccess.Read))
                {
                    stream.Position = 0;
                    byte[] actualContents = new byte[expectedContents.Length];
                    int bytesRead = stream.Read(actualContents, 0, actualContents.Length);
                    Assert.Equal(expectedContents.Length, bytesRead);
                    Assert.Equal(expectedContents, actualContents);
                }
            }
        }

        [Fact]
        public void TruncatingFile_ReducesLengthAndPreservesLeadingBytes()
        {
            // Arrange
            byte[] initialContents = [1, 2, 3, 4, 5];
            byte[] expectedContents = [1, 2];
            using (SafeFileHandle handle = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All))
            {
                FileIO.WriteAllBytes(handle, initialContents);

                // Act
                handle.Length = expectedContents.Length;
                long result = handle.Length;

                // Assert
                Assert.Equal(expectedContents.LongLength, result);
                using (FileStream stream = FileIO.CreateFileStream(handle, FileAccess.Read))
                {
                    stream.Position = 0;
                    byte[] actualContents = new byte[expectedContents.Length];
                    int bytesRead = stream.Read(actualContents, 0, actualContents.Length);
                    Assert.Equal(expectedContents.Length, bytesRead);
                    Assert.Equal(expectedContents, actualContents);
                }
            }
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class SourcePath
    {
        [Fact]
        public void ValidFileHandle_ReturnsExpectedSourcePath()
        {
            // Arrange
            string expected = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{nameof(SafeFileHandleExtensionsTests)}_{Guid.NewGuid():N}.tmp"));
            File.WriteAllText(expected, "test");

            try
            {
                using (SafeFileHandle handle = FileIO.OpenHandle(expected, FileOpenRequest.Open(FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete)))
                {
                    // Act
                    string? result = handle.SourcePath;

                    // Assert
#if NET6_0_OR_GREATER
                    Assert.Equal(expected, result);
#else
                    Assert.Null(result);
#endif
                }
            }
            finally
            {
                if (File.Exists(expected))
                {
                    File.Delete(expected);
                }
            }
        }
    }


}
