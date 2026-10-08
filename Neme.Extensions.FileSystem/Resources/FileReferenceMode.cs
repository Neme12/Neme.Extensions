using Neme.Extensions.IO;

namespace Neme.Extensions.FileSystem.Resources;

public enum FileReferenceMode
{
    None = FileModeExtensions.None,
    CreateNew = FileMode.CreateNew,
    Create = FileMode.Create,
    Open = FileMode.Open,
    OpenOrCreate = FileMode.OpenOrCreate,
}
