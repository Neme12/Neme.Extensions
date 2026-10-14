using System.Diagnostics;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;

namespace Neme.Extensions.FileSystem.Internal;

internal sealed class LeaveOpenFileStream : DelegatingFileStream
{
    private bool _disposed;

    public LeaveOpenFileStream([OwnershipTransfer] FileStream fileStream)
        : base(fileStream)
    {
    }

    ~LeaveOpenFileStream()
    {
        Debug.Fail($"{nameof(LeaveOpenFileStream)} should have been disposed.");
    }

    private static bool IsIoRelatedException(Exception e) =>
        e is IOException ||
        e is UnauthorizedAccessException ||
        e is NotSupportedException ||
        e is ArgumentException && e is not ArgumentNullException;

    public override void Close()
    {
        Dispose(true);
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _disposed = true;
#pragma warning disable CA1816
        GC.SuppressFinalize(this);
        GC.SuppressFinalize(InnerStream);
#pragma warning restore CA1816

        try
        {
            Flush();
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception e) when (!disposing && IsIoRelatedException(e))
        {
            // On finalization, ignore failures from trying to flush the write buffer,
            // e.g. if this stream is wrapping a pipe and the pipe is now broken.
        }
    }

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        GC.SuppressFinalize(this);
        GC.SuppressFinalize(InnerStream);

        try
        {
            await FlushAsync().ConfigureAwait(false);
        }
        catch (ObjectDisposedException)
        {
        }
    }
#endif
}
