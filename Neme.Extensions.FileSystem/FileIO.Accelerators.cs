using Microsoft.Win32.SafeHandles;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    public static SafeFileHandle CreateTempFileHandle(FileSystemAccess access) =>
        CreateTempFileHandle(access, FileOpenOptions.GetDefaultFileShare(access));

    public static SafeFileHandle CreateTempFileHandle(
        FileSystemAccess access,
        FileShare share,
        FileOptions options = FileOptions.DeleteOnClose,
        FileAttributes attributes = FileAttributes.Temporary)
    {
        var (filePath, fileOptions) = GetTempFilePathAndOptions(access, share, options, attributes);
        return FileIO.OpenHandle(filePath, fileOptions);
    }

    public static FileReference CreateTempFile(FileSystemAccess access) =>
        CreateTempFile(access, FileOpenOptions.GetDefaultFileShare(access));

    public static FileReference CreateTempFile(
        FileSystemAccess access,
        FileShare share,
        FileOptions options = FileOptions.DeleteOnClose,
        FileAttributes attributes = FileAttributes.Temporary)
    {
        var (filePath, fileOptions) = GetTempFilePathAndOptions(access, share, options, attributes);
        return FileIO.Open(filePath, fileOptions);
    }

    private static (string filePath, FileOpenOptions) GetTempFilePathAndOptions(
        FileSystemAccess access,
        FileShare share,
        FileOptions options,
        FileAttributes attributes)
    {
        var fileOptions = FileOpenOptions.CreateNew(access, share, options, attributes);
        var fileName = Invariant($"{Guid.NewGuid()}.tmp");
        return (Path.GetTempPath() + fileName, fileOptions);
    }
}
