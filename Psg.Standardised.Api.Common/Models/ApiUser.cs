using Microsoft.AspNetCore.Http;
using Psg.Standardised.Api.Common.Exceptions;
using Psg.Standardised.Api.Common.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Psg.Standardised.Api.Common.Models
{
    public class ApiUser
    {
        private readonly JwtSecurityToken _jwtSecurityToken;

        private string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    if (_jwtSecurityToken == null)
                        throw new PsgException($"Missing or empty Authorization header value.", null, StatusCodes.Status401Unauthorized);

                    _name = _jwtSecurityToken.Claims.FirstOrDefault(w => w.Type == "unique_name")?.Value;
                }

                return _name;
            }
        }

        private string _id;
        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                    _id = Name.Split('@').First();

                return _id;
            }
        }

        public string Jwt { get; }

        public string Secret { get; }

        public ApiUser(IRequestContextService requestContext)
        {
            var authValue = requestContext.GetRequestHeaderValue("Authorization");
            try
            {
                if (!string.IsNullOrEmpty(authValue))
                {
                    Jwt = authValue.ToString().Split(' ').LastOrDefault();
                    _jwtSecurityToken = new JwtSecurityToken(Jwt);
                }
            }
            catch (Exception ex)
            {
                throw new PsgException($"Invalid Authorization header value '{authValue}'.", ex, StatusCodes.Status401Unauthorized);
            }

            Secret = requestContext.GetRequestHeaderValue("X-Api-Secret");
        }

        /// <summary>
        /// Returns the value for the 'aro' JWT claim.
        /// </summary>
        public string GetUserRole()
        {
            if (_jwtSecurityToken == null)
                throw new PsgException($"Missing or empty Authorization header value.", null, StatusCodes.Status401Unauthorized);

            foreach (Claim cc in _jwtSecurityToken.Claims.ToList())
            {
                if (cc.Type == "aro")
                {
                    return cc.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the value for the 'aid' JWT claim.
        /// </summary>
        public int GetUserActiveDbId()
        {
            if (_jwtSecurityToken == null)
                throw new PsgException($"Missing or empty Authorization header value.", null, StatusCodes.Status401Unauthorized);

            foreach (Claim cc in _jwtSecurityToken.Claims.ToList())
            {
                if (cc.Type == "aid")
                {
                    return int.Parse(cc.Value);
                }
            }

            return 0;
        }

        /// <summary>
        /// Returns the value for the 'uid' JWT claim.
        /// </summary>
        public int GetUserDbId()
        {
            if (_jwtSecurityToken == null)
                throw new PsgException($"Missing or empty Authorization header value.", null, StatusCodes.Status401Unauthorized);

            foreach (Claim cc in _jwtSecurityToken.Claims.ToList())
            {
                if (cc.Type == "uid")
                {
                    return int.Parse(cc.Value);
                }
            }

            return 0;
        }
    }
}
