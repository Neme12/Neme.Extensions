using Neme.Extensions.Contracts;
using Neme.Extensions.Ownership;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

[SupportedOSPlatform("windows6.0.6000")]
public sealed class PartialFileStream :
    IDisposable
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    , IAsyncDisposable
#endif
{
    private CheckedFileStream? _fileStream;
    private readonly string _finalPath;
    private readonly FsFileOptions _options;
    private State _state;

    private PartialFileStream(CheckedFileStream partialFileStream, string finalPath, FsFileOptions options)
    {
        _fileStream = partialFileStream;
        _finalPath = finalPath;
        _options = options;
        _state = State.Open;
    }

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

    public string FinalPath
    {
        get
        {
            ObjectDisposedException.ThrowIf(_state == State.Disposed, this);
            return _finalPath;
        }
    }

    public string CurrentPath
    {
        get
        {
            ObjectDisposedException.ThrowIf(_state == State.Disposed, this);
            return _state == State.Finalized ? _finalPath : _finalPath + ".part";
        }
    }

    public static PartialFileStream Create(string finalPath, FsFileOptions options, bool createDirectory = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(finalPath);

        if ((options.Access & FileSystemAccess.Delete) == 0)
            throw new ArgumentException("Options must include delete access.", nameof(options));

        var partialPath = finalPath + ".part";

        if (createDirectory)
            Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

        var fileStream = FileIO.Open(partialPath, options).CreateFileStream();
        return new PartialFileStream(fileStream, finalPath, options);
    }

    public void Reopen()
    {
        ObjectDisposedException.ThrowIf(_state == State.Disposed, this);

        if (_state != State.Closed)
            throw new InvalidOperationException("File is not closed.");

        var reopenOptions = _options with { Mode = FileMode.Open };

        _fileStream = FileIO.Open(FinalPath + ".part", reopenOptions).CreateFileStream();
        _state = State.Open;
    }

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

    public void FinalizeFile(bool overwrite = false)
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
