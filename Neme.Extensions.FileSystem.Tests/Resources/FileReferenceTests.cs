using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.Resources;
using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.FileSystem.Tests.TestUtilities;
using Neme.Extensions.IO;
using Neme.Extensions.Tests.Utilities;
using NodaTime;
using IOPath = System.IO.Path;

namespace Neme.Extensions.FileSystem.Tests.Resources;

public sealed class FileReferenceTests
{
    public sealed class Handle
    {
        [Fact]
        public void CreatedReference_ReturnsOpenHandleForCreatedPath()
        {
            // Arrange
            var expected = CreateUniquePath();

            using (CreateTemporaryFileScope(expected))
            using (var fileReference = CreateFileReference(expected))
            {
                // Act
                SafeFileHandle result = fileReference.Handle;
                var openedPath = result.OpenedPath;

                // Assert
                Assert.False(result.IsClosed);
                Assert.False(result.IsInvalid);
#if !NETFRAMEWORK
                Assert.NotNull(openedPath);
                PathUtilities.AssertPathsEqual(expected, openedPath);
#endif
            }
        }
    }

    public sealed class OpenedPath
    {
        [Fact]
        public void CreatedReference_ReturnsOriginalPath()
        {
            // Arrange
            var expected = CreateUniquePath();

            using (CreateTemporaryFileScope(expected))
            using (var fileReference = CreateFileReference(expected))
            {
                // Act
                var result = fileReference.OpenedPath;

                // Assert
                Assert.NotNull(result);
                PathUtilities.AssertPathsEqual(expected, result);
            }
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedException()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => _ = fileReference.OpenedPath);

