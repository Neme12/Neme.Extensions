namespace Neme.Extensions.FileSystem.Tests;

public sealed class FileBookmarkTests
{
    [Fact]
    public void Constructor_SetsPathAndFileId()
    {
        var fileId = FileId.FromUnixId(new FileId.UnixId(123UL, 456UL));

        var sut = new FileBookmark("/tmp/test.txt", fileId);

        Assert.Equal("/tmp/test.txt", sut.Path);
        Assert.Equal(fileId, sut.FileId);
    }

    [Fact]
    public void EqualInstances_HaveEqualHashCodes()
    {
        var fileId = FileId.FromWindowsId(new FileId.WindowsId(1UL, 2UL, 3UL));
        var left = new FileBookmark("C:/temp/test.txt", fileId);
        var right = new FileBookmark("C:/temp/test.txt", fileId);

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }
}
