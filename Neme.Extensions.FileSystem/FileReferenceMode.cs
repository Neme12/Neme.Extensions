namespace Neme.Extensions.FileSystem;

public enum FileReferenceMode
{
    None = FileModeExtensions.None,
    CreateNew = FileMode.CreateNew,
    Create = FileMode.Create,
    Open = FileMode.Open,
    OpenOrCreate = FileMode.OpenOrCreate,
}
