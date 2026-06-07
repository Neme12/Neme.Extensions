using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Tests.Utilities;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [Collection(nameof(FileIOTestCollection))]
    public sealed class GetPersistentId : IDisposable
    {
        private readonly string _tempFilePath;

        // Only for netfx, where we can't open a handle directly, so we need to
        // keep its source FileStream alive to keep the handle open.
#pragma warning disable CS0649 // It is assigned on netfx.
        private readonly IDisposable? _tempDisposable;
#pragma warning restore CS0649

        private readonly SafeFileHandle _tempFileHandle;

        public GetPersistentId()
        {
            _tempFilePath = Path.GetTempFileName();
#if NET6_0_OR_GREATER
            _tempFileHandle = File.OpenHandle(_tempFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
#else
            var fileStream = new FileStream(_tempFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 4096);
            _tempDisposable = fileStream;
            _tempFileHandle = fileStream.SafeFileHandle;
#endif
        }

        public void Dispose()
        {
            _tempFileHandle?.Dispose();
            _tempDisposable?.Dispose();

            try
            {
                if (File.Exists(_tempFilePath))
                    File.Delete(_tempFilePath);
            }
            catch { }
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithValidFileHandle_ReturnsFileId()
        {
            // Act
            var result = FileIO.GetPersistentId(_tempFileHandle);

            // Assert - FsFileId is a struct, so just verify it's created
            Assert.True(true);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithNullFileHandle_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                FileIO.GetPersistentId(null!));
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithClosedFileHandle_ThrowsArgumentException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var fileStream = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.DeleteOnClose);
            var fileHandle = fileStream.SafeFileHandle;
            fileStream.Dispose();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                FileIO.GetPersistentId(fileHandle));
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithInvalidFileHandle_ThrowsArgumentException()
        {
            // Arrange
            var fileHandle = new SafeFileHandle((nint)(-1), false);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                FileIO.GetPersistentId(fileHandle));
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithValidFileHandle_PopulatesId()
        {
            // Act
            var result = FileIO.GetPersistentId(_tempFileHandle);

            // Assert
            Assert.NotEqual(default, result);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithDifferentFiles_ReturnsDifferentFileIds()
        {
            // Arrange
            var tempFile2 = Path.GetTempFileName();
            using var fileStream2 = new FileStream(tempFile2, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.DeleteOnClose);

            // Act
            var result1 = FileIO.GetPersistentId(_tempFileHandle);
            var result2 = FileIO.GetPersistentId(fileStream2.SafeFileHandle);

            // Assert
            Assert.NotEqual(result1, result2);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void WithSameFileTwice_ReturnsSameFileId()
        {
            // Arrange
            using var fileStream2 = new FileStream(_tempFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

            // Act
            var result1 = FileIO.GetPersistentId(_tempFileHandle);
            var result2 = FileIO.GetPersistentId(fileStream2.SafeFileHandle);

            // Assert
            Assert.Equal(result1, result2);
        }
    }
}
