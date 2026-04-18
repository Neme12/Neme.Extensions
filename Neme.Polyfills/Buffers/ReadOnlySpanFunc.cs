namespace Neme.Utilities.Buffers;

internal delegate void SpanAction<T>(Span<T> span);
internal delegate void SpanAction<T, in T1, in T2>(Span<T> span, T1 arg1, T2 arg2);
internal delegate void SpanAction<T, in T1, in T2, in T3>(Span<T> span, T1 arg1, T2 arg2, T3 arg3);
internal delegate void SpanAction<T, in T1, in T2, in T3, T4>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

internal delegate void ReadOnlySpanAction<T>(ReadOnlySpan<T> span);
internal delegate void ReadOnlySpanAction<T, in T1, in T2>(ReadOnlySpan<T> span, T1 arg1, T2 arg2);
internal delegate void ReadOnlySpanAction<T, in T1, in T2, in T3>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3);
internal delegate void ReadOnlySpanAction<T, in T1, in T2, in T3, T4>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

internal delegate TResult SpanFunc<T, out TResult>(Span<T> span);
internal delegate TResult SpanFunc<T, in T1, out TResult>(Span<T> span, T1 arg1);
internal delegate TResult SpanFunc<T, in T1, in T2, out TResult>(Span<T> span, T1 arg1, T2 arg2);
internal delegate TResult SpanFunc<T, in T1, in T2, in T3, out TResult>(Span<T> span, T1 arg1, T2 arg2, T3 arg3);
internal delegate TResult SpanFunc<T, in T1, in T2, in T3, T4, out TResult>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

internal delegate TResult ReadOnlySpanFunc<T, out TResult>(ReadOnlySpan<T> span);
internal delegate TResult ReadOnlySpanFunc<T, in T1, out TResult>(ReadOnlySpan<T> span, T1 arg1);
internal delegate TResult ReadOnlySpanFunc<T, in T1, in T2, out TResult>(ReadOnlySpan<T> span, T1 arg1, T2 arg2);
internal delegate TResult ReadOnlySpanFunc<T, in T1, in T2, in T3, out TResult>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3);
internal delegate TResult ReadOnlySpanFunc<T, in T1, in T2, in T3, T4, out TResult>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
