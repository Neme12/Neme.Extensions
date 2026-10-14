using Neme.Extensions.InteropServices;
using Neme.Polyfills.Internal;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32.Foundation;
using Windows.Win32.System.IO;
using Windows.Win32.Storage.FileSystem;
using System.Runtime.CompilerServices;
using Windows.Wdk.Storage.FileSystem;
using Win32PInvoke = Windows.Win32.PInvoke;
using WdkPInvoke = Windows.Wdk.PInvoke;

namespace Microsoft.Win32.SafeHandles;

public static partial class SafeFileHandlePolyfill
{
    [SupportedOSPlatform("windows6.0.6000")]
    private static class Windows
    {
        private const FileOptions NoBuffering = (FileOptions)0x20000000;

        internal static unsafe FileOptions GetFileOptions(SafeFileHandle handle)
        {
            var metadata = _metadataTable.GetValue(handle, (handle) => new SafeFileHandleMetadata());
            if (metadata.FileOptions != (FileOptions)(-1))
                return metadata.FileOptions;

            NTCREATEFILE_CREATE_OPTIONS options;
            NTSTATUS ntStatus;

            bool succeeded = false;
            handle.DangerousAddRef(ref succeeded);

            try
            {
                IO_STATUS_BLOCK ioStatusBlock;

                ntStatus = WdkPInvoke.NtQueryInformationFile(
                    FileHandle: (HANDLE)handle.DangerousGetHandle(),
                    IoStatusBlock: &ioStatusBlock,
                    FileInformation: &options,
                    Length: sizeof(uint),
                    FileInformationClass: FILE_INFORMATION_CLASS.FileModeInformation);
            }
            finally
            {
                if (succeeded)
                    handle.DangerousRelease();
            }

            if (ntStatus.SeverityCode != NTSTATUS.Severity.Success)
            {
                throw WinNtMarshal.GetExceptionForNtStatus(ntStatus);
            }

            var result = FileOptions.None;

            if ((options & (NTCREATEFILE_CREATE_OPTIONS.FILE_SYNCHRONOUS_IO_ALERT | NTCREATEFILE_CREATE_OPTIONS.FILE_SYNCHRONOUS_IO_NONALERT)) == 0)
            {
                result |= FileOptions.Asynchronous;
            }
            if ((options & NTCREATEFILE_CREATE_OPTIONS.FILE_WRITE_THROUGH) != 0)
            {
                result |= FileOptions.WriteThrough;
            }
            if ((options & NTCREATEFILE_CREATE_OPTIONS.FILE_RANDOM_ACCESS) != 0)
            {
                result |= FileOptions.RandomAccess;
            }
            if ((options & NTCREATEFILE_CREATE_OPTIONS.FILE_SEQUENTIAL_ONLY) != 0)
            {
                result |= FileOptions.SequentialScan;
            }
            if ((options & NTCREATEFILE_CREATE_OPTIONS.FILE_DELETE_ON_CLOSE) != 0)
            {
                result |= FileOptions.DeleteOnClose;
            }
            if ((options & NTCREATEFILE_CREATE_OPTIONS.FILE_NO_INTERMEDIATE_BUFFERING) != 0)
            {
                result |= NoBuffering;
            }

            return metadata.FileOptions = result;
        }

