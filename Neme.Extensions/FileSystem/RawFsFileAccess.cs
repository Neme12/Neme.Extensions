namespace Neme.Extensions.FileSystem;

[Flags]
internal enum RawFsFileAccess
{
    None = 0,
    ReadAttributes = 1 << 0,
    WriteAttributes = 1 << 1,
    Read = 1 << 2,
    Write = 1 << 3,
    Delete = 1 << 4,
    Execute = 1 << 5,
}
