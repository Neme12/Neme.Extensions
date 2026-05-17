using Neme.Extensions.Contracts;

namespace Neme.Extensions.IO;

public static class FileExtensions
{
    extension(File)
    {
#if NETCOREAPP3_0_OR_GREATER
        public static void MoveIfExists(
            string sourceFileName,
            string destFileName,
            bool overwrite = false,
            bool allowMissingDirectory = false)
        {
            try
            {
                File.Move(sourceFileName, destFileName, overwrite);
            }
            catch (Exception e) when (e is FileNotFoundException || allowMissingDirectory && e is DirectoryNotFoundException)
            {
            }
        }
#endif

        public static void MoveIfExists(
            string sourceFileName,
            string destFileName,
            bool allowMissingDirectory = false)
        {
            try
            {
                File.Move(sourceFileName, destFileName);
            }
            catch (Exception e) when (e is FileNotFoundException || allowMissingDirectory && e is DirectoryNotFoundException)
            {
            }
        }
    }

    public static FileStream OpenRead(string path, FileOptions options)
    {
        Require.ArgumentNotNull(path);

        return CreateFileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            options);
    }

    public static FileStream OpenWrite(string path, FileOptions options)
    {
        Require.ArgumentNotNull(path);

        return CreateFileStream(
            path,
            FileMode.OpenOrCreate,
            FileAccess.Write,
            FileShare.None,
            options);
    }

    public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share, FileOptions options)
    {
        Require.ArgumentNotNull(path);

        return CreateFileStream(
            path,
            mode,
            access,
            share,
            options);
    }

    public static Task CopyAsync(string sourceFileName, string destFileName, CancellationToken cancellationToken) =>
        CopyAsync(sourceFileName, destFileName, overwrite: false, cancellationToken);

    public static async Task CopyAsync(string sourceFileName, string destFileName, bool overwrite, CancellationToken cancellationToken)
    {
        Require.ArgumentNotNull(sourceFileName);
        Require.ArgumentNotNull(destFileName);

#pragma warning disable CA2000 // Dispose objects before losing scope
        var sourceFileStream = CreateCopyFromFileStream(sourceFileName);
        var destFileStream = CreateCopyToFileStream(destFileName, overwrite);
#pragma warning restore CA2000 // Dispose objects before losing scope

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        await using (sourceFileStream.ConfigureAwait(false))
        await using (destFileStream.ConfigureAwait(false))
#else
        using (sourceFileStream)
        using (destFileStream)
#endif
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            await sourceFileStream.CopyToAsync(destFileStream, cancellationToken).ConfigureAwait(false);
#else
            await sourceFileStream.CopyToAsync(destFileStream, GetCopyBufferSize(sourceFileStream), cancellationToken).ConfigureAwait(false);
#endif
        }
    }

    public static async Task CopyToAsync(string sourceFileName, Stream destStream, CancellationToken cancellationToken)
    {
        Require.ArgumentNotNull(sourceFileName);
        Require.ArgumentNotNull(destStream);

#pragma warning disable CA2000 // Dispose objects before losing scope
        var sourceFileStream = CreateCopyFromFileStream(sourceFileName);
#pragma warning restore CA2000 // Dispose objects before losing scope

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        await using (sourceFileStream.ConfigureAwait(false))
#else
        using (sourceFileStream)
#endif
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            await sourceFileStream.CopyToAsync(destStream, cancellationToken).ConfigureAwait(false);
#else
            await sourceFileStream.CopyToAsync(destStream, GetCopyBufferSize(sourceFileStream), cancellationToken).ConfigureAwait(false);
#endif
        }
    }

    private static FileStream CreateCopyFromFileStream(string path)
    {
        Assert.NotNull(path);

        return CreateFileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            FileOptions.SequentialScan | FileOptions.Asynchronous);
    }

    internal static FileStream CreateCopyToFileStream(string path, bool overwrite)
    {
        Assert.NotNull(path);

        return CreateFileStream(
            path,
            overwrite ? FileMode.Create : FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            FileOptions.SequentialScan | FileOptions.Asynchronous);
    }

    private static FileStream CreateFileStream(
        string path,
        FileMode mode,
        FileAccess access,
        FileShare share,
        FileOptions options)
    {
        Assert.NotNull(path);

#if NET6_0_OR_GREATER
        return new(path, new FileStreamOptions { Mode = mode, Access = access, Share = share, Options = options });
#else
        const int DefaultBufferSize = 4096;
        return new (path, mode, access, share, DefaultBufferSize, options);
#endif
    }

#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static int GetCopyBufferSize(Stream stream)
    {
        Assert.NotNull(stream);

        const int DefaultCopyBufferSize = 81920;

        int bufferSize = DefaultCopyBufferSize;

        if (stream.CanSeek)
        {
            long length = stream.Length;
            long position = stream.Position;
            if (length <= position) // Handles negative overflows
            {
                // There are no bytes left in the stream to copy.
                // However, because CopyTo{Async} is virtual, we need to
                // ensure that any override is still invoked to provide its
                // own validation, so we use the smallest legal buffer size here.
                bufferSize = 1;
            }
            else
            {
                long remaining = length - position;
                if (remaining > 0)
                {
                    // In the case of a positive overflow, stick to the default size
                    bufferSize = (int)Math.Min(bufferSize, remaining);
                }
            }
        }

        return bufferSize;
    }
#endif
}
