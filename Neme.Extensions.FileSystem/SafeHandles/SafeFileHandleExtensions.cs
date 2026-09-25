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
    }
}
