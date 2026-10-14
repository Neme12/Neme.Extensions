using Neme.Extensions.IO;

namespace Neme.Extensions.Tests.IO;

public sealed class StreamExtensionsTests
{
    public sealed class ReadToEnd
    {
        [Fact]
        public void InvalidArguments()
        {
            Stream? stream = null;

            Assert.Throws<ArgumentNullException>("stream", () => stream!.ReadToEnd());
        }

        [Fact]
        public void CancellationRequested()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            using var stream = new TestStream([1, 2, 3], canSeek: false);

            Assert.Throws<OperationCanceledException>(() => stream.ReadToEnd(cancellationTokenSource.Token));
            Assert.Equal(0, stream.ReadCallCount);
        }

        [Fact]
        public void SeekableStreamLongerThanMaximumLength()
        {
            using var stream = new TestStream([], canSeek: true, reportedLength: (long)Array.MaxLength + 1);

            IOException exception = Assert.Throws<IOException>(() => stream.ReadToEnd());

            Assert.Equal("The file is too long. This operation is currently limited to supporting files less than 2 gigabytes in size.", exception.Message);
            Assert.Equal(0, stream.ReadCallCount);
        }

        [Fact]
        public void NonSeekableStreamReturnsAllBytes()
        {
            byte[] expected = [1, 2, 3, 4, 5];
            using var stream = new TestStream(expected, canSeek: false, readChunkSizes: [2, 2, 1]);

            byte[] actual = stream.ReadToEnd();

            Assert.Equal(expected, actual);
            Assert.Equal(4, stream.ReadCallCount);
        }

#if !DEBUG
        [Fact]
        public void SeekableStreamWithKnownLengthReturnsAllBytes()
        {
            byte[] expected = [9, 8, 7, 6, 5];
            using var stream = new TestStream(expected, canSeek: true, reportedLength: expected.Length, readChunkSizes: [2, 3]);

            byte[] actual = stream.ReadToEnd();

            Assert.Equal(expected, actual);
            Assert.Equal(2, stream.ReadCallCount);
        }

        [Fact]
        public void SeekableStreamWithTruncatedDataThrowsEndOfStreamException()
        {
            using var stream = new TestStream([1, 2, 3], canSeek: true, reportedLength: 4, readChunkSizes: [2, 1]);

            EndOfStreamException exception = Assert.Throws<EndOfStreamException>(() => stream.ReadToEnd());

            Assert.Equal("Unable to read beyond the end of the stream.", exception.Message);
            Assert.Equal(3, stream.ReadCallCount);
        }
#endif
    }

    public sealed class ReadToEndAsync
    {
        [Fact]
        public void InvalidArguments()
        {
            Stream? stream = null;

            Assert.Throws<ArgumentNullException>("stream", () => Invoke(stream));

            static void Invoke(Stream? value)
                => _ = value!.ReadToEndAsync();
        }

        [Fact]
        public async Task CancellationRequested()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            using var stream = new TestStream([1, 2, 3], canSeek: false);

            Task<byte[]> task = stream.ReadToEndAsync(cancellationTokenSource.Token);
            OperationCanceledException exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);

            Assert.Equal(cancellationTokenSource.Token, exception.CancellationToken);
            Assert.True(task.IsCanceled);
            Assert.Equal(0, stream.ReadAsyncCallCount);
        }

        [Fact]
        public async Task SeekableStreamLongerThanMaximumLength()
        {
            using var stream = new TestStream([], canSeek: true, reportedLength: (long)Array.MaxLength + 1);

            Task<byte[]> task = stream.ReadToEndAsync();
            IOException exception = await Assert.ThrowsAsync<IOException>(async () => await task);

            Assert.Equal("The file is too long. This operation is currently limited to supporting files less than 2 gigabytes in size.", exception.Message);
            Assert.True(task.IsFaulted);
            Assert.Equal(0, stream.ReadAsyncCallCount);
        }

        [Fact]
        public async Task NonSeekableStreamReturnsAllBytes()
        {
            byte[] expected = [1, 2, 3, 4, 5];
            using var cancellationTokenSource = new CancellationTokenSource();
            using var stream = new TestStream(expected, canSeek: false, readChunkSizes: [2, 2, 1]);

            byte[] actual = await stream.ReadToEndAsync(cancellationTokenSource.Token);

            Assert.Equal(expected, actual);
            Assert.Equal(4, stream.ReadAsyncCallCount);
            Assert.All(stream.ReadAsyncCancellationTokens, token => Assert.Equal(cancellationTokenSource.Token, token));
        }

