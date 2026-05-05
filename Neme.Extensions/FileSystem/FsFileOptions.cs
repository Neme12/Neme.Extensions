using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Ownership;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

[StructLayout(LayoutKind.Auto)]
public readonly record struct FsFileOptions
{
    public FsFileOptions(FileMode mode, FsFileAccess access, FileShare share)
    {
        Mode = mode;
        Access = access;
        Share = share;
    }

    public FsFileOptions(FileMode mode, FsFileAccess access)
    {
        Mode = mode;
        Access = access;
        Share = (access & FsFileAccess.Write) != 0 || (access & FsFileAccess.Delete) != 0
            ? FileShare.None
            : FileShare.Read;
    }

    public FileMode Mode { get; init; }

    public FsFileAccess Access { get; init; }

    public FileShare Share { get; init; }

    public FileOptions Options { get; init; }

    public FileAttributes Attributes { get; init; }

    [Borrowed]
    public SafeFileHandle? TemplateFile { get; init; }

#if NET6_0_OR_GREATER
    public static FsFileOptions FromFileStreamOptions(FileStreamOptions options)
    {
        return new FsFileOptions
        {
            Mode = options.Mode,
            Access = FsFileAccess.FromFileAccess(options.Access),
            Share = options.Share,
            Options = options.Options,
        };
    }

    public static implicit operator FsFileOptions(FileStreamOptions options) =>
        FromFileStreamOptions(options);
#endif
}
