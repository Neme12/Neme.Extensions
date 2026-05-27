// Portions of this file are derived from the .NET runtime:
//   Licensed to the .NET Foundation under one or more agreements.
//   The .NET Foundation licenses this file to you under the MIT license.
// Source: https://github.com/dotnet/runtime/blob/v10.0.8/src/libraries/System.Private.CoreLib/src/Microsoft/Win32/SafeHandles/SafeFileHandle.Unix.cs

#if !NETFRAMEWORK
using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;
using Neme.Extensions.Contracts;
using Neme.Extensions.Internal;
using Neme.Extensions.Internal.Interop;
using Neme.Extensions.InteropServices;
using Neme.Extensions.Ownership;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

[UnsupportedOSPlatform("windows")]
internal sealed class UnixFileIOStrategy : FileIOStrategy
{
    protected override int MaxFileNameLength => 255;
    protected override int MaxPathLength => 4096;

    private const UnixFileMode DefaultCreateMode =
        UnixFileMode.UserRead |
        UnixFileMode.UserWrite |
        UnixFileMode.GroupRead |
        UnixFileMode.GroupWrite |
        UnixFileMode.OtherRead |
        UnixFileMode.OtherWrite;

    internal static bool DisableFileLocking { get; } =
#if NET5_0_OR_GREATER
        OperatingSystem.IsBrowser() ||
#endif
#if NET8_0_OR_GREATER
        OperatingSystem.IsWasi() || // #40065: Emscripten does not support file locking
#endif
        AppContextConfigHelper.GetBooleanConfig("System.IO.DisableFileLocking", "DOTNET_SYSTEM_IO_DISABLEFILELOCKING", defaultValue: false);

    [return: OwnershipTransfer]
    public override SafeFileHandle OpenHandle(string path, FsFileOptions options)
    {
        Debug.Assert(IsValidPath(path));

        return Open(
            Path.GetFullPath(path),
            options.Mode,
            options.Access,
            options.Share,
            options.Options,
            options.Attributes,
            options.UnixCreateMode ?? DefaultCreateMode,
            options.PreallocationSize);
    }

    [return: OwnershipTransfer]
    public override SafeFileHandle OpenHandle(FsFileId fileId, FsFileOptions options)
    {
        throw new NotImplementedException();
    }

    [return: OwnershipTransfer]
    public override SafeFileHandle OpenHandleBy([Borrow] SafeFileHandle? rootDirectory, string? path, FsFileOptions options)
    {
        throw new NotImplementedException();
    }

    [return: OwnershipTransfer]
    public override SafeFileHandle DuplicateHandle([Borrow] SafeFileHandle file, FsFileAccess? access)
    {
        throw new NotImplementedException();
    }

    public override string GetPath([Borrow] SafeFileHandle file)
    {
        throw new NotImplementedException();
    }

    public override void Move([Borrow] SafeFileHandle sourceFile, string destFileName, bool overwrite)
    {
        throw new NotImplementedException();
    }

    public override void Delete([Borrow] SafeFileHandle file)
    {
        throw new NotImplementedException();
    }

    public override void SetFileAttributes([Borrow] SafeFileHandle file, FileAttributes attributes)
    {
        throw new NotImplementedException();
    }

    public override FileAttributes GetFileAttributes([Borrow] SafeFileHandle file)
    {
        throw new NotImplementedException();
    }

    public override FsFileInfo GetFileInfo([Borrow] SafeFileHandle file)
    {
        throw new NotImplementedException();
    }

    public override FsFileId GetFileId([Borrow] SafeFileHandle file)
    {
        throw new NotImplementedException();
    }

    private static SafeFileHandle Open(
        string fullPath,
        FileMode mode,
        FsFileAccess access,
        FileShare share,
        FileOptions options,
        FileAttributes attributes,
        UnixFileMode openPermissions,
        long preallocationSize)
    {
        Debug.Assert(fullPath != null);

        var openFlags =
            mode.ToUnix() |
            access.ToUnix() |
            share.ToUnix() |
            options.ToUnix();

        using MutableDisposable<OwnedOrBorrowed<SafeFileHandle?>> mutableHandle =
            MutableDisposable.Create(OwnedOrBorrowed.Create<SafeFileHandle?>(null));

        while (true)
        {
            var rawHandle = Syscall.open(fullPath!, openFlags, (FilePermissions)openPermissions);
            mutableHandle.SetValue(OwnedOrBorrowed.Create<SafeFileHandle?>(new SafeFileHandle((nint)rawHandle, ownsHandle: true)));

            if (mutableHandle.Value.Value!.IsInvalid)
            {
                mutableHandle.Value.Dispose();

                var error = Stdlib.GetLastError();
                if (error == Errno.EISDIR)
                    error = Errno.EACCES;

                throw UnixMarshal.GetExceptionForUnixError(error, fullPath);
            }

            if (InitHandle(mutableHandle.Value.Value!, fullPath!, mode, access, share, options, attributes, preallocationSize))
            {
                return mutableHandle.Value.Move()!;
            }
            else
            {
                mutableHandle.Value.Dispose();
            }
        }
    }

