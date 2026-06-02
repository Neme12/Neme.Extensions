namespace Neme.Extensions.FileSystem.Tests;

public sealed class FsFileIdTests
{
    [Fact]
    public void FromWindowsId_ReturnsInstanceExposingWindowsFileId()
    {
        var windowsId = new PersistentFileId.WindowsId(1UL, 2UL, 3UL);

        var sut = PersistentFileId.FromWindowsId(windowsId);

        Assert.Equal(windowsId, sut.WindowsFileId);
        Assert.Equal(1UL, sut.WindowsFileId.VolumeSerialNumber);
        Assert.Equal(2UL, sut.WindowsFileId.FileIdHigh);
        Assert.Equal(3UL, sut.WindowsFileId.FileIdLow);
    }

    [Fact]
    public void FromLinuxId_ReturnsInstanceExposingLinuxFileId()
    {
        var array = new PersistentFileId.InlineByteArray();
        array.WithSpan((span, _) =>
        {
            span[0] = 3;
            span[1] = 4;
        }, default(ValueTuple));
        var linuxId = new PersistentFileId.LinuxId(1, 2, array, 4);

        var sut = PersistentFileId.FromLinuxId(linuxId);

        Assert.Equal(linuxId, sut.LinuxFileId);
        Assert.Equal(1, sut.LinuxFileId.MountId);
        Assert.Equal(2, sut.LinuxFileId.FileType);
        Assert.Equal(4, sut.LinuxFileId.BufferLength);

        sut.LinuxFileId.Buffer.WithSpan((span, _) =>
        {
            Assert.Equal(3, span[0]);
            Assert.Equal(4, span[1]);
            Assert.Equal(0, span[2]);
            Assert.Equal(0, span[3]);
        }, default(ValueTuple));
    }

    [Fact]
    public void FromWindowsId_ToString()
    {
        var windowsId = new PersistentFileId.WindowsId(5UL, 150UL, 250UL);

        var sut = PersistentFileId.FromWindowsId(windowsId);
        var str = sut.ToString();

        Assert.Equal($"v1:w:0000000000000005:0000000000000096:00000000000000fa", str);
    }

    [Fact]
    public void FromLinuxId_ToString()
    {
        var array = new PersistentFileId.InlineByteArray();
        array.WithSpan((span, _) =>
        {
            span[0] = 50;
            span[1] = 150;
            span[2] = 250;
        }, default(ValueTuple));

        var linuxId = new PersistentFileId.LinuxId(100, 200, array, 3);

        var sut = PersistentFileId.FromLinuxId(linuxId);
        var str = sut.ToString();

        Assert.Equal($"v1:l:00000064:000000c8:3296fa", str);
    }
}
