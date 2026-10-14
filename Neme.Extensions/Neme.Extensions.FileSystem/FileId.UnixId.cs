using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

public readonly partial record struct FileId
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly record struct UnixId
    {
        internal readonly ulong Device;
        internal readonly ulong Inode;

        public UnixId(ulong device, ulong inode)
        {
            Device = device;
            Inode = inode;
        }
    }
}
