using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.FileSystem.Resources;

namespace Neme.Extensions.FileSystem.Workflows;

public static class PartialFile
{
    public static string PartialExtension => ".part";

    public static PartialFile<SafeFileHandle> CreateHandleFile(
        string finalPath,
        FileHandleRequest request,
        bool createDirectory = false)
    {
        Require.ArgumentNotNullOrEmpty(finalPath);
        Require.ArgumentNotDefault(request);
        Require.Argument(request, request.Access.HasFlag(FileSystemAccess.Delete), "Options must include delete access.");

        var partialPath = finalPath + PartialExtension;

        if (createDirectory)
            Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

        var file = FileOperations.OpenHandle(partialPath, request);
        return new PartialFile<SafeFileHandle>(
            file,
            static file => file.ToFileResource(),
            finalPath,
            () => FileOperations.OpenHandle(partialPath, request with { Mode = FileMode.Open }));
    }

    /// <summary>
    /// Creates a new temporary file at <paramref name="finalPath"/> with the <c>.part</c> suffix.
    /// </summary>
    /// <param name="finalPath">The final destination path that will be used by <see cref="Commit(bool)"/>.</param>
    /// <param name="request">The options used to open the temporary file. Delete access is required so the temporary file can be cleaned up.</param>
    /// <param name="createDirectory"><see langword="true"/> to create the destination directory if it does not already exist.</param>
    /// <returns>A <see cref="PartialFile"/> for writing the temporary file.</returns>
    public static PartialFile<FileSession> CreateSessionFile(
        string finalPath,
        FileHandleRequest request,
        bool createDirectory = false)
    {
        Require.ArgumentNotNullOrEmpty(finalPath);
        Require.ArgumentNotDefault(request);
        Require.Argument(request, request.Access.HasFlag(FileSystemAccess.Delete), "Options must include delete access.");

        var partialPath = finalPath + PartialExtension;

        if (createDirectory)
            Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

        var file = FileSession.Open(partialPath, request);
        return new PartialFile<FileSession>(
            file,
            static file => file,
            finalPath,
            () => FileSession.Open(partialPath, request with { Mode = FileMode.Open }));
    }

    /// <summary>
    /// Creates a new temporary file at <paramref name="finalPath"/> with the <c>.part</c> suffix.
    /// </summary>
    /// <param name="finalPath">The final destination path that will be used by <see cref="Commit(bool)"/>.</param>
    /// <param name="request">The options used to open the temporary file. Delete access is required so the temporary file can be cleaned up.</param>
    /// <param name="createDirectory"><see langword="true"/> to create the destination directory if it does not already exist.</param>
    /// <returns>A <see cref="PartialFile"/> for writing the temporary file.</returns>
    public static PartialFile<FileReference> CreateReferenceFile(
        string finalPath,
        FileReferenceRequest request,
        bool createDirectory = false)
    {
        Require.ArgumentNotNullOrEmpty(finalPath);
        Require.ArgumentNotDefault(request);

        var partialPath = finalPath + PartialExtension;

        if (createDirectory)
            Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

        var file = FileReference.Create(partialPath, request);
        return new PartialFile<FileReference>(
            file,
            static file => file,
            finalPath,
            () => FileReference.Create(partialPath, request with { Mode = FileReferenceMode.Open }));
    }

    /// <summary>
    /// Creates a new temporary file at <paramref name="finalPath"/> with the <c>.part</c> suffix.
    /// </summary>
    /// <param name="finalPath">The final destination path that will be used by <see cref="Commit(bool)"/>.</param>
    /// <param name="request">The options used to open the temporary file. Delete access is required so the temporary file can be cleaned up.</param>
    /// <param name="createDirectory"><see langword="true"/> to create the destination directory if it does not already exist.</param>
    /// <returns>A <see cref="PartialFileWithStream"/> for writing the temporary file.</returns>
    public static PartialFile<FileStream> CreateStreamFile(
        string finalPath,
        FileHandleRequest request,
        bool createDirectory = false)
    {
        Require.ArgumentNotNullOrEmpty(finalPath);
        Require.ArgumentNotDefault(request);
        Require.Argument(request, request.Access.HasFlag(FileSystemAccess.Delete), "Options must include delete access.");

        var partialPath = finalPath + PartialExtension;

        if (createDirectory)
            Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

        var fileStream = FileSession.Open(partialPath, request).CreateFileStream(request.Access.ToFileAccess(), ownsHandle: true);
        return new PartialFile<FileStream>(
            fileStream,
            static file => file.ToFileResource(),
            finalPath,
            () => FileSession.Open(partialPath, request with { Mode = FileMode.Open }).CreateFileStream(request.Access.ToFileAccess(), ownsHandle: true));
    }
}