    private static bool InitHandle(
        SafeFileHandle handle,
        string path,
        FileMode mode,
        FsFileAccess access,
        FileShare share,
        FileOptions options,
        FileAttributes attributes,
        long preallocationSize)
    {
        Stat status = default;
        bool statusHasValue = false;

        // Check whether our handle is a directory.
        // We can omit the check when write access is requested. open will have failed with EISDIR.
        if ((access & (FsFileAccess)RawFsFileAccess.Write) == 0)
        {
            FStatCheckIO(handle, path, ref status, ref statusHasValue);

            var stMode = status.st_mode & FilePermissions.S_IFMT;
            var shouldBeDirectory = (attributes & FileAttributes.Directory) != 0;

            if (shouldBeDirectory && stMode != FilePermissions.S_IFDIR ||
                !shouldBeDirectory && stMode == FilePermissions.S_IFDIR)
            {
                throw UnixMarshal.GetExceptionForUnixError(Errno.EACCES, path);
            }
        }

// On previous .NET versions, there is no IsAsync property.
// On .NET 11+, IsAsync is set to false as it reflects the actual state
// of the file descriptor.
#if NET6_0_OR_GREATER && !NET11_0_OR_GREATER
        if ((options & FileOptions.Asynchronous) != 0)
        {
            SafeFileHandleAccessors.SetIsAsync(handle, true);
        }
#endif

        var isLocked = false;

        // Lock the file if requested via FileShare.  This is only advisory locking. FileShare.None implies an exclusive
        // lock on the file and all other modes use a shared lock.  While this is not as granular as Windows, not mandatory,
        // and not atomic with file opening, it's better than nothing.
        Interop.Libc.LockOperations lockOperation = (share == FileShare.None) ? Interop.Libc.LockOperations.LOCK_EX : Interop.Libc.LockOperations.LOCK_SH;
        if (!DisableFileLocking && !(isLocked = Interop.Libc.FLock(handle, lockOperation | Interop.Libc.LockOperations.LOCK_NB) >= 0))
        {
            // The only error we care about is EWOULDBLOCK, which indicates that the file is currently locked by someone
            // else and we would block trying to access it.  Other errors, such as ENOTSUP (locking isn't supported) or
            // EACCES (the file system doesn't allow us to lock), will only hamper FileStream's usage without providing value,
            // given again that this is only advisory / best-effort.
            var error = (Errno)Marshal.GetLastPInvokeError();
            if (error == Errno.EWOULDBLOCK)
                throw UnixMarshal.GetExceptionForUnixError(error, path);
        }

        if (isLocked)
        {
            // On previous .NET versions, there is no _isLocked field and
            // SafeFileHandle disposal unlocks the file unconditionally.
#if NET8_0_OR_GREATER
            SafeFileHandleAccessors.IsLocked(handle) = true;
#elif NET6_0_OR_GREATER
            SafeFileHandleAccessors.IsLockedField.SetValue(handle, true);
#endif
        }

        // On Windows, DeleteOnClose happens when all kernel handles to the file are closed.
        // Unix kernels don't have this feature, and .NET deletes the file when the Handle gets disposed.
        // When the file is opened with an exclusive lock, we can use it to check the file at the path
        // still matches the file we've opened.
        // When the delete is performed by another .NET Handle, it holds the lock during the delete.
        // Since we've just obtained the lock, the file will already be removed/replaced.
        // We limit performing this check to cases where our file was opened with DeleteOnClose with
        // a mode of OpenOrCreate.
        if (isLocked && (options & FileOptions.DeleteOnClose) != 0 &&
            share == FileShare.None && mode == FileMode.OpenOrCreate)
        {
            FStatCheckIO(handle, path, ref status, ref statusHasValue);

            Stat pathStatus;

            if (Syscall.stat(path, out pathStatus) != 0)
            {
                // If the file was removed, re-open.
                // Otherwise throw the error 'stat' gave us (assuming this is the
                // error 'open' will give us if we'd call it now).
                var error = Stdlib.GetLastError();
                if (error == Errno.ENOENT)
                    return false;

                throw UnixMarshal.GetExceptionForUnixError(error, path);
            }

            if (pathStatus.st_ino != status.st_ino ||
                pathStatus.st_dev != status.st_dev)
            {
                // The file was replaced, re-open
                return false;
            }
        }

        // Enable DeleteOnClose when we've successfully locked the file.
        // On Windows, the locking happens atomically as part of opening the file.
        if ((options & FileOptions.DeleteOnClose) != 0)
        {
            // On previous .NET versions, there is no _deleteOnClose field and
            // DeleteOnClose isn't supported. It only happens there as part of
            // FileStream disposal.
#if NET8_0_OR_GREATER
            SafeFileHandleAccessors.DeleteOnClose(handle) = true;
#elif NET6_0_OR_GREATER
            SafeFileHandleAccessors.DeleteOnCloseField.SetValue(handle, true);
#endif
        }

        // These provide hints around how the file will be accessed.  Specifying both RandomAccess
        // and Sequential together doesn't make sense as they are two competing options on the same spectrum,
        // so if both are specified, we prefer RandomAccess (behavior on Windows is unspecified if both are provided).

        PosixFadviseAdvice fadv =
            (options & FileOptions.RandomAccess) != 0 ? PosixFadviseAdvice.POSIX_FADV_RANDOM :
            (options & FileOptions.SequentialScan) != 0 ? PosixFadviseAdvice.POSIX_FADV_SEQUENTIAL :
            0;
        if (fadv != 0)
        {
            int result;

            using (var handleScope = handle.CreateScope())
                result = Syscall.posix_fadvise((int)handleScope.Handle, 0, 0, fadv);

            if (result != 0 && Stdlib.GetLastError() is var error and not Errno.ENOSYS) // just a hint.
                throw UnixMarshal.GetExceptionForUnixError(error, path);
        }

        if (mode is FileMode.Create or FileMode.Truncate && !DisableFileLocking)
        {
            // Truncate the file now if the file mode requires it. This ensures that the file only will be truncated
            // if opened successfully.

            int truncateResult;

            using (var scope = handle.CreateScope())
                truncateResult = Syscall.ftruncate((int)scope.Handle, 0);

            if (truncateResult != 0)
            {
                var error = Stdlib.GetLastError();
                if (error is not (Errno.EBADF or Errno.EINVAL))
                {
                    // We know the file descriptor is valid and we know the size argument to FTruncate is correct,
                    // so if EBADF or EINVAL is returned, it means we're dealing with a special file that can't be
                    // truncated.  Ignore the error in such cases; in all others, throw.
                    throw UnixMarshal.GetExceptionForUnixError(error, path);
                }
            }
        }

        if (preallocationSize > 0)
        {
            int result;

            using (var scope = handle.CreateScope())
                result = Syscall.posix_fallocate((int)scope.Handle, 0, (ulong)preallocationSize);

            if (result != 0)
            {
                var error = Stdlib.GetLastError();

                // Only throw for errors that indicate there is not enough space.
                if (error is Errno.EFBIG or Errno.ENOSPC)
                {
                    handle.Dispose();

                    // Delete the file we've created.
                    Debug.Assert(mode is FileMode.Create or FileMode.CreateNew);
                    Syscall.unlink(path);

                    throw new IOException(
                        string.Format(error == Errno.EFBIG
                                ? Strings.IO_FileTooLarge_Path_AllocationSize
                                : Strings.IO_DiskFull_Path_AllocationSize,
                            path, preallocationSize));
                }
            }
        }

        return true;
    }

