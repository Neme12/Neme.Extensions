using Neme.Extensions.IO;

namespace Neme.Extensions.Tests.IO;

public sealed class TextReaderExtensionsTests
{
    public sealed class ReadToEnd
    {
        [Fact]
        public void InvalidArguments()
        {
            TextReader? textReader = null;

            Assert.Throws<ArgumentNullException>("textReader", () => TextReaderExtensions.ReadToEnd(textReader!));
        }

        [Fact]
        public void CancellationRequested()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            using var textReader = new TestTextReader("abc");

            Assert.Throws<OperationCanceledException>(() => textReader.ReadToEnd(cancellationTokenSource.Token));
            Assert.Equal(0, textReader.ReadCallCount);
        }

        [Fact]
        public void ReturnsAllText()
        {
            const string expected = "Hello, world!";
            using var textReader = new TestTextReader(expected, readChunkSizes: [2, 5, 1, 5]);

            string actual = textReader.ReadToEnd();

            Assert.Equal(expected, actual);
            Assert.Equal(5, textReader.ReadCallCount);
        }

        [Fact]
        public void CancellationBetweenReadsStopsBeforeSecondAppend()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            using var textReader = new TestTextReader("abcdef", readChunkSizes: [3, 3], cancellationTokenSource: cancellationTokenSource, cancelAfterFirstRead: true);

            Assert.Throws<OperationCanceledException>(() => textReader.ReadToEnd(cancellationTokenSource.Token));
            Assert.Equal(1, textReader.ReadCallCount);
        }
    }

    private sealed class TestTextReader(string content, int[]? readChunkSizes = null, CancellationTokenSource? cancellationTokenSource = null, bool cancelAfterFirstRead = false) : TextReader
    {
        private readonly string _content = content;
        private readonly int[]? _readChunkSizes = readChunkSizes;
        private readonly CancellationTokenSource? _cancellationTokenSource = cancellationTokenSource;
        private readonly bool _cancelAfterFirstRead = cancelAfterFirstRead;
        private int _position;

        public int ReadCallCount { get; private set; }

        public override int Read(char[] buffer, int index, int count)
        {
            ReadCallCount++;

            if (_cancelAfterFirstRead && ReadCallCount == 1)
                _cancellationTokenSource!.Cancel();

            if (_position >= _content.Length)
                return 0;

            int requestedCount = count;
            if (_readChunkSizes is { Length: > 0 } && ReadCallCount - 1 < _readChunkSizes.Length)
                requestedCount = Math.Min(requestedCount, _readChunkSizes[ReadCallCount - 1]);

            int length = Math.Min(requestedCount, _content.Length - _position);
            _content.CopyTo(_position, buffer, index, length);
            _position += length;
            return length;
        }
    }
}
