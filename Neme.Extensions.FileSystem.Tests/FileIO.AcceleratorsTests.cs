namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class CreateTempFileHandle
    {
        [Fact]
        public void DeleteOnCloseDefault_CreatesGuidNamedTempFileThatIsDeletedWhenClosed()
        {
            // Arrange
            string? createdPath = null;
            var access = FileSystemAccess.ReadWrite;
            var share = FileShare.All;

            // Act
            using (var handle = FileIO.CreateTempFileHandle(access, share))
            {
                createdPath = FileIO.GetPath(handle);

                // Assert
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
                Assert.NotNull(createdPath);
                Assert.Equal(".tmp", Path.GetExtension(createdPath));
                Assert.True(Guid.TryParse(Path.GetFileNameWithoutExtension(createdPath), out _));

                var expectedDirectory = Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var actualDirectory = Path.GetDirectoryName(createdPath)!.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var comparison = Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

                Assert.True(string.Equals(expectedDirectory, actualDirectory, comparison));
            }

            Assert.NotNull(createdPath);
            Assert.False(File.Exists(createdPath));
        }

        [Fact]
        public void AsynchronousOptionWithoutDeleteOnClose_CreatesUsableFileThatPersistsAfterHandleClosed()
        {
            // Arrange
            string? createdPath = null;
            byte[] expected = [7, 8, 9];
            var access = FileSystemAccess.ReadWrite;
            var share = FileShare.All;
            var options = FileOptions.Asynchronous;
            var attributes = FileAttributes.Normal;

            try
            {
                using (var handle = FileIO.CreateTempFileHandle(access, share, options, attributes))
                {
                    createdPath = FileIO.GetPath(handle);
                    var fileOptions = FileOpenOptions.Open(access, share, options, attributes);
                    using var stream = FileIO.CreateFileStream(handle, FileAccess.ReadWrite, leaveOpen: true, bufferSize: 128);

                    // Act
                    stream.Write(expected, 0, expected.Length);
                    stream.Position = 0;
                    byte[] actual = new byte[expected.Length];
                    var bytesRead = stream.Read(actual, 0, actual.Length);

                    // Assert
                    Assert.True(stream.CanRead);
                    Assert.True(stream.CanWrite);
                    Assert.True(stream.IsAsync);
                    Assert.Equal(expected.Length, bytesRead);
                    Assert.Equal(expected, actual);
                }

                Assert.NotNull(createdPath);
                Assert.True(File.Exists(createdPath));
                Assert.Equal(expected, File.ReadAllBytes(createdPath));
            }
            finally
            {
                if (createdPath is not null && File.Exists(createdPath))
                    File.Delete(createdPath);
            }
        }
    }
}
