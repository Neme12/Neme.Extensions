using Neme.Extensions.Contracts;

namespace Neme.Extensions.IO;

public static class CopyToFileExtensions
{
    public static async Task CopyToFileAsync(this Stream sourceStream, string destFileName, bool overwrite, CancellationToken cancellationToken)
    {
        Require.ArgumentNotNull(sourceStream);
        Require.ArgumentNotNull(destFileName);

#pragma warning disable CA2000 // Dispose objects before losing scope
        var destFileStream = FileExtensions.CreateCopyToFileStream(destFileName, overwrite);
#pragma warning restore CA2000 // Dispose objects before losing scope
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        await using (destFileStream.ConfigureAwait(false))
#else
        using (destFileStream)
#endif
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            await sourceStream.CopyToAsync(destFileStream, cancellationToken).ConfigureAwait(false);
#else
            await sourceStream.CopyToAsync(destFileStream, FileExtensions.GetCopyBufferSize(sourceStream), cancellationToken).ConfigureAwait(false);
#endif
        }
    }
}
