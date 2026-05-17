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

        public static void Clear(string path)
        {
            var exceptions = new List<Exception>();

            foreach (var filePath in Directory.EnumerateFiles(path))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            foreach (var directoryPath in Directory.EnumerateDirectories(path))
            {
                try
                {
                    Directory.Delete(directoryPath, recursive: true);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            switch (exceptions.Count)
            {
                case > 1:
                    throw new AggregateException(exceptions);
                case 1:
                    throw exceptions[0];
            }
        }

        public static void CopyContent(string sourcePath, string destPath, bool overwrite = false)
        {
            foreach (string dirPath in Directory.EnumerateDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destPath));

            foreach (string filePath in Directory.EnumerateFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(filePath, filePath.Replace(sourcePath, destPath), overwrite);
        }
    }
}
