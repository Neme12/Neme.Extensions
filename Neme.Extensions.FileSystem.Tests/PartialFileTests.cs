namespace Neme.Extensions.FileSystem.Tests;

[Collection(nameof(FileIOTestCollection))]
public sealed class PartialFileTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_InvalidFinalPath_ThrowsArgumentException(string? finalPath)
    {
        // Arrange
        var options = CreateOptions();

        // Act & Assert
        var exception = Assert.ThrowsAny<ArgumentException>(() => PartialFile.Create(finalPath!, options));
        Assert.Equal("finalPath", exception.ParamName);
    }

    [Fact]
    public void Create_WithoutDeleteAccess_ThrowsArgumentException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = new FileOpenOptions(FileMode.CreateNew, FileSystemAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);

        try
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => PartialFile.Create(finalPath, options));
            Assert.Equal("options", exception.ParamName);
            Assert.Contains("Options must include delete access.", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void Create_CreateDirectoryTrue_ReturnsOpenPartialFileWithExpectedPaths()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "nested", "file.txt");
        var options = CreateOptions();

        try
        {
            // Act
            using var sut = PartialFile.Create(finalPath, options, createDirectory: true);

            // Assert
            Assert.True(Directory.Exists(Path.GetDirectoryName(finalPath)!));
            Assert.Equal(finalPath, sut.FinalPath);
            Assert.Equal(finalPath + ".part", sut.CurrentPath);
            Assert.Same(sut.File, sut.File);
            Assert.True(File.Exists(finalPath + ".part"));
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void File_WhenClosed_ThrowsInvalidOperationException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();

        try
        {
            using var sut = PartialFile.Create(finalPath, options, createDirectory: true);
            sut.Close();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _ = sut.File);
            Assert.Equal("File is closed.", exception.Message);
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void Properties_WhenDisposed_ThrowObjectDisposedException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();
        var sut = PartialFile.Create(finalPath, options, createDirectory: true);

        try
        {
            sut.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _ = sut.File);
            Assert.Throws<ObjectDisposedException>(() => _ = sut.FinalPath);
            Assert.Throws<ObjectDisposedException>(() => _ = sut.CurrentPath);
        }
        finally
        {
            sut.Dispose();
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void CurrentPath_AfterCommit_ReturnsFinalPath()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();

        try
        {
            using var sut = PartialFile.Create(finalPath, options, createDirectory: true);

            // Act
            sut.Commit();

            // Assert
            Assert.Equal(finalPath, sut.CurrentPath);
            Assert.True(File.Exists(finalPath));
            Assert.False(File.Exists(finalPath + ".part"));
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void Reopen_WhenOpen_ThrowsInvalidOperationException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();

        try
        {
            using var sut = PartialFile.Create(finalPath, options, createDirectory: true);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => sut.Reopen());
            Assert.Equal("File is not closed.", exception.Message);
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void Reopen_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();
        var sut = PartialFile.Create(finalPath, options, createDirectory: true);

        try
        {
            sut.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => sut.Reopen());
        }
        finally
        {
            sut.Dispose();
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void Reopen_WhenClosed_ReopensPartFileWithOpenMode()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();

        try
        {
            using var sut = PartialFile.Create(finalPath, options, createDirectory: true);
            var originalFile = sut.File;
            sut.Close();

            // Act
            sut.Reopen();

            // Assert
            Assert.Equal(finalPath, sut.FinalPath);
            Assert.Equal(finalPath + ".part", sut.CurrentPath);
            Assert.NotSame(originalFile, sut.File);
            Assert.Equal(FileMode.Open, sut.File.Options.Mode);
            Assert.True(File.Exists(finalPath + ".part"));
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }


    [Fact]
    public void Close_WhenOpen_ClosesHandleAndMarksFileClosed()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();

        try
        {
            using var sut = PartialFile.Create(finalPath, options, createDirectory: true);
            var handle = sut.File.Handle;

            // Act
            sut.Close();

            // Assert
            Assert.True(handle.IsClosed);
            Assert.True(File.Exists(finalPath + ".part"));
            Assert.Throws<InvalidOperationException>(() => _ = sut.File);
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }


    [Fact]
    public void Close_WhenClosed_ThrowsInvalidOperationException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();

        try
        {
            using var sut = PartialFile.Create(finalPath, options, createDirectory: true);
            sut.Close();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => sut.Close());
            Assert.Equal("File is not open.", exception.Message);
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void Close_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();
        var sut = PartialFile.Create(finalPath, options, createDirectory: true);

        try
        {
            sut.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => sut.Close());
        }
        finally
        {
            sut.Dispose();
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void Commit_WhenOverwriteTrue_ReplacesExistingFileAndUpdatesCurrentPath()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();
        Directory.CreateDirectory(tempDirectory);
        File.WriteAllBytes(finalPath, [1]);

        var sut = PartialFile.Create(finalPath, options);

        try
        {
            using var stream = sut.File.CreateFileStream(leaveOpen: true);
            stream.WriteByte(2);
            stream.Flush();

            // Act
            sut.Commit(overwrite: true);

            // Assert
            Assert.Equal(finalPath, sut.CurrentPath);
            Assert.True(File.Exists(finalPath));
            Assert.False(File.Exists(finalPath + ".part"));

            sut.Dispose();
            Assert.Equal([2], File.ReadAllBytes(finalPath));
        }
        finally
        {
            sut.Dispose();
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void Commit_WhenClosed_ThrowsInvalidOperationException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();

        try
        {
            using var sut = PartialFile.Create(finalPath, options, createDirectory: true);
            sut.Close();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => sut.Commit());
            Assert.Equal("File must be open to commit.", exception.Message);
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void Commit_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();
        var sut = PartialFile.Create(finalPath, options, createDirectory: true);

        try
        {
            sut.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => sut.Commit());
        }
        finally
        {
            sut.Dispose();
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void Dispose_WhenOpen_DeletesPartialFileAndClosesHandle()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();
        var sut = PartialFile.Create(finalPath, options, createDirectory: true);
        var handle = sut.File.Handle;

        try
        {
            // Act
            sut.Dispose();

            // Assert
            Assert.True(handle.IsClosed);
            Assert.False(File.Exists(finalPath + ".part"));
            Assert.Throws<ObjectDisposedException>(() => _ = sut.CurrentPath);
        }
        finally
        {
            sut.Dispose();
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void Dispose_WhenClosed_DeletesPartialFileAndCanBeCalledAgain()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();
        var sut = PartialFile.Create(finalPath, options, createDirectory: true);
        sut.Close();

        try
        {
            // Act
            sut.Dispose();
            sut.Dispose();

            // Assert
            Assert.False(File.Exists(finalPath + ".part"));
            Assert.Throws<ObjectDisposedException>(() => _ = sut.FinalPath);
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [Fact]
    public void Dispose_WhenCommitted_KeepsFinalFileAndClosesHandle()
    {
        // Arrange
        var tempDirectory = CreateTempDirectoryPath();
        var finalPath = Path.Combine(tempDirectory, "file.txt");
        var options = CreateOptions();

        try
        {
            using var sut = PartialFile.Create(finalPath, options, createDirectory: true);
            using var stream = sut.File.CreateFileStream(leaveOpen: true);
            stream.WriteByte(7);
            stream.Flush();
            sut.Commit();
            var handle = sut.File.Handle;

            // Act
            sut.Dispose();

            // Assert
            Assert.True(handle.IsClosed);
            Assert.True(File.Exists(finalPath));
            Assert.False(File.Exists(finalPath + ".part"));
            Assert.Equal([7], File.ReadAllBytes(finalPath));
            Assert.Throws<ObjectDisposedException>(() => _ = sut.File);
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    private static FileOpenOptions CreateOptions(FileMode mode = FileMode.CreateNew)
    {
        return new FileOpenOptions(mode, FileSystemAccess.ReadWrite | FileSystemAccess.Delete, FileShare.ReadWrite | FileShare.Delete);
    }

    private static string CreateTempDirectoryPath()
    {
        return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, recursive: true);
    }
}
