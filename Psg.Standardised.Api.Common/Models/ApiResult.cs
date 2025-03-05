using Psg.Standardised.Api.Common.Exceptions;

namespace Psg.Standardised.Api.Common.Models
{
    public class ApiResult<T>
    {
        internal ApiResult() { }
        internal ApiResult(T result)
        {
            Result = result;
        }

        public T Result { get; set; }
        public string Error { get; set; }
        public Dictionary<string, List<string>> Validation { get; set; }
        public MetaData[] MetaData { get; set; }
    }

    public static class ApiResult
    {
        public static ApiResult<TResult> FromResult<TResult>(TResult result, MetaData metaData = null)
        {
            if (metaData == null)
                return new ApiResult<TResult> { Result = result, MetaData = new MetaData[] { new MetaDataTrace() } };

            return new ApiResult<TResult> { Result = result, MetaData = new MetaData[] { metaData, new MetaDataTrace() } };
        }

        public static ApiResult<object> FromException(Exception exception, string message = default)
        { 
            var errorData = new List<MetaDataError.ErrorEntry>() { new MetaDataError.ErrorEntry(exception.Message, exception.StackTrace) };

            // insert custom message at top of list
            if(!string.IsNullOrEmpty(message))
            {
                errorData.Insert(0, new MetaDataError.ErrorEntry(message, ""));
            }

            Exception innerException = exception;
            while ((innerException = innerException.InnerException) != null)
            {
                errorData.Add(new MetaDataError.ErrorEntry(innerException.Message, innerException.StackTrace));
            }

            var metaDataTrace = new MetaDataTrace();

            string error = exception is PsgException
                ? $"Handled exception: {exception.Message} TraceId: {metaDataTrace.TraceData.First().Value}"
                : $"Unhandled exception: {exception.Message} TraceId: {metaDataTrace.TraceData.First().Value}";

            return new ApiResult<object> { Error = error, MetaData = new MetaData[] { new MetaDataError(errorData), metaDataTrace } };
        }
    }
}
