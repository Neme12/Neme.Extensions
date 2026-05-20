using Neme.Extensions.Contracts;

namespace Neme.Extensions;

public static class AggregateExceptionExtensions
{
    extension(AggregateException)
    {
        public static Exception SingleOrAggregate(IReadOnlyList<Exception> exceptions)
        {
            Require.ArgumentNotEmpty(exceptions);

            return exceptions.Count switch
            {
                1 => exceptions[0],
                _ => new AggregateException(exceptions),
            };
        }
    }
}
