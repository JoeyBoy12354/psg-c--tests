using Polly;
using Polly.Bulkhead;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Serilog;

namespace Psg.Common.Registrations.Polly.Policies
{
    /// <summary>
    /// Contains Policies for Polly.
    /// Can add more advanced policies such as CircuitBreaker, Fallback at a later stage    
    /// </summary>
    public static class PollyPolicies
    {

        // this is a basic retry policy
        // can be changed as needed
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return GetRetryPolicy(1, 3);
        }
        public static IAsyncPolicy GetRetryPolicyAsync()
        {
            return GetRetryPolicyAsync(1, 3);
        }

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retrySeconds, int retryCount)
        {
            // exponential backoff with jitter
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(retrySeconds), retryCount: retryCount);

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                //.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(sleepDurations: delay,
                                   onRetry: (exception, calculatedWaitDuration, retryAttempt, context) =>
                                   {
                                       Log.Information("Retry attempt: [{retryAttempt}]", retryAttempt);
                                   });
        }

        public static IAsyncPolicy GetRetryPolicyAsync(int retrySeconds, int retryCount)
        {
            // exponential backoff with jitter
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(retrySeconds), retryCount: retryCount);

            return Policy
                .Handle<Exception>() // handle any exception
                .WaitAndRetryAsync(sleepDurations: delay,
                                   onRetry: (exception, calculatedWaitDuration, retryAttempt, context) =>
                                   {
                                       Log.Information("Retry attempt: [{retryAttempt}], Exception: {Exception}", retryAttempt, exception.Message);
                                   });
        }




        public static AsyncBulkheadPolicy GetAsyncBulkheadPolicy(int maxParallelization, int maxQueue = int.MaxValue)
        {
            var bulkheadPolicy = Policy.BulkheadAsync(maxParallelization, maxQueue);

            return bulkheadPolicy;
        }

        public static IAsyncPolicy GetRetryPolicyCustom(string message, int retrySeconds, int retryCount)
        {
            // exponential backoff with jitter
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(retrySeconds), retryCount: retryCount);

            var polly = Policy
                              .Handle<Exception>()
                              .WaitAndRetryAsync(sleepDurations: delay,
                              (exception,
                                      retryCount,
                                      context)
                                      => Log.Error("{message}: Retrying [{retryCount}], Exception: [{message}]",
                                                     message,
                                                          retryCount,
                                                          exception.Message)
                                      );

            return polly;
        }
    }
}
