using Neme.Extensions.Contracts;

namespace Neme.Extensions.FileSystem.Resources;

public readonly record struct FileReferenceRequest
{
    public FileReferenceRequest(
        FileReferenceMode mode,
        FileReferenceOptions referenceOptions = default,
        FileCreationOptions creationOptions = default)
    {
        Require.ArgumentDefined(mode);

        Mode = mode;
        ReferenceOptions = referenceOptions;
        CreationOptions = creationOptions;
    }

    public void Deconstruct(
        out FileReferenceMode mode)
    {
        mode = Mode;
    }

    public void Deconstruct(
        out FileReferenceMode mode,
        out FileReferenceOptions referenceOptions,
        out FileCreationOptions creationOptions)
    {
        mode = Mode;
        referenceOptions = ReferenceOptions;
        creationOptions = CreationOptions;
    }

    public static FileReferenceRequest CreateNew(
        FileReferenceOptions referenceOptions = default,
        FileCreationOptions creationOptions = default)
    {
        return new(FileReferenceMode.CreateNew, referenceOptions, creationOptions);
    }

    public static FileReferenceRequest Create(
        FileReferenceOptions referenceOptions = default,
        FileCreationOptions creationOptions = default)
    {
        return new(FileReferenceMode.Create, referenceOptions, creationOptions);
    }

    public static FileReferenceRequest Open(
        FileReferenceOptions referenceOptions = default,
        FileCreationOptions creationOptions = default)
    {
        return new(FileReferenceMode.Open, referenceOptions, creationOptions);
    }

    public static FileReferenceRequest OpenOrCreate(
        FileReferenceOptions referenceOptions = default,
        FileCreationOptions creationOptions = default)
    {
        return new(FileReferenceMode.OpenOrCreate, referenceOptions, creationOptions);
    }

    public FileReferenceMode Mode
    {
        get;
        init
        {
            Require.ArgumentDefined(value);
            field = value;
        }
    }

    public FileReferenceOptions ReferenceOptions { get; init; }

    public FileCreationOptions CreationOptions { get; init; }
}
