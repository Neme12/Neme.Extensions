using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Ownership;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    [return: OwnershipTransfer]
    public static SafeFileHandle CreateTempFileHandle(FileSystemAccess access) =>
        CreateTempFileHandle(access, FileOpenRequest.GetDefaultFileShare(access));

    [return: OwnershipTransfer]
    public static SafeFileHandle CreateTempFileHandle(
        FileSystemAccess access,
        FileShare share,
        FileOptions options = FileOptions.DeleteOnClose,
        FileAttributes attributes = FileAttributes.Temporary)
    {
        var (path, request) = GetTempFilePathAndRequest(access, share, options, attributes);
        return OpenHandle(path, request);
    }

    internal static (string path, FileOpenRequest request) GetTempFilePathAndRequest(
        FileSystemAccess access,
        FileShare share,
        FileOptions options,
        FileAttributes attributes)
    {
        var fileOptions = FileOpenRequest.CreateNew(access, share, options, attributes);
        var fileName = Invariant($"{Guid.NewGuid()}.tmp");
        return (Path.GetTempPath() + fileName, fileOptions);
    }
}
