using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Ownership;
using Neme.Utilities.Contracts;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.FileSystem.FileIOStrategies;

internal abstract class FileIOStrategy
{
    protected abstract int MaxFileNameLength { get; }
    protected abstract int MaxPathLength { get; }

    [return: OwnershipTransfer]
    public abstract SafeFileHandle OpenHandle(string path, FsFileOptions options);

    [return: OwnershipTransfer]
    public abstract SafeFileHandle OpenHandle(FsFileId fileId, FsFileOptions options);

    [return: OwnershipTransfer]
    public abstract SafeFileHandle OpenHandleAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options);

    [return: OwnershipTransfer]
    public abstract SafeFileHandle DuplicateHandle([Borrow] SafeFileHandle file, FsFileAccess? access);

    public abstract string GetPath([Borrow] SafeFileHandle file);

    public abstract void Move([Borrow] SafeFileHandle sourceFile, string destFileName, bool overwrite);

    public abstract void Delete([Borrow] SafeFileHandle file);

    public abstract void SetFileAttributes([Borrow] SafeFileHandle file, FileAttributes attributes);

    public abstract FileAttributes GetFileAttributes([Borrow] SafeFileHandle file);

    public abstract FsFileInfo GetFileInfo([Borrow] SafeFileHandle file);

    public abstract FsFileId GetFileId([Borrow] SafeFileHandle file);

    internal void ValidateFileName(string? fileName, bool optional = false, [CallerArgumentExpression(nameof(fileName))] string? paramName = null)
    {
        if (optional && fileName is null)
            return;

        ArgumentException.ThrowIfNullOrEmpty(fileName, paramName);

        if (!IsValidFileName(fileName))
            Throw.ArgumentException(fileName, $"File name cannot exceed {MaxFileNameLength} characters.", paramName);
    }

    internal void ValidatePath(string? path, bool optional = false, [CallerArgumentExpression(nameof(path))] string? paramName = null)
    {
        if (optional && path is null)
            return;

        ArgumentException.ThrowIfNullOrEmpty(path, paramName);

        if (!IsValidPath(path))
            Throw.ArgumentException(path, $"Path cannot exceed {MaxPathLength} characters.", paramName);
    }

    internal void ValidateFileHandle([Borrow] SafeFileHandle? file, bool optional = false, [CallerArgumentExpression(nameof(file))] string? paramName = null)
    {
        if (optional && file is null)
            return;

        ArgumentNullException.ThrowIfNull(file, paramName);

        if (!IsValidFileHandle(file))
            Throw.ArgumentException(file, "File handle must be valid and open.", paramName);
    }

    internal void ValidateFileId(FsFileId fileId, [CallerArgumentExpression(nameof(fileId))] string? paramName = null)
    {
        if (!IsValidFileId(fileId))
            Throw.ArgumentException(fileId, "File ID must be valid.", paramName);
    }

    protected bool IsValidFileName(string? fileName) =>
        fileName is not null && fileName.Length <= MaxFileNameLength;

    protected bool IsValidPath(string? path) =>
        path is not null && path.Length <= MaxPathLength;

    protected static bool IsValidFileHandle(SafeFileHandle file) =>
        !file.IsClosed && !file.IsInvalid;

    protected static bool IsValidFileId(FsFileId fileId) =>
        fileId != default;
}
