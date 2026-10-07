using Neme.Extensions.Contracts;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

public readonly record struct FileCreationOptions
{
    public static FileCreationOptions None =>
        default;

    public FileCreationOptions(
        FileAttributes attributes,
        UnixFileMode? unixCreateMode = null,
        long preallocationSize = 0)
    {
        Require.ArgumentFlagsDefined(attributes);
        if (unixCreateMode is not null)
            Require.ArgumentFlagsDefined(unixCreateMode.Value);
        Require.ArgumentNotNegative(preallocationSize);

        Attributes = attributes;
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            UnixCreateMode = unixCreateMode;
        PreallocationSize = preallocationSize;
    }

    public void Deconstruct(
        out FileAttributes attributes)
    {
        attributes = Attributes;
    }

    public void Deconstruct(
        out FileAttributes attributes,
        out UnixFileMode? unixCreateMode,
        out long preallocationSize)
    {
        attributes = Attributes;
        unixCreateMode = UnixCreateMode;
        preallocationSize = PreallocationSize;
    }

    public FileAttributes Attributes
    {
        get;
        init
        {
            Require.ArgumentFlagsDefined(value);
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

#if NET6_0_OR_GREATER
    public static FileCreationOptions FromFileStreamOptions(FileStreamOptions options)
    {
        Require.ArgumentNotNull(options);

        var result = new FileCreationOptions
        {
            PreallocationSize = options.PreallocationSize,
        };

#if NET7_0_OR_GREATER
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            result = result with { UnixCreateMode = options.UnixCreateMode };
#endif

        return result;
    }

    public static implicit operator FileCreationOptions(FileStreamOptions options) =>
        FromFileStreamOptions(options);
#endif
}
