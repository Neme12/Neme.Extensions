// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using Windows.Win32.Foundation;

namespace Neme.Extensions.Utilities;

[SupportedOSPlatform("windows")]
internal static class Win32Marshal
{
    internal static Exception GetExceptionForLastWin32Error(string? path = "") =>
        GetExceptionForWin32Error(new Win32Exception(), path);

    internal static Exception GetExceptionForWin32Error(
        Win32Exception exception, string? path = "", string? errorDetails = null)
    {
        var errorCode = (WIN32_ERROR)exception.NativeErrorCode;
        Debug.Assert(errorCode != WIN32_ERROR.ERROR_SUCCESS);

        switch (errorCode)
        {
            case WIN32_ERROR.ERROR_FILE_NOT_FOUND:
                return new FileNotFoundException(
                    string.IsNullOrEmpty(path)
                        ? Strings.IO_FileNotFound
                        : string.Format(Strings.IO_FileNotFound_FileName, path),
                    path, exception);
            case WIN32_ERROR.ERROR_PATH_NOT_FOUND:
                return new DirectoryNotFoundException(
                    string.IsNullOrEmpty(path)
                        ? Strings.IO_PathNotFound_NoPathName
                        : string.Format(Strings.IO_PathNotFound_Path, path),
                    exception);
            case WIN32_ERROR.ERROR_ACCESS_DENIED:
                return new UnauthorizedAccessException(
                    string.IsNullOrEmpty(path)
                        ? Strings.UnauthorizedAccess_IODenied_NoPathName
                        : string.Format(Strings.UnauthorizedAccess_IODenied_Path, path),
                    exception);
            case WIN32_ERROR.ERROR_ALREADY_EXISTS:
                if (string.IsNullOrEmpty(path))
                    goto default;
                return new IOException(
                    string.Format(Strings.IO_AlreadyExists_Name, path),
                    exception);
            case WIN32_ERROR.ERROR_FILENAME_EXCED_RANGE:
                return new PathTooLongException(
                    string.IsNullOrEmpty(path)
                        ? Strings.IO_PathTooLong
                        : string.Format(Strings.IO_PathTooLong_Path, path),
                    exception);
            case WIN32_ERROR.ERROR_SHARING_VIOLATION:
                return new IOException(
                    string.IsNullOrEmpty(path)
                        ? Strings.IO_SharingViolation_NoFileName
                        : string.Format(Strings.IO_SharingViolation_File, path),
                    exception);
            case WIN32_ERROR.ERROR_FILE_EXISTS:
                if (string.IsNullOrEmpty(path))
                    goto default;
                return new IOException(
                    string.Format(Strings.IO_FileExists_Name, path),
                    exception);
            case WIN32_ERROR.ERROR_OPERATION_ABORTED:
                return new OperationCanceledException(null, exception);
            case WIN32_ERROR.ERROR_INVALID_PARAMETER:
            default:
                return new IOException(exception.Message, exception);
        }
    }
}
