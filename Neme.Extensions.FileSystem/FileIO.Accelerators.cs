using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Ownership;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    [return: OwnershipTransfer]
    public static SafeFileHandle CreateTempFileHandle(FileSystemAccess access) =>
        CreateTempFileHandle(access, FileOpenOptions.GetDefaultFileShare(access));

    [return: OwnershipTransfer]
    public static SafeFileHandle CreateTempFileHandle(
        FileSystemAccess access,
        FileShare share,
        FileOptions options = FileOptions.DeleteOnClose,
        FileAttributes attributes = FileAttributes.Temporary)
    {
        var (filePath, openOptions) = GetTempFilePathAndOptions(access, share, options, attributes);
        return OpenHandle(filePath, openOptions);
    }

    internal static (string filePath, FileOpenOptions openOptions) GetTempFilePathAndOptions(
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
