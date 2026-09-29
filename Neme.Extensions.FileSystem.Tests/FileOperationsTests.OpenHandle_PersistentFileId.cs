using Microsoft.Win32.SafeHandles;
using Neme.Extensions.IO;
using Neme.Extensions.Tests.Utilities;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileOperationsTests
{
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [Collection(nameof(FileOperationsTestCollection))]
    public sealed class OpenHandle_PersistentFileId : IDisposable
    {
        private readonly string _tempFilePath;
        private readonly string _tempDirectoryPath;

        // Only for netfx, where we can't open a handle directly, so we need to
        // keep its source FileStream alive to keep the handle open.
#pragma warning disable CS0649 // Field is never assigned to - false positive, assigned in .NET Framework build
        private readonly IDisposable? _tempDisposable;
#pragma warning restore CS0649

        private readonly SafeFileHandle _tempFileHandle;

        public OpenHandle_PersistentFileId()
        {
            _tempFilePath = Path.GetTempFileName();
            _tempDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectoryPath);
#if NET6_0_OR_GREATER
            _tempFileHandle = File.OpenHandle(_tempFilePath, FileMode.Open, FileAccess.Read, FileShare.All);
#else
            var fileStream = new FileStream(_tempFilePath, FileMode.Open, FileAccess.Read, FileShare.All, 4096);
            _tempDisposable = fileStream;
            _tempFileHandle = fileStream.SafeFileHandle;
#endif
        }

        public void Dispose()
        {
            _tempFileHandle?.Dispose();
            _tempDisposable?.Dispose();

            File.DeleteIfExists(_tempFilePath);
            Directory.DeleteIfExists(_tempDirectoryPath, recursive: true);
        }

        private SafeFileHandle OpenDirectoryHandle()
        {
            var options = FileOpenRequest.Open(FileSystemAccess.Read, FileShare.Read, 0, FileAttributes.Directory);
            return FileOperations.OpenHandle(_tempDirectoryPath, options);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithValidFileId_ReturnsFileHandle()
        {
            // Arrange - Get file ID from an existing file
            var fileId = FileOperations.GetPersistentId(_tempFileHandle);
            var options = FileOpenRequest.Open(FileSystemAccess.Read);

            // Act
            using var result = FileOperations.OpenHandle(fileId, options);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsInvalid);
            Assert.False(result.IsClosed);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithDefaultFileId_ThrowsArgumentException()
        {
            // Arrange
            var fileId = default(PersistentFileId);
            var options = FileOpenRequest.Open(FileSystemAccess.Read);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                FileOperations.OpenHandle(fileId, options));
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithValidOptions_OpensFileSuccessfully()
        {
            // Arrange
            var fileId = FileOperations.GetPersistentId(_tempFileHandle);
            var options = FileOpenRequest.Open(FileSystemAccess.Read, FileShare.ReadWrite);

            // Act
            using var result = FileOperations.OpenHandle(fileId, options);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsInvalid);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void OpenedHandleCanBeUsed_ToGetSameFileId()
        {
            // Arrange
            var originalFileId = FileOperations.GetPersistentId(_tempFileHandle);
            var options = FileOpenRequest.Open(FileSystemAccess.Read);

            // Act
            using var reopenedHandle = FileOperations.OpenHandle(originalFileId, options);
            var reopenedFileId = FileOperations.GetPersistentId(reopenedHandle);

            // Assert
            Assert.Equal(originalFileId, reopenedFileId);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithDifferentShareModes_RespectsShareSettings()
        {
            // Arrange
            var fileId = FileOperations.GetPersistentId(_tempFileHandle);
            var options = FileOpenRequest.Open(FileSystemAccess.Read, FileShare.Read);

            // Act
            using var result = FileOperations.OpenHandle(fileId, options);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsInvalid);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void ReturnsHandleThatOwnsResource()
        {
            // Arrange
            var fileId = FileOperations.GetPersistentId(_tempFileHandle);
            var options = FileOpenRequest.Open(FileSystemAccess.Read);

            // Act
            var result = FileOperations.OpenHandle(fileId, options);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsClosed);
            result.Dispose();
            Assert.True(result.IsClosed);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithRandomFileId_ThrowsFileNotFoundException()
        {
            // Arrange - Get a valid volume serial number but use random file IDs
            var validFileId = FileOperations.GetPersistentId(_tempFileHandle);
            var randomFileId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? PersistentFileId.FromWindowsId(new PersistentFileId.WindowsId(
                    volumeSerialNumber: validFileId.WindowsFileId.VolumeSerialNumber,
                    fileIdHigh: 0xDEADBEEFDEADBEEF,
                    fileIdLow: 0xCAFEBABECAFEBABE))
                : PersistentFileId.FromLinuxId(new PersistentFileId.LinuxId(
                    validFileId.LinuxFileId.MountPath,
                    validFileId.LinuxFileId.FileType,
                    []));

            var options = FileOpenRequest.Open(FileSystemAccess.Read);

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() =>
                FileOperations.OpenHandle(randomFileId, options));
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithNonExistentVolumeSerial_ThrowsDirectoryNotFoundException()
        {
            // Arrange - Use a completely invalid volume serial number and random file IDs
            var randomFileId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? PersistentFileId.FromWindowsId(new PersistentFileId.WindowsId(
                    volumeSerialNumber: 0xFFFFFFFFFFFFFFFF,
                    fileIdHigh: 0xDEADBEEFDEADBEEF,
                    fileIdLow: 0xCAFEBABECAFEBABE))
                : PersistentFileId.FromLinuxId(new PersistentFileId.LinuxId(
                    "/path/that/does/not/exist",
                    0,
                    []));
            var options = FileOpenRequest.Open(FileSystemAccess.Read);

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() =>
                FileOperations.OpenHandle(randomFileId, options));
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithValidDirectoryId_ReturnsDirectoryHandle()
        {
            // Arrange - Get directory ID from an existing directory
            using var tempDirHandle = OpenDirectoryHandle();
            var directoryId = FileOperations.GetPersistentId(tempDirHandle);
            var options = FileOpenRequest.Open(FileSystemAccess.Read, FileShare.Read, 0, FileAttributes.Directory);

            // Act
            using var result = FileOperations.OpenHandle(directoryId, options);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsInvalid);
            Assert.False(result.IsClosed);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void OpenedDirectoryHandle_CanBeUsedToGetSameDirectoryId()
        {
            // Arrange
            using var tempDirHandle = OpenDirectoryHandle();
            var originalDirectoryId = FileOperations.GetPersistentId(tempDirHandle);
            var options = FileOpenRequest.Open(FileSystemAccess.Read, FileShare.Read, 0, FileAttributes.Directory);

            // Act
            using var reopenedHandle = FileOperations.OpenHandle(originalDirectoryId, options);
            var reopenedDirectoryId = FileOperations.GetPersistentId(reopenedHandle);

            // Assert
            Assert.Equal(originalDirectoryId, reopenedDirectoryId);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithDirectoryIdAndDirectoryAttribute_OpensSuccessfully()
        {
            // Arrange
            using var tempDirHandle = OpenDirectoryHandle();
            var directoryId = FileOperations.GetPersistentId(tempDirHandle);
            var options = FileOpenRequest.Open(FileSystemAccess.Read, FileShare.ReadWrite, 0, FileAttributes.Directory);

            // Act
            using var result = FileOperations.OpenHandle(directoryId, options);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsInvalid);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void DirectoryHandle_OwnsResource()
        {
            // Arrange
            using var tempDirHandle = OpenDirectoryHandle();
            var directoryId = FileOperations.GetPersistentId(tempDirHandle);
            var options = FileOpenRequest.Open(FileSystemAccess.Read, FileShare.Read, 0, FileAttributes.Directory);

            // Act
            var result = FileOperations.OpenHandle(directoryId, options);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsClosed);
            result.Dispose();
            Assert.True(result.IsClosed);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithRandomDirectoryId_ThrowsFileNotFoundException()
        {
            // Arrange - Get a valid volume serial number but use random file IDs
            using var tempDirHandle = OpenDirectoryHandle();
            var validDirectoryId = FileOperations.GetPersistentId(tempDirHandle);
            var randomDirectoryId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? PersistentFileId.FromWindowsId(new PersistentFileId.WindowsId(
                    volumeSerialNumber: validDirectoryId.WindowsFileId.VolumeSerialNumber,
                    fileIdHigh: 0xDEADBEEFDEADBEEF,
                    fileIdLow: 0xCAFEBABECAFEBABE))
                : PersistentFileId.FromLinuxId(new PersistentFileId.LinuxId(
                    validDirectoryId.LinuxFileId.MountPath,
                    validDirectoryId.LinuxFileId.FileType,
                    []));
            var options = FileOpenRequest.Open(FileSystemAccess.Read, FileShare.Read, 0, FileAttributes.Directory);

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() =>
                FileOperations.OpenHandle(randomDirectoryId, options));
        }
    }
}
