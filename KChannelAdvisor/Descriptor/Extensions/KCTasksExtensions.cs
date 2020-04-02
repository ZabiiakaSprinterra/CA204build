using System;
using System.Threading;
using System.Threading.Tasks;

namespace KChannelAdvisor.Descriptor.Extensions
{
    public static class KCTasksExtensions
    {
        public static void StartAndWaitAll(this Task[] tasks, CancellationToken token)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

            Exception innerException = null;
            foreach (var task in tasks)
            {
                task.ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        if (t.IsFaulted)
                        {
                            innerException = t.Exception;
                        }
                        cts.Cancel(true);
                    }
                }, cts.Token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);

                task.Start();
            }

            try
            {
                Task.WaitAll(tasks, cts.Token);
            }
            catch (Exception e)
            {
                cts.Cancel(true);

                if (innerException != null)
                {
                    if (innerException is AggregateException aggregate)
                    {
                        throw aggregate.InnerException;
                    }

                    throw innerException;
                }

                throw e;
            }
        }
    }
}
