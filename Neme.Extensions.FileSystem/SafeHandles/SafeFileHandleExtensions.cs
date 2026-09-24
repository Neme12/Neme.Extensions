using Microsoft.Win32.SafeHandles;

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
    }
}
