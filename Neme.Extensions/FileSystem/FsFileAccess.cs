namespace Neme.Extensions.FileSystem;

[Flags]
public enum FsFileAccess
{
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    WriteAttributes = 8,
    ReadWrite = Read | Write,
    ReadWriteDelete = Read | Write | Delete,
}
