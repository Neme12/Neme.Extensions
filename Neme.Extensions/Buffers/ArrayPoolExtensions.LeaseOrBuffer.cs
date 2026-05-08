using Roslyn.Utilities;
using System.Buffers;
using System.Diagnostics;

namespace Neme.Extensions.Buffers;

#pragma warning disable CA2265 // Do not compare Span<T> to 'null' or 'default'

public static partial class ArrayPoolExtensions
{
    [NonCopyable]
    public ref struct LeaseOrBuffer<T> : IDisposable
    {
        private ArrayPool<T>? _arrayPool;
        private T[]? _array;
        private Span<T> _buffer;
        private readonly bool _clearBuffer;

        public LeaseOrBuffer(ArrayPool<T> arrayPool, int minimumLength, bool clearArray = false)
        {
            _arrayPool = arrayPool;
            _array = arrayPool.Rent(minimumLength);
            _buffer = default;
            _clearBuffer = clearArray;
        }

        public LeaseOrBuffer(Span<T> buffer, bool clearBuffer = false)
        {
            _arrayPool = null;
            _array = null;
            _buffer = buffer;
            _clearBuffer = clearBuffer;
        }

        public readonly Span<T> Buffer
        {
            get
            {
                ObjectDisposedException.ThrowIf(_array is null && _buffer == default, typeof(LeaseOrBuffer<T>));

                Debug.Assert(_array is null == _arrayPool is null);
                Debug.Assert(_array is null != (_buffer == default));

                return _array is not null
                    ? _array.AsSpan()
                    : _buffer;
            }
        }

        public readonly int Length
        {
            get
            {
                ObjectDisposedException.ThrowIf(_array is null && _buffer == default, typeof(LeaseOrBuffer<T>));

                Debug.Assert(_array is null == _arrayPool is null);
                Debug.Assert(_array is null != (_buffer == default));

                return _array is not null
                    ? _array.Length
                    : _buffer.Length;
            }
        }

        public void RentMore(int minimumLength = -1)
        {
            ObjectDisposedException.ThrowIf(_array is null && _buffer == default, typeof(LeaseOrBuffer<T>));

            Debug.Assert(_array is null == _arrayPool is null);
            Debug.Assert(_array is null != (_buffer == default));

            var length = _array is not null ? _array.Length : _buffer.Length;

            if (minimumLength == -1)
                minimumLength = length * 2;

            if (minimumLength <= length)
                return;

            if (_array is not null)
                _arrayPool!.Return(_array, _clearBuffer);
            else if (_clearBuffer)
                _buffer.Clear();

            _array = _arrayPool!.Rent(minimumLength);
            _buffer = default;
        }

        public void Dispose()
        {
            if (_array is not null || _buffer != default)
            {
                Debug.Assert(_array is null == _arrayPool is null);
                Debug.Assert(_array is null != (_buffer == default));

                if (_array is not null)
                    _arrayPool!.Return(_array, _clearBuffer);
                else if (_clearBuffer)
                    _buffer.Clear();

                _arrayPool = null;
                _array = null;
                _buffer = default;
            }
        }
    }
}
