using Neme.Extensions.Tests.Utilities;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileOperationsTests
{
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [Collection(nameof(FileOperationsTestCollection))]
    public sealed class GetPath_PersistentFileId
    {
        [PlatformOnlyFact(Platform.Windows)]
        public void GetPath_WithValidFileId_ReturnsPath()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                PersistentFileId fileId;
                using (var fileStream = File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fileId = FileOperations.GetPersistentId(fileStream.SafeFileHandle);
                }

                // Act
                var result = FileOperations.GetPath(fileId);

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [PlatformOnlyFact(Platform.Windows)]
        public void GetPath_WithFileIdFromExistingFile_ReturnsCorrectPath()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, "test content");

                PersistentFileId fileId;
                using (var fileStream = File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fileId = FileOperations.GetPersistentId(fileStream.SafeFileHandle);
                }

                // Act
                var result = FileOperations.GetPath(fileId);

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
