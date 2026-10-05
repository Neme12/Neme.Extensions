namespace Neme.Extensions.FileSystem;

public readonly record struct FileReferenceRequest
{
    public FileReferenceRequest(
        FileReferenceMode mode,
        FileReferenceFlags flags,
        FileCreationOptions creationOptions)
    {
        Mode = mode;
        Flags = flags;
        CreationOptions = creationOptions;
    }

    public static FileReferenceRequest CreateNew(
        FileReferenceFlags flags = FileReferenceFlags.None,
        FileCreationOptions creationOptions = default)
    {
        return new(FileReferenceMode.CreateNew, flags, creationOptions);
    }

    public static FileReferenceRequest Create(
        FileReferenceFlags flags = FileReferenceFlags.None,
        FileCreationOptions creationOptions = default)
    {
        return new(FileReferenceMode.Create, flags, creationOptions);
    }

    public static FileReferenceRequest Open(
        FileReferenceFlags flags = FileReferenceFlags.None,
        FileCreationOptions creationOptions = default)
    {
        return new(FileReferenceMode.Open, flags, creationOptions);
    }

    public static FileReferenceRequest OpenOrCreate(
        FileReferenceFlags flags = FileReferenceFlags.None,
        FileCreationOptions creationOptions = default)
    {
        return new(FileReferenceMode.OpenOrCreate, flags, creationOptions);
    }

    public FileReferenceMode Mode { get; init; }

    public FileReferenceFlags Flags { get; init; }

    public FileCreationOptions CreationOptions { get; init; }
}
