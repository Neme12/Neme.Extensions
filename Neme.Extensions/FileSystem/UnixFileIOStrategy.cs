#if !NETFRAMEWORK
using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.InteropServices;
using Neme.Extensions.Ownership;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Mono.Unix.Native;

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
        
        var rawHandle = Syscall.open(fullPath!, openFlags, (FilePermissions)openPermissions);
        using var handle = OwnedOrBorrowed.Create(new SafeFileHandle((nint)rawHandle, ownsHandle: true));

        if (handle.Value.IsInvalid)
        {
            handle.Dispose();

            var error = Stdlib.GetLastError();
            if (error == Errno.EISDIR)
                error = Errno.EACCES;

            throw UnixMarshal.GetExceptionForUnixError(error, fullPath);
        }

        InitHandle(handle.Value, fullPath!, mode, access, share, options, attributes, preallocationSize);
        return handle.Move();
    }

    private static void InitHandle(
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

#if NET6_0_OR_GREATER && !NET11_0_OR_GREATER
        if ((options & FileOptions.Asynchronous) != 0)
        {
            SafeFileHandleAccessors.SetIsAsync(handle, true);
        }
#endif
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
#elif NET6_0_OR_GREATER
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
    }
}

#endif
