namespace Neme.Extensions.FileSystem.Resources;

public enum FileReferenceFlags
{
    None = 0,
    DeleteOnClose = 1 << 0,
    Transient = 1 << 1,
}
