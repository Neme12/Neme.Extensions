using Microsoft.Win32.SafeHandles;
using Neme.Extensions.IO;
using Neme.Extensions.Tests.Utilities;
using System.IO.Pipes;

namespace Neme.Extensions.Tests.IO;

public sealed class SafeFileHandleExtensionsTests
{
    public sealed class CanSeek
    {
        [Fact]
        public void CanSeek_RegularFileHandle_ReturnsTrue()
        {
            var path = Path.Combine(Path.GetTempPath(), $"{nameof(SafeFileHandleExtensionsTests)}_{Guid.NewGuid():N}.tmp");

            try
            {
                using var fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
                SafeFileHandle handle = fileStream.SafeFileHandle;

                var canSeek = handle.CanSeek;

                Assert.True(canSeek);
            }
            finally
            {
                File.Delete(path);
            }
        }

#if NETCOREAPP
        [PlatformOnlyFact(Platform.Windows)]
        public void CanSeek_NamedPipeHandle_ReturnsFalse()
        {
            var pipeName = $"{nameof(SafeFileHandleExtensionsTests)}_{Guid.NewGuid():N}";
            var pipePath = $@"\\.\pipe\{pipeName}";

            using var serverStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            var waitForConnectionTask = Task.Run(serverStream.WaitForConnection);
            using var clientStream = new FileStream(pipePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            waitForConnectionTask.GetAwaiter().GetResult();
            SafeFileHandle handle = clientStream.SafeFileHandle;

            var canSeek = handle.CanSeek;

            Assert.False(canSeek);
        }
#endif
    }
}
