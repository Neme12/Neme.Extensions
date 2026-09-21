using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.Internal;
using Neme.Extensions.IO;

namespace Neme.Extensions.FileSystem.Tests;

public sealed partial class FileIOTests
{
    private static void SetTempFileLength(SafeFileHandle file, long length)
    {
        using var stream = new LeaveOpenFileStream(file, FileAccess.ReadWrite, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
        stream.SetLength(length);
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class ReadAllBytesAsync
    {
        [Fact]
        public async Task WithNonEmptyFile_ReturnsFileContents()
        {
            // Arrange
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            byte[] expected = [1, 2, 3, 4, 5];
            FileIO.WriteAllBytes(tempFile, expected);

            var options = FileOpenOptions.Open(FileSystemAccess.Read, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            byte[] result = await FileIO.ReadAllBytesAsync(handle);

            // Assert
            Assert.Equal(expected, result);
            Assert.False(handle.IsClosed);
        }

        [Fact]
        public async Task WithCanceledToken_ReturnsCanceledTaskWithoutClosingHandle()
        {
            // Arrange
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            byte[] expected = [9, 8, 7];
            FileIO.WriteAllBytes(tempFile, expected);

            var options = FileOpenOptions.Open(FileSystemAccess.Read, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
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

        [Fact]
        public async Task WithFileLongerThanArrayMaxLength_ClosesHandleAndThrowsIOException()
        {
            // Arrange
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            SetTempFileLength(tempFile, (long)Array.MaxLength + 1);

            var options = FileOpenOptions.Open(FileSystemAccess.Read, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            Task<byte[]> task = FileIO.ReadAllBytesAsync(handle);

            // Assert
            IOException exception = await Assert.ThrowsAsync<IOException>(() => task);
            Assert.Contains("2 gigabytes", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.False(handle.IsClosed);
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class ReadAllBytes
    {
        [Fact]
        public void WithNonEmptyFile_ReturnsFileContents()
        {
            // Arrange
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            byte[] expected = [1, 2, 3, 4, 5];
            FileIO.WriteAllBytes(tempFile, expected);

            var options = FileOpenOptions.Open(FileSystemAccess.Read, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            byte[] result = FileIO.ReadAllBytes(handle);

            // Assert
            Assert.Equal(expected, result);
            Assert.False(handle.IsClosed);
        }

        [Fact]
        public void WithEmptyFile_ReturnsEmptyArray()
        {
            // Arrange
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            var options = FileOpenOptions.Open(FileSystemAccess.Read, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            byte[] result = FileIO.ReadAllBytes(handle);

            // Assert
            Assert.Empty(result);
            Assert.False(handle.IsClosed);
        }

        [Fact]
        public void WithCanceledToken_ThrowsOperationCanceledExceptionWithoutClosingHandle()
        {
            // Arrange
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            byte[] expected = [9, 8, 7];
            FileIO.WriteAllBytes(tempFile, expected);

            var options = FileOpenOptions.Open(FileSystemAccess.Read, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act & Assert
            Assert.ThrowsAny<OperationCanceledException>(() => FileIO.ReadAllBytes(handle, cancellationTokenSource.Token));
            Assert.False(handle.IsClosed);

            byte[] result = FileIO.ReadAllBytes(handle);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void WithFileLongerThanArrayMaxLength_ThrowsIOExceptionWithoutClosingHandle()
        {
            // Arrange
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            SetTempFileLength(tempFile, (long)Array.MaxLength + 1);

            var options = FileOpenOptions.Open(FileSystemAccess.Read, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            IOException exception = Assert.Throws<IOException>(() => FileIO.ReadAllBytes(handle));

            // Assert
            Assert.Contains("2 gigabytes", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.False(handle.IsClosed);
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
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllText(tempFile, expected.AsSpan(), System.Text.Encoding.Unicode);

            var options = FileOpenOptions.Open(FileSystemAccess.Read, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            string result = FileIO.ReadAllText(handle, System.Text.Encoding.ASCII);

            // Assert
            Assert.Equal(expected, result);
            Assert.False(handle.IsClosed);
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
        public void WithCanceledToken_ThrowsOperationCanceled()
        {
            // Arrange
            const string expected = "Canceled reads should not consume the handle.";
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllText(tempFile, expected.AsSpan());
            var options = FileOpenOptions.Open(FileSystemAccess.Read, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act
            var result = Assert.ThrowsAny<OperationCanceledException>(() => FileIO.ReadAllText(handle, System.Text.Encoding.UTF8, cancellationTokenSource.Token));

            // Assert
            Assert.False(handle.IsClosed);
            Assert.Equal(expected, FileIO.ReadAllText(handle, System.Text.Encoding.UTF8));
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
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllText(tempFile, expected.AsSpan(), System.Text.Encoding.Unicode);

            var options = FileOpenOptions.Open(FileSystemAccess.Read, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            string result = await FileIO.ReadAllTextAsync(handle, System.Text.Encoding.ASCII);

            // Assert
            Assert.Equal(expected, result);
            Assert.False(handle.IsClosed);
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
        public async Task WithCanceledToken_ReturnsCanceledTaskWithoutClosingHandle()
        {
            // Arrange
            const string expected = "Canceled reads should not consume the handle.";
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllText(tempFile, expected.AsSpan());
            var options = FileOpenOptions.Open(FileSystemAccess.Read, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
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
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class WriteAllText
    {
        [Fact]
        public void WithUtf8DefaultEncoding_WritesWithoutByteOrderMarkAndLeavesHandleOpen()
        {
            // Arrange
            const string expected = "Hello, 世界";
            var expectedBytes = System.Text.Encoding.UTF8.GetBytes(expected);
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            FileIO.WriteAllText(handle, expected.AsSpan());

            // Assert
            Assert.False(handle.IsClosed);
            Assert.Equal(expectedBytes, FileIO.ReadAllBytes(handle));
        }

        [Fact]
        public void WithNullHandle_ThrowsArgumentNullException()
        {
            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => FileIO.WriteAllText(null!, "test".AsSpan(), System.Text.Encoding.UTF8));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void WithCanceledToken_ThrowsOperationCanceledExceptionWithoutChangingFileOrClosingHandle()
        {
            // Arrange
            const string originalContents = "Original contents";
            const string replacementContents = "Replacement contents";
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllText(tempFile, originalContents.AsSpan());
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act & Assert
            Assert.ThrowsAny<OperationCanceledException>(() => FileIO.WriteAllText(handle, replacementContents.AsSpan(), System.Text.Encoding.UTF8, cancellationTokenSource.Token));
            Assert.False(handle.IsClosed);
            Assert.Equal(originalContents, FileIO.ReadAllText(handle, System.Text.Encoding.UTF8));
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class WriteAllTextAsync
    {
        [Fact]
        public async Task WithUtf8DefaultEncoding_WritesWithoutByteOrderMarkAndLeavesHandleOpen()
        {
            // Arrange
            const string expected = "Hello, 世界";
            var expectedBytes = System.Text.Encoding.UTF8.GetBytes(expected);
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            await FileIO.WriteAllTextAsync(handle, expected.AsMemory());

            // Assert
            Assert.False(handle.IsClosed);
            Assert.Equal(expectedBytes, FileIO.ReadAllBytes(handle));
        }

        [Fact]
        public void WithNullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            void Act() => _ = FileIO.WriteAllTextAsync(null!, "test".AsMemory(), System.Text.Encoding.UTF8);

            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(Act);

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public async Task WithCanceledToken_ReturnsCanceledTaskWithoutChangingFileOrClosingHandle()
        {
            // Arrange
            const string originalContents = "Original contents";
            const string replacementContents = "Replacement contents";
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllText(tempFile, originalContents.AsSpan());
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act
            Task task = FileIO.WriteAllTextAsync(handle, replacementContents.AsMemory(), System.Text.Encoding.UTF8, cancellationTokenSource.Token);

            // Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
            Assert.True(task.IsCanceled);
            Assert.False(handle.IsClosed);
            Assert.Equal(originalContents, FileIO.ReadAllText(handle, System.Text.Encoding.UTF8));
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class WriteAllBytes
    {
        [Fact]
        public void WithBytes_WritesContentsAndLeavesHandleOpen()
        {
            // Arrange
            byte[] expected = [1, 2, 3, 4, 5];
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            FileIO.WriteAllBytes(handle, expected.AsSpan());

            // Assert
            Assert.False(handle.IsClosed);
            Assert.Equal(expected, FileIO.ReadAllBytes(handle));
        }

        [Fact]
        public void WithNullHandle_ThrowsArgumentNullException()
        {
            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => FileIO.WriteAllBytes(null!, new byte[] { 1 }.AsSpan()));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void WithCanceledToken_ThrowsOperationCanceledExceptionWithoutChangingFileOrClosingHandle()
        {
            // Arrange
            byte[] originalBytes = [9, 8, 7];
            byte[] replacementBytes = [1, 2, 3, 4];
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(tempFile, originalBytes);
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act & Assert
            Assert.ThrowsAny<OperationCanceledException>(() => FileIO.WriteAllBytes(handle, replacementBytes.AsSpan(), cancellationTokenSource.Token));
            Assert.False(handle.IsClosed);
            Assert.Equal(originalBytes, FileIO.ReadAllBytes(handle));
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class WriteAllBytesAsync
    {
        [Fact]
        public async Task WithBytes_WritesContentsAndLeavesHandleOpen()
        {
            // Arrange
            byte[] expected = [1, 2, 3, 4, 5];
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            await FileIO.WriteAllBytesAsync(handle, expected.AsMemory());

            // Assert
            Assert.False(handle.IsClosed);
            Assert.Equal(expected, FileIO.ReadAllBytes(handle));
        }

        [Fact]
        public void WithNullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            void Act() => _ = FileIO.WriteAllBytesAsync(null!, new byte[] { 1 }.AsMemory());

            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(Act);

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public async Task WithCanceledToken_ReturnsCanceledTaskWithoutChangingFileOrClosingHandle()
        {
            // Arrange
            byte[] originalBytes = [9, 8, 7];
            byte[] replacementBytes = [1, 2, 3, 4];
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(tempFile, originalBytes);
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act
            Task task = FileIO.WriteAllBytesAsync(handle, replacementBytes.AsMemory(), cancellationTokenSource.Token);

            // Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
            Assert.True(task.IsCanceled);
            Assert.False(handle.IsClosed);
            Assert.Equal(originalBytes, FileIO.ReadAllBytes(handle));
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class AppendAllBytes
    {
        [Fact]
        public void WithBytes_AppendsContentsAndLeavesHandleOpen()
        {
            // Arrange
            byte[] originalBytes = [9, 8, 7];
            byte[] appendedBytes = [1, 2, 3, 4];
            byte[] expectedBytes = [9, 8, 7, 1, 2, 3, 4];
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(tempFile, originalBytes);
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            FileIO.AppendAllBytes(handle, appendedBytes.AsSpan());

            // Assert
            Assert.False(handle.IsClosed);
            using var stream = FileIO.CreateFileStream(handle, options, leaveOpen: true, bufferSize: 128);
            stream.Position = 0;
            var actualBytes = new byte[stream.Length];
            _ = stream.Read(actualBytes, 0, actualBytes.Length);
            Assert.Equal(expectedBytes, actualBytes);
        }

        [Fact]
        public void WithNullHandle_ThrowsArgumentNullException()
        {
            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => FileIO.AppendAllBytes(null!, new byte[] { 1 }.AsSpan()));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void WithCanceledToken_ThrowsOperationCanceledExceptionWithoutChangingFileOrClosingHandle()
        {
            // Arrange
            byte[] originalBytes = [9, 8, 7];
            byte[] appendedBytes = [1, 2, 3, 4];
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(tempFile, originalBytes);
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act & Assert
            Assert.ThrowsAny<OperationCanceledException>(() => FileIO.AppendAllBytes(handle, appendedBytes.AsSpan(), cancellationTokenSource.Token));
            Assert.False(handle.IsClosed);
            using var stream = FileIO.CreateFileStream(handle, options, leaveOpen: true, bufferSize: 128);
            stream.Position = 0;
            var actualBytes = new byte[stream.Length];
            _ = stream.Read(actualBytes, 0, actualBytes.Length);
            Assert.Equal(originalBytes, actualBytes);
        }

        [Fact]
        public void WithClosedHandle_ThrowsArgumentInvalidException()
        {
            // Arrange
            byte[] originalBytes = [9, 8, 7];
            byte[] appendedBytes = [1, 2, 3, 4];
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(tempFile, originalBytes);
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
            handle.Dispose();

            // Act
            var exception = Assert.Throws<Neme.Extensions.Contracts.ArgumentInvalidException>(() => FileIO.AppendAllBytes(handle, appendedBytes.AsSpan()));

            // Assert
            Assert.Equal("file", exception.ParamName);
            Assert.Equal(originalBytes, FileIO.ReadAllBytes(tempFile));
        }

    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class AppendAllBytesAsync
    {
        [Fact]
        public async Task WithBytes_AppendsContentsAndLeavesHandleOpen()
        {
            // Arrange
            byte[] originalBytes = [9, 8, 7];
            byte[] appendedBytes = [1, 2, 3, 4];
            byte[] expectedBytes = [9, 8, 7, 1, 2, 3, 4];
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(tempFile, originalBytes);
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            await FileIO.AppendAllBytesAsync(handle, appendedBytes.AsMemory());

            // Assert
            Assert.False(handle.IsClosed);
            using var stream = FileIO.CreateFileStream(handle, options, leaveOpen: true, bufferSize: 128);
            stream.Position = 0;
            var actualBytes = new byte[stream.Length];
            _ = await stream.ReadAsync(actualBytes, 0, actualBytes.Length);
            Assert.Equal(expectedBytes, actualBytes);
        }

        [Fact]
        public void WithNullHandle_ThrowsArgumentNullException()
        {
            // Arrange
            void Act() => _ = FileIO.AppendAllBytesAsync(null!, new byte[] { 1 }.AsMemory());

            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(Act);

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public async Task WithCanceledToken_ReturnsCanceledTaskWithoutChangingFileOrClosingHandle()
        {
            // Arrange
            byte[] originalBytes = [9, 8, 7];
            byte[] appendedBytes = [1, 2, 3, 4];
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(tempFile, originalBytes);
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act
            Task task = FileIO.AppendAllBytesAsync(handle, appendedBytes.AsMemory(), cancellationTokenSource.Token);

            // Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
            Assert.True(task.IsCanceled);
            Assert.False(handle.IsClosed);
            using var stream = FileIO.CreateFileStream(handle, options, leaveOpen: true, bufferSize: 128);
            stream.Position = 0;
            var actualBytes = new byte[stream.Length];
            _ = await stream.ReadAsync(actualBytes, 0, actualBytes.Length);
            Assert.Equal(originalBytes, actualBytes);
        }

        [Fact]
        public void WithClosedHandle_ThrowsArgumentInvalidException()
        {
            // Arrange
            byte[] originalBytes = [9, 8, 7];
            byte[] appendedBytes = [1, 2, 3, 4];
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllBytes(tempFile, originalBytes);
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
            handle.Dispose();

            // Arrange
            void Act() => _ = FileIO.AppendAllBytesAsync(handle, appendedBytes.AsMemory());

            // Act
            var exception = Assert.Throws<Neme.Extensions.Contracts.ArgumentInvalidException>(Act);

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void WithNonSeekableHandle_ThrowsArgumentInvalidException()
        {
            // Arrange
            using var pipe = new System.IO.Pipes.AnonymousPipeServerStream(System.IO.Pipes.PipeDirection.Out, System.IO.HandleInheritability.None);
            using var handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(pipe.SafePipeHandle.DangerousGetHandle(), ownsHandle: false);

            // Arrange
            void Act() => FileIO.AppendAllBytesAsync(handle, new byte[] { 1 }.AsMemory());

            // Act
            var exception = Assert.Throws<Neme.Extensions.Contracts.ArgumentInvalidException>(Act);

            // Assert
            Assert.Equal("file", exception.ParamName);
        }
    }

    [Collection(nameof(FileIOTestCollection))]
    public sealed class AppendAllText
    {
        [Fact]
        public void WithText_AppendsContentsAndLeavesHandleOpen()
        {
            // Arrange
            const string originalContents = "Hello";
            const string appendedContents = ", 世界";
            const string expectedContents = "Hello, 世界";
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllText(tempFile, originalContents.AsSpan(), new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            FileIO.AppendAllText(handle, appendedContents.AsSpan(), encoding: null);

            // Assert
            Assert.False(handle.IsClosed);
            using var stream = FileIO.CreateFileStream(handle, options, leaveOpen: true, bufferSize: 128);
            stream.Position = 0;
            using var reader = new StreamReader(stream, new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false), detectEncodingFromByteOrderMarks: true, bufferSize: 128, leaveOpen: true);
            Assert.Equal(expectedContents, reader.ReadToEnd());
        }

        [Fact]
        public void WithSpecifiedEncoding_AppendsUsingThatEncoding()
        {
            // Arrange
            const string originalContents = "Hello";
            const string appendedContents = " World";
            const string expectedContents = "Hello World";
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllText(tempFile, originalContents.AsSpan(), System.Text.Encoding.ASCII);

            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);

            // Act
            FileIO.AppendAllText(handle, appendedContents.AsSpan(), System.Text.Encoding.ASCII);

            // Assert
            Assert.False(handle.IsClosed);
            using var stream = FileIO.CreateFileStream(handle, options, leaveOpen: true, bufferSize: 128);
            stream.Position = 0;
            var actualBytes = new byte[stream.Length];
            _ = stream.Read(actualBytes, 0, actualBytes.Length);
            Assert.Equal(System.Text.Encoding.ASCII.GetBytes(expectedContents), actualBytes);
        }

        [Fact]
        public void WithNullHandle_ThrowsArgumentNullException()
        {
            // Act
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => FileIO.AppendAllText(null!, "test".AsSpan(), System.Text.Encoding.UTF8));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void WithCanceledToken_ThrowsOperationCanceledExceptionWithoutChangingFileOrClosingHandle()
        {
            // Arrange
            const string originalContents = "Original contents";
            const string appendedContents = " plus appended contents";
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllText(tempFile, originalContents.AsSpan(), new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act & Assert
            Assert.ThrowsAny<OperationCanceledException>(() => FileIO.AppendAllText(handle, appendedContents.AsSpan(), encoding: null, cancellationTokenSource.Token));
            Assert.False(handle.IsClosed);
            using var stream = FileIO.CreateFileStream(handle, options, leaveOpen: true, bufferSize: 128);
            stream.Position = 0;
            using var reader = new StreamReader(stream, new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false), detectEncodingFromByteOrderMarks: true, bufferSize: 128, leaveOpen: true);
            Assert.Equal(originalContents, reader.ReadToEnd());
        }

        [Fact]
        public void WithClosedHandle_ThrowsArgumentInvalidException()
        {
            // Arrange
            const string originalContents = "Original contents";
            using var tempFile = FileIO.CreateTempFileHandle(FileSystemAccess.ReadWrite, FileShare.All);
            FileIO.WriteAllText(tempFile, originalContents.AsSpan(), new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            var options = FileOpenOptions.Open(FileSystemAccess.ReadWrite, FileShare.All);
            using var handle = FileIO.ReopenHandle(tempFile, options);
            handle.Dispose();

            // Act
            var exception = Assert.Throws<Neme.Extensions.Contracts.ArgumentInvalidException>(() => FileIO.AppendAllText(handle, " more text".AsSpan(), encoding: null));

            // Assert
            Assert.Equal("file", exception.ParamName);
            Assert.Equal(originalContents, FileIO.ReadAllText(tempFile, new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false)));
        }

        [Fact]
        public void WithNonSeekableHandle_ThrowsArgumentInvalidException()
        {
            // Arrange
            using var pipe = new System.IO.Pipes.AnonymousPipeServerStream(System.IO.Pipes.PipeDirection.Out, System.IO.HandleInheritability.None);
            using var handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(pipe.SafePipeHandle.DangerousGetHandle(), ownsHandle: false);

            // Arrange
            void Act() => FileIO.AppendAllText(handle, "text".AsSpan(), System.Text.Encoding.UTF8);

            // Act
            var exception = Assert.Throws<Neme.Extensions.Contracts.ArgumentInvalidException>(Act);

            // Assert
            Assert.Equal("file", exception.ParamName);
        }
    }


}
