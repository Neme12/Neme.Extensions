using System.IO.Pipelines;

namespace Neme.Extensions.IO.Pipelines;

public static class PipeWriterExtensions
{
    public static Scope CreateScope(Stream stream, StreamPipeWriterOptions? writerOptions = null)
    {
        var pipeWriter = PipeWriter.Create(stream, writerOptions);
        return new Scope(pipeWriter);
    }

    public struct Scope : IDisposable, IAsyncDisposable
    {
        private PipeWriter _pipeWriter;

        internal Scope(PipeWriter pipeWriter)
        {
            _pipeWriter = pipeWriter;
        }

        public readonly PipeWriter PipeWriter =>
            _pipeWriter;

        public void Dispose()
        {
            if (_pipeWriter is not null)
            {
                _pipeWriter.Complete();
                _pipeWriter = null!;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_pipeWriter is not null)
            {
                await _pipeWriter.CompleteAsync();
                _pipeWriter = null!;
            }
        }
    }
}
