namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    [Collection(nameof(FileIOTestCollection))]
    public sealed class ReadAllBytesAsync
    {
        [Fact]
        public async Task WithNonEmptyFile_ReturnsFileContents()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                byte[] expected = [1, 2, 3, 4, 5];
                File.WriteAllBytes(tempFile, expected);
                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                byte[] result = await FileIO.ReadAllBytesAsync(handle);

                // Assert
                Assert.Equal(expected, result);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task WithCanceledToken_ReturnsCanceledTaskWithoutClosingHandle()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                byte[] expected = [9, 8, 7];
                File.WriteAllBytes(tempFile, expected);
                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);
                using var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                // Act
                Task<byte[]> task = FileIO.ReadAllBytesAsync(handle, cancellationTokenSource.Token);

                // Assert
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
                Assert.True(task.IsCanceled);
                Assert.False(handle.IsClosed);

                byte[] result = await FileIO.ReadAllBytesAsync(handle);
                Assert.Equal(expected, result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task WithFileLongerThanArrayMaxLength_ClosesHandleAndThrowsIOException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                using (var stream = new FileStream(tempFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
                {
                    stream.SetLength((long)Array.MaxLength + 1);
                }

                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                Task<byte[]> task = FileIO.ReadAllBytesAsync(handle);

                // Assert
                IOException exception = await Assert.ThrowsAsync<IOException>(() => task);
                Assert.Contains("2 gigabytes", exception.Message, StringComparison.OrdinalIgnoreCase);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class ReadAllBytes
    {
        [Fact]
        public void WithNonEmptyFile_ReturnsFileContents()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                byte[] expected = [1, 2, 3, 4, 5];
                File.WriteAllBytes(tempFile, expected);
                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                byte[] result = FileIO.ReadAllBytes(handle);

                // Assert
                Assert.Equal(expected, result);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void WithEmptyFile_ReturnsEmptyArray()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                byte[] result = FileIO.ReadAllBytes(handle);

                // Assert
                Assert.Empty(result);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void WithCanceledToken_ThrowsOperationCanceledExceptionWithoutClosingHandle()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                byte[] expected = [9, 8, 7];
                File.WriteAllBytes(tempFile, expected);
                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);
                using var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                // Act & Assert
                Assert.ThrowsAny<OperationCanceledException>(() => FileIO.ReadAllBytes(handle, cancellationTokenSource.Token));
                Assert.False(handle.IsClosed);

                byte[] result = FileIO.ReadAllBytes(handle);
                Assert.Equal(expected, result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void WithFileLongerThanArrayMaxLength_ThrowsIOExceptionWithoutClosingHandle()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                using (var stream = new FileStream(tempFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
                {
                    stream.SetLength((long)Array.MaxLength + 1);
                }

                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                IOException exception = Assert.Throws<IOException>(() => FileIO.ReadAllBytes(handle));

                // Assert
                Assert.Contains("2 gigabytes", exception.Message, StringComparison.OrdinalIgnoreCase);
                Assert.False(handle.IsClosed);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
