using Neme.Extensions.Contracts;
using System.Diagnostics;

namespace Neme.Extensions.FileSystem;

internal static class FileReferenceModeExtensions
{
    extension(FileReferenceMode mode)
    {
        public FileMode ToFileMode()
        {
            Debug.AssertInRange(mode, FileReferenceMode.CreateNew, FileReferenceMode.OpenOrCreate);

            // The values of FileReferenceMode map directly to FileMode.
            return (FileMode)mode;
        }
    }
}