        internal static FileHandleType GetFileTypeCore(SafeFileHandle handle)
        {
            var kernelFileType = Win32PInvoke.GetFileType(handle);
            // GetFileType returns FILE_TYPE_UNKNOWN both when the type is unknown and when an error occurs.
            // Check GetLastError to distinguish the two cases.
            if (kernelFileType == FILE_TYPE.FILE_TYPE_UNKNOWN)
            {
                var error = (WIN32_ERROR)Marshal.GetLastPInvokeError();
                if (error != WIN32_ERROR.ERROR_SUCCESS)
                    throw Win32Marshal.GetExceptionForWin32Error(new Win32Exception((int)error));

                return FileHandleType.Unknown;
            }

            return kernelFileType switch
            {
                FILE_TYPE.FILE_TYPE_CHAR => FileHandleType.CharacterDevice,
                FILE_TYPE.FILE_TYPE_PIPE => GetPipeOrSocketType(handle),
                // GetFileType can return FILE_TYPE_DISK for regular files, directories and symbolic links.
                // When Path is not null, it means that the handle was created by SafeFileHandle.Open.
                // This method resolves symbolic links, so it can't be a symbolic link.
                // However, it accepts FILE_FLAG_BACKUP_SEMANTICS as an option,
                // which makes it possible to open a directory.
                // So when Path is not null and options don't include FILE_FLAG_BACKUP_SEMANTICS,
                // it's a regular file. In such case, we don't need to call GetDiskBasedType() which would be
                // an extra sys-call.
                FILE_TYPE.FILE_TYPE_DISK => GetDiskBasedType(handle),
                _ => FileHandleType.Unknown
            };
        }

        private static unsafe FileHandleType GetPipeOrSocketType(SafeFileHandle handle)
        {
            // When GetFileType returns FILE_TYPE_PIPE, the handle can be either a pipe or a socket.
            // Use GetNamedPipeInfo to determine if it's a pipe.
            if (Win32PInvoke.GetNamedPipeInfo(handle, out var flags, out Unsafe.NullRef<uint>(), out Unsafe.NullRef<uint>(), out Unsafe.NullRef<uint>()))
            {
                return FileHandleType.Pipe;
            }

            var error = (WIN32_ERROR)Marshal.GetLastPInvokeError();
            return error switch
            {
                WIN32_ERROR.ERROR_PIPE_NOT_CONNECTED => FileHandleType.Pipe,
                WIN32_ERROR.ERROR_INVALID_FUNCTION => FileHandleType.Socket,
                _ => throw Win32Marshal.GetExceptionForWin32Error(new Win32Exception((int)error))
            };
        }

        private static unsafe FileHandleType GetDiskBasedType(SafeFileHandle handle)
        {
            // First check if it's a directory using GetFileInformationByHandle
            if (Win32PInvoke.GetFileInformationByHandle(handle, out var fileInfo))
            {
                if ((fileInfo.dwFileAttributes & (uint)FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_DIRECTORY) != 0)
                {
                    return FileHandleType.Directory;
                }

                // Check if it's a reparse point - only symlinks should return SymbolicLink
                if ((fileInfo.dwFileAttributes & (uint)FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_REPARSE_POINT) != 0)
                {
                    // Check the reparse tag to distinguish symlinks from other reparse points (junctions, mount points, etc.)
                    ref var tagInfo = ref AllocateFileInfo<FILE_ATTRIBUTE_TAG_INFO>(stackalloc byte[sizeof(FILE_ATTRIBUTE_TAG_INFO)], out var tagInfoBuffer);

                    if (Win32PInvoke.GetFileInformationByHandleEx(handle, FILE_INFO_BY_HANDLE_CLASS.FileAttributeTagInfo, tagInfoBuffer))
                    {
                        if (tagInfo.ReparseTag == Win32PInvoke.IO_REPARSE_TAG_SYMLINK)
                            return FileHandleType.SymbolicLink;
                    }

                    // Other reparse points (junctions, mount points, etc.) are not recognized as of now
                    return FileHandleType.Unknown;
                }
            }

            return FileHandleType.RegularFile;
        }

        private static unsafe ref T AllocateFileInfo<T>(Span<byte> buffer, out Span<byte> fileInfoBuffer) where T : unmanaged
        {
            Debug.Assert(buffer.Length >= sizeof(T));

            fileInfoBuffer = buffer;

#if NETCOREAPP3_0_OR_GREATER
        return ref MemoryMarshal.AsRef<T>(buffer);
#else
            return ref Unsafe.As<byte, T>(ref MemoryMarshal.GetReference(buffer));
#endif
        }
    }
}
