namespace Neme.Extensions.FileSystem.Tests;

public sealed class FileIdTests
{
    [Fact]
    public void FromWindowsId_ReturnsInstanceExposingWindowsFileId()
    {
        var windowsId = new FileId.WindowsId(1UL, 2UL, 3UL);

        var sut = FileId.FromWindowsId(windowsId);

        Assert.Equal(windowsId, sut.WindowsFileId);
        Assert.Equal(1UL, sut.WindowsFileId.VolumeSerialNumber);
        Assert.Equal(2UL, sut.WindowsFileId.FileIdHigh);
        Assert.Equal(3UL, sut.WindowsFileId.FileIdLow);
    }

    [Fact]
    public void FromUnixId_ReturnsInstanceExposingUnixFileId()
    {
        var unixId = new FileId.UnixId(123UL, 456UL);

        var sut = FileId.FromUnixId(unixId);

        Assert.Equal(unixId, sut.UnixFileId);
        Assert.Equal(123UL, sut.UnixFileId.Device);
        Assert.Equal(456UL, sut.UnixFileId.Inode);
    }

    [Fact]
    public void FromWindowsId_WhenAccessingUnixFileId_ThrowsInvalidOperationException()
    {
        var sut = FileId.FromWindowsId(new FileId.WindowsId(1UL, 2UL, 3UL));

        Assert.Throws<InvalidOperationException>(() => _ = sut.UnixFileId);
    }

    [Fact]
    public void FromUnixId_WhenAccessingWindowsFileId_ThrowsInvalidOperationException()
    {
        var sut = FileId.FromUnixId(new FileId.UnixId(123UL, 456UL));

        Assert.Throws<InvalidOperationException>(() => _ = sut.WindowsFileId);
    }

    [Fact]
    public void EqualInstances_HaveEqualHashCodes()
    {
        var left = FileId.FromUnixId(new FileId.UnixId(123UL, 456UL));
        var right = FileId.FromUnixId(new FileId.UnixId(123UL, 456UL));

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void DifferentPlatformPayloads_AreNotEqual()
    {
        var windows = FileId.FromWindowsId(new FileId.WindowsId(1UL, 2UL, 3UL));
        var unix = FileId.FromUnixId(new FileId.UnixId(1UL, 2UL));

        Assert.NotEqual(windows, unix);
    }
}
