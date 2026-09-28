using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

public readonly record struct FileCreationOptions
{
    public FileAttributes Attributes { get; init; }

    public UnixFileMode? UnixCreateMode { get; [UnsupportedOSPlatform("windows")] init; }

    public long PreallocationSize { get; init; }
}
