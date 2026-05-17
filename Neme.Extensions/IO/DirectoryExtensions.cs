using Neme.Extensions.Win32.InteropServices;
using Windows.Win32.Foundation;

namespace Neme.Extensions.IO;

public static class DirectoryExtensions
{
    private static readonly HResult s_directoryNotEmpty =
        HResult.FromWin32((ushort)WIN32_ERROR.ERROR_DIR_NOT_EMPTY);

    extension(Directory)
    {
        public static void DeleteIfEmpty(string path)
        {
            try
            {
                Directory.Delete(path);
            }
            catch (IOException e) when (e.HResult == s_directoryNotEmpty)
            {
            }
        }
    }
}
