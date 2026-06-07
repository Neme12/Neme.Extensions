using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

[StructLayout(LayoutKind.Auto)]
public readonly record struct FileBookmark(string Path, FileId FileId);
