using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Neme.Extensions.FileSystem;

using var direcory = FileIO.OpenHandle(@"C:\Users\neme1\OneDrive\Devices\Simča-PC\Desktop\test-file.txt", new(FileMode.Open, FsFileAccess.Read, FileShare.ReadWrite | FileShare.Delete));

using var file2 = FileIO.Open(FileIO.GetFileId(direcory), new(FileMode.Open, FsFileAccess.Read, FileShare.ReadWrite | FileShare.Delete) { });

{

}