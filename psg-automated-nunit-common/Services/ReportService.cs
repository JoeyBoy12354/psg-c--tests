using psg_automated_nunit_common.Contracts;
using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_common.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportWriter _writer;

        public ReportService(IReportWriter writer)
        {
            _writer = writer;
        }

        public async Task GenerateReportAsync(List<TestResultDto> models)
        {
            await _writer.GenerateReportAsync(models);
        }
    }
}
