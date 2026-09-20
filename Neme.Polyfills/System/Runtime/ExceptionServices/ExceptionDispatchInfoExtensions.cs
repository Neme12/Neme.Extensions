using System.Diagnostics;

namespace System.Runtime.ExceptionServices;

public static class ExceptionDispatchInfoExtensions
{
    extension(ExceptionDispatchInfo info)
    {
        [StackTraceHidden]
        public static Exception SetCurrentStackTrace(Exception source)
        {
            ArgumentNullException.ThrowIfNull(source);

            source.SetCurrentStackTrace();

            return source;
        }
    }
}
