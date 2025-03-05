using Microsoft.Extensions.Configuration;
using Psg.Standardised.Api.Common.Exceptions;

namespace Psg.Standardised.Api.Common.Services
{
    public class FeatureFlagService : IFeatureFlagService
    {
        private readonly IConfiguration _config;
        private readonly IRequestContextService _requestContext;

        public FeatureFlagService(IConfiguration config, IRequestContextService requestContext)
        {
            _config = config;
            _requestContext = requestContext;
        }

        public bool FeatureIsEnabled(string featureId)
        {
            try
            {
                string val = _config.GetRequiredSection("FeatureFlags")
                    .GetRequiredSection(featureId)
                    .Value;

                return bool.Parse(val);
            }
            catch (Exception ex)
            {
                throw new PsgException($"FeatureFlags section or featureId '{featureId}' not defined.", ex);
            }
        }

        public bool ShouldRemapPageNumber()
        {
            var headerVal = _requestContext.GetRequestHeaderValue("x-api-remappagenumber");

            if (headerVal != null)
            {
                return headerVal.Equals("1", StringComparison.OrdinalIgnoreCase) || headerVal.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }

    public interface IFeatureFlagService
    {
        /// <summary>
        /// Checks whether a certain feature is enabled or disabled using the specified feature id.
        /// </summary>
        /// <param name="featureId">Id of the ticket, feature or project as defined in local or global configuration.</param>
        bool FeatureIsEnabled(string featureId);

        /// <summary>
        /// Indicates that an offset value was passed into PageNumber instead of the real page number and that we need to apply a conversion to fix it.
        /// </summary>
        bool ShouldRemapPageNumber();
    }
}