//#if !DEBUG
        [Fact]
        public async Task SeekableStreamWithKnownLengthReturnsAllBytes()
        {
            byte[] expected = [9, 8, 7, 6, 5];
            using var cancellationTokenSource = new CancellationTokenSource();
            using var stream = new TestStream(expected, canSeek: true, reportedLength: expected.Length, readChunkSizes: [2, 3]);

            byte[] actual = await stream.ReadToEndAsync(cancellationTokenSource.Token);

            Assert.Equal(expected, actual);
            Assert.Equal(2, stream.ReadAsyncCallCount);
            Assert.All(stream.ReadAsyncCancellationTokens, token => Assert.Equal(cancellationTokenSource.Token, token));
        }

        [Fact]
        public async Task SeekableStreamWithTruncatedDataThrowsEndOfStreamException()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            using var stream = new TestStream([1, 2, 3], canSeek: true, reportedLength: 4, readChunkSizes: [2, 1]);

            Task<byte[]> task = stream.ReadToEndAsync(cancellationTokenSource.Token);
            EndOfStreamException exception = await Assert.ThrowsAsync<EndOfStreamException>(async () => await task);

            Assert.Equal("Unable to read beyond the end of the stream.", exception.Message);
            Assert.Equal(3, stream.ReadAsyncCallCount);
            Assert.All(stream.ReadAsyncCancellationTokens, token => Assert.Equal(cancellationTokenSource.Token, token));
        }
