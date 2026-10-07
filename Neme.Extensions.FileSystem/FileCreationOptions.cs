using Neme.Extensions.Contracts;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

public readonly record struct FileCreationOptions
{
    public FileAttributes Attributes
    {
        get;
        init
        {
            Require.ArgumentDefined(value);
            field = value;
        }
    }

    public UnixFileMode? UnixCreateMode
    {
        get;
        [UnsupportedOSPlatform("windows")]
        init
        {
            if (value is not null)
                Require.ArgumentFlagsDefined(value.Value);
            field = value;
        }
    }

    public long PreallocationSize
    {
        get;
        init
        {
            Require.ArgumentNotNegative(value);
            field = value;
        }
    }
}
