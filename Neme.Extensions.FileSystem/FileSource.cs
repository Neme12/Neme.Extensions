using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.Resources;
using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.IO;

namespace Neme.Extensions.FileSystem;

public readonly union FileSource(string, FileResource)
{
    public string FilePath =>
        this is string path ? path : throw InvalidTypeException();

    public FileResource FileResource =>
        this is FileResource resource ? resource : throw InvalidTypeException();

    public SafeFileHandle FileHandle =>
        this is FileResource resource && resource is SafeFileHandle handle ? handle : throw InvalidTypeException();

    public FileStream FileStream =>
        this is FileResource resource && resource is FileStream stream ? stream : throw InvalidTypeException();

    public FileSession FileSession =>
        this is FileResource resource && resource is FileSession session ? session : throw InvalidTypeException();

    public FileReference FileReference =>
        this is FileResource resource && resource is FileReference reference ? reference : throw InvalidTypeException();

    public bool IsValid
    {
        get
        {
            return this switch
            {
                string path => File.Exists(path),
                FileResource resource => resource.IsOpen,
                _ => throw InvalidTypeException(),
            };
        }
    }

    public bool CanSeek
    {
        get
        {
            switch (this)
            {
                case string path:
                {
                    SafeFileHandle handle;

                    try
                    {
                        handle = FileOperations.OpenHandle(path,
                            FileHandleRequest.Open(FileSystemAccess.None, FileShare.All));
                    }
                    catch (IOException)
                    {
                        return false;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return false;
                    }

                    using (handle)
                        return handle.CanSeek;
                }
                case FileResource resource:
                {
                    return resource.CanSeek;
                }
                default:
                    throw InvalidTypeException();
            };
        }
    }

    public static implicit operator FileSource(SafeFileHandle fileHandle) =>
        new FileSource(fileHandle);

    public static implicit operator FileSource(FileStream fileStream) =>
        new FileSource(fileStream);

    public static implicit operator FileSource(FileSession fileSession) =>
        new FileSource(fileSession);

    public static implicit operator FileSource(FileReference fileReference) =>
        new FileSource(fileReference);

    private static InvalidOperationException InvalidTypeException() =>
        new InvalidOperationException("Invalid file source type.");
}
