namespace Neme.Extensions.FileSystem;

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
    public static PartialFile<FileSession> CreateSessionFile(string finalPath, FileOpenRequest request, bool createDirectory = false)
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
            finalPath,
            finalPath => FileSession.Open(finalPath, FileOpenRequest.Open(request.HandleOptions)));
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
        FileReferenceOptions options,
        bool createDirectory = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(finalPath);

        var partialPath = finalPath + PartialExtension;

        if (createDirectory)
            Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

        var file = FileReference.Create(partialPath, options);
        return new PartialFile<FileReference>(
            file,
            finalPath,
            finalPath => FileReference.Create(finalPath, options));
    }
}
