using Neme.Extensions.Contracts;

namespace Neme.Extensions.FileSystem;

public readonly record struct FileReferenceOptions
{
    private readonly FileReferenceFlags _flags;

    public FileReferenceOptions(FileReferenceFlags flags)
    {
        Require.ArgumentFlagsDefined(flags);

        _flags = flags;
    }

    public void Deconstruct(out FileReferenceFlags flags)
    {
        flags = Flags;
    }

    public FileReferenceFlags Flags
    {
        get => _flags;
        init
        {
            Require.ArgumentFlagsDefined(value);
            _flags = value;
        }
    }

#if NET6_0_OR_GREATER
    public static FileReferenceOptions FromFileStreamOptions(FileStreamOptions options)
    {
        Require.ArgumentNotNull(options);

        var deleteOnClose = options.Options.HasFlag(FileOptions.DeleteOnClose)
            ? FileReferenceFlags.DeleteOnClose
            : FileReferenceFlags.None;

        return new FileReferenceOptions
        {
            Flags = deleteOnClose,
        };
    }
#endif
}
