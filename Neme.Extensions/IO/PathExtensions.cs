namespace Neme.Extensions.IO;

public static class PathExtensions
{
    extension(Path)
    {
        public static string ChangePathWithoutExtension<TState>(string path, Func<string, TState, string> changePath, TState state)
        {
            var extension = Path.GetExtension(path);
            var pathWithoutExtension = Path.ChangeExtension(path, null);
            var newPathWithoutExtension = changePath(pathWithoutExtension, state);
            return Path.ChangeExtension(newPathWithoutExtension, extension);

        }

        public static string ChangePathWithoutExtension<TState>(string path, Func<string, string> changePath)
        {
            var extension = Path.GetExtension(path);
            var pathWithoutExtension = Path.ChangeExtension(path, null);
            var newPathWithoutExtension = changePath(pathWithoutExtension);
            return Path.ChangeExtension(newPathWithoutExtension, extension);

        }
    }
}
