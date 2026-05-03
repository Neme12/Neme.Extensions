using System.Buffers;

namespace Neme.Extensions.Buffers;

public static partial class ArrayPoolExtensions
{
    public struct Lease<T> : IDisposable
    {
        private ArrayPool<T> _arrayPool;
        private T[] _array;
        private readonly bool _clearArray;

        public Lease(ArrayPool<T> arrayPool, int minimumLength, bool clearArray)
        {
            _arrayPool = arrayPool;
            _array = arrayPool.Rent(minimumLength);
            _clearArray = clearArray;
        }

        public readonly T[] Array
        {
            get
            {
                ObjectDisposedException.ThrowIf(_arrayPool is null, this);
                return _array;
            }
        }

        public readonly Span<T> Buffer
        {
            get
            {
                ObjectDisposedException.ThrowIf(_arrayPool is null, this);
                return _array.AsSpan();
            }
        }

        public readonly int Length
        {
            get
            {
                ObjectDisposedException.ThrowIf(_arrayPool is null, this);
                return _array.Length;
            }
        }

        public void RentMore(int minimumLength = -1)
        {
            ObjectDisposedException.ThrowIf(_arrayPool is null, this);

            if (minimumLength == -1)
                minimumLength = _array.Length * 2;

            if (minimumLength <= _array.Length)
                return;

            _arrayPool.Return(_array, _clearArray);
            _array = _arrayPool.Rent(minimumLength);
        }

        public void Dispose()
        {
            if (_arrayPool is not null)
            {
                _arrayPool.Return(_array, _clearArray);
                _arrayPool = null!;
                _array = null!;
            }
        }
    }
}
