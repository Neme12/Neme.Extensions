using Neme.Extensions.FileSystem.Internal;
using Neme.Extensions.FileSystem.Resources;

namespace Neme.Extensions.FileSystem.Workflows;

public static class PartialFile
{
    public static string PartialExtension => ".part";

    /// <summary>
    /// Creates a new temporary file at <paramref name="finalPath"/> with the <c>.part</c> suffix.
    /// </summary>
    /// <param name="finalPath">The final destination path that will be used by <see cref="Commit(bool)"/>.</param>
    /// <param name="request">The options used to open the temporary file. Delete access is required so the temporary file can be cleaned up.</param>
    /// <param name="createDirectory"><see langword="true"/> to create the destination directory if it does not already exist.</param>
    /// <returns>A <see cref="PartialFile"/> for writing the temporary file.</returns>
    public static PartialFile<FileSession> CreateSessionFile(string finalPath, FileHandleRequest request, bool createDirectory = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(finalPath);

        if ((request.Access & FileSystemAccess.Delete) == 0)
            throw new ArgumentException("Options must include delete access.", nameof(request));

        var partialPath = finalPath + PartialExtension;

        if (createDirectory)
            Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

        var file = FileSession.Open(partialPath, request);
        return new PartialFile<FileSession>(
            file,
            file => file,
            finalPath,
            finalPath => FileSession.Open(finalPath + PartialExtension, request with { Mode = FileMode.Open }));
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
        FileReferenceRequest options,
        bool createDirectory = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(finalPath);

        var partialPath = finalPath + PartialExtension;

        if (createDirectory)
            Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

        var file = FileReference.Create(partialPath, options);
        return new PartialFile<FileReference>(
            file,
            file => file,
            finalPath,
            finalPath => FileReference.Create(finalPath + PartialExtension, options with { Mode = FileReferenceMode.Open}));
    }

    /// <summary>
    /// Creates a new temporary file at <paramref name="finalPath"/> with the <c>.part</c> suffix.
    /// </summary>
    /// <param name="finalPath">The final destination path that will be used by <see cref="Commit(bool)"/>.</param>
    /// <param name="request">The options used to open the temporary file. Delete access is required so the temporary file can be cleaned up.</param>
    /// <param name="createDirectory"><see langword="true"/> to create the destination directory if it does not already exist.</param>
    /// <returns>A <see cref="PartialFileWithStream"/> for writing the temporary file.</returns>
    public static PartialFile<FileStream> CreateFileStream(
        string finalPath,
        FileHandleRequest request,
        bool createDirectory = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(finalPath);

        if ((request.Access & FileSystemAccess.Delete) == 0)
            throw new ArgumentException("Options must include delete access.", nameof(request));

        var partialPath = finalPath + PartialExtension;

        if (createDirectory)
            Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

        var fileStream = FileSession.Open(partialPath, request).CreateFileStream(request.Access.ToFileAccess(), ownsHandle: true);
        return new PartialFile<FileStream>(
            fileStream,
            file => new FileStreamAdapter(file),
            finalPath,
            finalPath => FileSession.Open(finalPath + PartialExtension, request with { Mode = FileMode.Open }).CreateFileStream(request.Access.ToFileAccess(), ownsHandle: true));
    }
}
