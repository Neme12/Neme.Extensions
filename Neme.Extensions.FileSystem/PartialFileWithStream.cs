using Neme.Extensions.Contracts;
using Neme.Extensions.Ownership;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

/// <summary>
/// Creates a file by writing to a temporary <c>.part</c> file and atomically moving it to the final path when the write is complete.
/// </summary>
/// <remarks>
/// <para>
/// Use <see cref="Create(string, FileOpenOptions, bool)"/> to create the temporary file, write the contents through <see cref="FileStream"/>,
/// and then call <see cref="Commit(bool)"/> to move the file to <see cref="FinalPath"/> without exposing a partially written file at the
/// destination.
/// </para>
/// <para>
/// If the instance is disposed before <see cref="Commit(bool)"/> is called, the temporary file is deleted. The temporary file can also be
/// <see cref="Close()">closed</see> and later <see cref="Reopen()">reopened</see> while it is still in its uncommitted <c>.part</c> state.
/// </para>
/// </remarks>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class PartialFileWithStream :
    IDisposable
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    , IAsyncDisposable
#endif
{
    private CheckedFileStream? _fileStream;
    private readonly string _finalPath;
    private readonly FileOpenOptions _options;
    private State _state;

    private PartialFileWithStream(CheckedFileStream partialFileStream, string finalPath, FileOpenOptions options)
    {
        _fileStream = partialFileStream;
        _finalPath = finalPath;
        _options = options;
        _state = State.Open;
    }

    /// <summary>
    /// Gets the stream for the temporary <c>.part</c> file while the file is open.
    /// </summary>
    [Owned]
    public CheckedFileStream FileStream
    {
        get
        {
            ObjectDisposedException.ThrowIf(_state == State.Disposed, this);

            if (_state == State.Closed)
                throw new InvalidOperationException("File is closed.");

            return _fileStream.NotNull();
        }
        private set
        {
            _fileStream = value;
        }
    }

    /// <summary>
    /// Gets the destination path that the temporary file will be moved to when committed.
    /// </summary>
    public string FinalPath
    {
        get
        {
            ObjectDisposedException.ThrowIf(_state == State.Disposed, this);
            return _finalPath;
        }
    }

    /// <summary>
    /// Gets the current on-disk path for the file.
    /// </summary>
    /// <remarks>
    /// This is the <see cref="FinalPath"/> with a <c>.part</c> suffix until <see cref="Commit(bool)"/> succeeds.
    /// </remarks>
    public string CurrentPath
    {
        get
        {
            ObjectDisposedException.ThrowIf(_state == State.Disposed, this);
            return _state == State.Finalized ? _finalPath : _finalPath + ".part";
        }
    }

    /// <summary>
    /// Creates a new temporary file at <paramref name="finalPath"/> with the <c>.part</c> suffix.
    /// </summary>
    /// <param name="finalPath">The final destination path that will be used by <see cref="Commit(bool)"/>.</param>
    /// <param name="options">The options used to open the temporary file. Delete access is required so the temporary file can be cleaned up.</param>
    /// <param name="createDirectory"><see langword="true"/> to create the destination directory if it does not already exist.</param>
    /// <returns>A <see cref="PartialFileWithStream"/> for writing the temporary file.</returns>
    public static PartialFileWithStream Create(string finalPath, FileOpenOptions options, bool createDirectory = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(finalPath);

        if ((options.Access & FileSystemAccess.Delete) == 0)
            throw new ArgumentException("Options must include delete access.", nameof(options));

        var partialPath = finalPath + ".part";

        if (createDirectory)
            Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

        var fileStream = FileIO.Open(partialPath, options).CreateFileStream();
        return new PartialFileWithStream(fileStream, finalPath, options);
    }

    /// <summary>
    /// Reopens the temporary <c>.part</c> file after it has been closed and before it has been committed.
    /// </summary>
    public void Reopen()
    {
        ObjectDisposedException.ThrowIf(_state == State.Disposed, this);

        if (_state != State.Closed)
            throw new InvalidOperationException("File is not closed.");

        var reopenOptions = _options with { Mode = FileMode.Open };

        _fileStream = FileIO.Open(FinalPath + ".part", reopenOptions).CreateFileStream();
        _state = State.Open;
    }

    /// <summary>
    /// Closes <see cref="FileStream"/> without committing the file.
    /// </summary>
    /// <remarks>
    /// Call <see cref="Reopen()"/> to continue writing later, or dispose the instance to delete the temporary file.
    /// </remarks>
    public void Close()
    {
        ObjectDisposedException.ThrowIf(_state == State.Disposed, this);

        if (_state != State.Open)
            throw new InvalidOperationException("File is not open.");

        _fileStream!.Dispose();
        _fileStream = null;
        _state = State.Closed;
    }

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Asynchronously closes <see cref="FileStream"/> without committing the file.
    /// </summary>
    /// <remarks>
    /// Call <see cref="Reopen()"/> to continue writing later, or dispose the instance to delete the temporary file.
    /// </remarks>
    public async Task CloseAsync()
    {
        ObjectDisposedException.ThrowIf(_state == State.Disposed, this);

        if (_state != State.Open)
            throw new InvalidOperationException("File is not open.");

        await _fileStream!.DisposeAsync();
        _fileStream = null;
        _state = State.Closed;
    }
#endif

    /// <summary>
    /// Atomically moves the temporary <c>.part</c> file to <see cref="FinalPath"/>.
    /// </summary>
    /// <param name="overwrite"><see langword="true"/> to overwrite an existing destination file; otherwise, the move fails if the destination exists.</param>
    /// <remarks>
    /// This should be called only after all data has been written to <see cref="FileStream"/> and the file is ready to replace or create the final file.
    /// </remarks>
    public void Commit(bool overwrite = false)
    {
        ObjectDisposedException.ThrowIf(_state == State.Disposed, this);

        if (_state != State.Open)
            throw new InvalidOperationException("File must be open to finalize.");

        FileIO.Move(FileStream.SafeFileHandle, FinalPath, overwrite);
        _state = State.Finalized;
    }

    public void Dispose()
    {
        if (_state == State.Disposed)
            return;

        if (_state != State.Finalized)
        {
            if (_state == State.Open)
                FileIO.Delete(FileStream.SafeFileHandle);
            else
                File.Delete(CurrentPath);
        }

        if (_state != State.Closed)
            _fileStream!.Dispose();

        _state = State.Disposed;
    }

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public async ValueTask DisposeAsync()
    {
        if (_state == State.Disposed)
            return;

        if (_state != State.Finalized)
        {
            if (_state == State.Open)
                FileIO.Delete(FileStream.SafeFileHandle);
            else
                File.Delete(CurrentPath);
        }

        if (_state != State.Closed)
            await _fileStream!.DisposeAsync();

        _state = State.Disposed;
    }
#endif

    private enum State : byte
    {
        Open,
        Closed,
        Finalized,
        Disposed,
    }
}
