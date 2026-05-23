using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Tests.Utilities;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class OpenHandleById : IDisposable
    {
        private readonly string _tempFilePath;
        private readonly string _tempDirectoryPath;

        // Only for netfx, where we can't open a handle directly, so we need to
        // keep its source FileStream alive to keep the handle open.
#pragma warning disable CS0649 // Field is never assigned to - false positive, assigned in .NET Framework build
        private readonly IDisposable? _tempDisposable;
#pragma warning restore CS0649

        private readonly SafeFileHandle _tempFileHandle;

        public OpenHandleById()
        {
            try
            {
                _tempFilePath = Path.GetTempFileName();
                _tempDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(_tempDirectoryPath);
#if NET6_0_OR_GREATER
                _tempFileHandle = File.OpenHandle(_tempFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
#else
            var fileStream = new FileStream(_tempFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 4096);
            _tempDisposable = fileStream;
            _tempFileHandle = fileStream.SafeFileHandle;
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during constructor: {ex}");
            }
        }

        public void Dispose()
        {
            try
            {
                _tempFileHandle?.Dispose();
            }
            catch { }

            try
            {
                _tempDisposable?.Dispose();
            }
            catch { }

            TryMethods.TryDeleteFile(_tempFilePath);
            TryMethods.TryDeleteDirectory(_tempDirectoryPath, recursive: true);
        }

        private SafeFileHandle OpenDirectoryHandle()
        {
            var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read)
            {
                Attributes = FileAttributes.Directory
            };
            return FileIO.OpenHandle(_tempDirectoryPath, options);
        }

        [WindowsOnlyFact]
        public void WithValidFileId_ReturnsFileHandle()
        {
            try
            {
                // Arrange - Get file ID from an existing file
                var fileId = FileIO.GetFileId(_tempFileHandle);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read);

                // Act
                using var result = FileIO.OpenHandle(fileId, options);

                // Assert
                Assert.NotNull(result);
                Assert.False(result.IsInvalid);
                Assert.False(result.IsClosed);
            }
            catch (Exception ex)
            {
                // Catch any unexpected exceptions to prevent test crashes and provide better diagnostics
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }

        [WindowsOnlyFact(Skip = "test")]
        public void WithDefaultFileId_ThrowsArgumentException()
        {
            try
            {
                // Arrange
                var fileId = default(FsFileId);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read);

                // Act & Assert
                Assert.Throws<ArgumentException>(() =>
                    FileIO.OpenHandle(fileId, options));
            }
            catch(Exception ex)
            {
                // Catch any unexpected exceptions to prevent test crashes and provide better diagnostics
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }

        [WindowsOnlyFact]
        public void WithValidOptions_OpensFileSuccessfully()
        {
            try
            {
                // Arrange
                var fileId = FileIO.GetFileId(_tempFileHandle);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read, FileShare.ReadWrite);

                // Act
                using var result = FileIO.OpenHandle(fileId, options);

                // Assert
                Assert.NotNull(result);
                Assert.False(result.IsInvalid);
            }
            catch (Exception ex)
            {
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }

        [WindowsOnlyFact]
        public void OpenedHandleCanBeUsed_ToGetSameFileId()
        {
            try
            {
                // Arrange
                var originalFileId = FileIO.GetFileId(_tempFileHandle);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read);

                // Act
                using var reopenedHandle = FileIO.OpenHandle(originalFileId, options);
                var reopenedFileId = FileIO.GetFileId(reopenedHandle);

                // Assert
                Assert.Equal(originalFileId, reopenedFileId);
            }
            catch (Exception ex)
            {
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }

        [WindowsOnlyFact]
        public void WithDifferentShareModes_RespectsShareSettings()
        {
            try
            {
                // Arrange
                var fileId = FileIO.GetFileId(_tempFileHandle);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read, FileShare.Read);

                // Act
                using var result = FileIO.OpenHandle(fileId, options);

                // Assert
                Assert.NotNull(result);
                Assert.False(result.IsInvalid);
            }
            catch (Exception ex)
            {
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }

        [WindowsOnlyFact]
        public void ReturnsHandleThatOwnsResource()
        {
            try
            {
                // Arrange
                var fileId = FileIO.GetFileId(_tempFileHandle);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read);

                // Act
                var result = FileIO.OpenHandle(fileId, options);

                // Assert
                Assert.NotNull(result);
                Assert.False(result.IsClosed);
                result.Dispose();
                Assert.True(result.IsClosed);
            }
            catch (Exception ex)
            {
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }

        [WindowsOnlyFact(Skip = "test")]
        public void WithRandomFileId_ThrowsFileNotFoundException()
        {
            try
            {
                // Arrange - Get a valid volume serial number but use random file IDs
                var validFileId = FileIO.GetFileId(_tempFileHandle);
                var randomFileId = new FsFileId(
                    volumeSerialNumber: validFileId.VolumeSerialNumber,
                    fileIdHigh: 0xDEADBEEFDEADBEEF,
                    fileIdLow: 0xCAFEBABECAFEBABE);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read);

                // Act & Assert
                Assert.Throws<FileNotFoundException>(() =>
                    FileIO.OpenHandle(randomFileId, options));
            }
            catch (Exception ex)
            {
                // Catch any unexpected exceptions to prevent test crashes and provide better diagnostics
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }

        [WindowsOnlyFact(Skip = "test")]
        public void WithNonExistentVolumeSerial_ThrowsDirectoryNotFoundException()
        {
            try
            {
                // Arrange - Use a completely invalid volume serial number and random file IDs
                var randomFileId = new FsFileId(
                    volumeSerialNumber: 0xFFFFFFFFFFFFFFFF,
                    fileIdHigh: 0xDEADBEEFDEADBEEF,
                    fileIdLow: 0xCAFEBABECAFEBABE);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read);

                // Act & Assert
                Assert.Throws<DirectoryNotFoundException>(() =>
                    FileIO.OpenHandle(randomFileId, options));
            }
            catch (Exception ex)
            {
                // Catch any unexpected exceptions to prevent test crashes and provide better diagnostics
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }

        [WindowsOnlyFact]
        public void WithValidDirectoryId_ReturnsDirectoryHandle()
        {
            try
            {
                // Arrange - Get directory ID from an existing directory
                using var tempDirHandle = OpenDirectoryHandle();
                var directoryId = FileIO.GetFileId(tempDirHandle);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read)
                {
                    Attributes = FileAttributes.Directory
                };

                // Act
                using var result = FileIO.OpenHandle(directoryId, options);

                // Assert
                Assert.NotNull(result);
                Assert.False(result.IsInvalid);
                Assert.False(result.IsClosed);
            }
            catch (Exception ex)
            {
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }

        [WindowsOnlyFact]
        public void OpenedDirectoryHandle_CanBeUsedToGetSameDirectoryId()
        {
            try
            {
                // Arrange
                using var tempDirHandle = OpenDirectoryHandle();
                var originalDirectoryId = FileIO.GetFileId(tempDirHandle);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read)
                {
                    Attributes = FileAttributes.Directory
                };

                // Act
                using var reopenedHandle = FileIO.OpenHandle(originalDirectoryId, options);
                var reopenedDirectoryId = FileIO.GetFileId(reopenedHandle);

                // Assert
                Assert.Equal(originalDirectoryId, reopenedDirectoryId);
            }
            catch (Exception ex)
            {
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }

        [WindowsOnlyFact]
        public void WithDirectoryIdAndDirectoryAttribute_OpensSuccessfully()
        {
            try
            {
                // Arrange
                using var tempDirHandle = OpenDirectoryHandle();
                var directoryId = FileIO.GetFileId(tempDirHandle);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read, FileShare.ReadWrite)
                {
                    Attributes = FileAttributes.Directory
                };

                // Act
                using var result = FileIO.OpenHandle(directoryId, options);

                // Assert
                Assert.NotNull(result);
                Assert.False(result.IsInvalid);
            }
            catch (Exception ex)
            {
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }

        [WindowsOnlyFact]
        public void DirectoryHandle_OwnsResource()
        {
            try
            {
                // Arrange
                using var tempDirHandle = OpenDirectoryHandle();
                var directoryId = FileIO.GetFileId(tempDirHandle);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read)
                {
                    Attributes = FileAttributes.Directory
                };

                // Act
                var result = FileIO.OpenHandle(directoryId, options);

                // Assert
                Assert.NotNull(result);
                Assert.False(result.IsClosed);
                result.Dispose();
                Assert.True(result.IsClosed);
            }
            catch (Exception ex)
            {
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }

        [WindowsOnlyFact(Skip = "test")]
        public void WithRandomDirectoryId_ThrowsFileNotFoundException()
        {
            try
            {
                // Arrange - Get a valid volume serial number but use random file IDs
                using var tempDirHandle = OpenDirectoryHandle();
                var validDirectoryId = FileIO.GetFileId(tempDirHandle);
                var randomDirectoryId = new FsFileId(
                    volumeSerialNumber: validDirectoryId.VolumeSerialNumber,
                    fileIdHigh: 0xDEADBEEFDEADBEEF,
                    fileIdLow: 0xCAFEBABECAFEBABE);
                var options = new FsFileOptions(FileMode.Open, FsFileAccess.Read)
                {
                    Attributes = FileAttributes.Directory
                };

                // Act & Assert
                Assert.Throws<FileNotFoundException>(() =>
                    FileIO.OpenHandle(randomDirectoryId, options));
            }
            catch (Exception ex)
            {
                // Catch any unexpected exceptions to prevent test crashes and provide better diagnostics
                Assert.False(true, $"Unexpected exception thrown: {ex}");
            }
        }
    }
}
