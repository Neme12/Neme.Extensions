using System.ComponentModel;
using System.Runtime.Versioning;

namespace Neme.Extensions.InteropServices;

[UnsupportedOSPlatform("windows")]
internal static class UnixMarshal
{
    public static Exception GetExceptionForLastUnixError(string? path = "", bool isDirError = false) =>
        GetExceptionForUnixError(new Win32Exception(), path, isDirError);

    public static Exception GetExceptionForUnixError(
        Win32Exception exception, string? path = "", bool isDirError = false)
    {
        var errorCode = (UnixErrorCode)exception.NativeErrorCode;

        switch (errorCode)
        {
            case UnixErrorCode.Error_ENOENT:
                // For Windows compatibility, throw DirectoryNotFoundException instead of FileNotFoundException
                // when the parent folder does not exist.
                if (!isDirError && (path is null || ParentDirectoryExists(path)))
                {
                    return !string.IsNullOrEmpty(path) ?
                        new FileNotFoundException(string.Format(Strings.IO_FileNotFound_FileName, path), path, exception) :
                        new FileNotFoundException(Strings.IO_FileNotFound, exception);
                }
                goto case UnixErrorCode.Error_ENOTDIR;

            case UnixErrorCode.Error_ENOTDIR:
                return !string.IsNullOrEmpty(path) ?
#if NET11_0_OR_GREATER
                    new DirectoryNotFoundException(string.Format(Strings.IO_PathNotFound_Path, path), path, exception) :
#else
                    new DirectoryNotFoundException(string.Format(Strings.IO_PathNotFound_Path, path), exception) :
#endif
                    new DirectoryNotFoundException(Strings.IO_PathNotFound_NoPathName, exception);

            case UnixErrorCode.Error_EACCES:
            case UnixErrorCode.Error_EBADF:
            case UnixErrorCode.Error_EPERM:
                Exception inner = new IOException(exception.Message, exception);
                return !string.IsNullOrEmpty(path) ?
                    new UnauthorizedAccessException(string.Format(Strings.UnauthorizedAccess_IODenied_Path, path), inner) :
                    new UnauthorizedAccessException(Strings.UnauthorizedAccess_IODenied_NoPathName, inner);

            case UnixErrorCode.Error_ENAMETOOLONG:
                return !string.IsNullOrEmpty(path) ?
                    new PathTooLongException(string.Format(Strings.IO_PathTooLong_Path, path), exception) :
                    new PathTooLongException(Strings.IO_PathTooLong, exception);

            case UnixErrorCode.Error_EWOULDBLOCK:
                return !string.IsNullOrEmpty(path) ?
                    new IOException(string.Format(Strings.IO_SharingViolation_File, path), exception) :
                    new IOException(Strings.IO_SharingViolation_NoFileName, exception);

            case UnixErrorCode.Error_ECANCELED:
                return new OperationCanceledException(null, exception);

            case UnixErrorCode.Error_EFBIG:
                return new ArgumentOutOfRangeException("value", Strings.ArgumentOutOfRange_FileLengthTooBig);

            case UnixErrorCode.Error_EEXIST:
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
