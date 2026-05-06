using System.Runtime.InteropServices;

namespace Neme.Extensions.InteropServices;

public static class SafeHandleExtensions
{
    extension<THandle>(THandle handle)
        where THandle : SafeHandle
    {
        public Scope<THandle> CreateScope()
        {
            return new Scope<THandle>(handle);
        }
    }

    public struct Scope<THandle> : IDisposable
        where THandle : SafeHandle
    {
        private THandle _handle;
        private readonly bool _succeeded;

        internal Scope(THandle handle)
        {
            handle.DangerousAddRef(ref _succeeded);
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
