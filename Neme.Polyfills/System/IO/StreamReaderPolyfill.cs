namespace System.IO;

public static class StreamReaderPolyfill
{
    extension(StreamReader streamReader)
    {
        public ValueTask<string?> ReadLineAsync(CancellationToken cancellationToken)
        {
#if NET7_0_OR_GREATER
            return streamReader.ReadLineAsync(cancellationToken);
#else
            cancellationToken.ThrowIfCancellationRequested();
            return new(streamReader.ReadLineAsync());
#endif
        }

        public Task<string> ReadToEndAsync(CancellationToken cancellationToken)
        {
#if NET7_0_OR_GREATER
            return streamReader.ReadToEndAsync(cancellationToken);
#else
            cancellationToken.ThrowIfCancellationRequested();
            return streamReader.ReadToEndAsync();
#endif
        }
    }
}
