using Microsoft.AspNetCore.Mvc;
using Refit;

namespace psg_automated_nunit_common.Refit
{
    public interface IAuthorisationApi
    {
        [Post("/service/token-validate/access")]
        Task<ApiResponse<string>> ValidateTokenAccessAsync([FromQuery] string token,
                                                           [HeaderCollection] IDictionary<string, string> headers);
    }
}
