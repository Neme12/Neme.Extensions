using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Buffers;
using Neme.Extensions.InteropServices;
using Neme.Extensions.Ownership;
using Neme.Utilities.Contracts;
using NodaTime;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    private const int MaxWindowsFileNameLength = 255;
    private const int MaxWindowsPathLength = short.MaxValue - 4; // 4 for the \\?\ prefix.

    [SupportedOSPlatform("windows6.0.6000")]
    public static string GetPath([Borrow] SafeFileHandle file)
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
    public static unsafe void Move([Borrow] SafeFileHandle sourceFile, string destFileName, bool overwrite = false)
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
    public static unsafe void Delete([Borrow] SafeFileHandle file)
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
    public static unsafe void SetFileAttributes([Borrow] SafeFileHandle file, FileAttributes attributes)
    {
        ValidateFileHandle(file);

        ref var fileInfo = ref AllocateFileInfo<FILE_BASIC_INFO>(
            stackalloc byte[sizeof(FILE_BASIC_INFO)],
            out var fileInfoBuffer);

        fileInfo.FileAttributes = (uint)attributes.ToWin32();

        if (!PInvoke.SetFileInformationByHandle(file, FILE_INFO_BY_HANDLE_CLASS.FileBasicInfo, fileInfoBuffer))
            throw Win32Marshal.GetExceptionForLastWin32Error();
    }

    [SupportedOSPlatform("windows5.1.2600")]
    public static FileAttributes GetFileAttributes([Borrow] SafeFileHandle file)
    {
        ValidateFileHandle(file);

        if (!PInvoke.GetFileInformationByHandle(file, out var fileInformation))
            throw Win32Marshal.GetExceptionForLastWin32Error();

        // The values of FileAttributes map directly to Win32.
        return FileAttributes.FromWin32((FILE_FLAGS_AND_ATTRIBUTES)fileInformation.dwFileAttributes);
    }

    [SupportedOSPlatform("windows5.1.2600")]
    public static FsFileInfo GetFileInfo([Borrow] SafeFileHandle file)
    {
        ValidateFileHandle(file);

        if (!PInvoke.GetFileInformationByHandle(file, out var fileInformation))
            throw Win32Marshal.GetExceptionForLastWin32Error();

        return new FsFileInfo
        {
            Attributes = FileAttributes.FromWin32((FILE_FLAGS_AND_ATTRIBUTES)fileInformation.dwFileAttributes),
            CreationTime = InstantFromFileTime(fileInformation.ftCreationTime),
            LastAccessTime = InstantFromFileTime(fileInformation.ftLastAccessTime),
            LastWriteTime = InstantFromFileTime(fileInformation.ftLastWriteTime),
            Size = (long)(((ulong)fileInformation.nFileSizeHigh << 32) | fileInformation.nFileSizeLow),
        };
    }

    [SupportedOSPlatform("windows6.0.6000")]
    public static unsafe FsFileId GetFileId([Borrow] SafeFileHandle file)
    {
        ValidateFileHandle(file);
        
        ref var fileInfo = ref AllocateFileInfo<FILE_ID_INFO>(
            stackalloc byte[sizeof(FILE_ID_INFO)],
            out var fileInfoBuffer);

        if (!PInvoke.GetFileInformationByHandleEx(file, FILE_INFO_BY_HANDLE_CLASS.FileIdInfo, fileInfoBuffer))
            throw Win32Marshal.GetExceptionForLastWin32Error();

        ulong fileIdHigh;
        ulong fileIdLow;

        fixed (byte* idBuffer = fileInfo.FileId.Identifier.Value)
        {
            fileIdHigh = Unsafe.AsRef<ulong>(idBuffer + 8);
            fileIdLow = Unsafe.AsRef<ulong>(idBuffer);
        }

        return new FsFileId
        {
            VolumeSerialNumber = fileInfo.VolumeSerialNumber,
            FileIdHigh = fileIdHigh,
            FileIdLow = fileIdLow,
        };
    }

    private static Instant InstantFromFileTime(FILETIME fileTime)
    {
        if (fileTime.dwHighDateTime == 0 && fileTime.dwLowDateTime == 0)
            return Instant.MinValue;

        var ticks = (long)(((ulong)fileTime.dwHighDateTime << 32) | (uint)fileTime.dwLowDateTime);
        return Instant.FromDateTimeOffset(DateTimeOffset.FromFileTime(ticks));
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
    [return: OwnershipTransfer]
    public static SafeFileHandle OpenHandle(string path, FsFileOptions options)
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

    [SupportedOSPlatform("windows5.1.2600")]
    public static bool TryOpenHandle(
        string path,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out SafeFileHandle? handle,
        bool requireDirectory = true)
    {
        try
        {
            handle = OpenHandle(path, options);
            return true;

        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            handle = null;
            return false;
        }
    }

    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static FsFile Open(string path, FsFileOptions options) =>
        new(OpenHandle(path, options), options);

    [SupportedOSPlatform("windows5.1.2600")]
    public static bool TryOpen(
        string path,
        FsFileOptions options,
        [NotNullWhen(true)] [OwnershipTransfer] out FsFile? file,
        bool requireDirectory = true)
    {
        try
        {
            file = Open(path, options);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            file = null;
            return false;
        }
    }

    [SupportedOSPlatform("windows5.0")]
    [return: OwnershipTransfer]
    public static SafeFileHandle DuplicateHandle([Borrow] SafeFileHandle file, FsFileAccess? access)
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

    [SupportedOSPlatform("windows5.0")]
    [return: OwnershipTransfer]
    public static FsFile Duplicate([Borrow] FsFile file) =>
        new(DuplicateHandle(file.Handle, file.Options.Access), file.Options);

    [return: OwnershipTransfer]
    public static CheckedFileStream CreateFileStream(
        [OwnershipTransfer] SafeFileHandle file,
        FsFileOptions options,
        bool leaveOpen = false,
        int bufferSize = 4096)
    {
        ValidateFileHandle(file);

        using var handle = OwnedOrBorrowed.Create(leaveOpen
            ? new SafeFileHandle(file.DangerousGetHandle(), ownsHandle: false)
            : file);

        FileAccess access = 0;

        if ((options.Access & FsFileAccess.Read) != 0)
            access |= FileAccess.Read;

        if ((options.Access & FsFileAccess.Write) != 0)
            access |= FileAccess.Write;

        return new CheckedFileStream(handle.Move(), access, bufferSize, isAsync: (options.Options & FileOptions.Asynchronous) != 0);
    }

    private static void ValidateFileHandle([Borrow] SafeFileHandle? file, bool optional = false,  [CallerArgumentExpression(nameof(file))] string? paramName = null)
    {
        if (optional && file is null)
            return;

        ArgumentNullException.ThrowIfNull(file, paramName);

        if (file.IsClosed || file.IsInvalid)
            Throw.ArgumentException(file, "File handle must be valid and open.", paramName);
    }

    private static void ValidateFileName(string? fileName, bool optional = false, [CallerArgumentExpression(nameof(fileName))] string? paramName = null)
    {
        if (optional && fileName is null)
            return;

        ArgumentException.ThrowIfNullOrEmpty(fileName, paramName);

        if (fileName.Length > MaxWindowsFileNameLength)
            Throw.ArgumentException(fileName, $"File name cannot exceed {MaxWindowsFileNameLength} characters.", paramName);
    }

    private static void ValidatePath(string? path, bool optional = false, [CallerArgumentExpression(nameof(path))] string? paramName = null)
    {
        if (optional && path is null)
            return;

        ArgumentException.ThrowIfNullOrEmpty(path, paramName);

        if (path.Length > MaxWindowsPathLength)
            Throw.ArgumentException(path, $"Path cannot exceed {MaxWindowsPathLength} characters.", paramName);
    }
}
