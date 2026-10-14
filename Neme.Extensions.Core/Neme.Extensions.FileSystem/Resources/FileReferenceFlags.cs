namespace Neme.Extensions.FileSystem.Resources;

[Flags]
public enum FileReferenceFlags
{
    None = 0,
    DeleteOnClose = 1 << 0,
    Transient = 1 << 1,
}
