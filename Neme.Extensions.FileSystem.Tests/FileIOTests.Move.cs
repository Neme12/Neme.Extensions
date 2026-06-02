using Microsoft.Win32.SafeHandles;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class Move
    {
        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            var sourceFile = (SafeFileHandle)null!;
            var destinationFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileIO.Move(sourceFile, destinationFile));
        }

        [Fact]
        public void EmptyDestination_ThrowsArgumentException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.ReadWrite | FileSystemAccess.Delete, FileShare.ReadWrite | FileShare.Delete);

            try
            {
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act & Assert
                Assert.Throws<ArgumentException>(() => FileIO.Move(handle, string.Empty));
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void ValidHandleAndDestination_MovesFileToDestination()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var sourceFile = Path.Combine(tempDir, "source.txt");
            var destinationFile = Path.Combine(tempDir, "destination.txt");
            File.WriteAllText(sourceFile, "content");
            var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.ReadWrite | FileSystemAccess.Delete, FileShare.ReadWrite | FileShare.Delete);

            try
            {
                using (var handle = FileIO.OpenHandle(sourceFile, options))
                {
                    // Act
                    FileIO.Move(handle, destinationFile);
                }

                // Assert
                Assert.False(File.Exists(sourceFile));
                Assert.True(File.Exists(destinationFile));
                Assert.Equal("content", File.ReadAllText(destinationFile));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
        }

        [Fact]
        public void ExistingDestinationAndOverwriteTrue_ReplacesDestination()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var sourceFile = Path.Combine(tempDir, "source.txt");
            var destinationFile = Path.Combine(tempDir, "destination.txt");
            File.WriteAllText(sourceFile, "source content");
            File.WriteAllText(destinationFile, "destination content");
            var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.ReadWrite | FileSystemAccess.Delete, FileShare.ReadWrite | FileShare.Delete);

            try
            {
                using (var handle = FileIO.OpenHandle(sourceFile, options))
                {
                    // Act
                    FileIO.Move(handle, destinationFile, overwrite: true);
                }

                // Assert
                Assert.False(File.Exists(sourceFile));

                Assert.True(File.Exists(destinationFile));
                Assert.Equal("source content", File.ReadAllText(destinationFile));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
        }

        [Fact]
        public void ExistingDestinationAndOverwriteFalse_DoesNotReplaceDestination()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var sourceFile = Path.Combine(tempDir, "source.txt");
            var destinationFile = Path.Combine(tempDir, "destination.txt");
            File.WriteAllText(sourceFile, "source content");
            File.WriteAllText(destinationFile, "destination content");
            var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.ReadWrite | FileSystemAccess.Delete, FileShare.ReadWrite | FileShare.Delete);

            try
            {
                using (var handle = FileIO.OpenHandle(sourceFile, options))
                {
                    Assert.ThrowsAny<IOException>(() => FileIO.Move(handle, destinationFile, overwrite: false));
                }

                // Assert
                Assert.True(File.Exists(sourceFile));
                Assert.Equal("source content", File.ReadAllText(sourceFile));

                Assert.True(File.Exists(destinationFile));
                Assert.Equal("destination content", File.ReadAllText(destinationFile));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
        }
    }
}
