using Neme.Extensions.Contracts;

namespace Neme.Extensions.FileSystem.References;

public readonly record struct FileReferenceOptions
{
    private readonly FileHandleType _type;
    private readonly FileReferenceFlags _flags;

    public static FileReferenceOptions None =>
        default;

    public FileReferenceOptions(
        FileHandleType type = FileHandleType.RegularFile,
        FileReferenceFlags flags = FileReferenceFlags.None)
    {
        Require.ArgumentDefined(type);
        Require.ArgumentFlagsDefined(flags);

        _type = type;
        _flags = flags;
    }

    public void Deconstruct(out FileHandleType type, out FileReferenceFlags flags)
    {
        type = Type;
        flags = Flags;
    }

    public FileHandleType Type
    {
        get => _type;
        init
        {
            Require.ArgumentDefined(value);
            _type = value;
        }
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
