using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Psg.Standardised.Api.Common.Exceptions;
using psg_automated_nunit_common.Configurations;
using psg_automated_nunit_common.Contracts;

namespace psg_automated_nunit_common.Services
{
    public sealed class AuthorisationService : IAuthorisationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientSecretConfiguration _clientSecretConfiguration;
        private readonly IAuthorisationClient _authorisationClient;


        public AuthorisationService(IHttpContextAccessor httpContext,
                                    ClientSecretConfiguration clientSecretConfiguration,
                                    IAuthorisationClient authorisationClient)
        {
            _httpContextAccessor = httpContext;
            _clientSecretConfiguration = clientSecretConfiguration;
            _authorisationClient = authorisationClient;
        }

        public async Task<bool> CheckAuthorisationAsync()
        {
            var headers = _httpContextAccessor?.HttpContext?.Request.Headers;

            if (headers == null)
                throw new PsgException("Cannot Authorise, No Header on request!", null, StatusCodes.Status401Unauthorized);

            string secret = headers["X-Api-Secret"].ToString();

            // validate on secret first
            if (!string.IsNullOrWhiteSpace(secret))
            {
                if (secret == _clientSecretConfiguration.Secret)
                {
                    return true;
                }
                else
                {
                    throw new PsgException("X-Api-Secret Invalid!", null, StatusCodes.Status401Unauthorized);
                }
            }

            // if no secret, validate token
            string accessToken = headers[HeaderNames.Authorization].ToString().Replace($"{JwtBearerDefaults.AuthenticationScheme} ", string.Empty);

            var valid = await _authorisationClient.ValidateTokenAsync(accessToken);

            if (!valid)
                throw new PsgException("Invalid Token!", null, StatusCodes.Status401Unauthorized);

            return true;
        }
    }
}
