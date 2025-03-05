using psg_automated_nunit_common.Models;

namespace psg_automated_nunit_common.Contracts
{
    public interface IProcessService
    {
        Task<TestRunnerResult> ExecuteProcessAsync(string argument, string? testFilePath);
    }
}
