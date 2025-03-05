using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Psg.Common.Registrations.Serilog
{
    internal class SerilogPsgTraceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SerilogPsgTraceMiddleware> _logger;

        public SerilogPsgTraceMiddleware(RequestDelegate next, ILogger<SerilogPsgTraceMiddleware> logger)
        {
            _next = next;
            _logger = logger;

        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var uri = $"{context.Request.Method} {context.Request.Host}{context.Request.Path}";

                using var _ = SerilogTraceHelper.GetTraceToSharedLogger();

                using var activity = SerilogTraceHelper.StartActivity("Calling {RequestPath}", uri);

                try
                {
                    await _next(context);

                    activity.Complete();
                }
                catch (Exception ex)
                {
                    activity.Complete(LogEventLevel.Error, ex);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Trace middleware: {Message}", ex.Message);

                // await _next(context);
            }
        }
    }


}
