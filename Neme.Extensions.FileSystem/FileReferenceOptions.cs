namespace Neme.Extensions.FileSystem;

public readonly record struct FileReferenceOptions
{
    public FileReferenceOptions(
        FileReferenceMode mode,
        FileReferenceFlags flags,
        FileCreationOptions creationOptions)
    {
        Mode = mode;
        Flags = flags;
        CreationOptions = creationOptions;
    }

    public static FileReferenceOptions CreateNew(
        FileReferenceFlags flags = FileReferenceFlags.None,
        FileCreationOptions creationOptions = default)
    {
        return new(FileReferenceMode.CreateNew, flags, creationOptions);
    }

    public static FileReferenceOptions Create(
        FileReferenceFlags flags = FileReferenceFlags.None,
        FileCreationOptions creationOptions = default)
    {
        return new(FileReferenceMode.Create, flags, creationOptions);
    }

    public static FileReferenceOptions Open(
        FileReferenceFlags flags = FileReferenceFlags.None,
        FileCreationOptions creationOptions = default)
    {
        return new(FileReferenceMode.Open, flags, creationOptions);
    }

    public static FileReferenceOptions OpenOrCreate(
        FileReferenceFlags flags = FileReferenceFlags.None,
        FileCreationOptions creationOptions = default)
    {
        return new(FileReferenceMode.OpenOrCreate, flags, creationOptions);
    }

    public FileReferenceMode Mode { get; init; }

    public FileReferenceFlags Flags { get; init; }

    public FileCreationOptions CreationOptions { get; init; }
}
