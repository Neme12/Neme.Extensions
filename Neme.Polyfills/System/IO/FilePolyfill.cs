using Microsoft.Win32.SafeHandles;
using Neme.Extensions.InteropServices;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.IO;

public static partial class FilePolyfill
{
    private static string NullDevicePath =>
#if NETFRAMEWORK
        Windows.NullDevicePath;
#else
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
#pragma warning disable CA1416 // Old Windows versions are not supported
            ? Windows.NullDevicePath
#pragma warning restore CA1416
            : Unix.NullDevicePath;
#endif

    extension(File)
    {
        public static SafeFileHandle OpenNullHandle()
        {
#if NET11_0_OR_GREATER
            return File.OpenNullHandle();
#elif NET6_0
            return File.OpenHandle(NullDevicePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
#else
            return
#if NETFRAMEWORK
                Windows.OpenNullHandle();
#else
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
#pragma warning disable CA1416 // Old Windows versions are not supported
                    ? Windows.OpenNullHandle()
#pragma warning restore CA1416
                    : Unix.OpenNullHandle();
#endif
#endif
        }
    }

    private static class SafeFileHandleAccessors
    {
#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_path")]
        public static extern ref string? Path(SafeFileHandle handle);
#else
        public static readonly FieldInfo? PathField =
            RuntimeInformation.IsNetCoreVersionOrGreater(6, 0)
            ? typeof(SafeFileHandle).GetField(
                "_path",
                BindingFlags.NonPublic | BindingFlags.Instance).NotNull()
            : null;
#endif
    }

    private static T NotNull<T>(this T? value)
    {
        if (value is null)
            Fail();

        return value;

        [DoesNotReturn]
        static void Fail([CallerMemberName] string? valueSource = null)
        {
            Debug.Assert(valueSource is not null);

            var failMessage = $"NotNull() failed for value '{valueSource}'.";
            throw new InvalidOperationException(failMessage);
        }
    }
}
