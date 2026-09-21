using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Tests.Utilities;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class SetAttributes
    {
        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            var handle = (SafeFileHandle)null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileIO.SetAttributes(handle, FileAttributes.ReadOnly));
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using var handle = new SafeFileHandle((nint)(-1), ownsHandle: false);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileIO.SetAttributes(handle, FileAttributes.ReadOnly));
        }

        [Fact]
        public void ValidHandle_UpdatesReadOnlyAttribute()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = FileOpenOptions.Open(FileSystemAccess.WriteAttributes, FileShare.All);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                FileIO.SetAttributes(handle, FileAttributes.ReadOnly);
                var readOnlyAttributes = FileIO.GetAttributes(handle);
                FileIO.SetAttributes(handle, FileAttributes.Normal);
                var normalAttributes = FileIO.GetAttributes(handle);

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
                var options = FileOpenOptions.Open(FileSystemAccess.Read, FileShare.All);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act & Assert
                Assert.Throws<UnauthorizedAccessException>(() => FileIO.SetAttributes(handle, FileAttributes.ReadOnly));
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
                var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                FileIO.SetAttributes(handle, FileAttributes.Hidden);
                var hiddenAttributes = FileIO.GetAttributes(handle);
                FileIO.SetAttributes(handle, FileAttributes.Normal);
                var normalAttributes = FileIO.GetAttributes(handle);

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
