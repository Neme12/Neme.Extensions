using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Utilities;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

public static class FileIO
{
    private const int MaxWindowsFileNameLength = 255;
    private const int MaxWindowsPathLength = short.MaxValue - 4; // 4 for the \\?\ prefix.

    [SupportedOSPlatform("windows6.0.6000")]
    public static unsafe void Move(SafeFileHandle sourceFile, string destFileName, bool overwrite = false)
    {
        ValidateFileHandle(sourceFile);
        ValidateFileName(destFileName);

        if (destFileName.Length > MaxWindowsFileNameLength)
            throw new ArgumentException($"Destination file name cannot exceed {MaxWindowsFileNameLength}.", nameof(destFileName));

        ref var fileInfo = ref AllocateFileInfo<FILE_RENAME_INFO>(
            stackalloc byte[FILE_RENAME_INFO.SizeOf(destFileName.Length + 1)],
            out var fileInfoBuffer);

        fileInfo.FileNameLength = (uint)(destFileName.Length * sizeof(char));

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        var fileNameBuffer = MemoryMarshal.CreateSpan(ref fileInfo.FileName.e0, destFileName.Length);
        destFileName.CopyTo(fileNameBuffer);
#else
        fixed (char* fileNamePtr = &fileInfo.FileName.e0)
        {
            var fileNameBuffer = new Span<char>(fileNamePtr, destFileName.Length);
            destFileName.CopyTo(fileNameBuffer);
        }
#endif

        fileInfo.Anonymous.ReplaceIfExists = overwrite;

        if (!PInvoke.SetFileInformationByHandle(sourceFile, FILE_INFO_BY_HANDLE_CLASS.FileRenameInfo, fileInfoBuffer))
            throw Win32Marshal.GetExceptionForLastWin32Error(destFileName);
    }

    [SupportedOSPlatform("windows6.0.6000")]
    public static unsafe void Delete(SafeFileHandle file)
    {
        ValidateFileHandle(file);

        ref var fileInfo = ref AllocateFileInfo<FILE_DISPOSITION_INFO>(
            stackalloc byte[sizeof(FILE_DISPOSITION_INFO)],
            out var fileInfoBuffer);

        fileInfo.DeleteFile = true;

        if (!PInvoke.SetFileInformationByHandle(file, FILE_INFO_BY_HANDLE_CLASS.FileDispositionInfo, fileInfoBuffer))
            throw Win32Marshal.GetExceptionForLastWin32Error();
    }

    [SupportedOSPlatform("windows6.0.6000")]
    public static unsafe void SetFileAttributes(SafeFileHandle file, FileAttributes attributes)
    {
        ValidateFileHandle(file);

        ref var fileInfo = ref AllocateFileInfo<FILE_BASIC_INFO>(
            stackalloc byte[sizeof(FILE_BASIC_INFO)],
            out var fileInfoBuffer);

        // The values of FileAttributes map directly to Win32.
        fileInfo.FileAttributes = (uint)attributes;

        if (!PInvoke.SetFileInformationByHandle(file, FILE_INFO_BY_HANDLE_CLASS.FileBasicInfo, fileInfoBuffer))
            throw Win32Marshal.GetExceptionForLastWin32Error();
    }

    private static unsafe ref T AllocateFileInfo<T>(Span<byte> buffer, out Span<byte> fileInfoBuffer) where T : unmanaged
    {
        if (buffer.Length < sizeof(T))
            throw new ArgumentException($"Buffer must be at least {sizeof(T)} bytes long.", nameof(buffer));

        fileInfoBuffer = buffer;

#if NETCOREAPP3_0_OR_GREATER
        return ref MemoryMarshal.AsRef<T>(buffer);
#else
        return ref Unsafe.As<byte, T>(ref MemoryMarshal.GetReference(buffer));
#endif
    }

    [SupportedOSPlatform("windows5.1.2600")]
    public static SafeFileHandle Open(string path, FsFileOptions options)
    {
        ValidatePath(path);

        FILE_ACCESS_RIGHTS desiredAccess = 0;

        if ((options.Access & FsFileAccess.Read) != 0)
            desiredAccess |= FILE_ACCESS_RIGHTS.FILE_GENERIC_READ;

        if ((options.Access & FsFileAccess.Write) != 0)
            desiredAccess |= FILE_ACCESS_RIGHTS.FILE_GENERIC_WRITE;

        if ((options.Access & FsFileAccess.Delete) != 0)
            desiredAccess |= FILE_ACCESS_RIGHTS.DELETE;

        if ((options.Access & FsFileAccess.WriteAttributes) != 0)
            desiredAccess |= FILE_ACCESS_RIGHTS.FILE_WRITE_ATTRIBUTES;

        // The values of FileShare map directly to FILE_SHARE_MODE.
        var shareMode = (FILE_SHARE_MODE)options.Share;

        var creationDisposition = options.Mode switch
        {
            FileMode.CreateNew => FILE_CREATION_DISPOSITION.CREATE_NEW,
            FileMode.Create => FILE_CREATION_DISPOSITION.CREATE_ALWAYS,
            FileMode.Open => FILE_CREATION_DISPOSITION.OPEN_EXISTING,
            FileMode.OpenOrCreate => FILE_CREATION_DISPOSITION.OPEN_ALWAYS,
            FileMode.Truncate => FILE_CREATION_DISPOSITION.TRUNCATE_EXISTING,
            FileMode.Append => FILE_CREATION_DISPOSITION.OPEN_ALWAYS,
            _ => throw new ArgumentOutOfRangeException(nameof(options), "Invalid FileMode value.")
        };

        // The values of FileOptions map directly to FILE_FLAGS_AND_ATTRIBUTES,
        // and so do the values of FileAttributes, so we don't have to do any mapping.
        var flagsAndAttributes = (FILE_FLAGS_AND_ATTRIBUTES)((uint)options.Options | (uint)options.Attributes);

        var handle = PInvoke.CreateFile(
            path,
            (uint)desiredAccess,
            shareMode,
            null,
            creationDisposition,
            flagsAndAttributes,
            options.TemplateFile);

        if (handle.IsInvalid)
            throw Win32Marshal.GetExceptionForLastWin32Error(path);

        return handle;
    }

    public static FileStream CreateFileStream(SafeFileHandle file, FsFileOptions options, int bufferSize = 4096)
    {
        ValidateFileHandle(file);

        FileAccess access = 0;

        if ((options.Access & FsFileAccess.Read) != 0)
            access |= FileAccess.Read;

        if ((options.Access & FsFileAccess.Write) != 0)
            access |= FileAccess.Write;

        return new FileStream(file, access, bufferSize, isAsync: (options.Options & FileOptions.Asynchronous) != 0);
    }

    private static void ValidateFileHandle(SafeFileHandle file, [CallerArgumentExpression(nameof(file))] string? paramName = null)
    {
        if (file.IsClosed || file.IsInvalid)
            throw new ArgumentException("File handle must be valid and open.", paramName);
    }

    private static void ValidateFileName(string fileName, [CallerArgumentExpression(nameof(fileName))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName, paramName);

        if (fileName.Length > MaxWindowsFileNameLength)
            throw new ArgumentException($"File name cannot exceed {MaxWindowsFileNameLength} characters.", paramName);
    }

    private static void ValidatePath(string path, [CallerArgumentExpression(nameof(path))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(path, paramName);

        if (path.Length > MaxWindowsPathLength)
            throw new ArgumentException($"Path cannot exceed {MaxWindowsPathLength} characters.", paramName);
    }
}
