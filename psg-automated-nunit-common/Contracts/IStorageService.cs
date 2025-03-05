using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_common.Contracts
{
    public interface IStorageService
    {
        bool Enabled { get; set; }
        Task SaveAllAsync(IEnumerable<TestResultDto> models);
        Task<IEnumerable<TestResultDto>> GetAllAsync();
        Task ClearSavedResultsAsync();
    }
}
