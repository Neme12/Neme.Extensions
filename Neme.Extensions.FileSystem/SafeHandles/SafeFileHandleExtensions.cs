using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.InteropServices;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem.SafeHandles;

public static class SafeFileHandleExtensions
{
    extension(SafeFileHandle file)
    {
        public string Path =>
            FileIO.GetPath(file);

        public FileAccess Access =>
            FileIO.GetAccess(file);

        public bool CanRead =>
            FileIO.GetAccess(file).HasFlag(FileAccess.Read);

        public bool CanWrite =>
            FileIO.GetAccess(file).HasFlag(FileAccess.Write);

        public bool CanSeek =>
            FileIO.CanSeek(file);

        public long Position
        {
            get => FileIO.Seek(file, 0, SeekOrigin.Current);
            set => FileIO.Seek(file, value, SeekOrigin.Begin);
        }

        public long Length
        {
            get
            {
#if NET6_0_OR_GREATER
                return RandomAccess.GetLength(file);
#else
                return FileIO.GetLength(file);
#endif
            }
            set
            {
#if NET7_0_OR_GREATER
                RandomAccess.SetLength(file, value);
#else
                FileIO.SetLength(file, value);
#endif
            }
        }

        public string? OpenedPath
        {
            get
            {
#if NET8_0_OR_GREATER
                return SafeFileHandleAccessors.GetPath(file);
#else
                return SafeFileHandleAccessors.GetPath?.Invoke(file);
#endif
            }
        }
    }

    private static class SafeFileHandleAccessors
    {
#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Path")]
        public extern static string? GetPath(SafeFileHandle handle);
#else
        public static PathDelegate? GetPath { get; } =
            RuntimeInformation.IsNetCoreVersionOrGreater(6, 0)
            ? typeof(SafeFileHandle).GetMethod(
                "get_Path",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                Type.EmptyTypes)!.CreateDelegate<PathDelegate>().NotNull()
            : null;

        public delegate string? PathDelegate(SafeFileHandle handle);
#endif
    }
}
