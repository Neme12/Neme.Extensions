#if !NETFRAMEWORK
using Mono.Unix.Native;
using System.ComponentModel;
using System.Runtime.Versioning;

namespace Neme.Extensions.InteropServices;

[UnsupportedOSPlatform("windows")]
internal static class UnixMarshal
{
    public static Exception GetExceptionForLastUnixError(string? path = "", bool isDirError = false) =>
        GetExceptionForUnixError(new Win32Exception((int)Stdlib.GetLastError()), path, isDirError);

    public static Exception GetExceptionForUnixError(Errno errorCode, string? path = "", bool isDirError = false) =>
        GetExceptionForUnixError(new Win32Exception((int)errorCode), path, isDirError);

    public static Exception GetExceptionForUnixError(
        Win32Exception exception, string? path = "", bool isDirError = false)
    {
        var errorCode = (Errno)exception.NativeErrorCode;

        switch (errorCode)
        {
            case Errno.ENOENT:
                // For Windows compatibility, throw DirectoryNotFoundException instead of FileNotFoundException
                // when the parent folder does not exist.
                if (!isDirError && (path is null || ParentDirectoryExists(path)))
                {
                    return !string.IsNullOrEmpty(path) ?
                        new FileNotFoundException(string.Format(Strings.IO_FileNotFound_FileName, path), path, exception) :
                        new FileNotFoundException(Strings.IO_FileNotFound, exception);
                }
                goto case Errno.ENOTDIR;

            case Errno.ENOTDIR:
                return !string.IsNullOrEmpty(path) ?
#if NET11_0_OR_GREATER
                    new DirectoryNotFoundException(string.Format(Strings.IO_PathNotFound_Path, path), path, exception) :
#else
                    new DirectoryNotFoundException(string.Format(Strings.IO_PathNotFound_Path, path), exception) :
#endif
                    new DirectoryNotFoundException(Strings.IO_PathNotFound_NoPathName, exception);

            case Errno.EACCES:
            case Errno.EBADF:
            case Errno.EPERM:
                Exception inner = new IOException(exception.Message, exception);
                return !string.IsNullOrEmpty(path) ?
                    new UnauthorizedAccessException(string.Format(Strings.UnauthorizedAccess_IODenied_Path, path), inner) :
                    new UnauthorizedAccessException(Strings.UnauthorizedAccess_IODenied_NoPathName, inner);

            case Errno.ENAMETOOLONG:
                return !string.IsNullOrEmpty(path) ?
                    new PathTooLongException(string.Format(Strings.IO_PathTooLong_Path, path), exception) :
                    new PathTooLongException(Strings.IO_PathTooLong, exception);

            case Errno.EWOULDBLOCK:
                return !string.IsNullOrEmpty(path) ?
                    new IOException(string.Format(Strings.IO_SharingViolation_File, path), exception) :
                    new IOException(Strings.IO_SharingViolation_NoFileName, exception);

            case Errno.ECANCELED:
                return new OperationCanceledException(null, exception);

            case Errno.EFBIG:
                return new ArgumentOutOfRangeException("value", Strings.ArgumentOutOfRange_FileLengthTooBig);

            case Errno.EEXIST:
                if (!string.IsNullOrEmpty(path))
                {
                    return new IOException(string.Format(Strings.IO_FileExists_Name, path), exception);
                }
                goto default;

            default:
                return new IOException(exception.Message, exception);
        }

        static bool ParentDirectoryExists(string fullPath)
        {
            string? parentPath = Path.GetDirectoryName(Path.TrimEndingDirectorySeparator(fullPath));

            return Directory.Exists(parentPath);
        }
    }
}
#endif
