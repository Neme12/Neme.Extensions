using Neme.Extensions.InteropServices;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Wdk;
using Windows.Wdk.Storage.FileSystem;
using Windows.Win32.Foundation;
using Windows.Win32.System.IO;

namespace Microsoft.Win32.SafeHandles;

public static class SafeFileHandlePolyfill
{
    private const FileOptions NoBuffering = (FileOptions)0x20000000;

    extension(SafeFileHandle handle)
    {
        public bool IsAsync
        {
            get
            {
#if NET6_0_OR_GREATER
                return handle.IsAsync;
#else
                return RuntimeInformation.IsNetCore
                    ? SafeFileHandleAccessors.IsAsync!.Invoke(handle)
                    : (handle.GetWindowsFileOptions() & FileOptions.Asynchronous) != 0;
#endif
            }
        }

        [SupportedOSPlatform("windows5.1.2600")]
        private unsafe FileOptions GetWindowsFileOptions()
        {
            IO_STATUS_BLOCK ioStatusBlock;
            NTCREATEFILE_CREATE_OPTIONS options;
            NTSTATUS ntStatus;

            bool succeeded = false;
            handle.DangerousAddRef(ref succeeded);

            try
            {
                ntStatus = PInvoke.NtQueryInformationFile(
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

            FileOptions result = FileOptions.None;

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

            return result;
        }
    }

    private static class SafeFileHandleAccessors
    {
#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_IsAsync")]
        public extern static bool IsAsync(SafeFileHandle handle);
#else
        public static IsAsyncDelegate? IsAsync { get; } =
            RuntimeInformation.IsNetCore
            ? typeof(SafeFileHandle).GetMethod(
                "get_IsAsync",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                Type.EmptyTypes)!.CreateDelegate<IsAsyncDelegate>()
            : null;

        public delegate bool IsAsyncDelegate(SafeFileHandle handle);
#endif
    }
}
