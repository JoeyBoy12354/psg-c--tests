using Microsoft.AspNetCore.Http;

namespace Psg.Standardised.Api.Common.Services
{
    public class RequestContextService : IRequestContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestContextService(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        public string GetRequestHeaderValue(string key)
        {
            return _httpContextAccessor.HttpContext.Request.Headers.FirstOrDefault(f => f.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
        }

        public string GetBaseRequestUrl()
        {
            return $"{_httpContextAccessor.HttpContext.Request.Method.ToString()} {_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.ToUriComponent()}{_httpContextAccessor.HttpContext.Request.Path.ToUriComponent()}";
        }
    }

    public interface IRequestContextService
    {
        string GetRequestHeaderValue(string key);

        string GetBaseRequestUrl();
    }
}
