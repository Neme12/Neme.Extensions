using Neme.Extensions.Contracts;
using Roslyn.Utilities;
using System.Runtime.InteropServices;

namespace Neme.Extensions.InteropServices;

public static class SafeHandleExtensions
{
    extension<THandle>(THandle handle)
        where THandle : SafeHandle
    {
        public Scope CreateScope()
        {
            Require.ArgumentNotNull(handle);
            return new Scope(handle);
        }
    }

    [NonDefaultable]
    [NonCopyable]
    public struct Scope : IDisposable
    {
        private SafeHandle _handle;
        private readonly bool _succeeded;

        internal Scope(SafeHandle handle)
        {
            bool success = false;

            try
            {
                handle.DangerousAddRef(ref _succeeded);
                success = true;
            }
            finally
            {
                if (!success && _succeeded)
                    handle.DangerousRelease();
            }

            _handle = handle;
        }

        public readonly nint Handle =>
            _handle.DangerousGetHandle();

        public void Dispose()
        {
            if (_handle != null)
            {
                if (_succeeded)
                    _handle.DangerousRelease();

                _handle = null!;
            }
        }
    }
}
