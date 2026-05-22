namespace Neme.Extensions.FileSystem.Tests;

/// <summary>
/// Collection definition to ensure FileIO tests run sequentially.
/// FileIO tests manipulate file system resources and should not run in parallel.
/// </summary>
[CollectionDefinition(nameof(FileIOTestCollection), DisableParallelization = true)]
public class FileIOTestCollection
{
}
