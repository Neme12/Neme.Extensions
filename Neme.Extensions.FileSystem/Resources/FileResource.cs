using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.SafeHandles;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem.Resources;

public readonly union FileResource(
    SafeFileHandle,
    FileStream,
    FileSession,
    FileReference)
    : IDisposable, IAsyncDisposable
{
    public SafeFileHandle FileHandle =>
        this is SafeFileHandle handle ? handle : throw InvalidTypeException();

    public FileStream FileStream =>
        this is FileStream stream ? stream : throw InvalidTypeException();

    public FileSession FileSession =>
        this is FileSession session ? session : throw InvalidTypeException();

    public FileReference FileReference =>
        this is FileReference reference ? reference : throw InvalidTypeException();

    public string? OpenedPath =>
        this switch
        {
            SafeFileHandle handle => handle.OpenedPath,
            FileStream stream => stream.SafeFileHandle.OpenedPath,
            FileSession session => session.OpenedPath,
            FileReference reference => reference.OpenedPath,
            _ => throw InvalidTypeException(),
        };

    public bool IsOpen =>
        this switch
        {
            SafeFileHandle handle => handle.IsOpen,
            FileStream stream => stream.SafeFileHandle.IsOpen,
            FileSession session => session.IsOpen,
            FileReference reference => reference.IsOpen,
            _ => throw InvalidTypeException(),
        };

    public bool IsClosed =>
        this switch
        {
            SafeFileHandle handle => handle.IsClosed,
            FileStream stream => stream.SafeFileHandle.IsClosed,
            FileSession session => session.IsClosed,
            FileReference reference => reference.IsClosed,
            _ => true
        };

    public bool CanSeek =>
        this switch
        {
            SafeFileHandle handle => handle.CanSeek,
            FileStream stream => stream.CanSeek,
            FileSession session => session.CanSeek,
            FileReference reference => reference.CanSeek,
            _ => throw InvalidTypeException(),
        };

    public string GetPath() =>
        this switch
        {
            SafeFileHandle handle => handle.Path,
            FileStream stream => stream.SafeFileHandle.Path,
            FileSession session => session.GetPath(),
            FileReference reference => reference.GetPath(),
            _ => throw InvalidTypeException(),
        };

    public FileId GetId() =>
        this switch
        {
            SafeFileHandle handle => FileOperations.GetId(handle),
            FileStream stream => FileOperations.GetId(stream.SafeFileHandle),
            FileSession session => session.GetId(),
            FileReference reference => reference.GetId(),
            _ => throw InvalidTypeException(),
        };

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public PersistentFileId GetPersistentId() =>
        this switch
        {
            SafeFileHandle handle => FileOperations.GetPersistentId(handle),
            FileStream stream => FileOperations.GetPersistentId(stream.SafeFileHandle),
            FileSession session => session.GetPersistentId(),
            FileReference reference => reference.GetPersistentId(),
            _ => throw InvalidTypeException(),
        };

    public FileAttributes GetAttributes() =>
        this switch
        {
            SafeFileHandle handle => FileOperations.GetAttributes(handle),
            FileStream stream => FileOperations.GetAttributes(stream.SafeFileHandle),
            FileSession session => session.GetAttributes(),
            FileReference reference => reference.GetAttributes(),
            _ => throw InvalidTypeException(),
        };

    public void SetAttributes(FileAttributes attributes)
    {
        switch (this)
        {
            case SafeFileHandle handle:
                FileOperations.SetAttributes(handle, attributes);
                break;
            case FileStream stream:
                FileOperations.SetAttributes(stream.SafeFileHandle, attributes);
                break;
            case FileSession session:
                session.SetAttributes(attributes);
                break;
            case FileReference reference:
                reference.SetAttributes(attributes);
                break;
            default:
                throw InvalidTypeException();
        }
    }

    public FileBasicInfo GetBasicInfo() =>
        this switch
        {
            SafeFileHandle handle => FileOperations.GetBasicInfo(handle),
            FileStream stream => FileOperations.GetBasicInfo(stream.SafeFileHandle),
            FileSession session => session.GetBasicInfo(),
            FileReference reference => reference.GetBasicInfo(),
            _ => throw InvalidTypeException(),
        };

    public long GetLength() =>
        this switch
        {
            SafeFileHandle handle => FileOperations.GetLength(handle),
            FileStream stream => FileOperations.GetLength(stream.SafeFileHandle),
            FileSession session => session.GetLength(),
            FileReference reference => reference.GetLength(),
            _ => throw InvalidTypeException(),
        };

    public void SetLength(long length)
    {
        switch (this)
        {
            case SafeFileHandle handle:
                FileOperations.SetLength(handle, length);
                break;
            case FileStream stream:
                FileOperations.SetLength(stream.SafeFileHandle, length);
                break;
            case FileSession session:
                session.SetLength(length);
                break;
            case FileReference reference:
                reference.SetLength(length);
                break;
            default:
                throw InvalidTypeException();
        }
    }

    public void Move(string destFileName, bool overwrite = false)
    {
        switch (this)
        {
            case SafeFileHandle handle:
                FileOperations.Move(handle, destFileName, overwrite);
                break;
            case FileStream stream:
                FileOperations.Move(stream.SafeFileHandle, destFileName, overwrite);
                break;
            case FileSession session:
                session.Move(destFileName, overwrite);
                break;
            case FileReference reference:
                reference.Move(destFileName, overwrite);
                break;
            default:
                throw InvalidTypeException();
        }
    }


    public void Delete()
    {
        switch (this)
        {
            case SafeFileHandle handle:
                FileOperations.Delete(handle);
                break;
            case FileStream stream:
                FileOperations.Delete(stream.SafeFileHandle);
                break;
            case FileSession session:
                session.Delete();
                break;
            case FileReference reference:
                reference.Delete();
                break;
            default:
                throw InvalidTypeException();
        }
    }

    public void Dispose()
    {
        switch (this)
        {
            case SafeFileHandle handle:
                handle.Dispose();
                break;
            case FileStream stream:
                stream.Dispose();
                break;
            case FileSession session:
                session.Dispose();
                break;
            case FileReference reference:
                reference.Dispose();
                break;
            default:
                throw InvalidTypeException();
        }
    }

    public ValueTask DisposeAsync()
    {
        switch (this)
        {
            case SafeFileHandle handle:
                handle.Dispose();
                return ValueTask.CompletedTask;
            case FileStream stream:
                return stream.DisposeAsync();
            case FileSession session:
                session.Dispose();
                return ValueTask.CompletedTask;
            case FileReference reference:
                reference.Dispose();
                return ValueTask.CompletedTask;
            default:
                throw InvalidTypeException();
        }
    }

    private static InvalidOperationException InvalidTypeException() =>
        new InvalidOperationException("Invalid file resource type.");
}
