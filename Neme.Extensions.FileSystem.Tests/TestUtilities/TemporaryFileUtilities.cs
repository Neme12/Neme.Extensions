using Neme.Extensions.IO;

namespace Neme.Extensions.FileSystem.Tests.TestUtilities;

internal static class TemporaryFileUtilities
{
    public static TemporaryFileScope CreateTemporaryFileScope(string filePath)
    {
        return new TemporaryFileScope(filePath);
    }

    public struct TemporaryFileScope : IDisposable
    {
        private string _filePath;

        public TemporaryFileScope(string filePath)
        {
            _filePath = filePath;
        }

        public void Dispose()
        {
            if (_filePath is not null)
            {
                File.DeleteIfExists(_filePath);
                _filePath = null!;
            }
        }
    }
}
