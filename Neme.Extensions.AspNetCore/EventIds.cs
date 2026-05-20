namespace Neme.Extensions.AspNetCore;

internal static class EventIds
{
    private const string _prefix = "Neme.Extensions.AspNetCore.";

    public const int ErrorDuringDisposal = 12000;
    public const int CompleteWasNotCalled = 12001;
    public const int CompleteWasNotAwaited = 12002;

    public const string ErrorDuringDisposalName = _prefix + nameof(ErrorDuringDisposal);
    public const string CompleteWasNotCalledName = _prefix + nameof(CompleteWasNotCalled);
    public const string CompleteWasNotAwaitedName = _prefix + nameof(CompleteWasNotAwaited);
}