                // Assert
                Assert.NotNull(exception);
            }
        }
    }

    public sealed class Options
    {
        [Fact]
        public void CreatedReference_ReturnsRequestOptions()
        {
            // Arrange
            var expected = default(FileReferenceOptions);
            var path = CreateUniquePath();

            using (CreateTemporaryFileScope(path))
            using (var fileReference = CreateFileReference(path))
            {
                // Act
                var result = fileReference.Options;

                // Assert
                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedException()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => _ = fileReference.Options);

                // Assert
                Assert.NotNull(exception);
            }
        }
    }

    public sealed class Type
    {
        [Fact]
        public void CreatedReference_ReturnsRegularFile()
        {
            // Arrange
            var path = CreateUniquePath();

            using (CreateTemporaryFileScope(path))
            using (var fileReference = CreateFileReference(path))
            {
                // Act
                var result = fileReference.Type;

                // Assert
                Assert.Equal(FileHandleType.RegularFile, result);
            }
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedException()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => _ = fileReference.Type);

                // Assert
                Assert.NotNull(exception);
            }
        }
    }

    public sealed class IsOpen
    {
        [Fact]
        public void CreatedReference_ReturnsTrue()
        {
            // Arrange
            var path = CreateUniquePath();
            bool result;

            using (CreateTemporaryFileScope(path))
            {
                using (var fileReference = CreateFileReference(path))
                    result = fileReference.IsOpen;

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public void DisposedReference_ReturnsFalse()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Dispose();

                // Act
                var result = fileReference.IsOpen;

                // Assert
                Assert.False(result);
            }
        }
    }

    private static FileReference CreateFileReference(string path)
        => FileReference.Create(path, FileReferenceRequest.CreateNew(creationOptions: new FileCreationOptions(FileAttributes.Temporary)));

    private static string CreateUniquePath()
        => $"{IOPath.GetTempPath()}{nameof(FileReferenceTests)}_{Guid.NewGuid():N}.tmp";

    public sealed class IsClosed
    {
        [Fact]
        public void CreatedReference_ReturnsFalse()
        {
            // Arrange
            var path = CreateUniquePath();
            bool result;

            using (CreateTemporaryFileScope(path))
            {
                using (var fileReference = CreateFileReference(path))
                    result = fileReference.IsClosed;

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public void ClosedHandle_ReturnsTrue()
        {
            // Arrange
            var path = CreateUniquePath();
            bool result;

            using (CreateTemporaryFileScope(path))
            using (var fileReference = CreateFileReference(path))
            {
                fileReference.Handle.Dispose();
                result = fileReference.IsClosed;
            }

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DisposedReference_ReturnsTrue()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Dispose();

                // Act
                var result = fileReference.IsClosed;

                // Assert
                Assert.True(result);
            }
        }
    }


    public sealed class CanSeek
    {
        [Fact]
        public void CreatedReference_ReturnsTrue()
        {
            // Arrange
            var path = CreateUniquePath();
            bool result;

            using (CreateTemporaryFileScope(path))
            {
                using (var fileReference = CreateFileReference(path))
                    result = fileReference.CanSeek;

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public void ClosedHandle_ThrowsObjectDisposedException()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Handle.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => _ = fileReference.CanSeek);

                // Assert
                Assert.NotNull(exception);
                fileReference.Dispose();
            }
        }
    }

    public sealed class GetPath
    {
        [Fact]
        public void CreatedReference_ReturnsOriginalPath()
        {
            // Arrange
            var expected = CreateUniquePath();
            string result;

            using (CreateTemporaryFileScope(expected))
            {
                using (var fileReference = CreateFileReference(expected))
                    result = fileReference.GetPath();

                // Assert
                PathUtilities.AssertPathsEqual(expected, result);
            }
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedException()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => _ = fileReference.GetPath());

                // Assert
                Assert.NotNull(exception);
            }
        }
    }

    public sealed class GetId
    {
        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void CreatedReference_ReturnsSameIdAsIndependentHandle()
        {
            // Arrange
            var path = CreateUniquePath();
            FileId expected;
            FileId result;

            using (CreateTemporaryFileScope(path))
            using (var fileReference = CreateFileReference(path))
            using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                expected = FileOperations.GetId(fileStream.SafeFileHandle);
                result = fileReference.GetId();
            }

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedException()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => _ = fileReference.GetId());

                // Assert
                Assert.NotNull(exception);
            }
        }
    }

    public sealed class GetAttributes
    {
        [Fact]
        public void CreatedReference_ReturnsFileSystemAttributes()
        {
            // Arrange
            var path = CreateUniquePath();
            FileAttributes expected;
            FileAttributes result;

            using (CreateTemporaryFileScope(path))
            using (var fileReference = CreateFileReference(path))
            {
                expected = File.GetAttributes(path);
                result = fileReference.GetAttributes();
            }

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedException()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => _ = fileReference.GetAttributes());

                // Assert
                Assert.NotNull(exception);
            }
        }
    }

    public sealed class SetAttributes
    {
        [Fact]
        public void CreatedReference_UpdatesFileSystemAttributes()
        {
            // Arrange
            var path = CreateUniquePath();
            var expected = FileAttributes.ReadOnly;
            FileAttributes result;

            using (CreateTemporaryFileScope(path))
            {
                using (var fileReference = CreateFileReference(path))
                {
                    // Act
                    fileReference.SetAttributes(expected);
                    result = File.GetAttributes(path);
                }

                // Assert
                Assert.Equal(expected, result);

                File.SetAttributes(path, FileAttributes.Normal);
            }
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedExceptionWithoutChangingAttributes()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                var expected = File.GetAttributes(path);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => fileReference.SetAttributes(FileAttributes.ReadOnly));
                var result = File.GetAttributes(path);

                // Assert
                Assert.NotNull(exception);
                Assert.Equal(expected, result);
                Assert.True(File.Exists(path));
            }
        }
    }

    public sealed class GetBasicInfo
    {
        [Fact]
        public void CreatedReference_ReturnsCurrentFileInfo()
        {
            // Arrange
            var path = CreateUniquePath();
            var expectedSize = 4;
            var earliestCreationTime = SystemClock.Instance.GetCurrentInstant() - Duration.FromMilliseconds(1);

            using (CreateTemporaryFileScope(path))
            using (var fileReference = CreateFileReference(path))
            {
                var latestCreationTime = SystemClock.Instance.GetCurrentInstant() + Duration.FromMilliseconds(1);

                var earliestWriteTime = SystemClock.Instance.GetCurrentInstant() - Duration.FromMilliseconds(1);
                File.WriteAllBytes(path, [1, 2, 3, 4]);
                var latestWriteTime = SystemClock.Instance.GetCurrentInstant() + Duration.FromMilliseconds(1);
                var latestAccessTime = SystemClock.Instance.GetCurrentInstant() + Duration.FromMilliseconds(100);

                // Act
                var result = fileReference.GetBasicInfo();

                // Assert
                Assert.Equal(expectedSize, result.Size);
                Assert.Equal(File.GetAttributes(path), result.Attributes);
                Assert.InRange(result.CreationTime!.Value, earliestCreationTime, latestCreationTime);
                Assert.InRange(result.LastWriteTime, earliestWriteTime, latestWriteTime);
                Assert.InRange(result.LastAccessTime, earliestWriteTime, latestAccessTime);
                Assert.InRange(result.LastChangeTime, earliestWriteTime, latestAccessTime);
            }
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedException()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => _ = fileReference.GetBasicInfo());

                // Assert
                Assert.NotNull(exception);
            }
        }
    }

    public sealed class Move
    {
        [Fact]
        public void CreatedReference_MovesFileToDestination()
        {
            // Arrange
            var directoryPath = Path.Combine(Path.GetTempPath(), $"{nameof(FileReferenceTests)}_{Guid.NewGuid():N}");
            var sourcePath = Path.Combine(directoryPath, "source.tmp");
            var destinationPath = Path.Combine(directoryPath, "destination.tmp");
            Directory.CreateDirectory(directoryPath);
            File.WriteAllText(sourcePath, "source content");

            using (var fileReference = FileReference.Create(sourcePath, FileReferenceRequest.Open()))
            {
                // Act
                fileReference.Move(destinationPath);
            }

            // Assert
            Assert.False(File.Exists(sourcePath));
            Assert.True(File.Exists(destinationPath));
            Assert.Equal("source content", File.ReadAllText(destinationPath));
            Directory.Delete(directoryPath, recursive: true);
        }

        [Fact]
        public void ExistingDestinationAndOverwriteTrue_ReplacesDestinationContents()
        {
            // Arrange
            var directoryPath = Path.Combine(Path.GetTempPath(), $"{nameof(FileReferenceTests)}_{Guid.NewGuid():N}");
            var sourcePath = Path.Combine(directoryPath, "source.tmp");
            var destinationPath = Path.Combine(directoryPath, "destination.tmp");
            Directory.CreateDirectory(directoryPath);
            File.WriteAllText(sourcePath, "source content");
            File.WriteAllText(destinationPath, "destination content");

            using (var fileReference = FileReference.Create(sourcePath, FileReferenceRequest.Open()))
            {
                // Act
                fileReference.Move(destinationPath, overwrite: true);
            }

            // Assert
            Assert.False(File.Exists(sourcePath));
            Assert.True(File.Exists(destinationPath));
            Assert.Equal("source content", File.ReadAllText(destinationPath));
            Directory.Delete(directoryPath, recursive: true);
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedExceptionWithoutMovingFile()
        {
            // Arrange
            var directoryPath = Path.Combine(Path.GetTempPath(), $"{nameof(FileReferenceTests)}_{Guid.NewGuid():N}");
            var sourcePath = Path.Combine(directoryPath, "source.tmp");
            var destinationPath = Path.Combine(directoryPath, "destination.tmp");
            Directory.CreateDirectory(directoryPath);
            var fileReference = CreateFileReference(sourcePath);
            fileReference.Dispose();

            // Act
            var exception = Assert.Throws<ObjectDisposedException>(() => fileReference.Move(destinationPath));

            // Assert
            Assert.NotNull(exception);
            Assert.True(File.Exists(sourcePath));
            Assert.False(File.Exists(destinationPath));
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    public sealed class Delete
    {
        [Fact]
        public void CreatedReference_RemovesFileFromPath()
        {
            // Arrange
            var path = CreateUniquePath();

            using (var fileReference = CreateFileReference(path))
                fileReference.Delete();

            // Assert
            Assert.False(File.Exists(path));
            Assert.Throws<FileNotFoundException>(() => File.Open(path, FileMode.Open, FileAccess.Read, System.IO.FileShare.ReadWrite | System.IO.FileShare.Delete));
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedExceptionWithoutDeletingFile()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => fileReference.Delete());

                // Assert
                Assert.NotNull(exception);
                Assert.True(File.Exists(path));
            }
        }
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    [System.Runtime.Versioning.SupportedOSPlatform("linux")]
    public sealed class GetPersistentId
    {
        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void CreatedReference_ReturnsSameIdAsIndependentHandle()
        {
            // Arrange
            var path = CreateUniquePath();
            var expected = default(PersistentFileId);
            var result = default(PersistentFileId);

            using (CreateTemporaryFileScope(path))
            {
                File.WriteAllText(path, "content");

                using (var fileReference = FileReference.Create(path, FileReferenceRequest.Open()))
                using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, System.IO.FileShare.ReadWrite | System.IO.FileShare.Delete))
                {
                    expected = FileOperations.GetPersistentId(fileStream.SafeFileHandle);

                    // Act
                    result = fileReference.GetPersistentId();
                }
            }

            // Assert
            Assert.Equal(expected, result);
        }

        [PlatformOnlyFact(Platform.Windows, Platform.Linux)]
        public void DisposedReference_ThrowsObjectDisposedException()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => _ = fileReference.GetPersistentId());

                // Assert
                Assert.NotNull(exception);
            }
        }
    }
    public sealed class GetLength
    {
        [Fact]
        public void CreatedReference_ReturnsCurrentFileLength()
        {
            // Arrange
            var path = CreateUniquePath();
            var expected = 5L;
            long result;

            using (CreateTemporaryFileScope(path))
            using (var fileReference = CreateFileReference(path))
            {
                File.WriteAllBytes(path, [1, 2, 3, 4, 5]);

                // Act
                result = fileReference.GetLength();
            }

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedException()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => _ = fileReference.GetLength());

                // Assert
                Assert.NotNull(exception);
            }
        }
    }
    public sealed class SetLength
    {
        [Fact]
        public void CreatedReference_UpdatesFileLength()
        {
            // Arrange
            var path = CreateUniquePath();
            var expected = 8L;
            long result;

            using (CreateTemporaryFileScope(path))
            using (var fileReference = CreateFileReference(path))
            {
                // Act
                fileReference.SetLength(expected);
                result = new FileInfo(path).Length;
            }

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NegativeLength_ThrowsArgumentExceptionWithoutChangingFileLength()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                File.WriteAllBytes(path, [1, 2, 3]);
                var expected = new FileInfo(path).Length;

                // Act
                var exception = Assert.ThrowsAny<ArgumentException>(() => fileReference.SetLength(-1));
                var result = new FileInfo(path).Length;

                // Assert
                Assert.Equal("length", exception.ParamName);
                Assert.Equal(expected, result);
                fileReference.Dispose();
            }
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedExceptionWithoutChangingFileLength()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                File.WriteAllBytes(path, [1, 2, 3, 4]);
                var expected = new FileInfo(path).Length;
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => fileReference.SetLength(2));
                var result = new FileInfo(path).Length;

                // Assert
                Assert.NotNull(exception);
                Assert.Equal(expected, result);
            }
        }
    }

    public sealed class CreateFromHandle
    {
        [Fact]
        public void NullHandle_ThrowsArgumentNullException()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => FileReference.CreateFromHandle(null!));

            // Assert
            Assert.Equal("handle", exception.ParamName);
        }

        [Fact]
        public void InvalidHandle_ThrowsArgumentException()
        {
            // Arrange
            using (var handle = new SafeFileHandle((nint)(-1), ownsHandle: false))
            {
                // Act
                var exception = Assert.ThrowsAny<ArgumentException>(() => FileReference.CreateFromHandle(handle));

                // Assert
                Assert.Equal("handle", exception.ParamName);
            }
        }

        [Fact]
        public void ValidHandle_ReturnsIndependentReferenceWithSamePathAndOptions()
        {
            // Arrange
            var path = CreateUniquePath();
            var expectedOptions = default(FileReferenceOptions);
            string? expectedPath;
            string? resultPath;
            FileReferenceOptions resultOptions;
            long resultLength;
            bool originalHandleIsOpen;

            using (CreateTemporaryFileScope(path))
            using (var source = CreateFileReference(path))
            {
                File.WriteAllBytes(path, [1, 2, 3, 4]);
                expectedPath = source.Handle.OpenedPath;

                using (var fileReference = FileReference.CreateFromHandle(source.Handle, expectedOptions))
                {
                    // Act
                    resultPath = fileReference.OpenedPath;
                    resultOptions = fileReference.Options;
                    resultLength = fileReference.GetLength();
                }

                originalHandleIsOpen = !source.Handle.IsClosed;
            }

            // Assert
            Assert.Equal(expectedPath, resultPath);
            Assert.Equal(expectedOptions, resultOptions);
            Assert.Equal(4, resultLength);
            Assert.True(originalHandleIsOpen);
        }
    }

    public sealed class Create
    {
        [Fact]
        public void NullPath_ThrowsArgumentNullException()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => FileReference.Create(null!));

            // Assert
            Assert.Equal("path", exception.ParamName);
        }

        [Fact]
        public void CreateNewRequest_ReturnsOpenReferenceWithRequestedOptions()
        {
            // Arrange
            var path = CreateUniquePath();
            var expectedOptions = default(FileReferenceOptions);
            string? resultPath;
            FileReferenceOptions resultOptions;
            bool resultIsOpen;
            bool resultFileExists;

            using (CreateTemporaryFileScope(path))
            using (var fileReference = FileReference.Create(path, FileReferenceRequest.CreateNew(expectedOptions, new FileCreationOptions(FileAttributes.Temporary))))
            {
                // Act
                resultPath = fileReference.OpenedPath;
                resultOptions = fileReference.Options;
                resultIsOpen = fileReference.IsOpen;
                resultFileExists = File.Exists(path);
            }

            // Assert
            Assert.NotNull(resultPath);
            PathUtilities.AssertPathsEqual(path, resultPath);
            Assert.Equal(expectedOptions, resultOptions);
            Assert.True(resultIsOpen);
            Assert.True(resultFileExists);
        }
    }

    public sealed class Duplicate
    {
        [Fact]
        public void NullFile_ThrowsArgumentNullException()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => FileReference.Duplicate(null!));

            // Assert
            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void OpenReference_ReturnsIndependentReferenceWithSamePathOptionsAndLength()
        {
            // Arrange
            var path = CreateUniquePath();
            string? resultPath;
            FileReferenceOptions resultOptions;
            long resultLength;
            bool duplicateIsOpen;

            using (CreateTemporaryFileScope(path))
            using (var fileReference = CreateFileReference(path))
            {
                File.WriteAllBytes(path, [1, 2, 3]);

                using (var duplicate = FileReference.Duplicate(fileReference))
                {
                    Assert.NotSame(fileReference.Handle, duplicate.Handle);
                    fileReference.Dispose();

                    // Act
                    resultPath = duplicate.OpenedPath;
                    resultOptions = duplicate.Options;
                    resultLength = duplicate.GetLength();
                    duplicateIsOpen = duplicate.IsOpen;
                }
            }

            // Assert
            Assert.NotNull(resultPath);
            PathUtilities.AssertPathsEqual(path, resultPath);
            Assert.Equal(default, resultOptions);
            Assert.Equal(3, resultLength);
            Assert.True(duplicateIsOpen);
        }
    }





    public sealed class CreateTempFile
    {
        [Fact]
        public void ParameterlessOverload_DelegatesToDefaultTempFileCreation()
        {
            // Arrange
            string? resultPath = null;
            FileReferenceOptions resultOptions = default;

            // Act
            using (var fileReference = FileReference.CreateTempFile())
            {
                resultPath = fileReference.OpenedPath;
                resultOptions = fileReference.Options;

                Assert.False(fileReference.IsClosed);
                Assert.NotNull(resultPath);
                Assert.Equal(".tmp", Path.GetExtension(resultPath));
                Assert.True(Guid.TryParse(Path.GetFileNameWithoutExtension(resultPath), out _));

                var expectedDirectory = Path.GetTempPath();
                var actualDirectory = Path.GetDirectoryName(resultPath)!;
                PathUtilities.AssertPathsEqual(expectedDirectory, actualDirectory);
            }

            // Assert
            var createdPath = Assert.IsType<string>(resultPath);
            Assert.Equal(FileHandleType.RegularFile, resultOptions.Type);
            Assert.Equal(FileReferenceFlags.DeleteOnClose, resultOptions.Flags);
            Assert.False(File.Exists(createdPath));
        }

        [Fact]
        public void TypeFlagsAndAttributes_ReturnsReferenceWithRequestedOptionsWithoutAddingDeleteOnClose()
        {
            // Arrange
            var expectedType = FileHandleType.RegularFile;
            var expectedFlags = FileReferenceFlags.None;
            var expectedAttributes = FileAttributes.ReadOnly;
            string? resultPath;
            FileReferenceOptions resultOptions;
            FileAttributes resultAttributes;

            using (var fileReference = FileReference.CreateTempFile(expectedType, expectedFlags, expectedAttributes))
            {
                // Act
                resultPath = fileReference.OpenedPath;
                resultOptions = fileReference.Options;
                resultAttributes = fileReference.GetAttributes();
            }

            // Assert
            var createdPath = Assert.IsType<string>(resultPath);
            using (CreateTemporaryFileScope(createdPath))
            {
                var fileExistsAfterDispose = File.Exists(createdPath);
                if (fileExistsAfterDispose)
                    File.SetAttributes(createdPath, FileAttributes.Normal);

                Assert.Equal(expectedType, resultOptions.Type);
                Assert.Equal(expectedFlags, resultOptions.Flags);
                Assert.True((resultAttributes & expectedAttributes) == expectedAttributes);
                Assert.True(fileExistsAfterDispose);
            }
        }

        [Fact]
        public void ReferenceAndCreationOptions_AddsDeleteOnCloseToRequestedFlags()
        {
            // Arrange
            var expectedType = FileHandleType.RegularFile;
            var requestedFlags = FileReferenceFlags.Transient;
            var expectedFlags = requestedFlags | FileReferenceFlags.DeleteOnClose;
            var requestedAttributes = FileAttributes.Hidden;
            var expectedAttributes = FileAttributes.Hidden | FileAttributes.Archive | FileAttributes.Temporary;
            string? resultPath = null;
            FileReferenceOptions resultOptions = default;
            FileAttributes resultAttributes = default;

            // Act
            using (var fileReference = FileReference.CreateTempFile(new(expectedType, requestedFlags), new(requestedAttributes)))
            {
                resultPath = fileReference.OpenedPath;
                resultOptions = fileReference.Options;
                resultAttributes = fileReference.GetAttributes();

                Assert.False(fileReference.IsClosed);
                Assert.NotNull(resultPath);
                Assert.Equal(".tmp", Path.GetExtension(resultPath));
                Assert.True(Guid.TryParse(Path.GetFileNameWithoutExtension(resultPath), out _));

                var expectedDirectory = Path.GetTempPath();
                var actualDirectory = Path.GetDirectoryName(resultPath)!;
                PathUtilities.AssertPathsEqual(expectedDirectory, actualDirectory);
            }

            // Assert
            var createdPath = Assert.IsType<string>(resultPath);
            Assert.Equal(expectedType, resultOptions.Type);
            Assert.Equal(expectedFlags, resultOptions.Flags);
            Assert.Equal(expectedAttributes, resultAttributes);
            Assert.False(File.Exists(createdPath));
        }
    }

    public sealed class OpenSession
    {
        [Fact]
        public void OpenReference_ReturnsIndependentSessionThatKeepsReferenceOpen()
        {
            // Arrange
            var path = CreateUniquePath();
            var options = new FileHandleOptions(FileSystemAccess.Write, FileShare.ReadWrite | FileShare.Delete);
            var expectedBytes = new byte[] { 5, 6, 7 };
            var expectedType = FileHandleType.RegularFile;
            FileHandleOptions resultOptions;
            FileHandleType resultType;
            bool resultCanWrite;
            byte[] resultBytes;
            bool referenceIsOpenAfterSessionDisposed;

            using (CreateTemporaryFileScope(path))
            using (var fileReference = CreateFileReference(path))
            {
                using (var session = fileReference.OpenSession(options))
                {
                    Assert.NotSame(fileReference.Handle, session.Handle);

                    // Act
                    resultOptions = session.Options;
                    resultType = session.Type;
                    resultCanWrite = session.CanWrite;

                    using (var fileStream = new FileStream(session.Handle, FileAccess.Write))
                        fileStream.Write(expectedBytes, 0, expectedBytes.Length);
                }

                referenceIsOpenAfterSessionDisposed = fileReference.IsOpen;
                resultBytes = File.ReadAllBytes(path);
            }

            // Assert
            Assert.Equal(options, resultOptions);
            Assert.Equal(expectedType, resultType);
            Assert.True(resultCanWrite);
            Assert.Equal(expectedBytes, resultBytes);
            Assert.True(referenceIsOpenAfterSessionDisposed);
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedExceptionWithoutChangingFile()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                var expectedBytes = new byte[] { 8, 9 };
                File.WriteAllBytes(path, expectedBytes);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => fileReference.OpenSession(new FileHandleOptions(FileSystemAccess.Read)));

                // Assert
                Assert.NotNull(exception);
                Assert.Equal(expectedBytes, File.ReadAllBytes(path));
            }
        }
    }

    public sealed class OpenHandle
    {
        [Fact]
        public void OpenReference_ReturnsIndependentHandleThatKeepsReferenceOpen()
        {
            // Arrange
            var path = CreateUniquePath();
            var options = new FileHandleOptions(FileSystemAccess.Write, FileShare.ReadWrite | FileShare.Delete);
            var expectedBytes = new byte[] { 1, 2, 3, 4 };
            var expectedType = FileHandleType.RegularFile;
            FileHandleType resultType;
            bool handleIsOpen;
            byte[] resultBytes;
            bool referenceIsOpenAfterHandleDisposed;

            using (CreateTemporaryFileScope(path))
            using (var fileReference = CreateFileReference(path))
            {
                using (var handle = fileReference.OpenHandle(options))
                {
                    Assert.NotSame(fileReference.Handle, handle);

                    // Act
                    resultType = handle.Type;
                    handleIsOpen = !handle.IsClosed;

                    using (var fileStream = new FileStream(handle, FileAccess.Write))
                        fileStream.Write(expectedBytes, 0, expectedBytes.Length);
                }

                referenceIsOpenAfterHandleDisposed = fileReference.IsOpen;
                resultBytes = File.ReadAllBytes(path);
            }

            // Assert
            Assert.Equal(expectedType, resultType);
            Assert.True(handleIsOpen);
            Assert.Equal(expectedBytes, resultBytes);
            Assert.True(referenceIsOpenAfterHandleDisposed);
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedExceptionWithoutChangingFile()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                var expectedBytes = new byte[] { 10, 11, 12 };
                File.WriteAllBytes(path, expectedBytes);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => fileReference.OpenHandle(new FileHandleOptions(FileSystemAccess.Read)));

                // Assert
                Assert.NotNull(exception);
                Assert.Equal(expectedBytes, File.ReadAllBytes(path));
            }
        }
    }

    public sealed class OpenFileStream
    {
        [Fact]
        public void OpenReference_ReturnsStreamThatLeavesReferenceOpen()
        {
            // Arrange
            var path = CreateUniquePath();
            var expectedBytes = new byte[] { 1, 2, 3 };
            long resultLength;
            bool streamCanRead;
            bool referenceIsOpenAfterStreamDisposed;

            using (CreateTemporaryFileScope(path))
            using (var fileReference = CreateFileReference(path))
            {
                File.WriteAllBytes(path, expectedBytes);

                using (var stream = fileReference.OpenFileStream(FileAccess.Read))
                {
                    // Act
                    resultLength = stream.Length;
                    streamCanRead = stream.CanRead;
                }

                referenceIsOpenAfterStreamDisposed = fileReference.IsOpen;
            }

            // Assert
            Assert.Equal(expectedBytes.Length, resultLength);
            Assert.True(streamCanRead);
            Assert.True(referenceIsOpenAfterStreamDisposed);
        }

        [Fact]
        public void DisposedReference_ThrowsObjectDisposedExceptionWithoutChangingFile()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);
                var expectedBytes = new byte[] { 4, 5, 6 };
                File.WriteAllBytes(path, expectedBytes);
                fileReference.Dispose();

                // Act
                var exception = Assert.Throws<ObjectDisposedException>(() => fileReference.OpenFileStream(FileAccess.Read));

                // Assert
                Assert.NotNull(exception);
                Assert.Equal(expectedBytes, File.ReadAllBytes(path));
            }
        }

    }

    public sealed class Dispose
    {
        [Fact]
        public void OpenReference_ClosesReferenceAndCanBeCalledTwice()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);

                // Act
                fileReference.Dispose();
                var exception = Record.Exception(() => fileReference.Dispose());
                var resultIsClosed = fileReference.IsClosed;
                var resultIsOpen = fileReference.IsOpen;

                // Assert
                Assert.Null(exception);
                Assert.True(resultIsClosed);
                Assert.False(resultIsOpen);
                Assert.True(File.Exists(path));
            }
        }
    }

    public sealed class DisposeAsync
    {
        [Fact]
        public async Task OpenReference_DisposesReferenceAndReturnsCompletedTask()
        {
            // Arrange
            var path = CreateUniquePath();
            using (CreateTemporaryFileScope(path))
            {
                var fileReference = CreateFileReference(path);

                // Act
                var result = fileReference.DisposeAsync();
                await result;
                var exception = Assert.Throws<ObjectDisposedException>(() => _ = fileReference.OpenedPath);

                // Assert
                Assert.True(result.IsCompletedSuccessfully);
                Assert.NotNull(exception);
                Assert.True(fileReference.IsClosed);
                Assert.False(fileReference.IsOpen);
            }
        }
    }

    private static TemporaryFileUtilities.TemporaryFileScope CreateTemporaryFileScope(string path)
        => TemporaryFileUtilities.CreateTemporaryFileScope(path);
}
