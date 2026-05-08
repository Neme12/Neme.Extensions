using Roslyn.Utilities;
using System.Buffers;

namespace Neme.Extensions.Buffers;

public static partial class ArrayPoolExtensions
{
    [NonCopyable]
    public struct Lease<T> : IDisposable
    {
#pragma warning disable RS0040
        private ArrayPool<T> _arrayPool;
#pragma warning restore RS0040
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
#pragma warning disable RS0042
                ObjectDisposedException.ThrowIf(_arrayPool is null, this);
#pragma warning restore RS0042
                return _array;
            }
        }

        public readonly Span<T> Buffer
        {
            get
            {
#pragma warning disable RS0042
                ObjectDisposedException.ThrowIf(_arrayPool is null, this);
#pragma warning restore RS0042
                return _array.AsSpan();
            }
        }

        public readonly int Length
        {
            get
            {
#pragma warning disable RS0042
                ObjectDisposedException.ThrowIf(_arrayPool is null, this);
#pragma warning restore RS0042
                return _array.Length;
            }
        }

        public void RentMore(int minimumLength = -1)
        {
#pragma warning disable RS0042
            ObjectDisposedException.ThrowIf(_arrayPool is null, this);
#pragma warning restore RS0042

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
