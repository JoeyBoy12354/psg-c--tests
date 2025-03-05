using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.IdentityModel.Tokens.Jwt;

namespace Psg.Common.Registrations.Serilog
{
    internal class SerilogPsgMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<SerilogPsgMiddleware> _logger;

        public SerilogPsgMiddleware(RequestDelegate next, ILogger<SerilogPsgMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Retrieve JWT token from HttpContext
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (!string.IsNullOrEmpty(token))
                {
                    // Parse JWT token
                    var jwtHandler = new JwtSecurityTokenHandler();
                    var jwtToken = jwtHandler.ReadJwtToken(token);

                    // Extract claims from JWT token
                    var uidClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
                    var uroClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "uro")?.Value;
                    var aidClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "aid")?.Value;
                    var aroClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "aro")?.Value;

                    // Push claims to Serilog enriched properties
                    if (!string.IsNullOrEmpty(uidClaim))
                        LogContext.PushProperty("uid", uidClaim);

                    if (!string.IsNullOrEmpty(uroClaim))
                        LogContext.PushProperty("uro", uroClaim);

                    if (!string.IsNullOrEmpty(aidClaim))
                        LogContext.PushProperty("aid", aidClaim);

                    if (!string.IsNullOrEmpty(aroClaim))
                        LogContext.PushProperty("aro", aroClaim);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SerilogPsgMiddleware: {Error}", ex.Message);
            }
            finally
            {
                await _next(context);
            }



        }
    }
}
