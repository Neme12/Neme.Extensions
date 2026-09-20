using Neme.Extensions.IO;
using System.Text;

namespace Neme.Extensions.Tests.IO;

public sealed class TextWriterExtensionsTests
{
    public sealed class WriteBuffered
    {
        [Fact]
        public void InvalidArguments()
        {
            TextWriter? textWriter = null;

            Assert.Throws<ArgumentNullException>("textWriter", () => textWriter!.WriteBuffered("abc"));
        }

        [Fact]
        public void CancellationRequested()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            using var textWriter = new TestTextWriter();

            Assert.Throws<OperationCanceledException>(() => textWriter.WriteBuffered("abc", cancellationTokenSource.Token));
            Assert.Equal(0, textWriter.WriteCallCount);
            Assert.Empty(textWriter.WrittenSegments);
        }

        [Fact]
        public void EmptyTextDoesNotWrite()
        {
            using var textWriter = new TestTextWriter();

            textWriter.WriteBuffered(string.Empty);

            Assert.Equal(0, textWriter.WriteCallCount);
            Assert.Empty(textWriter.WrittenSegments);
        }

        [Fact]
        public void LargeTextIsWrittenInChunks()
        {
            string text = CreateSequence(5000);
            using var textWriter = new TestTextWriter();

            textWriter.WriteBuffered(text);

            Assert.Equal(2, textWriter.WriteCallCount);
            Assert.Collection(
                textWriter.WrittenSegments,
                first =>
                {
                    Assert.Equal(4096, first.Length);
                    Assert.Equal(GetSegment(text, 0, 4096), first);
                },
                second =>
                {
                    Assert.Equal(904, second.Length);
                    Assert.Equal(GetSegment(text, 4096, 904), second);
                });
        }

        [Fact]
        public void CancellationBetweenChunksStopsBeforeSecondWrite()
        {
            string text = CreateSequence(5000);
            using var cancellationTokenSource = new CancellationTokenSource();
            using var textWriter = new TestTextWriter(cancellationTokenSource, cancelAfterFirstWrite: true);

            Assert.Throws<OperationCanceledException>(() => textWriter.WriteBuffered(text, cancellationTokenSource.Token));
            Assert.Equal(1, textWriter.WriteCallCount);
            Assert.Single(textWriter.WrittenSegments);
            Assert.Equal(GetSegment(text, 0, 4096), textWriter.WrittenSegments[0]);
        }
    }

    public sealed class WriteBufferedAsync
    {
        [Fact]
        public void InvalidArguments()
        {
            TextWriter? textWriter = null;

            Assert.Throws<ArgumentNullException>("textWriter", () => Invoke(textWriter));

            static void Invoke(TextWriter? value)
                => _ = value!.WriteBufferedAsync("abc".AsMemory());
        }

        [Fact]
        public async Task CancellationRequested()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            using var textWriter = new TestTextWriter();

            Task task = textWriter.WriteBufferedAsync("abc".AsMemory(), cancellationTokenSource.Token);
            OperationCanceledException exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);

            Assert.Equal(cancellationTokenSource.Token, exception.CancellationToken);
            Assert.True(task.IsCanceled);
            Assert.Equal(0, textWriter.WriteAsyncCallCount);
            Assert.Empty(textWriter.WrittenSegments);
        }

        [Fact]
        public async Task EmptyTextDoesNotWrite()
        {
            using var textWriter = new TestTextWriter();

            await textWriter.WriteBufferedAsync(ReadOnlyMemory<char>.Empty);

            Assert.Equal(0, textWriter.WriteAsyncCallCount);
            Assert.Empty(textWriter.WrittenSegments);
        }

        [Fact]
        public async Task LargeTextIsWrittenInChunks()
        {
            string text = CreateSequence(5000);
            using var cancellationTokenSource = new CancellationTokenSource();
            using var textWriter = new TestTextWriter();

            await textWriter.WriteBufferedAsync(text.AsMemory(), cancellationTokenSource.Token);

            Assert.Equal(2, textWriter.WriteAsyncCallCount);
            Assert.Collection(
                textWriter.WrittenSegments,
                first =>
                {
                    Assert.Equal(4096, first.Length);
                    Assert.Equal(GetSegment(text, 0, 4096), first);
                },
                second =>
                {
                    Assert.Equal(904, second.Length);
                    Assert.Equal(GetSegment(text, 4096, 904), second);
                });
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            Assert.All(textWriter.WriteAsyncCancellationTokens, token => Assert.Equal(cancellationTokenSource.Token, token));
#endif
        }

        [Fact]
        public async Task CancellationBetweenChunksStopsBeforeSecondWrite()
        {
            string text = CreateSequence(5000);
            using var cancellationTokenSource = new CancellationTokenSource();
            using var textWriter = new TestTextWriter(cancellationTokenSource, cancelAfterFirstWrite: true);

            Task task = textWriter.WriteBufferedAsync(text.AsMemory(), cancellationTokenSource.Token);
            OperationCanceledException exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);

            Assert.Equal(cancellationTokenSource.Token, exception.CancellationToken);
            Assert.Equal(1, textWriter.WriteAsyncCallCount);
            Assert.Single(textWriter.WrittenSegments);
            Assert.Equal(GetSegment(text, 0, 4096), textWriter.WrittenSegments[0]);
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            Assert.All(textWriter.WriteAsyncCancellationTokens, token => Assert.Equal(cancellationTokenSource.Token, token));
#endif
        }
    }

    private static string CreateSequence(int length)
        => new(Enumerable.Range(0, length).Select(index => (char)('A' + (index % 26))).ToArray());

    private static string GetSegment(string text, int offset, int length)
        => text.Substring(offset, length);

    private sealed class TestTextWriter(CancellationTokenSource? cancellationTokenSource = null, bool cancelAfterFirstWrite = false) : TextWriter
    {
        private readonly CancellationTokenSource? cancellationTokenSource = cancellationTokenSource;
        private readonly bool cancelAfterFirstWrite = cancelAfterFirstWrite;

        public int WriteCallCount { get; private set; }

        public int WriteAsyncCallCount { get; private set; }

        public List<string> WrittenSegments { get; } = [];

        public List<CancellationToken> WriteAsyncCancellationTokens { get; } = [];

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char[] buffer, int index, int count)
        {
            WriteCallCount++;
            WriteCore(buffer, index, count);
        }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            WriteAsyncCallCount++;
            WriteAsyncCancellationTokens.Add(cancellationToken);
            WriteCore(buffer.Span);
            return Task.CompletedTask;
        }
#else
        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            WriteAsyncCallCount++;
            WriteCore(buffer, index, count);
            return Task.CompletedTask;
        }
#endif

        private void WriteCore(ReadOnlySpan<char> buffer)
        {
            WrittenSegments.Add(new string(buffer.ToArray()));

            if (cancelAfterFirstWrite && WrittenSegments.Count == 1)
                cancellationTokenSource!.Cancel();
        }

        private void WriteCore(char[] buffer, int index, int count)
            => WriteCore(buffer.AsSpan(index, count));
    }
}
