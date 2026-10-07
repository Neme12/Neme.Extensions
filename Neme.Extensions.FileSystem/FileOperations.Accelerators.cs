using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Ownership;

namespace Neme.Extensions.FileSystem;

public static partial class FileOperations
{
    [return: OwnershipTransfer]
    public static SafeFileHandle CreateTempFileHandle(FileSystemAccess access) =>
        CreateTempFileHandle(access, FileHandleOptions.GetDefaultFileShare(access));

    [return: OwnershipTransfer]
    public static SafeFileHandle CreateTempFileHandle(
        FileSystemAccess access,
        FileShare share,
        FileOptions options = FileOptions.DeleteOnClose,
        FileAttributes attributes = FileAttributes.Temporary)
    {
        var path = GetTempFilePath();
        var request = FileHandleRequest.CreateNew(access, share, options, attributes);
        return OpenHandle(path, request);
    }

    [return: OwnershipTransfer]
    public static SafeFileHandle CreateTempFileHandle(
        FileHandleOptions handleOptions,
        FileCreationOptions creationOptions = default)
    {
        handleOptions = handleOptions with { Flags = handleOptions.Flags | FileOptions.DeleteOnClose };
        creationOptions = creationOptions with { Attributes = creationOptions.Attributes | FileAttributes.Temporary };

        var path = GetTempFilePath();
        var request = new FileHandleRequest(FileMode.CreateNew, handleOptions, creationOptions);
        return OpenHandle(path, request);
    }

    internal static string  GetTempFilePath()
    {
        var fileName = Invariant($"{Guid.NewGuid()}.tmp");
        return Path.GetTempPath() + fileName;
    }
}
