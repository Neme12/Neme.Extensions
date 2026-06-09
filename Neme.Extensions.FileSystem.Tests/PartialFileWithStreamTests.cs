namespace Neme.Extensions.FileSystem.Tests;

[Collection(nameof(FileIOTestCollection))]
public sealed class PartialFileWithStreamTests
{
    [Fact]
    public void Create_WithValidOptions_ReturnsOpenPartialFileWithExpectedPathsAndStream()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();

        try
        {
            // Act
            using var sut = PartialFileWithStream.Create(finalPath, options);

            // Assert
            Assert.Equal(finalPath, sut.FinalPath);
            Assert.Equal(finalPath + ".part", sut.CurrentPath);
            Assert.NotNull(sut.FileStream);
            Assert.True(File.Exists(finalPath + ".part"));
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Create_WhenFinalPathIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        string? finalPath = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => PartialFileWithStream.Create(finalPath!, CreateOptions()));
        Assert.Equal("finalPath", exception.ParamName);
    }

    [Fact]
    public void Create_WhenFinalPathIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        const string finalPath = "";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => PartialFileWithStream.Create(finalPath, CreateOptions()));
        Assert.Equal("finalPath", exception.ParamName);
    }

    [Fact]
    public void Create_WhenOptionsDoNotIncludeDeleteAccess_ThrowsArgumentException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = new FileOpenOptions(FileMode.Create, FileSystemAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);

        try
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => PartialFileWithStream.Create(finalPath, options));
            Assert.Equal("options", exception.ParamName);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Create_WhenCreateDirectoryIsTrue_CreatesMissingDirectoryAndPartialFile()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var nestedDirectory = Path.Combine(tempDirectory, "nested", "child");
        var finalPath = Path.Combine(nestedDirectory, "file.txt");

        try
        {
            // Act
            using var sut = PartialFileWithStream.Create(finalPath, CreateOptions(), createDirectory: true);

            // Assert
            Assert.True(Directory.Exists(nestedDirectory));
            Assert.True(File.Exists(finalPath + ".part"));
            Assert.Equal(finalPath, sut.FinalPath);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void FileStream_WhenClosed_ThrowsInvalidOperationException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");

        try
        {
            using var sut = PartialFileWithStream.Create(finalPath, CreateOptions());
            sut.Close();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _ = sut.FileStream);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void FileStream_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            sut.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _ = sut.FileStream);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void FinalPath_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            sut.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _ = sut.FinalPath);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void CurrentPath_WhenCommitted_ReturnsFinalPath()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");

        try
        {
            using var sut = PartialFileWithStream.Create(finalPath, CreateOptions());
            sut.FileStream.WriteByte(42);

            // Act
            sut.Commit();

            // Assert
            Assert.Equal(finalPath, sut.CurrentPath);
            Assert.True(File.Exists(finalPath));
            Assert.False(File.Exists(finalPath + ".part"));
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void CurrentPath_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            sut.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _ = sut.CurrentPath);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Reopen_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            sut.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => sut.Reopen());
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Reopen_WhenFileIsOpen_ThrowsInvalidOperationException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");

        try
        {
            using var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sut.Reopen());
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Reopen_WhenClosed_ReopensPartialFileAndAllowsAccess()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");

        try
        {
            using var sut = PartialFileWithStream.Create(finalPath, CreateOptions());
            var originalStream = sut.FileStream;
            originalStream.Write([1, 2], 0, 2);
            originalStream.Flush();
            sut.Close();

            // Act
            sut.Reopen();

            // Assert
            var reopenedStream = sut.FileStream;
            Assert.NotSame(originalStream, reopenedStream);
            Assert.Equal(2, reopenedStream.Length);

            reopenedStream.Position = reopenedStream.Length;
            reopenedStream.WriteByte(3);
            reopenedStream.Flush();
            reopenedStream.Position = 0;

            var buffer = new byte[3];
            var bytesRead = reopenedStream.Read(buffer, 0, buffer.Length);

            Assert.Equal(3, bytesRead);
            Assert.Equal([1, 2, 3], buffer);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Close_WhenFileIsOpen_ClosesFileAndAllowsReopen()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");

        try
        {
            using var sut = PartialFileWithStream.Create(finalPath, CreateOptions());
            sut.FileStream.WriteByte(42);
            sut.FileStream.Flush();

            // Act
            sut.Close();

            // Assert
            Assert.True(File.Exists(finalPath + ".part"));
            Assert.Throws<InvalidOperationException>(() => _ = sut.FileStream);

            sut.Reopen();
            Assert.Equal(1, sut.FileStream.Length);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }


    [Fact]
    public void Close_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            sut.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => sut.Close());
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Close_WhenFileIsNotOpen_ThrowsInvalidOperationException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");

        try
        {
            using var sut = PartialFileWithStream.Create(finalPath, CreateOptions());
            sut.Close();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => sut.Close());
            Assert.Equal("File is not open.", exception.Message);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Commit_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            sut.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => sut.Commit());
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Commit_WhenFileIsNotOpen_ThrowsInvalidOperationException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");

        try
        {
            using var sut = PartialFileWithStream.Create(finalPath, CreateOptions());
            sut.Close();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => sut.Commit());
            Assert.Equal("File must be open to commit.", exception.Message);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Commit_WhenOverwriteIsTrue_ReplacesExistingDestinationFile()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");

        try
        {
            File.WriteAllText(finalPath, "old");
            using var sut = PartialFileWithStream.Create(finalPath, CreateOptions());
            var bytes = new byte[] { 1, 2, 3 };
            sut.FileStream.Write(bytes, 0, bytes.Length);
            sut.FileStream.Flush();

            // Act
            sut.Commit(overwrite: true);
            var currentPath = sut.CurrentPath;
            sut.Dispose();

            // Assert
            Assert.Equal(finalPath, currentPath);
            Assert.True(File.Exists(finalPath));
            Assert.False(File.Exists(finalPath + ".part"));
            Assert.Equal(bytes, File.ReadAllBytes(finalPath));
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Dispose_WhenOpen_DeletesPartialFileAndDisposesStream()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());
        var stream = sut.FileStream;
        stream.WriteByte(42);
        stream.Flush();

        try
        {
            // Act
            sut.Dispose();

            // Assert
            Assert.False(File.Exists(finalPath + ".part"));
            Assert.Throws<ObjectDisposedException>(() => stream.WriteByte(43));
            Assert.Throws<ObjectDisposedException>(() => _ = sut.CurrentPath);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Dispose_WhenClosed_DeletesPartialFile()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            sut.Close();

            // Act
            sut.Dispose();

            // Assert
            Assert.False(File.Exists(finalPath + ".part"));
            Assert.Throws<ObjectDisposedException>(() => _ = sut.FinalPath);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Dispose_WhenCommitted_PreservesCommittedFile()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());
        sut.FileStream.WriteByte(42);
        sut.FileStream.Flush();

        try
        {
            sut.Commit();

            // Act
            sut.Dispose();

            // Assert
            Assert.True(File.Exists(finalPath));
            Assert.Equal(new byte[] { 42 }, File.ReadAllBytes(finalPath));
            Assert.Throws<ObjectDisposedException>(() => _ = sut.FileStream);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Dispose_WhenAlreadyDisposed_DoesNothing()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            sut.Dispose();

            // Act
            var exception = Record.Exception(() => sut.Dispose());

            // Assert
            Assert.Null(exception);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }


