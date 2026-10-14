using System.Runtime.CompilerServices;

namespace Neme.Extensions.Tasks;

public static class AwaiterExtensions
{
    extension(TaskAwaiter awaiter)
    {
        public async void GetCompletedResult()
        {
            if (!awaiter.IsCompleted)
                throw new InvalidOperationException();

            awaiter.GetResult();
        }
    }

    extension<T>(TaskAwaiter<T> awaiter)
    {
        public T GetCompletedResult()
        {
            if (!awaiter.IsCompleted)
                throw new InvalidOperationException();

            return awaiter.GetResult();
        }
    }

    extension(ValueTaskAwaiter awaiter)
    {
        public void GetCompletedResult()
        {
            if (!awaiter.IsCompleted)
                throw new InvalidOperationException();

            awaiter.GetResult();
        }
    }

    extension<T>(ValueTaskAwaiter<T> awaiter)
    {
        public T GetCompletedResult()
        {
            if (!awaiter.IsCompleted)
                throw new InvalidOperationException();

            return awaiter.GetResult();
        }
    }
}