//#endif
    }

    public sealed class WriteBuffered
    {
        [Fact]
        public void InvalidArguments()
        {
            Stream? stream = null;
            byte[] buffer = [1, 2, 3];

            Assert.Throws<ArgumentNullException>("stream", () => stream!.WriteBuffered(buffer));
        }

        [Fact]
        public void CancellationRequested()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            using var stream = new TestStream();
            byte[] buffer = [1, 2, 3];

            Assert.Throws<OperationCanceledException>(() => stream.WriteBuffered(buffer, cancellationTokenSource.Token));
            Assert.Equal(0, stream.WriteCallCount);
            Assert.Empty(stream.WrittenSegments);
        }

        [Fact]
        public void EmptyBufferDoesNotWrite()
        {
            using var stream = new TestStream();

            stream.WriteBuffered([]);

            Assert.Equal(0, stream.WriteCallCount);
            Assert.Empty(stream.WrittenSegments);
        }

        [Fact]
        public void LargeBufferIsWrittenInChunks()
        {
            byte[] buffer = CreateSequence(5000);
            using var stream = new TestStream();

            stream.WriteBuffered(buffer);

            Assert.Equal(2, stream.WriteCallCount);
            Assert.Collection(
                stream.WrittenSegments,
                first =>
                {
                    Assert.Equal(4096, first.Length);
                    Assert.Equal(GetSegment(buffer, 0, 4096), first);
                },
                second =>
                {
                    Assert.Equal(904, second.Length);
                    Assert.Equal(GetSegment(buffer, 4096, 904), second);
                });
        }

        [Fact]
        public void CancellationBetweenChunksStopsBeforeSecondWrite()
        {
            byte[] buffer = CreateSequence(5000);
            using var cancellationTokenSource = new CancellationTokenSource();
            using var stream = new TestStream(cancellationTokenSource: cancellationTokenSource, cancelAfterFirstWrite: true);

            Assert.Throws<OperationCanceledException>(() => stream.WriteBuffered(buffer, cancellationTokenSource.Token));
            Assert.Equal(1, stream.WriteCallCount);
            Assert.Single(stream.WrittenSegments);
            Assert.Equal(GetSegment(buffer, 0, 4096), stream.WrittenSegments[0]);
        }
    }

    public sealed class WriteBufferedAsync
    {
        [Fact]
        public void InvalidArguments()
        {
            Stream? stream = null;
            byte[] buffer = [1, 2, 3];

            Assert.Throws<ArgumentNullException>("stream", () => Invoke(stream, buffer));

            static void Invoke(Stream? value, byte[] bytes)
                => _ = value!.WriteBufferedAsync(bytes);
        }

        [Fact]
        public async Task CancellationRequested()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            using var stream = new TestStream();
            byte[] buffer = [1, 2, 3];

            Task task = stream.WriteBufferedAsync(buffer, cancellationTokenSource.Token);
            OperationCanceledException exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);

            Assert.Equal(cancellationTokenSource.Token, exception.CancellationToken);
            Assert.True(task.IsCanceled);
            Assert.Equal(0, stream.WriteAsyncCallCount);
            Assert.Empty(stream.WrittenSegments);
        }

        [Fact]
        public async Task EmptyBufferDoesNotWrite()
        {
            using var stream = new TestStream();

            await stream.WriteBufferedAsync(ReadOnlyMemory<byte>.Empty);

            Assert.Equal(0, stream.WriteAsyncCallCount);
            Assert.Empty(stream.WrittenSegments);
        }

        [Fact]
        public async Task LargeBufferIsWrittenInChunks()
        {
            byte[] buffer = CreateSequence(5000);
            using var cancellationTokenSource = new CancellationTokenSource();
            using var stream = new TestStream();

            await stream.WriteBufferedAsync(buffer, cancellationTokenSource.Token);

            Assert.Equal(2, stream.WriteAsyncCallCount);
            Assert.Collection(
                stream.WrittenSegments,
                first =>
                {
                    Assert.Equal(4096, first.Length);
                    Assert.Equal(GetSegment(buffer, 0, 4096), first);
                },
                second =>
                {
                    Assert.Equal(904, second.Length);
                    Assert.Equal(GetSegment(buffer, 4096, 904), second);
                });
            Assert.All(stream.WriteAsyncCancellationTokens, token => Assert.Equal(cancellationTokenSource.Token, token));
        }

        [Fact]
        public async Task CancellationBetweenChunksStopsBeforeSecondWrite()
        {
            byte[] buffer = CreateSequence(5000);
            using var cancellationTokenSource = new CancellationTokenSource();
            using var stream = new TestStream(cancellationTokenSource: cancellationTokenSource, cancelAfterFirstWrite: true);

            Task task = stream.WriteBufferedAsync(buffer, cancellationTokenSource.Token);
            OperationCanceledException exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);

            Assert.Equal(cancellationTokenSource.Token, exception.CancellationToken);
            Assert.Equal(1, stream.WriteAsyncCallCount);
            Assert.Single(stream.WrittenSegments);
            Assert.Equal(GetSegment(buffer, 0, 4096), stream.WrittenSegments[0]);
            Assert.All(stream.WriteAsyncCancellationTokens, token => Assert.Equal(cancellationTokenSource.Token, token));
        }
    }

    private static byte[] CreateSequence(int length)
        => Enumerable.Range(0, length).Select(value => (byte)(value % 251)).ToArray();

    private static byte[] GetSegment(byte[] buffer, int offset, int length)
    {
        byte[] segment = new byte[length];
        Array.Copy(buffer, offset, segment, 0, length);
        return segment;
    }

    private sealed class TestStream : Stream
    {
        private readonly byte[] readData;
        private readonly bool canSeek;
        private readonly long? reportedLength;
        private readonly int[] readChunkSizes;
        private readonly CancellationTokenSource? cancellationTokenSource;
        private readonly bool cancelAfterFirstWrite;
        private int readChunkIndex;
        private int readPosition;

        public TestStream(
            byte[]? readData = null,
            bool canSeek = false,
            long? reportedLength = null,
            int[]? readChunkSizes = null,
            CancellationTokenSource? cancellationTokenSource = null,
            bool cancelAfterFirstWrite = false)
        {
            this.readData = readData ?? [];
            this.canSeek = canSeek;
            this.reportedLength = reportedLength;
            this.readChunkSizes = readChunkSizes ?? [];
            this.cancellationTokenSource = cancellationTokenSource;
            this.cancelAfterFirstWrite = cancelAfterFirstWrite;
            WrittenSegments = [];
            ReadAsyncCancellationTokens = [];
            WriteAsyncCancellationTokens = [];
        }

        public int ReadCallCount { get; private set; }

        public int ReadAsyncCallCount { get; private set; }

        public int WriteCallCount { get; private set; }

        public int WriteAsyncCallCount { get; private set; }

        public List<byte[]> WrittenSegments { get; }

        public List<CancellationToken> ReadAsyncCancellationTokens { get; }

        public List<CancellationToken> WriteAsyncCancellationTokens { get; }

        public override bool CanRead => true;

        public override bool CanSeek => canSeek;

        public override bool CanWrite => true;

        public override long Length => reportedLength ?? readData.Length;

        public override long Position
        {
            get => readPosition;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ReadCallCount++;
            return ReadCore(buffer, offset, count);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ReadAsyncCallCount++;
            ReadAsyncCancellationTokens.Add(cancellationToken);
            return Task.FromResult(ReadCore(buffer, offset, count));
        }

        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException();

        public override void SetLength(long value)
            => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteCallCount++;
            WriteCore(buffer, offset, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            WriteAsyncCallCount++;
            WriteAsyncCancellationTokens.Add(cancellationToken);
            WriteCore(buffer, offset, count);
            return Task.CompletedTask;
        }

        private int ReadCore(byte[] buffer, int offset, int count)
        {
            int remaining = readData.Length - readPosition;
            if (remaining == 0)
                return 0;

            int length = Math.Min(remaining, count);
            if (readChunkIndex < readChunkSizes.Length)
                length = Math.Min(length, readChunkSizes[readChunkIndex++]);

            Array.Copy(readData, readPosition, buffer, offset, length);
            readPosition += length;
            return length;
        }

        private void WriteCore(byte[] buffer, int offset, int count)
        {
            byte[] copy = new byte[count];
            Array.Copy(buffer, offset, copy, 0, count);
            WrittenSegments.Add(copy);

            if (cancelAfterFirstWrite && WrittenSegments.Count == 1)
                cancellationTokenSource!.Cancel();
        }
    }
}
