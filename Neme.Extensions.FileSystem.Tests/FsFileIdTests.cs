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
        var linuxId = new PersistentFileId.LinuxId("/mnt/test", 2, [3, 4, 0, 0]);

        var sut = PersistentFileId.FromLinuxId(linuxId);

        Assert.Equal(linuxId, sut.LinuxFileId);
        Assert.Equal("/mnt/test", sut.LinuxFileId.MountPath);
        Assert.Equal(2, sut.LinuxFileId.FileType);
        Assert.Equal(4, sut.LinuxFileId.InlineBufferLength);
        Assert.Null(sut.LinuxFileId.Buffer);

        sut.LinuxFileId.InlineBuffer.WithSpan((span, _) =>
        {
            Assert.Equal(3, span[0]);
            Assert.Equal(4, span[1]);
            Assert.Equal(0, span[2]);
            Assert.Equal(0, span[3]);
        }, default(ValueTuple));
    }

    [Fact]
    public void FromLinuxId_WithAllocatedBuffer_ReturnsInstanceExposingLinuxFileId()
    {
        var range = Enumerable.Range(0, 128).Select(i => (byte)i).ToArray();
        var linuxId = new PersistentFileId.LinuxId("/mnt/test", 2, range);

        var sut = PersistentFileId.FromLinuxId(linuxId);

        Assert.Equal(linuxId, sut.LinuxFileId);
        Assert.Equal("/mnt/test", sut.LinuxFileId.MountPath);
        Assert.Equal(2, sut.LinuxFileId.FileType);
        Assert.Equal(0, sut.LinuxFileId.InlineBufferLength);

        Assert.Equal(range, sut.LinuxFileId.Buffer);
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
    public void FromWindowsId_Parse()
    {
        var sut = PersistentFileId.Parse("v1:w:0000000000000005:0000000000000096:00000000000000fa");

        Assert.Equal(5ul, sut.WindowsFileId.VolumeSerialNumber);
        Assert.Equal(150ul, sut.WindowsFileId.FileIdHigh);
        Assert.Equal(250ul, sut.WindowsFileId.FileIdLow);
    }

    [Fact]
    public void FromWindowsId_ParseInvalid()
    {
        Assert.Throws<FormatException>(() => PersistentFileId.Parse("abc"));
        Assert.Throws<FormatException>(() => PersistentFileId.Parse("v1:w:abc"));
    }

    [Fact]
    public void FromLinuxId_ToString()
    {
        var linuxId = new PersistentFileId.LinuxId("/mnt/test", 200, [50, 150, 250]);

        var sut = PersistentFileId.FromLinuxId(linuxId);
        var str = sut.ToString();

        Assert.Equal($"v1:l:2f6d6e742f74657374:000000c8:3296fa", str);
    }

    [Fact]
    public void FromLinuxId_Parse()
    {
        var sut = PersistentFileId.Parse("v1:l:2f6d6e742f74657374:000000c8:3296fa");

        Assert.Equal("/mnt/test", sut.LinuxFileId.MountPath);
        Assert.Equal(200, sut.LinuxFileId.FileType);
        Assert.Equal(3, sut.LinuxFileId.InlineBufferLength);
        Assert.Null(sut.LinuxFileId.Buffer);

        sut.LinuxFileId.InlineBuffer.WithSpan((span, _) =>
        {
            Assert.Equal(50, span[0]);
            Assert.Equal(150, span[1]);
            Assert.Equal(250, span[2]);
        }, default(ValueTuple));
    }

    [Fact]
    public void FromLinuxId_ParseInvalid()
    {
        Assert.Throws<FormatException>(() => PersistentFileId.Parse("abc"));
        Assert.Throws<FormatException>(() => PersistentFileId.Parse("v1:l:abc"));
    }
}
