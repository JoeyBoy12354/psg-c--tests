using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_common.Contracts
{
    public interface IReportWriter
    {
        Task GenerateReportAsync(List<TestResultDto> models);
    }
}
