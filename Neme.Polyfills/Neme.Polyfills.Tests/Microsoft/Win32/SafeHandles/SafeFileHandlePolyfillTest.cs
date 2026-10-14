using System.IO.Pipes;

namespace Microsoft.Win32.SafeHandles.Tests;

public sealed class SafeFileHandlePolyfillTest
{
    public sealed class IsAsync
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ReturnsSameValueAsFileStream(bool useAsyncOption)
        {
            var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.tmp");
            var options = FileOptions.DeleteOnClose;
            if (useAsyncOption)
                options |= FileOptions.Asynchronous;

            using (var stream = new FileStream(
                path,
                FileMode.CreateNew,
                FileAccess.ReadWrite,
                FileShare.ReadWrite | FileShare.Delete,
                1,
                options))
            {
                var result = SafeFileHandlePolyfill.get_IsAsync(stream.SafeFileHandle);

                Assert.Equal(stream.IsAsync, result);
            }
        }
    }

    public sealed class Type
    {
        [Fact]
        public void ForRegularFile_ReturnsRegularFile()
        {
            var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.tmp");
            using (var stream = new FileStream(
                path,
                FileMode.CreateNew,
                FileAccess.ReadWrite,
                FileShare.ReadWrite | FileShare.Delete,
                1,
                FileOptions.DeleteOnClose))
            {
                var result = SafeFileHandlePolyfill.get_Type(stream.SafeFileHandle);

                Assert.Equal(FileHandleType.RegularFile, result);
            }
        }

        [Fact]
        public void ForAnonymousPipe_ReturnsPipe()
        {
            using (var pipe = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.None))
            using (var handle = new SafeFileHandle(pipe.SafePipeHandle.DangerousGetHandle(), ownsHandle: false))
            {
                var result = SafeFileHandlePolyfill.get_Type(handle);

                Assert.Equal(FileHandleType.Pipe, result);
            }
        }
    }
}
