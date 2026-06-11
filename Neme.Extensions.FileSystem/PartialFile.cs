using Neme.Extensions.Contracts;
using Neme.Extensions.Ownership;

namespace Neme.Extensions.FileSystem;

/// <summary>
/// Creates a file by writing to a temporary <c>.part</c> file and atomically moving it to the final path when the write is complete.
/// </summary>
/// <remarks>
/// <para>
/// Use <see cref="Create(string, FileOpenOptions, bool)"/> to create the temporary file, write the contents through <see cref="File"/>,
/// and then call <see cref="Commit(bool)"/> to move the file to <see cref="FinalPath"/> without exposing a partially written file at the
/// destination.
/// </para>
/// <para>
/// If the instance is disposed before <see cref="Commit(bool)"/> is called, the temporary file is deleted. The temporary file can also be
/// <see cref="Close()">closed</see> and later <see cref="Reopen()">reopened</see> while it is still in its uncommitted <c>.part</c> state.
/// </para>
/// </remarks>
public sealed class PartialFile :
    IDisposable
{
    private OpenFile? _file;
    private readonly string _finalPath;
    private readonly FileOpenOptions _options;
    private State _state;

    private PartialFile(OpenFile partialFile, string finalPath)
    {
        _file = partialFile;
        _finalPath = finalPath;
        _options = partialFile.Options;
        _state = State.Open;
    }

    public static string Extension => ".part";

    /// <summary>
    /// Gets the <see cref="OpenFile"/> for the temporary <c>.part</c> file while the file is open.
    /// </summary>
    [Owned]
    public OpenFile File
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
            return _state == State.Committed ? _finalPath : _finalPath + Extension;
        }
    }

    /// <summary>
    /// Creates a new temporary file at <paramref name="finalPath"/> with the <c>.part</c> suffix.
    /// </summary>
    /// <param name="finalPath">The final destination path that will be used by <see cref="Commit(bool)"/>.</param>
    /// <param name="options">The options used to open the temporary file. Delete access is required so the temporary file can be cleaned up.</param>
    /// <param name="createDirectory"><see langword="true"/> to create the destination directory if it does not already exist.</param>
    /// <returns>A <see cref="PartialFile"/> for writing the temporary file.</returns>
    public static PartialFile Create(string finalPath, FileOpenOptions options, bool createDirectory = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(finalPath);

        if ((options.Access & FileSystemAccess.Delete) == 0)
            throw new ArgumentException("Options must include delete access.", nameof(options));

        var partialPath = finalPath + Extension;

        if (createDirectory)
            Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

        var file = FileIO.Open(partialPath, options);
        return new PartialFile(file, finalPath);
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

        _file = FileIO.Open(FinalPath + Extension, reopenOptions);
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

        _file!.Dispose();
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

        FileIO.Move(File.Handle, FinalPath, overwrite);
        _state = State.Committed;
    }

    public void Dispose()
    {
        if (_state == State.Disposed)
            return;

        if (_state != State.Committed)
        {
            if (_state == State.Open)
                FileIO.Delete(File.Handle);
            else
                System.IO.File.Delete(CurrentPath);
        }

        if (_state != State.Closed)
            _file!.Dispose();

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
