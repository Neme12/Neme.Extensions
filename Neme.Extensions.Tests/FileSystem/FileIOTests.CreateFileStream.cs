using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Tests.Utilities;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class CreateFileStream
    {
        [WindowsOnlyFact]
        public void LeaveOpenFalse_CreatesReadWriteAsyncStreamAndClosesHandle()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadWrite)
                {
                    Options = FileOptions.Asynchronous,
                };
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                using (var stream = FileIO.CreateFileStream(handle, options, bufferSize: 128))
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

        [WindowsOnlyFact]
        public void LeaveOpenTrue_LeavesOriginalHandleOpen()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tempFile, [42]);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                using (var stream = FileIO.CreateFileStream(handle, options, leaveOpen: true, bufferSize: 128))
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

        [WindowsOnlyFact]
        public void WriteOnlyOptions_CreatesWriteOnlyStream()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Write);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                using (var stream = FileIO.CreateFileStream(handle, options, bufferSize: 128))
                {
                    stream.WriteByte(99);

                    // Assert
                    Assert.False(stream.CanRead);
                    Assert.True(stream.CanWrite);
                    Assert.False(stream.IsAsync);
                }

                Assert.Equal([99], File.ReadAllBytes(tempFile));
                Assert.True(handle.IsClosed);
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
            var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileIO.CreateFileStream(handle, options));
        }
    }
}
