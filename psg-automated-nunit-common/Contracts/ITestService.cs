using psg_automated_nunit_common.Models;
using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_common.Contracts
{
    public interface ITestService
    {
        Task AddAsync(IEnumerable<TestResultDto> models);
        Task<TestResults> GetResultsAsync();
        Task ReloadCacheAsync();
        void ClearCache();
        Task ClearSavedResultsAsync();
    }
}
