using psg_automated_nunit_shared.Models;
using Refit;

namespace psg_automated_nunit_shared.Refit
{
    public interface IAzureExternalTokenProviderApi
    {
        [Post("/v1/ExternalTokenProvider/GenerateTokenAsync")]
        Task<ApiResponse<string>> GenerateTokenAsync([HeaderCollection] IDictionary<string, string> headers,
                                                     [Body] GenerateTokenModel model);

        
       
        [Get("/v1/CpbCosting/GetCpbCost")]
        Task<ApiResponse<string>> GetCpbCostAsync([HeaderCollection] IDictionary<string, string> headers);


        [Get("/v1/CpbCosting/GenerateConsolidatedCpbCostReport")]
        Task<ApiResponse<string>> GenerateConsolidatedCpbCostReportAsync([HeaderCollection] IDictionary<string, string> headers);


        [Get("/v1/FICAReport/GenerateFICAEventReport")]
        Task<ApiResponse<string>> GenerateFICAEventReportAsync([HeaderCollection] IDictionary<string, string> headers,
                                                               string eventId);

        [Post("/v1/VerificationEvent/VerifyAll")]
        Task<ApiResponse<string>> VerifyAllAsync([HeaderCollection] IDictionary<string, string> headers,
                                                              [Body] VerifyAllModel model);


        [Get("/v1/VerificationEvent/GetVerificationEventsByDateRange")]
        Task<ApiResponse<string>> GetVerificationEventsByDateRangeAsync([HeaderCollection] IDictionary<string, string> headers,
                                                                         string startDate,
                                                                         string endDate);

        [Put("/v2/Address/OverrideBestMatch")]
        Task<ApiResponse<string>> OverrideBestMatchAsync([HeaderCollection] IDictionary<string, string> headers,
                                                                 string matchingDetailsId);


    }
}
