namespace Neme.Extensions.MicrosoftExtensions.InternalUtilities;

internal interface IAsyncState
{
    static abstract bool IsAsync { get; }

    sealed class Sync : IAsyncState
    {
        public static bool IsAsync => false;
    }

    sealed class Async : IAsyncState
    {
        public static bool IsAsync => true;
    }
}
