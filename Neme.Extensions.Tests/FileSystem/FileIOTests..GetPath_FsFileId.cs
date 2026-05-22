using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem.Tests;

[SupportedOSPlatform("windows6.0.6000")]
public sealed partial class FileIOTests
{
    [SupportedOSPlatform("windows6.0.6000")]
    public sealed class GetPath_FsFileId
    {
        [Fact]
        public void GetPath_WithValidFileId_ReturnsPath()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                FsFileId fileId;
                using (var fileStream = File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fileId = FileIO.GetFileId(fileStream.SafeFileHandle);
                }

                // Act
                var result = FileIO.GetPath(fileId);

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetPath_WithFileIdFromExistingFile_ReturnsCorrectPath()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, "test content");

                FsFileId fileId;
                using (var fileStream = File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fileId = FileIO.GetFileId(fileStream.SafeFileHandle);
                }

                // Act
                var result = FileIO.GetPath(fileId);

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.DoesNotContain(@"\\?\", result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
