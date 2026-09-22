namespace Neme.Extensions.FileSystem.Tests;

public sealed class FileHandleOptionsTests
{
    public sealed class Constructor
    {
        [Fact]
        public void WithExplicitValues_SetsAccessShareAndOptions()
        {
            var expectedAccess = FileSystemAccess.ReadWriteDelete | FileSystemAccess.Execute;
            var expectedShare = FileShare.ReadWrite | FileShare.Delete | FileShare.Inheritable;
            var expectedOptions =
                FileOptions.Asynchronous
                | FileOptions.DeleteOnClose
                | FileOptions.RandomAccess
                | FileOptions.SequentialScan
                | FileOptions.WriteThrough;

            var sut = new FileHandleOptions(expectedAccess, expectedShare, expectedOptions);

            Assert.Equal(expectedAccess, sut.Access);
            Assert.Equal(expectedShare, sut.Share);
            Assert.Equal(expectedOptions, sut.Options);
        }

        [Fact]
        public void WithDefaultValues_SetsEmptyFlags()
        {
            var sut = new FileHandleOptions(FileSystemAccess.None, FileShare.None, 0);

            Assert.Equal(FileSystemAccess.None, sut.Access);
            Assert.Equal(FileShare.None, sut.Share);
            Assert.Equal((FileOptions)0, sut.Options);
        }
    }

    public sealed class Access
    {
        [Fact]
        public void WhenSetAfterShareAndOptions_PreservesExistingValues()
        {
            var expectedAccess = FileSystemAccess.Read | FileSystemAccess.Delete;
            var expectedShare = FileShare.Read | FileShare.Delete;
            var expectedOptions = FileOptions.Asynchronous | FileOptions.RandomAccess;

            var sut = new FileHandleOptions
            {
                Share = expectedShare,
                Options = expectedOptions,
                Access = expectedAccess,
            };

            Assert.Equal(expectedAccess, sut.Access);
            Assert.Equal(expectedShare, sut.Share);
            Assert.Equal(expectedOptions, sut.Options);
        }
    }

    public sealed class Share
    {
        [Fact]
        public void WhenSetAfterAccessAndOptions_PreservesExistingValues()
        {
            var expectedAccess = FileSystemAccess.Write | FileSystemAccess.Execute;
            var expectedShare = FileShare.Write | FileShare.Inheritable;
            var expectedOptions = FileOptions.DeleteOnClose | FileOptions.SequentialScan;

            var sut = new FileHandleOptions
            {
                Access = expectedAccess,
                Options = expectedOptions,
                Share = expectedShare,
            };

            Assert.Equal(expectedAccess, sut.Access);
            Assert.Equal(expectedShare, sut.Share);
            Assert.Equal(expectedOptions, sut.Options);
        }
    }

    public sealed class Options
    {
        [Fact]
        public void WhenSetAfterAccessAndShare_PreservesExistingValues()
        {
            var expectedAccess = FileSystemAccess.ReadWrite;
            var expectedShare = FileShare.ReadWrite | FileShare.Delete;
            var expectedOptions = FileOptions.WriteThrough | FileOptions.Encrypted;

            var sut = new FileHandleOptions
            {
                Access = expectedAccess,
                Share = expectedShare,
                Options = expectedOptions,
            };

            Assert.Equal(expectedAccess, sut.Access);
            Assert.Equal(expectedShare, sut.Share);
            Assert.Equal(expectedOptions, sut.Options);
        }
    }
}
