using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace System;

internal static class ExceptionPolyfill
{
    extension(Exception exception)
    {
        [StackTraceHidden]
        internal void SetCurrentStackTrace()
        {
            if (!exception.CanSetRemoteStackTrace())
                return; // early-exit

            // Store the current stack trace into the "remote" stack trace, which was originally introduced to support
            // remoting of exceptions cross app-domain boundaries, and is thus concatenated into Exception.StackTrace
            // when it's retrieved.
            var sb = new StringBuilder(256);
            sb.Append(new StackTrace(fNeedFileInfo: true));
            sb.AppendLine(SR.Exception_EndStackTraceFromPreviousThrow);

#if NET8_0_OR_GREATER
            ExceptionAccessors.RemoteStackTraceString(exception) = sb.ToString();
#else
            ExceptionAccessors.RemoteStackTraceStringField.SetValue(exception, sb.ToString());
#endif
        }

        private bool CanSetRemoteStackTrace()
        {
            if (Type.GetType("Mono.Runtime") is not null)
            {
#if NET8_0_OR_GREATER
                var traceIPs = ExceptionAccessors.TraceIPs(exception);
                var stackTraceString = ExceptionAccessors.StackTraceString(exception);
                var remoteStackTraceString = ExceptionAccessors.RemoteStackTraceString(exception);
#else
                var traceIPs = ExceptionAccessors.TraceIPsField.GetValue(exception);
                var stackTraceString = ExceptionAccessors.StackTraceStringField.GetValue(exception);
                var remoteStackTraceString = ExceptionAccessors.RemoteStackTraceStringField.GetValue(exception);
#endif

                if (traceIPs != null || stackTraceString != null || remoteStackTraceString != null)
                    ThrowHelper.ThrowInvalidOperationException();

                return true; // mono runtime doesn't have immutable agile exceptions, always return true
            }
            else
            {
                // If this is a preallocated singleton exception, silently skip the operation,
                // regardless of the value of throwIfHasExistingStack.
#if NET8_0_OR_GREATER
                if (ExceptionAccessors.IsImmutableAgileException(exception))
#else
                if (ExceptionAccessors.IsImmutableAgileExceptionMethod(exception))
#endif
                {
                    return false;
                }

#if NET8_0_OR_GREATER
                var stackTrace = ExceptionAccessors.StackTrace(exception);
                var stackTraceString = ExceptionAccessors.StackTraceString(exception);
                var remoteStackTraceString = ExceptionAccessors.RemoteStackTraceString(exception);
#else
                var stackTrace = ExceptionAccessors.StackTraceField.GetValue(exception);
                var stackTraceString = ExceptionAccessors.StackTraceStringField.GetValue(exception);
                var remoteStackTraceString = ExceptionAccessors.RemoteStackTraceStringField.GetValue(exception);
#endif

                // Check to see if the exception already has a stack set in it.
                if (stackTrace != null || stackTraceString != null || remoteStackTraceString != null)
                    ThrowHelper.ThrowInvalidOperationException();
                
                return true;
            }
        }

    }

    private static class ExceptionAccessors
    {
#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_traceIPs")]
        public static extern ref object? TraceIPs(Exception exception);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_stackTrace")]
        public static extern ref object? StackTrace(Exception exception);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_stackTraceString")]
        public static extern ref string? StackTraceString(Exception exception);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_remoteStackTraceString")]
        public static extern ref string? RemoteStackTraceString(Exception exception);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "IsImmutableAgileException")]
        public static extern bool IsImmutableAgileException(Exception e);
#else
        public static FieldInfo TraceIPsField { get; } = typeof(Exception)
            .GetField(
                "_traceIPs",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)!;

        public static FieldInfo StackTraceField { get; } = typeof(Exception)
            .GetField(
                "_stackTrace",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)!;

        public static FieldInfo StackTraceStringField { get; } = typeof(Exception)
            .GetField(
                "_stackTraceString",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)!;
        
        public static FieldInfo RemoteStackTraceStringField { get; } = typeof(Exception)
            .GetField(
                "_remoteStackTraceString",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)!;

        public static IsImmutableAgileExceptionDelegate IsImmutableAgileExceptionMethod { get; } = typeof(Exception)
            .GetMethod(
                "IsImmutableAgileException",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
                [typeof(Exception)])!
            .CreateDelegate<IsImmutableAgileExceptionDelegate>();

        public delegate bool IsImmutableAgileExceptionDelegate(Exception e);
#endif
    }
}