    private static void FStatCheckIO(
        SafeFileHandle handle,
        string path,
        ref Stat status,
        ref bool statusHasValue)
    {
        if (!statusHasValue)
        {
            int result;

            using (var handleScope = handle.CreateScope())
                result = Syscall.fstat((int)handleScope.Handle, out status);

            if (result != 0)
                throw UnixMarshal.GetExceptionForLastUnixError(path);

            statusHasValue = true;
        }
    }

    private static class SafeFileHandleAccessors
    {
#if NET8_0_OR_GREATER && !NET11_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_IsAsync")]
        public static extern void SetIsAsync(SafeFileHandle handle, bool value);
#elif NET6_0_OR_GREATER && !NET11_0_OR_GREATER
        private static readonly MethodInfo s_setIsAsyncMethod =
            typeof(SafeFileHandle).GetMethod(
                "set_IsAsync",
                genericParameterCount: 0,
                BindingFlags.NonPublic | BindingFlags.Instance,
                [typeof(bool)])
            .NotNull();

        public static readonly SetIsAsyncDelegate SetIsAsync =
            (SetIsAsyncDelegate)s_setIsAsyncMethod.CreateDelegate(typeof(SetIsAsyncDelegate));

        public delegate void SetIsAsyncDelegate(SafeFileHandle handle, bool value);
#endif

#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_isLocked")]
        public static extern ref bool IsLocked(SafeFileHandle handle);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_deleteOnClose")]
        public static extern ref bool DeleteOnClose(SafeFileHandle handle);
#elif NET6_0_OR_GREATER
        public static readonly FieldInfo IsLockedField =
            typeof(SafeFileHandle).GetField(
                "_isLocked",
                BindingFlags.NonPublic | BindingFlags.Instance)
            .NotNull();

        public static readonly FieldInfo DeleteOnCloseField =
            typeof(SafeFileHandle).GetField(
                "_deleteOnClose",
                BindingFlags.NonPublic | BindingFlags.Instance)
            .NotNull();
#endif
    }
}

#endif
