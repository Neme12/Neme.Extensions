namespace Neme.Extensions.Buffers;

public delegate void SpanAction<T>(Span<T> span);
public delegate void SpanAction<T, in T1, in T2>(Span<T> span, T1 arg1, T2 arg2);
public delegate void SpanAction<T, in T1, in T2, in T3>(Span<T> span, T1 arg1, T2 arg2, T3 arg3);
public delegate void SpanAction<T, in T1, in T2, in T3, T4>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public delegate void ReadOnlySpanAction<T>(ReadOnlySpan<T> span);
public delegate void ReadOnlySpanAction<T, in T1, in T2>(ReadOnlySpan<T> span, T1 arg1, T2 arg2);
public delegate void ReadOnlySpanAction<T, in T1, in T2, in T3>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3);
public delegate void ReadOnlySpanAction<T, in T1, in T2, in T3, T4>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public delegate TResult SpanFunc<T, out TResult>(Span<T> span);
public delegate TResult SpanFunc<T, in T1, out TResult>(Span<T> span, T1 arg1);
public delegate TResult SpanFunc<T, in T1, in T2, out TResult>(Span<T> span, T1 arg1, T2 arg2);
public delegate TResult SpanFunc<T, in T1, in T2, in T3, out TResult>(Span<T> span, T1 arg1, T2 arg2, T3 arg3);
public delegate TResult SpanFunc<T, in T1, in T2, in T3, T4, out TResult>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public delegate TResult ReadOnlySpanFunc<T, out TResult>(ReadOnlySpan<T> span);
public delegate TResult ReadOnlySpanFunc<T, in T1, out TResult>(ReadOnlySpan<T> span, T1 arg1);
public delegate TResult ReadOnlySpanFunc<T, in T1, in T2, out TResult>(ReadOnlySpan<T> span, T1 arg1, T2 arg2);
public delegate TResult ReadOnlySpanFunc<T, in T1, in T2, in T3, out TResult>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3);
public delegate TResult ReadOnlySpanFunc<T, in T1, in T2, in T3, T4, out TResult>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
