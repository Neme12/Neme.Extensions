using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Tests.Utilities;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class OpenHandleBy
    {
        [WindowsOnlyFact]
        public void BothParametersNull_ThrowsArgumentException()
        {
            // Arrange
            SafeFileHandle? rootDirectory = null;
            string? path = null;
            var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                FileIO.OpenHandleBy(rootDirectory, path, options));
            Assert.Contains("rootDirectory", ex.Message);
            Assert.Contains("path", ex.Message);
        }

        [WindowsOnlyFact]
        public void RootDirectoryNull_PathProvided_OpensFileRegularly()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, tempFile, options);

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

        [WindowsOnlyFact]
        public void RootDirectoryNull_PathProvided_OpensDirectoryRegularly()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes)
                {
                    Attributes = FileAttributes.Directory
                };

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, tempDir, options);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
        }

        [WindowsOnlyFact(Skip = "Causes race condition in CI by changing process-wide current directory")]
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
                    var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes);

                    // Act
                    using var handle = FileIO.OpenHandleBy(rootDirectory, fileName, options);

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
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
        }

        [WindowsOnlyFact(Skip = "Causes race condition in CI by changing process-wide current directory")]
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
                    var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes)
                    {
                        Attributes = FileAttributes.Directory
                    };

                    // Act
                    using var handle = FileIO.OpenHandleBy(rootDirectory, dirName, options);

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
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
        }

        [WindowsOnlyFact]
        public void RootDirectoryProvided_PathProvided_OpensRelativePath()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                var tempFile = Path.Combine(tempDir, "testfile.txt");
                File.WriteAllText(tempFile, "test");

                var dirOptions = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes)
                {
                    Attributes = FileAttributes.Directory
                };
                using var rootDirectory = FileIO.OpenHandle(tempDir, dirOptions);

                var fileName = Path.GetFileName(tempFile);
                var fileOptions = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, fileName, fileOptions);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
        }

        [WindowsOnlyFact]
        public void RootDirectoryProvided_PathProvided_OpensRelativeDirectory()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                var subDir = Path.Combine(tempDir, "subdir");
                Directory.CreateDirectory(subDir);

                var dirOptions = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes)
                {
                    Attributes = FileAttributes.Directory
                };
                using var rootDirectory = FileIO.OpenHandle(tempDir, dirOptions);

                var subDirName = Path.GetFileName(subDir);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, subDirName, dirOptions);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
        }

        [WindowsOnlyFact]
        public void RootDirectoryProvided_PathNull_ReopensRootDirectory()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                var dirOptions = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes)
                {
                    Attributes = FileAttributes.Directory
                };
                using var rootDirectory = FileIO.OpenHandle(tempDir, dirOptions);

                string? path = null;

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, path, dirOptions);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
        }

        [WindowsOnlyFact]
        public void RootDirectoryProvided_PathNull_ReopensFileHandle()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes);
                using var rootDirectory = FileIO.OpenHandle(tempFile, options);

                string? path = null;

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, path, options);

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

        [WindowsOnlyFact]
        public void PathNotFound_ThrowsException()
        {
            // Arrange
            SafeFileHandle? rootDirectory = null;
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() =>
                FileIO.OpenHandleBy(rootDirectory, path, options));
        }

        [WindowsOnlyFact]
        public void CreateMode_CreatesNewFile()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = new FsFileOptions(FileMode.Create, FsFileAccess.Write);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, tempFile, options);

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

        [WindowsOnlyFact]
        public void DifferentAccessModes_OpensFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, tempFile, options);

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

        [WindowsOnlyFact]
        public void ShareModeNone_OpensFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes, FileShare.None);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, tempFile, options);

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

        [WindowsOnlyFact]
        public void InvalidFileHandle_ThrowsException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var rootDirectory = new SafeFileHandle(IntPtr.Zero, ownsHandle: false);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes);

                // Act & Assert
                Assert.ThrowsAny<Exception>(() =>
                    FileIO.OpenHandleBy(rootDirectory, "test.txt", options));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [WindowsOnlyFact]
        public void OpenOrCreateMode_CreatesFileIfNotExists()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = new FsFileOptions(FileMode.OpenOrCreate, FsFileAccess.Write);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, tempFile, options);

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

        [WindowsOnlyFact]
        public void OpenOrCreateMode_OpensExistingFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = new FsFileOptions(FileMode.OpenOrCreate, FsFileAccess.Write);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, tempFile, options);

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

        [WindowsOnlyFact]
        public void CombinedAccessFlags_OpensFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read | FsFileAccess.Write);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, tempFile, options);

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

        [WindowsOnlyFact]
        public void ShareModeReadWrite_OpensFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes, FileShare.ReadWrite);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, tempFile, options);

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

        [WindowsOnlyFact]
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

                var dirOptions = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes)
                {
                    Attributes = FileAttributes.Directory
                };
                using var rootDirectory = FileIO.OpenHandle(tempDir, dirOptions);

                var relativePath = Path.Combine("subdir", "testfile.txt");
                var fileOptions = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, relativePath, fileOptions);

                // Assert
                Assert.NotNull(handle);
                Assert.False(handle.IsInvalid);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
        }

        [WindowsOnlyFact]
        public void CreateNewMode_CreatesNewFile()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = new FsFileOptions(FileMode.CreateNew, FsFileAccess.Write);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, tempFile, options);

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

        [WindowsOnlyFact]
        public void AppendMode_OpensFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                SafeFileHandle? rootDirectory = null;
                var options = new FsFileOptions(FileMode.Append, FsFileAccess.Write);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, tempFile, options);

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

        [WindowsOnlyFact]
        public void TruncateMode_OpensExistingFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, "existing content");
                SafeFileHandle? rootDirectory = null;
                var options = new FsFileOptions(FileMode.Truncate, FsFileAccess.Write);

                // Act
                using var handle = FileIO.OpenHandleBy(rootDirectory, tempFile, options);

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
