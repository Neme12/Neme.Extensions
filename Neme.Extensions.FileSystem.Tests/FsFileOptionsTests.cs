using Neme.Extensions.Tests.Utilities;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem.Tests;

public sealed class FsFileOptionsTests
{
    [Fact]
    public void Constructor_WithExplicitShare_SetsModeAccessAndShare()
    {
        var sut = new FileOpenOptions(FileMode.CreateNew, FileSystemAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);

        Assert.Equal(FileMode.CreateNew, sut.Mode);
        Assert.Equal(FileSystemAccess.ReadWrite, sut.Access);
        Assert.Equal(FileShare.ReadWrite | FileShare.Delete, sut.Share);
        Assert.Equal(FileOptions.None, sut.Options);
        Assert.Equal(default, sut.Attributes);
        Assert.Null(sut.UnixCreateMode);
    }

    [Theory]
    [InlineData(FileSystemAccess.None, FileShare.Read)]
    [InlineData(FileSystemAccess.Read, FileShare.Read)]
    [InlineData(FileSystemAccess.Execute, FileShare.Read)]
    [InlineData(FileSystemAccess.Write, FileShare.None)]
    [InlineData(FileSystemAccess.Delete, FileShare.None)]
    [InlineData(FileSystemAccess.ReadWrite, FileShare.None)]
    public void Constructor_WithImplicitShare_ComputesExpectedShare(FileSystemAccess access, FileShare expectedShare)
    {
        var sut = new FileOpenOptions(FileMode.Open, access);

        Assert.Equal(FileMode.Open, sut.Mode);
        Assert.Equal(access, sut.Access);
        Assert.Equal(expectedShare, sut.Share);
    }

    [Fact]
    public void InitProperties_RoundTripAllValues()
    {
        var expectedOptions =
            FileOptions.Encrypted
            | FileOptions.DeleteOnClose
            | FileOptions.SequentialScan
            | FileOptions.RandomAccess
            | FileOptions.Asynchronous
            | FileOptions.WriteThrough;

        var expectedAttributes = FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.Archive;

        var sut = new FileOpenOptions
        {
            Mode = FileMode.Append,
            Access = FileSystemAccess.Delete | FileSystemAccess.Write,
            Share = FileShare.ReadWrite | FileShare.Delete,
            Options = expectedOptions,
            Attributes = expectedAttributes,
        };

        Assert.Equal(FileMode.Append, sut.Mode);
        Assert.Equal(FileSystemAccess.Delete | FileSystemAccess.Write, sut.Access);
        Assert.Equal(FileShare.ReadWrite | FileShare.Delete, sut.Share);
        Assert.Equal(expectedOptions, sut.Options);
        Assert.Equal(expectedAttributes, sut.Attributes);
        Assert.Null(sut.UnixCreateMode);
    }

    [PlatformOnlyFact(Platform.Unix)]
    public void UnixCreateMode_OnUnix_RoundTripsValue()
    {
        var expected = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead;

        var sut = new FileOpenOptions
        {
#pragma warning disable CA1416
            UnixCreateMode = expected,
#pragma warning restore CA1416
        };

        Assert.Equal(expected, sut.UnixCreateMode);
    }

    [PlatformOnlyFact(Platform.Unix)]
    public void UnixCreateMode_OnUnix_CanBeSetToNull()
    {
        var sut = new FileOpenOptions
        {
#pragma warning disable CA1416
            UnixCreateMode = null,
#pragma warning restore CA1416
        };

        Assert.Null(sut.UnixCreateMode);
    }

    [PlatformOnlyFact(Platform.Windows)]
    public void UnixCreateMode_OnWindows_ThrowsPlatformNotSupportedException()
    {
        Assert.Throws<PlatformNotSupportedException>(() => new FileOpenOptions
        {
#pragma warning disable CA1416
            UnixCreateMode = UnixFileMode.UserRead,
#pragma warning restore CA1416
        });
    }

#if NET6_0_OR_GREATER
    [Fact]
    public void FromFileStreamOptions_MapsSupportedProperties()
    {
        var options = new FileStreamOptions
        {
            Mode = FileMode.Truncate,
            Access = FileAccess.ReadWrite,
            Share = FileShare.Read,
            Options = FileOptions.Asynchronous | FileOptions.WriteThrough | FileOptions.RandomAccess,
        };

        var sut = FileOpenOptions.FromFileStreamOptions(options);

        Assert.Equal(FileMode.Truncate, sut.Mode);
        Assert.Equal(FileSystemAccess.ReadWrite, sut.Access);
        Assert.Equal(FileShare.Read, sut.Share);
        Assert.Equal(FileOptions.Asynchronous | FileOptions.WriteThrough | FileOptions.RandomAccess, sut.Options);
        Assert.Equal(default, sut.Attributes);
        Assert.Null(sut.UnixCreateMode);
    }

    [Fact]
    public void ImplicitConversion_FromFileStreamOptions_MatchesFactoryMethod()
    {
        var options = new FileStreamOptions
        {
            Mode = FileMode.OpenOrCreate,
            Access = FileAccess.Write,
            Share = FileShare.Delete,
            Options = FileOptions.DeleteOnClose | FileOptions.SequentialScan,
        };

        FileOpenOptions converted = options;
        var expected = FileOpenOptions.FromFileStreamOptions(options);

        Assert.Equal(expected, converted);
    }
#endif

#if NET7_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [PlatformOnlyFact(Platform.Unix)]
    public void FromFileStreamOptions_MapsSupportedProperties_WithUnixCreateMode()
    {
        var options = new FileStreamOptions
        {
            Mode = FileMode.Truncate,
            Access = FileAccess.ReadWrite,
            Share = FileShare.Read,
            Options = FileOptions.Asynchronous | FileOptions.WriteThrough | FileOptions.RandomAccess,
            UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite,
        };

        var sut = FileOpenOptions.FromFileStreamOptions(options);

        Assert.Equal(FileMode.Truncate, sut.Mode);
        Assert.Equal(FileSystemAccess.ReadWrite, sut.Access);
        Assert.Equal(FileShare.Read, sut.Share);
        Assert.Equal(FileOptions.Asynchronous | FileOptions.WriteThrough | FileOptions.RandomAccess, sut.Options);
        Assert.Equal(default, sut.Attributes);
        Assert.Equal(UnixFileMode.UserRead | UnixFileMode.UserWrite, sut.UnixCreateMode);
    }
#endif
}
