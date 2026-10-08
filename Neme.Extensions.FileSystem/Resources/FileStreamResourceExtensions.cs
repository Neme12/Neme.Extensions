using Neme.Extensions.Contracts;
using Neme.Extensions.FileSystem.Internal;
using Neme.Extensions.SafeHandles;

namespace Neme.Extensions.FileSystem.Resources;

public static class FileStreamResourceExtensions
{
    extension(FileStream fileStream)
    {
        public IFileResource ToFileResource()
        {
            Require.ArgumentNotNull(fileStream);
            Require.Argument(fileStream, fileStream.SafeFileHandle.IsOpen);

            return new FileStreamResource(fileStream);
        }
    }
}
