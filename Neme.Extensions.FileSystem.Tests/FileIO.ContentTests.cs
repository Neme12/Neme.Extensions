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

    [Collection(nameof(FileIOTestCollection))]
    public sealed class ReadAllText
    {
        [Fact]
        public void WithByteOrderMark_UsesDetectedEncodingAndLeavesHandleOpen()
        {
            // Arrange
            const string expected = "Hello, 世界";
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, expected, System.Text.Encoding.Unicode);
                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                string result = FileIO.ReadAllText(handle, System.Text.Encoding.ASCII);

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
        public void WithNullHandle_ThrowsArgumentNullException()
        {
            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => FileIO.ReadAllText(null!, System.Text.Encoding.UTF8));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void WithNullEncoding_ThrowsArgumentNullExceptionWithoutClosingHandle()
        {
            // Arrange
            const string expected = "Handle remains readable";
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, expected);
                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => FileIO.ReadAllText(handle, null!));

                // Assert
                Assert.Equal("encoding", exception.ParamName);
                Assert.False(handle.IsClosed);
                Assert.Equal(expected, FileIO.ReadAllText(handle, System.Text.Encoding.UTF8));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class ReadAllTextAsync
    {
        [Fact]
        public async Task WithByteOrderMark_UsesDetectedEncodingAndLeavesHandleOpen()
        {
            // Arrange
            const string expected = "Hello, 世界";
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, expected, System.Text.Encoding.Unicode);
                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Act
                string result = await FileIO.ReadAllTextAsync(handle, System.Text.Encoding.ASCII);

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
        public void WithNullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            void Act() => _ = FileIO.ReadAllTextAsync(null!, System.Text.Encoding.UTF8);

            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(Act);

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public async Task WithNullEncoding_ThrowsArgumentNullExceptionWithoutClosingHandle()
        {
            // Arrange
            const string expected = "Handle remains readable";
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, expected);
                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);

                // Arrange
                void Act() => _ = FileIO.ReadAllTextAsync(handle, null!);

                // Act
                ArgumentNullException exception = Assert.Throws<ArgumentNullException>(Act);

                // Assert
                Assert.Equal("encoding", exception.ParamName);
                Assert.False(handle.IsClosed);
                Assert.Equal(expected, await FileIO.ReadAllTextAsync(handle, System.Text.Encoding.UTF8));
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
            const string expected = "Canceled reads should not consume the handle.";
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, expected);
                var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var handle = FileIO.OpenHandle(tempFile, options);
                using var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                // Act
                Task<string> task = FileIO.ReadAllTextAsync(handle, System.Text.Encoding.UTF8, cancellationTokenSource.Token);

                // Assert
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
                Assert.True(task.IsCanceled);
                Assert.False(handle.IsClosed);
                Assert.Equal(expected, await FileIO.ReadAllTextAsync(handle, System.Text.Encoding.UTF8));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

    }

}
