namespace Neme.Extensions.FileSystem;

[Flags]
public enum FileSystemAccess
{
    None = 0,
    ReadAttributes = 1 << 0,
    WriteAttributes = 1 << 1,
    Read = 1 << 2 | ReadAttributes,
    Write = 1 << 3 | WriteAttributes,
    Delete = 1 << 4,
    Execute = 1 << 5 | ReadAttributes,
    ReadWrite = Read | Write,
}
