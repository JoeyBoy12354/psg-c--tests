using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Psg.Standardised.Api.Common.Exceptions;
using Psg.Standardised.Api.Common.Models;
using System.Web.Http;


namespace Psg.Standardised.Api.Common.Services
{

    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorMiddleware> _log;
      

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> log)
        {
            _next = next;
            _log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string token = "NONE";
            try
            {
                if (exception is PsgException psgException)
                {
                    var userService = context.RequestServices.GetService<UserService>();
                    token = userService?.User?.Jwt;

                    bool isSuccess = psgException.ApiStatusCode >= 200 && psgException.ApiStatusCode <= 299;

                    if (isSuccess)
                    {
                        await WriteResponseContentAsync(context, ApiResult.FromResult<string>(null), psgException.ApiStatusCode);
                        return;
                    }

                    if (psgException.ApiStatusCode == 401 || psgException.ApiStatusCode == 403)
                    {
                        string user = null;

                        if (userService != null)
                            user = $"{userService.User.GetUserActiveDbId()} ({userService.User.GetUserRole()})";

                        await LogErrorWithApiResultAsync(context, psgException, user, token);

                        await WriteResponseContentAsync(context, ApiResult.FromException(exception), psgException.ApiStatusCode);
                        return;
                    }

                    await LogErrorAsync(context, psgException, token);

                    if (psgException.LogInnerException)
                    {
                        Exception innerException = psgException;

                        while ((innerException = innerException.InnerException) != null)
                        {
                            _log.LogError(innerException, "{message}", innerException.Message);
                        }
                    }

                    await WriteResponseContentAsync(context, ApiResult.FromException(exception), psgException.ApiStatusCode);
                    return;
                }
               
                //check if myPractice Api Client JWT error
                if (exception is HttpResponseException ex)
                {
                    if (ex.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        if(ex.Response.ReasonPhrase.ToLower().Contains("myPractice Api Client".ToLower()))
                        {
                            var msg = "Please check myPracticeClient token!";

                            _log.LogError(msg);

                            Exception innerException = ex;

                            while ((innerException = innerException.InnerException) != null)
                            {
                                _log.LogError(innerException, "{message}", innerException.Message);
                            }

                            await LogErrorAsync(context, ex, token);
                            await WriteResponseContentAsync(context, ApiResult.FromException(ex, msg), StatusCodes.Status400BadRequest); 
                            return;
                        }
                    }
                }

                _log.LogError(exception, "Unhandled exception while executing path '{path}': {message} \nStackTrace: {stackTrace}",
                    context.Request.Path, exception.Message, exception.StackTrace);

                await WriteResponseContentAsync(context, ApiResult.FromException(exception), StatusCodes.Status500InternalServerError);
            }
            catch (PsgException psgException)
            {
                await LogErrorAsync(context, psgException, token);
                await WriteResponseContentAsync(context, ApiResult.FromException(psgException), psgException.ApiStatusCode);
            }
            catch (Exception ex)
            {
                await LogErrorAsync(context, ex, token);
                await WriteResponseContentAsync(context, ApiResult.FromException(ex), StatusCodes.Status500InternalServerError);
            }
        }

        private async Task LogErrorAsync(HttpContext context, Exception ex, string token)
        {
            await LogErrorAndPathAsync(context, ex);

            _log.LogError("token=[{token}]", token);

            LogApiResult(ex);
        }

        private async Task LogErrorWithApiResultAsync(HttpContext context, PsgException pex, string user, string token)
        {
            await LogErrorAndPathAsync(context, pex, user);

            _log.LogError("token=[{token}]", token);

            LogApiResult(pex);
        }

        private async Task LogErrorAndPathAsync(HttpContext context, Exception ex)
        {
            var path = GetPath(context);
            var body = await GetBodyAsync(context);

            _log.LogError(ex, "error-message=[{messsage}] error-request-path=[{method} {path}] body=[{body}]",
                                                                                     ex.Message, context.Request.Method, path, body);
        }

        private async Task LogErrorAndPathAsync(HttpContext context, Exception ex, string user)
        {
            var path = GetPath(context);
            var body = await GetBodyAsync(context);

            _log.LogError(ex, "error-message=[{messsage}] error-request-path=[{method} {path}] body=[{body}] user=[{user}]",
                                                                                     ex.Message, context.Request.Method, path, body, user);
        }
       

        private void LogApiResult(Exception ex)
        {
            var result = ApiResult.FromException(ex);
            var json = JsonConvert.SerializeObject(result);
            _log.LogError("JSON={json}", json);
        }

        private string GetPath(HttpContext context)
        {
            return context.Request.Scheme + @"://" + context.Request.Host + context.Request.Path;
        }

        private async Task<string> GetBodyAsync(HttpContext context)
        {
            if (context.Request.ContentLength > 0 && context.Request.Method is "POST" or "PUT")
            {
                var body = await ReadBodyFromContextAsync(context);

                if (!string.IsNullOrEmpty(body))
                {
                    return body;
                }
            }

            return "NONE";
        }

        private async Task<string> ReadBodyFromContextAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            context.Request.Body.Position = 0;

            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();

            // reset body for next read in pipeline
            context.Request.Body.Position = 0;

            return body;
        }

        private static async Task WriteResponseContentAsync(HttpContext context, object result, int code)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = code;
            await response.WriteAsync(JsonConvert.SerializeObject(result));
        }



    }
}
