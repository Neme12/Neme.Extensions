namespace Neme.Extensions.FileSystem;

/// <summary>
/// Raw counterpart to <see cref="FsFileAccess"/> without any combined flags.
/// The main enum has combined flags, and is therefore unusable for bitwise operations.
/// This enum is intended to be used for flag checks instead of the main enum, and
/// is not intended to be used by users of the library.
/// </summary>
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
