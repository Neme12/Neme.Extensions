using Neme.Extensions.Contracts;

namespace Neme.Extensions.FileSystem;

public readonly record struct FileReferenceOptions
{
    private readonly FileReferenceFlags _flags;

    public static FileReferenceOptions None =>
        default;

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
}
