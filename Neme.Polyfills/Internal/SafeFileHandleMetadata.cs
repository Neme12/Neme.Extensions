namespace Neme.Polyfills.Internal;

internal sealed record class SafeFileHandleMetadata
{
    public FileOptions FileOptions { get; set; } = (FileOptions)(-1);

    public FileHandleType FileType { get; set; } = (FileHandleType)(-1);
}
