using Neme.Extensions.AspNetCore.Logging;
using Neme.Extensions.MicrosoftExtensions.Caching;

namespace Neme.Extensions.AspNetCore;

internal static class EventIds
{
    private const string _prefix = "Neme.Extensions.AspNetCore.";

    public static class ResponseLogger
    {
        private const string _classPrefix = _prefix + nameof(ResponseLogger<>) + "`1.";

        public const int ErrorDuringDisposal = 13000;
        public const int CompleteWasNotCalled = 13001;
        public const int CompleteWasNotAwaited = 13002;

        public const string ErrorDuringDisposalName = _classPrefix + nameof(ErrorDuringDisposal);
        public const string CompleteWasNotCalledName = _classPrefix + nameof(CompleteWasNotCalled);
        public const string CompleteWasNotAwaitedName = _classPrefix + nameof(CompleteWasNotAwaited);
    }
}
