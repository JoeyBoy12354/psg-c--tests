using psg_automated_nunit_shared.Models;
using Refit;

namespace psg_automated_nunit_shared.Refit
{
    public interface ITestApi
    {
     
        [Post($"/TestQA/TestResultsRobotBulk")]
        Task<ApiResponse<string>> UploadResultsQaAsync([HeaderCollection] IDictionary<string, string> headers,
                                                       [Body] IEnumerable<TestResultDto> models);

        [Post($"/Test/TestResultsRobotBulk")]
        Task<ApiResponse<string>> UploadResultsProdAsync([HeaderCollection] IDictionary<string, string> headers,
                                                         [Body] IEnumerable<TestResultDto> models);
    }
}
