using Microsoft.AspNetCore.Http;

namespace Psg.Standardised.Api.Common.Exceptions
{
    public class PsgException : Exception
    {
        public PsgException(string message) : base(message)
            => ApiStatusCode = StatusCodes.Status500InternalServerError;

        public PsgException(string message, Exception innerException) : base(message, innerException)
            => ApiStatusCode = StatusCodes.Status500InternalServerError;

        public PsgException(string message, Exception innerException, int apiStatusCode) : base(message, innerException)
            => ApiStatusCode = apiStatusCode;

        public int ApiStatusCode { get; set; }

        public bool LogInnerException { get; set; } = true;
    }
}
