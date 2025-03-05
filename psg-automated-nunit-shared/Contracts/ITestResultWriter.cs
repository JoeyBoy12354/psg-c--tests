using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_shared.Contracts
{
    /// <summary>
    /// Use this to build writers to save Test results.
    /// </summary>
    public interface ITestResultWriter
    {
        bool Enabled { get; set; }
        ValueTask SaveAsync(IEnumerable<TestResultDto> results);
    }
}