#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [Fact]
    public async Task CloseAsync_WhenFileIsOpen_ClosesFileAndDisposesStream()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            var stream = sut.FileStream;
            stream.WriteByte(42);
            await stream.FlushAsync();

            // Act
            await sut.CloseAsync();

            // Assert
            Assert.True(File.Exists(finalPath + ".part"));
            Assert.Throws<ObjectDisposedException>(() => stream.WriteByte(43));
            Assert.Throws<InvalidOperationException>(() => _ = sut.FileStream);
        }
        finally
        {
            await sut.DisposeAsync();
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public async Task CloseAsync_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            await sut.DisposeAsync();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => sut.CloseAsync());
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public async Task CloseAsync_WhenFileIsNotOpen_ThrowsInvalidOperationException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            await sut.CloseAsync();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CloseAsync());
            Assert.Equal("File is not open.", exception.Message);
        }
        finally
        {
            await sut.DisposeAsync();
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public async Task DisposeAsync_WhenOpen_DeletesPartialFileAndDisposesStream()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());
        var stream = sut.FileStream;
        stream.WriteByte(42);
        await stream.FlushAsync();

        try
        {
            // Act
            await sut.DisposeAsync();

            // Assert
            Assert.False(File.Exists(finalPath + ".part"));
            Assert.Throws<ObjectDisposedException>(() => stream.WriteByte(43));
            Assert.Throws<ObjectDisposedException>(() => _ = sut.CurrentPath);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public async Task DisposeAsync_WhenClosed_DeletesPartialFile()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            await sut.CloseAsync();

            // Act
            await sut.DisposeAsync();

            // Assert
            Assert.False(File.Exists(finalPath + ".part"));
            Assert.Throws<ObjectDisposedException>(() => _ = sut.FinalPath);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public async Task DisposeAsync_WhenCommitted_PreservesCommittedFile()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());
        sut.FileStream.WriteByte(42);
        await sut.FileStream.FlushAsync();

        try
        {
            sut.Commit();

            // Act
            await sut.DisposeAsync();

            // Assert
            Assert.True(File.Exists(finalPath));
            Assert.Equal(new byte[] { 42 }, File.ReadAllBytes(finalPath));
            Assert.Throws<ObjectDisposedException>(() => _ = sut.FileStream);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public async Task DisposeAsync_WhenAlreadyDisposed_DoesNothing()
    {
        // Arrange
        var tempDirectory = CreateTempDirectory();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var sut = PartialFileWithStream.Create(finalPath, CreateOptions());

        try
        {
            await sut.DisposeAsync();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.DisposeAsync());

            // Assert
            Assert.Null(exception);
        }
        finally
        {
            DeleteDirectory(tempDirectory);
        }
    }
#endif


    private static FileOpenOptions CreateOptions() =>
        new(FileMode.Create, FileSystemAccess.ReadWrite | FileSystemAccess.Delete, FileShare.ReadWrite | FileShare.Delete);

    private static string CreateTempDirectory()
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);
        return tempDirectory;
    }

    private static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, recursive: true);
    }
}
