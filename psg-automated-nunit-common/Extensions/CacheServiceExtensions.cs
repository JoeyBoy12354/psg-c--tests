using Psg.Standardised.Api.Common.Services;
using psg_automated_nunit_common.Constants;
using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_common.Extensions
{
    public static class CacheServiceExtensions
    {
        public static Dictionary<string, TestResultDto> GetTestResults(this CacheService _cacheService)
        {
            var cache = _cacheService.Get<Dictionary<string, TestResultDto>>(TestResultsKey.Value) ?? [];

            return cache;
        }
    }
}
