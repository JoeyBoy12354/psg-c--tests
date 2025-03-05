using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Psg.Standardised.Api.Common.Exceptions;
using Psg.Standardised.Api.Common.Services;
using psg_automated_nunit_common.Configurations;
using psg_automated_nunit_common.Contracts;
using psg_automated_nunit_common.Models;
using psg_automated_nunit_common.Refit;

namespace psg_automated_nunit_common.Services
{
    public sealed class AuthorisationClient : IAuthorisationClient
    {
        private readonly AuthorisationConfiguration _configuration;
        private readonly IAuthorisationApi _authorisationApi;
        private readonly CacheService _cacheService;

        public AuthorisationClient(AuthorisationConfiguration configuration,
                                   IAuthorisationApi authorisationApi,
                                   CacheService cacheService)
        {
            _configuration = configuration;
            _authorisationApi = authorisationApi;
            _cacheService = cacheService;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new PsgException("Token cannot be empty!", null, StatusCodes.Status401Unauthorized);
            }

            var cacheResults = _cacheService.Get<bool?>(token);

            if (cacheResults.HasValue)
            {
                if (!cacheResults.Value)
                    throw new PsgException("Invalid Token! (From Cache)", null, StatusCodes.Status401Unauthorized);

                return true;
            }

            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" },
                { "X-Api-Secret", _configuration.Secret }
            };

            var response = await _authorisationApi.ValidateTokenAccessAsync(token, headers);

            if (response == null)
                throw new PsgException("No response received from Auth API", null, StatusCodes.Status500InternalServerError);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _cacheService.Add(token, false);
                throw new PsgException("Auth Error", null, (int)response.StatusCode);
            }

            if (string.IsNullOrWhiteSpace(response.Content))
                throw new PsgException("Auth Error: Content empty", null, (int)response.StatusCode);

            var result = JsonConvert.DeserializeObject<AuthorisationResult>(response.Content);

            _cacheService.Add(token, result?.Result);

            if (result?.Result != true)
                throw new PsgException("Unauthorised", null, StatusCodes.Status401Unauthorized);

            return true;
        }
    }
}
