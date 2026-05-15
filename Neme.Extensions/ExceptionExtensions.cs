namespace Neme.Extensions;

internal static class ExceptionExtensions
{
    extension(Exception)
    {
        public static T TryCatch<T, TException>(Func<T> action, Func<TException, T> handler)
            where TException : Exception
        {
            try
            {
                return action();
            }
            catch (TException ex)
            {
                return handler(ex);
            }
        }
    }
}
