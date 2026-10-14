using Microsoft.Win32.SafeHandles;
using Neme.Extensions.IO;
using Neme.Extensions.Tests.Utilities;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileOperationsTests
{
    [Collection(nameof(FileOperationsTestCollection))]
    public sealed class OpenHandleAt
    {
        [Fact]
        public void BothParametersNull_ThrowsArgumentException()
        {
            // Arrange
            SafeFileHandle? rootDirectory = null;
            string? path = null;
            var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                FileOperations.OpenHandleAt(rootDirectory, path, options));
            Assert.Contains("rootDirectory", ex.Message);
            Assert.Contains("path", ex.Message);
        }

        [Fact]
        public void RootDirectoryNull_PathProvided_OpensFileRegularly()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, tempFile, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void RootDirectoryNull_PathProvided_OpensDirectoryRegularly()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes, FileShare.Read, FileHandleType.Directory);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, tempDir, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
                Assert.Equal(FileHandleType.Directory, handle.Type);
                Assert.True(FileOperations.GetAttributes(handle).HasFlag(FileAttributes.Directory));
            }
            finally
            {
                Directory.DeleteIfExists(tempDir, recursive: true);
            }
        }

        [Fact(Skip = "Causes race condition in CI by changing process-wide current directory")]
        public void RootDirectoryNull_RelativePath_OpensFileWithAbsolutePath()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                var tempFile = Path.Combine(tempDir, "testfile.txt");
                File.WriteAllText(tempFile, "test");
                var fileName = Path.GetFileName(tempFile);
                var originalDir = Directory.GetCurrentDirectory();
                try
                {
                    Directory.SetCurrentDirectory(tempDir);
                    SafeFileHandle? rootDirectory = null;
                    var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes);

                    // Act
                    using var handle = FileOperations.OpenHandleAt(rootDirectory, fileName, options);

                    // Assert
                    Assert.NotNull(handle);
                    Assert.False(handle.IsInvalid);
                    Assert.False(handle.IsClosed);
                }
                finally
                {
                    Directory.SetCurrentDirectory(originalDir);
                }
            }
            finally
            {
                Directory.DeleteIfExists(tempDir, recursive: true);
            }
        }

        [Fact(Skip = "Causes race condition in CI by changing process-wide current directory")]
        public void RootDirectoryNull_RelativePath_OpensDirectoryWithAbsolutePath()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                var subDir = Path.Combine(tempDir, "subdir");
                Directory.CreateDirectory(subDir);
                var dirName = Path.GetFileName(subDir);
                var originalDir = Directory.GetCurrentDirectory();
                try
                {
                    Directory.SetCurrentDirectory(tempDir);
                    SafeFileHandle? rootDirectory = null;
                    var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes, FileShare.Read, FileHandleType.Directory);

                    // Act
                    using var handle = FileOperations.OpenHandleAt(rootDirectory, dirName, options);

                    // Assert
                    Assert.NotNull(handle);
                    Assert.False(handle.IsInvalid);
                    Assert.False(handle.IsClosed);
                    Assert.Equal(FileHandleType.Directory, handle.Type);
                    Assert.True(FileOperations.GetAttributes(handle).HasFlag(FileAttributes.Directory));
                }
                finally
                {
                    Directory.SetCurrentDirectory(originalDir);
                }
            }
            finally
            {
                Directory.DeleteIfExists(tempDir, recursive: true);
            }
        }

        [Fact]
        public void RootDirectoryProvided_PathProvided_OpensRelativePath()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                var tempFile = Path.Combine(tempDir, "testfile.txt");
                File.WriteAllText(tempFile, "test");

                var dirOptions = FileHandleRequest.Open(FileSystemAccess.ReadAttributes, FileShare.Read, FileHandleType.Directory);
                using var rootDirectory = FileOperations.OpenHandle(tempDir, dirOptions);

                Assert.NotNull(rootDirectory);
                Assert.False(rootDirectory.IsInvalid);
                Assert.False(rootDirectory.IsClosed);
                Assert.Equal(FileHandleType.Directory, rootDirectory.Type);
                Assert.True(FileOperations.GetAttributes(rootDirectory).HasFlag(FileAttributes.Directory));

                var fileName = Path.GetFileName(tempFile);
                var fileOptions = FileHandleRequest.Open(FileSystemAccess.ReadAttributes);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, fileName, fileOptions);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                Directory.DeleteIfExists(tempDir, recursive: true);
            }
        }

        [Fact]
        public void RootDirectoryProvided_PathProvided_OpensRelativeDirectory()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                var subDir = Path.Combine(tempDir, "subdir");
                Directory.CreateDirectory(subDir);

                var dirOptions = FileHandleRequest.Open(FileSystemAccess.ReadAttributes, FileShare.Read, FileHandleType.Directory);
                using var rootDirectory = FileOperations.OpenHandle(tempDir, dirOptions);

                Assert.NotNull(rootDirectory);
                Assert.False(rootDirectory.IsInvalid);
                Assert.False(rootDirectory.IsClosed);
                Assert.Equal(FileHandleType.Directory, rootDirectory.Type);
                Assert.True(FileOperations.GetAttributes(rootDirectory).HasFlag(FileAttributes.Directory));

                var subDirName = Path.GetFileName(subDir);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, subDirName, dirOptions);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
                Assert.Equal(FileHandleType.Directory, handle.Type);
                Assert.True(FileOperations.GetAttributes(handle).HasFlag(FileAttributes.Directory));
            }
            finally
            {
                Directory.DeleteIfExists(tempDir, recursive: true);
            }
        }

        [Fact]
        public void RootDirectoryProvided_PathNull_ReopensRootDirectory()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                var dirOptions = FileHandleRequest.Open(FileSystemAccess.ReadAttributes, FileShare.Read, FileHandleType.Directory);
                using var rootDirectory = FileOperations.OpenHandle(tempDir, dirOptions);

                Assert.NotNull(rootDirectory);
                Assert.False(rootDirectory.IsInvalid);
                Assert.False(rootDirectory.IsClosed);
                Assert.Equal(FileHandleType.Directory, rootDirectory.Type);
                Assert.True(FileOperations.GetAttributes(rootDirectory).HasFlag(FileAttributes.Directory));

                string? path = null;

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, path, dirOptions);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
                Assert.Equal(FileHandleType.Directory, handle.Type);
                Assert.True(FileOperations.GetAttributes(handle).HasFlag(FileAttributes.Directory));
            }
            finally
            {
                Directory.DeleteIfExists(tempDir, recursive: true);
            }
        }

        [Fact]
        public void RootDirectoryProvided_PathNull_ReopensFileHandle()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes);
                using var rootDirectory = FileOperations.OpenHandle(tempFile, options);

                string? path = null;

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, path, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void PathNotFound_ThrowsException()
        {
            // Arrange
            SafeFileHandle? rootDirectory = null;
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() =>
                FileOperations.OpenHandleAt(rootDirectory, path, options));
        }

        [Fact]
        public void CreateMode_CreatesNewFile()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = FileHandleRequest.Create(FileSystemAccess.Write);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, tempFile, options);

                // Assert
                Assert.NotNull(handle);

                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
                Assert.True(File.Exists(tempFile));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void DifferentAccessModes_OpensFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = FileHandleRequest.Open(FileSystemAccess.Read);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, tempFile, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ShareModeNone_OpensFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes, FileShare.None);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, tempFile, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void InvalidFileHandle_ThrowsException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var rootDirectory = new SafeFileHandle(IntPtr.Zero, ownsHandle: false);
                var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes);

                // Act & Assert
                Assert.ThrowsAny<Exception>(() =>
                    FileOperations.OpenHandleAt(rootDirectory, "test.txt", options));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void OpenOrCreateMode_CreatesFileIfNotExists()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = FileHandleRequest.OpenOrCreate(FileSystemAccess.Write);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, tempFile, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
                Assert.True(File.Exists(tempFile));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void OpenOrCreateMode_OpensExistingFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = FileHandleRequest.OpenOrCreate(FileSystemAccess.Write);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, tempFile, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void CombinedAccessFlags_OpensFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = FileHandleRequest.Open(FileSystemAccess.Read | FileSystemAccess.Write);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, tempFile, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ShareModeReadWrite_OpensFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = FileHandleRequest.Open(FileSystemAccess.ReadAttributes, FileShare.ReadWrite);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, tempFile, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void RootDirectoryProvided_NestedPath_OpensFile()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var subDir = Path.Combine(tempDir, "subdir");
            Directory.CreateDirectory(subDir);
            try
            {
                var tempFile = Path.Combine(subDir, "testfile.txt");
                File.WriteAllText(tempFile, "test");

                var dirOptions = FileHandleRequest.Open(FileSystemAccess.ReadAttributes, FileShare.Read, FileHandleType.Directory);
                using var rootDirectory = FileOperations.OpenHandle(tempDir, dirOptions);

                Assert.NotNull(rootDirectory);
                Assert.False(rootDirectory.IsInvalid);
                Assert.False(rootDirectory.IsClosed);
                Assert.Equal(FileHandleType.Directory, rootDirectory.Type);
                Assert.True(FileOperations.GetAttributes(rootDirectory).HasFlag(FileAttributes.Directory));

                var relativePath = Path.Combine("subdir", "testfile.txt");
                var fileOptions = FileHandleRequest.Open(FileSystemAccess.ReadAttributes);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, relativePath, fileOptions);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                Directory.DeleteIfExists(tempDir, recursive: true);
            }
        }

        [Fact]
        public void CreateNewMode_CreatesNewFile()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = FileHandleRequest.CreateNew(FileSystemAccess.Write);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, tempFile, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
                Assert.True(File.Exists(tempFile));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void AppendMode_OpensFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = FileHandleRequest.Append(FileSystemAccess.Write);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, tempFile, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void TruncateMode_OpensExistingFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, "existing content");
                SafeFileHandle? rootDirectory = null;
                var options = new FileHandleRequest(FileMode.Truncate, FileSystemAccess.Write);

                // Act
                using var handle = FileOperations.OpenHandleAt(rootDirectory, tempFile, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
