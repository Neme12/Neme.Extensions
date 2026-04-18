#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
namespace System.Buffers;

public delegate void SpanAction<T, in TArg>(Span<T> span, TArg arg);
#endif
