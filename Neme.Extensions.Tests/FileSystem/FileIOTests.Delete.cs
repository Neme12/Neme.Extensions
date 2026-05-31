using Neme.Extensions.Tests.Utilities;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class Delete
    {
        [PlatformOnlyFact(Platform.Windows)]
        public void WithOpenHandle_RemovesDirectoryEntry()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                FileIO.Delete(handle);

                // Assert
                Assert.False(File.Exists(tempFile));
                Assert.Throws<FileNotFoundException>(() =>
                    File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete));
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [PlatformOnlyFact(Platform.Windows)]
        public void WithOpenHandle_LeavesHandleUsableUntilClosed()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                FileIO.Delete(handle);

                // Assert
                using (var stream = FileIO.CreateFileStream(handle, options, leaveOpen: true, bufferSize: 128))
                {
                    stream.WriteByte(123);
                    stream.Position = 0;
                    var result = stream.ReadByte();
                    Assert.Equal(123, result);
                }
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}
