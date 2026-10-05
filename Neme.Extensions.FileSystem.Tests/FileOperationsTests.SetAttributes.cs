using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Tests.Utilities;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileOperationsTests
{
    [Collection(nameof(FileOperationsTestCollection))]
    public sealed class SetAttributes
    {
        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            var handle = (SafeFileHandle)null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileOperations.SetAttributes(handle, FileAttributes.ReadOnly));
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using var handle = new SafeFileHandle((nint)(-1), ownsHandle: false);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileOperations.SetAttributes(handle, FileAttributes.ReadOnly));
        }

        [Fact]
        public void ValidHandle_UpdatesReadOnlyAttribute()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = FileHandleRequest.Open(FileSystemAccess.WriteAttributes, FileShare.All);
                using var handle = FileOperations.OpenHandle(tempFile, options);

                // Act
                FileOperations.SetAttributes(handle, FileAttributes.ReadOnly);
                var readOnlyAttributes = FileOperations.GetAttributes(handle);
                FileOperations.SetAttributes(handle, FileAttributes.Normal);
                var normalAttributes = FileOperations.GetAttributes(handle);

                // Assert
                Assert.True(readOnlyAttributes.HasFlag(FileAttributes.ReadOnly));
                Assert.False(normalAttributes.HasFlag(FileAttributes.ReadOnly));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void WithoutWriteAttributesAccess_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = FileHandleRequest.Open(FileSystemAccess.Read, FileShare.All);
                using var handle = FileOperations.OpenHandle(tempFile, options);

                // Act & Assert
                Assert.Throws<UnauthorizedAccessException>(() => FileOperations.SetAttributes(handle, FileAttributes.ReadOnly));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [PlatformOnlyFact(Platform.Windows, Platform.MacOS)]
        public void ValidHandle_UpdatesHiddenAttribute()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = FileHandleRequest.Open(FileSystemAccess.ReadWrite, FileShare.All);
                using var handle = FileOperations.OpenHandle(tempFile, options);

                // Act
                FileOperations.SetAttributes(handle, FileAttributes.Hidden);
                var hiddenAttributes = FileOperations.GetAttributes(handle);
                FileOperations.SetAttributes(handle, FileAttributes.Normal);
                var normalAttributes = FileOperations.GetAttributes(handle);

                // Assert
                Assert.True(hiddenAttributes.HasFlag(FileAttributes.Hidden));
                Assert.False(normalAttributes.HasFlag(FileAttributes.Hidden));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
