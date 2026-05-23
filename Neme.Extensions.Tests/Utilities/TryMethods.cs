using System;
using System.Collections.Generic;
using System.Text;

namespace Neme.Extensions.Tests.Utilities;

internal static class TryMethods
{
    public static void TryDeleteFile(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch
        {
        }
    }

    public static void TryDeleteDirectory(string path, bool recursive)
    {
        try
        {
            Directory.Delete(path, recursive);
        }
        catch
        {
        }
    }
}
