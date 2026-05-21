namespace Neme.Extensions;

#if NET6_0_OR_GREATER

/// <summary>
/// A type parameter interface for distinguishing between synchronous and asynchronous execution paths
/// in generic methods. Use <see cref="Sync"/> for synchronous operations and <see cref="Async"/> for asynchronous operations.
/// </summary>
/// <remarks>
/// This pattern allows sharing implementation code between sync and async methods while maintaining proper
/// execution semantics (no blocking on synchronous paths). The generic method checks <see cref="IsAsync"/>
/// at runtime to branch between synchronous and asynchronous I/O operations.
/// </remarks>
/// <example>
/// <code>
/// private async Task&lt;string&gt; ReadFileAsync&lt;TAsync&gt;(string path, CancellationToken ct)
///     where TAsync : IAsyncState
/// {
///     if (TAsync.IsAsync)
///         return await File.ReadAllTextAsync(path, ct);
///     else
///         return File.ReadAllText(path);
/// }
/// 
/// // Called from sync method
/// var content = ReadFileAsync&lt;IAsyncState.Sync&gt;(path, ct).GetAwaiter().GetResult();
/// 
/// // Called from async method
/// var content = await ReadFileAsync&lt;IAsyncState.Async&gt;(path, ct);
/// </code>
/// </example>
public interface IAsyncState
{
    /// <summary>
    /// Gets a value indicating whether this represents asynchronous execution.
    /// </summary>
    static abstract bool IsAsync { get; }

    /// <summary>
    /// Represents synchronous execution. Use as a type parameter to indicate a synchronous code path.
    /// </summary>
    public sealed class Sync : IAsyncState
    {
        public static bool IsAsync => false;
    }

    /// <summary>
    /// Represents asynchronous execution. Use as a type parameter to indicate an asynchronous code path.
    /// </summary>
    public sealed class Async : IAsyncState
    {
        public static bool IsAsync => true;
    }
}

#endif
