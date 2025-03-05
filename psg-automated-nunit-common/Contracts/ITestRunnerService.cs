using psg_automated_nunit_common.Models;

namespace psg_automated_nunit_common.Contracts
{
    public interface ITestRunnerService
    { 
        Task<object> GetTestsAsync(string? testFilePath);
        Task<TestRunnerResult> RunTestsAsync(string[]? body, string? commaDelimetedTestNames, string? testFilePath);
    }
}
