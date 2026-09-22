using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Tests.Utilities;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class CreateFileStream
    {
        [Fact]
        public void OwnsHandleTrue_CreatesReadWriteAsyncStreamAndClosesHandle()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.None, FileOptions.Asynchronous);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                using (var stream = FileIO.CreateFileStream(handle, FileAccess.ReadWrite, ownsHandle: true, bufferSize: 128))
                {
                    stream.WriteByte(123);
                    stream.Position = 0;
                    var result = stream.ReadByte();

                    // Assert
                    Assert.True(stream.CanRead);
                    Assert.True(stream.CanWrite);
                    Assert.True(stream.IsAsync);
                    Assert.Equal(123, result);
                }

                Assert.True(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void OwnsHandleFalse_LeavesOriginalHandleOpen()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tempFile, [42]);
                var options = FileOpenOptions.Open(FileSystemAccess.Read);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                using (var stream = FileIO.CreateFileStream(handle, FileAccess.Read, bufferSize: 128))
                {
                    var result = stream.ReadByte();

                    // Assert
                    Assert.True(stream.CanRead);
                    Assert.False(stream.CanWrite);
                    Assert.False(stream.IsAsync);
                    Assert.Equal(42, result);
                }

                Assert.False(handle.IsClosed);
                Assert.False(handle.IsInvalid);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void WriteOnlyOptions_CreatesWriteOnlyStream()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                using (var stream = FileIO.CreateFileStream(handle, FileAccess.Write, bufferSize: 128))
                {
                    stream.WriteByte(99);

                    // Assert
                    Assert.False(stream.CanRead);
                    Assert.True(stream.CanWrite);
                    Assert.False(stream.IsAsync);
                }

                Assert.Equal([99], FileIO.ReadAllBytes(handle));
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            var handle = new SafeFileHandle((nint)(-1), ownsHandle: false);
            var options = FileOpenOptions.Open(FileSystemAccess.Read);
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileIO.CreateFileStream(handle, FileAccess.Read));
        }
    }
}
