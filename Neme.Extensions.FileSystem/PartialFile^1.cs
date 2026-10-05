using Neme.Extensions.Contracts;
using Neme.Extensions.Ownership;

namespace Neme.Extensions.FileSystem;

/// <summary>
/// Creates a file by writing to a temporary <c>.part</c> file and atomically moving it to the final path when the write is complete.
/// </summary>
/// <remarks>
/// <para>
/// Use <see cref="Create(string, FileHandleRequest, bool)"/> to create the temporary file, write the contents through <see cref="File"/>,
/// and then call <see cref="Commit(bool)"/> to move the file to <see cref="FinalPath"/> without exposing a partially written file at the
/// destination.
/// </para>
/// <para>
/// If the instance is disposed before <see cref="Commit(bool)"/> is called, the temporary file is deleted. The temporary file can also be
/// <see cref="Close()">closed</see> and later <see cref="Reopen()">reopened</see> while it is still in its uncommitted <c>.part</c> state.
/// </para>
/// </remarks>
public sealed class PartialFile<TFile> : IDisposable, IAsyncDisposable
    where TFile : class
{
    private TFile? _file;
    private IFileObject? _fileEntry;
    private readonly Func<TFile, IFileObject> _fileEntryAdapter;
    private readonly string _finalPath;
    private readonly Func<string, TFile> _reopenFile;
    private State _state;

    internal PartialFile(
        TFile partialFile,
        Func<TFile, IFileObject> fileEntryAdaper,
        string finalPath,
        Func<string, TFile> reopenFile)
    {
        _file = partialFile;
        _fileEntry = fileEntryAdaper(partialFile);
        _fileEntryAdapter = fileEntryAdaper;
        _finalPath = finalPath;
        _reopenFile = reopenFile;
        _state = State.Open;
    }

    /// <summary>
    /// Gets the <see cref="FileSession"/> for the temporary <c>.part</c> file while the file is open.
    /// </summary>
    [Owned]
    public TFile File
    {
        get
        {
            ObjectDisposedException.ThrowIf(_state == State.Disposed, this);

            if (_state == State.Closed)
                throw new InvalidOperationException("File is closed.");

            return _file.NotNull();
        }
        private set
        {
            _file = value;
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
            return _state == State.Committed ? _finalPath : _finalPath + PartialFile.PartialExtension;
        }
    }

    /// <summary>
    /// Reopens the temporary <c>.part</c> file after it has been closed and before it has been committed.
    /// </summary>
    public void Reopen()
    {
        ObjectDisposedException.ThrowIf(_state == State.Disposed, this);

        if (_state != State.Closed)
            throw new InvalidOperationException("File is not closed.");

        _file = _reopenFile(FinalPath);
        _fileEntry = _fileEntryAdapter(_file);
        _state = State.Open;
    }

    /// <summary>
    /// Closes <see cref="File"/> without committing the file.
    /// </summary>
    /// <remarks>
    /// Call <see cref="Reopen()"/> to continue writing later, or dispose the instance to delete the temporary file.
    /// </remarks>
    public void Close()
    {
        ObjectDisposedException.ThrowIf(_state == State.Disposed, this);

        if (_state != State.Open)
            throw new InvalidOperationException("File is not open.");

        _fileEntry!.Dispose();
        _fileEntry = null;
        _file = null;
        _state = State.Closed;
    }

    /// <summary>
    /// Asynchronously closes <see cref="FileStream"/> without committing the file.
    /// </summary>
    /// <remarks>
    /// Call <see cref="Reopen()"/> to continue writing later, or dispose the instance to delete the temporary file.
    /// </remarks>
    public async ValueTask CloseAsync()
    {
        ObjectDisposedException.ThrowIf(_state == State.Disposed, this);

        if (_state != State.Open)
            throw new InvalidOperationException("File is not open.");

        await _fileEntry!.DisposeAsync();
        _fileEntry = null;
        _file = null;
        _state = State.Closed;
    }

    /// <summary>
    /// Atomically moves the temporary <c>.part</c> file to <see cref="FinalPath"/>.
    /// </summary>
    /// <param name="overwrite"><see langword="true"/> to overwrite an existing destination file; otherwise, the move fails if the destination exists.</param>
    /// <remarks>
    /// This should be called only after all data has been written to <see cref="File"/> and the file is ready to replace or create the final file.
    /// </remarks>
    public void Commit(bool overwrite = false)
    {
        ObjectDisposedException.ThrowIf(_state == State.Disposed, this);

        if (_state != State.Open)
            throw new InvalidOperationException("File must be open to commit.");

        _fileEntry!.Move(FinalPath, overwrite);
        _state = State.Committed;
    }

    public void Dispose()
    {
        if (_state == State.Disposed)
            return;

        if (_state != State.Committed)
        {
            if (_state == State.Open)
                _fileEntry!.Delete();
            else
                System.IO.File.Delete(CurrentPath);
        }

        if (_state != State.Closed)
            _fileEntry!.Dispose();

        _state = State.Disposed;
    }

    public async ValueTask DisposeAsync()
    {
        if (_state == State.Disposed)
            return;

        if (_state != State.Committed)
        {
            if (_state == State.Open)
                _fileEntry!.Delete();
            else
                System.IO.File.Delete(CurrentPath);
        }

        if (_state != State.Closed)
            await _fileEntry!.DisposeAsync();

        _state = State.Disposed;
    }

    private enum State : byte
    {
        Open,
        Closed,
        Committed,
        Disposed,
    }
}
