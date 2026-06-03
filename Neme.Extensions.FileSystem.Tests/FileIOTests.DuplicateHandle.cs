using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class DuplicateHandle
    {
        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            var handle = (SafeFileHandle)null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileIO.DuplicateHandle(handle, access: null));
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using var handle = new SafeFileHandle((nint)(-1), ownsHandle: false);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileIO.DuplicateHandle(handle, access: null));
        }

        [Fact]
        public void ValidHandleWithNullAccess_ReturnsIndependentDuplicateHandle()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "duplicate handle content");
            var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);

            try
            {
                using var originalHandle = FileIO.OpenHandle(tempFile, options);
                using var duplicatedHandle = FileIO.DuplicateHandle(originalHandle, access: null);
                originalHandle.Dispose();

                // Act
                using var stream = FileIO.CreateFileStream(duplicatedHandle, options, leaveOpen: true);
                using var reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                // Assert
                Assert.NotSame(originalHandle, duplicatedHandle);
                Assert.False(duplicatedHandle.IsClosed);
                Assert.Equal("duplicate handle content", result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ValidHandleWithSpecifiedAccess_UsesPlatformSpecificBehavior()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var originalOptions = new FileOpenOptions(FileMode.Open, FileSystemAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);

            try
            {
                using var originalHandle = FileIO.OpenHandle(tempFile, originalOptions);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var duplicatedOptions = new FileOpenOptions(FileMode.Open, FileSystemAccess.Write, FileShare.ReadWrite | FileShare.Delete);
                    using var duplicatedHandle = FileIO.DuplicateHandle(originalHandle, FileSystemAccess.Write);
                    originalHandle.Dispose();

                    using (var stream = FileIO.CreateFileStream(duplicatedHandle, duplicatedOptions, leaveOpen: true))
                    {
                        stream.WriteByte(42);
                    }

                    duplicatedHandle.Dispose();

                    // Act
                    var result = File.ReadAllBytes(tempFile);

                    // Assert
                    Assert.Single(result);
                    Assert.Equal((byte)42, result[0]);
                }
                else
                {
                    // Act & Assert
                    Assert.Throws<PlatformNotSupportedException>(() => FileIO.DuplicateHandle(originalHandle, FileSystemAccess.Read));
                }
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
