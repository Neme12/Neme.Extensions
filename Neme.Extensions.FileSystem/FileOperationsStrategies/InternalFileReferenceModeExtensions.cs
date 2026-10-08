using Neme.Extensions.Contracts;
using Neme.Extensions.FileSystem.Resources;
using System.Diagnostics;

namespace Neme.Extensions.FileSystem.FileOperationsStrategies;

internal static class InternalFileReferenceModeExtensions
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
