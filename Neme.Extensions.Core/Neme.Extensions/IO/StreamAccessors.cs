using System.Reflection;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.IO;

internal static class StreamAccessors
{
#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Method)]
        public extern static void Dispose(Stream stream, bool disposing);

        [UnsafeAccessor(UnsafeAccessorKind.Method)]
        public extern static WaitHandle CreateWaitHandle(Stream stream);

        [UnsafeAccessor(UnsafeAccessorKind.Method)]
        public extern static void ObjectInvariant(Stream stream);
#else
    private const BindingFlags _bindingFlags =
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding;

    public static readonly DisposeDelegate Dispose =
        typeof(Stream).GetMethod(
            "Dispose",
            _bindingFlags,
            [typeof(bool)])!
        .CreateDelegate<DisposeDelegate>();

    public static readonly CreateWaitHandleDelegate CreateWaitHandle =
        typeof(Stream).GetMethod(
            "CreateWaitHandle",
            _bindingFlags,
            [])!
        .CreateDelegate<CreateWaitHandleDelegate>();

    public static readonly ObjectInvariantDelegate ObjectInvariant =
        typeof(Stream).GetMethod(
            "ObjectInvariant",
            _bindingFlags,
            [])!
        .CreateDelegate<ObjectInvariantDelegate>();

    public delegate void DisposeDelegate(Stream stream, bool disposing);
    public delegate WaitHandle CreateWaitHandleDelegate(Stream stream);
    public delegate void ObjectInvariantDelegate(Stream stream);
#endif
}
