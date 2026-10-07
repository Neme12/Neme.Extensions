using Neme.Extensions.InteropServices;
using Neme.Polyfills.Internal;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles;

public static partial class SafeFileHandlePolyfill
{
    private static ConditionalWeakTable<SafeFileHandle, SafeFileHandleMetadata> _metadataTable = new();

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
                    : (Windows.GetFileOptions(handle) & FileOptions.Asynchronous) != 0;
#endif
            }
        }

        public FileHandleType Type
        {
            get
            {
#if NET11_0_OR_GREATER
                return handle.Type;
#else
                ObjectDisposedException.ThrowIf(handle.IsClosed, handle);

                var metadata = _metadataTable.GetValue(handle, (handle) => new SafeFileHandleMetadata());
                if (metadata.FileType != (FileHandleType)(-1))
                    return metadata.FileType;

                return metadata.FileType = GetFileTypeCore(handle);
#endif
            }
        }
    }

#if !NET11_0_OR_GREATER
    private static FileHandleType GetFileTypeCore(SafeFileHandle handle) =>
#if NETFRAMEWORK
        Windows.GetFileTypeCore(handle);
#else
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
#pragma warning disable CA1416 // Old Windows versions are not supported
            ? Windows.GetFileTypeCore(handle)
#pragma warning restore CA1416
            : Unix.GetFileTypeCore(handle);
#endif
#endif

    private static class SafeFileHandleAccessors
    {
#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_IsAsync")]
        public static extern bool IsAsync(SafeFileHandle handle);
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
