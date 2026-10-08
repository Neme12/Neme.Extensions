using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.SafeHandles;

namespace Neme.Extensions.FileSystem.Resources;

public static class SafeFileHandleResourceExtensions
{
    extension(SafeFileHandle handle)
    {
        public IFileResource ToFileResource()
        {
            Require.ArgumentNotNull(handle);
            Require.Argument(handle, handle.IsOpen);

            return new SafeFileHandleResource(handle);
        }
    }
}
