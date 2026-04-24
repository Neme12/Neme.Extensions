using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Ownership;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

[StructLayout(LayoutKind.Auto)]
public readonly record struct FsFileOptions
{
    [Borrowed]
    public SafeFileHandle? TemplateFile { get; init; }

    public FileMode Mode { get; init; }

    public FsFileAccess Access { get; init; }

    public FileShare Share { get; init; }

    public FileOptions Options { get; init; }

    public FileAttributes Attributes { get; init; }

#if NET6_0_OR_GREATER
    public static FsFileOptions FromFileStreamOptions(FileStreamOptions options)
    {
        return new FsFileOptions
        {
            Mode = options.Mode,
            Access = options.Access switch
            {
                0 => FsFileAccess.None,
                FileAccess.Read => FsFileAccess.Read,
                FileAccess.Write => FsFileAccess.Write,
                FileAccess.ReadWrite => FsFileAccess.ReadWrite,
                _ => throw new ArgumentOutOfRangeException(nameof(options), $"Invalid FileAccess value: {options.Access}"),
            },
            Share = options.Share,
            Options = options.Options,
        };
    }

    public static implicit operator FsFileOptions(FileStreamOptions options) =>
        FromFileStreamOptions(options);
#endif
}
