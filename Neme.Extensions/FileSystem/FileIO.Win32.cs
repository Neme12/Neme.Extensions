using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Buffers;
using Neme.Extensions.InteropServices;
using System.Buffers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    private const int MaxWindowsFileNameLength = 255;
    private const int MaxWindowsPathLength = short.MaxValue - 4; // 4 for the \\?\ prefix.

    [SupportedOSPlatform("windows6.0.6000")]
    public static string GetPath(SafeFileHandle file)
    {
        ValidateFileHandle(file);

        using var bufferLease = ArrayPool<char>.Shared.RentLeaseOrStackalloc(
            256, stackalloc char[256]);

        uint charsWritten;

        while (true)
        {
            charsWritten = PInvoke.GetFinalPathNameByHandle(file, bufferLease.Buffer, 0u);
            if (charsWritten != bufferLease.Length)
                break;

            var exception = new Win32Exception();
            if (exception.NativeErrorCode != (int)WIN32_ERROR.ERROR_NOT_ENOUGH_MEMORY)
                throw exception;

            bufferLease.RentMore();
        }

        const string prefix = @"\\?\";
        return bufferLease.Buffer.StartsWith(prefix)
            ? bufferLease.Buffer[prefix.Length..(int)charsWritten].ToString()
            : bufferLease.Buffer[..(int)charsWritten].ToString();
    }

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

        var handle = PInvoke.CreateFile(
            path,
            (uint)options.Access.ToWin32(),
            options.Share.ToWin32(),
            null,
            options.Mode.ToWin32(),
            options.Options.ToWin32() | options.Attributes.ToWin32(),
            null);

        if (handle.IsInvalid)
            throw Win32Marshal.GetExceptionForLastWin32Error(path);

        return handle;
    }

    [SupportedOSPlatform("windows5.0")]
    public static SafeFileHandle Duplicate(SafeFileHandle file, FsFileAccess? access)
    {
        ValidateFileHandle(file);

        var currentProcess = new SafeProcessHandle((nint)(-1), ownsHandle: false); // Pseudo-handle for the current process

        if (!PInvoke.DuplicateHandle(
            currentProcess,
            file,
            currentProcess,
            out var duplicatedHandle,
            access is null ? 0u : (uint)access.Value.ToWin32(),
            false,
            access is null ? DUPLICATE_HANDLE_OPTIONS.DUPLICATE_SAME_ACCESS : 0u))
        {
            throw Win32Marshal.GetExceptionForLastWin32Error();
        }

        return duplicatedHandle;
    }

    public static CheckedFileStream CreateFileStream(SafeFileHandle file, FsFileOptions options, int bufferSize = 4096)
    {
        ValidateFileHandle(file);

        FileAccess access = 0;

        if ((options.Access & FsFileAccess.Read) != 0)
            access |= FileAccess.Read;

        if ((options.Access & FsFileAccess.Write) != 0)
            access |= FileAccess.Write;

        return new CheckedFileStream(file, access, bufferSize, isAsync: (options.Options & FileOptions.Asynchronous) != 0);
    }

    private static void ValidateFileHandle(SafeFileHandle? file, bool optional = false,  [CallerArgumentExpression(nameof(file))] string? paramName = null)
    {
        if (optional && file is null)
            return;

        ArgumentNullException.ThrowIfNull(file, paramName);

        if (file.IsClosed || file.IsInvalid)
            throw new ArgumentException("File handle must be valid and open.", paramName);
    }

    private static void ValidateFileName(string? fileName, bool optional = false, [CallerArgumentExpression(nameof(fileName))] string? paramName = null)
    {
        if (optional && fileName is null)
            return;

        ArgumentException.ThrowIfNullOrEmpty(fileName, paramName);

        if (fileName.Length > MaxWindowsFileNameLength)
            throw new ArgumentException($"File name cannot exceed {MaxWindowsFileNameLength} characters.", paramName);
    }

    private static void ValidatePath(string? path, bool optional = false, [CallerArgumentExpression(nameof(path))] string? paramName = null)
    {
        if (optional && path is null)
            return;

        ArgumentException.ThrowIfNullOrEmpty(path, paramName);

        if (path.Length > MaxWindowsPathLength)
            throw new ArgumentException($"Path cannot exceed {MaxWindowsPathLength} characters.", paramName);
    }
}
