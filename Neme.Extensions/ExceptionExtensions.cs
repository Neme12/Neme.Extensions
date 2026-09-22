namespace Neme.Extensions;

public static class ExceptionExtensions
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
            catch (TException e)
            {
                return handler(e);
            }
        }

        public static T TryCatch<T, TException>(Func<T> action, Func<TException, bool> condition, Func<TException, T> handler)
            where TException : Exception
        {
            try
            {
                return action();
            }
            catch (TException e) when (condition(e))
            {
                return handler(e);
            }
        }
    }
}
