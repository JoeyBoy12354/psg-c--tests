using Polly;
using Polly.Retry;
using Serilog;

namespace Psg.Common.Registrations.Helpers
{
    /// <summary>
    /// Class to Execute async Code with retry
    /// </summary>
    public static class RetryHelper
    {
        private static readonly AsyncRetryPolicy _defaultAsyncRetryPolicy = GetAsyncRetryPolicy();
        private static readonly RetryPolicy _defaultRetryPolicy = GetRetryPolicy();

        /// <summary>
        /// Gets an Polly AsyncRetryPolicy to execute code with retry
        /// </summary>      
        /// <returns></returns>
        public static AsyncRetryPolicy GetAsyncRetryPolicy(int retryCount = 3, int retryDelayInSeconds = 2)
        {
            AsyncRetryPolicy retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(retryDelayInSeconds, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Log.Information("Retry {RetryCount} encountered an error: {Message}. Waiting {TimeSpan} before next retry.", 
                                                                                                                     retryCount, 
                                                                                                                      exception.Message, 
                                                                                                                      timeSpan);
                    });

            return retryPolicy;
        }

        /// <summary>
        /// Gets an Polly RetryPolicy to execute code with retry
        /// </summary>      
        public static RetryPolicy GetRetryPolicy(int retryCount = 3, int retryDelayInSeconds = 2)
        {
            RetryPolicy retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetry(retryCount, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(retryDelayInSeconds, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    Log.Information("Retry {RetryCount} encountered an error: {Message}. Waiting {TimeSpan} before next retry.",
                                                                                                                     retryCount,
                                                                                                                      exception.Message,
                                                                                                                      timeSpan);
                });

            return retryPolicy;
        }

        /// <summary>
        /// Execute async code with retry and return result.
        /// </summary>       
        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> action, int retryCount = 3, int retryDelayInSeconds = 2)
        {
            if (retryCount == 3 && retryDelayInSeconds == 2)
            {
                return await _defaultAsyncRetryPolicy.ExecuteAsync(action);
            }

            var retryPolicy = GetAsyncRetryPolicy(retryCount, retryDelayInSeconds);
            return await retryPolicy.ExecuteAsync(action);
        }

        /// <summary>
        /// Execute async code with retry.
        /// </summary>  
        public static async Task ExecuteAsync(Func<Task> action, int retryCount = 3, int retryDelayInSeconds = 2)
        {
            if (retryCount == 3 && retryDelayInSeconds == 2)
            {
                await _defaultAsyncRetryPolicy.ExecuteAsync(action);
                return;
            }

            var retryPolicy = GetAsyncRetryPolicy(retryCount, retryDelayInSeconds);
            await retryPolicy.ExecuteAsync(action);
        }

        /// <summary>
        /// Execute code with retry and return result.
        /// </summary>       
        public static T Execute<T>(Func<T> action, int retryCount = 3, int retryDelayInSeconds = 2)
        {
            if (retryCount == 3 && retryDelayInSeconds == 2)
            {
                return _defaultRetryPolicy.Execute(action);
            }

            var retryPolicy = GetRetryPolicy(retryCount, retryDelayInSeconds);
            return retryPolicy.Execute(action);
        }

        /// <summary>
        /// Execute code with retry.
        /// </summary>       
        public static void Execute(Action action, int retryCount = 3, int retryDelayInSeconds = 2)
        {
            if (retryCount == 3 && retryDelayInSeconds == 2)
            {
                _defaultRetryPolicy.Execute(action);
                return;
            }

            var retryPolicy = GetRetryPolicy(retryCount, retryDelayInSeconds);
            retryPolicy.Execute(action);
        }
    }
}
