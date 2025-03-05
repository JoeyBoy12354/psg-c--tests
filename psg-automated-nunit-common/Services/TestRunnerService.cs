using psg_automated_nunit_common.Contracts;
using psg_automated_nunit_common.Models;

namespace psg_automated_nunit_common.Services
{
    public class TestRunnerService : ITestRunnerService
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly IProcessService _processService;

        public TestRunnerService(IProcessService processService,
                                 IAuthorisationService authorisationService)
        {
            _processService = processService;
            _authorisationService = authorisationService;
        }

        public async Task<object> GetTestsAsync(string? testFilePath)
        {
            await _authorisationService.CheckAuthorisationAsync();

            string filterArgument = $"-t";

            var result = await _processService.ExecuteProcessAsync(filterArgument, testFilePath);

            if (result.IsSucces)
            {
                var index = result.Output.IndexOf("available:");

                if (index > -1)
                {
                    var testString = result.Output.Substring(index, result.Output.Length - index);
                    var tests = testString.Split(Environment.NewLine);

                    if (tests.Length > 0)
                    {
                        return tests.Where(x => !string.IsNullOrWhiteSpace(x)
                                           && !x.Contains("available:"))
                                           .Select(x => x.Trim());
                    }
                }
            }

            return result;
        }


        public async Task<TestRunnerResult> RunTestsAsync(string[]? body,
                                                    string? commaDelimetedTestNames, 
                                                    string? testFilePath)
        {
            await _authorisationService.CheckAuthorisationAsync();

            var argument = (body?.Length > 0)? string.Join('|', body) : "";

            if(!string.IsNullOrWhiteSpace(commaDelimetedTestNames))
            {
                if (string.IsNullOrWhiteSpace(argument))
                {
                    argument = commaDelimetedTestNames.Replace(',', '|');
                }
                else
                {
                    argument = "|" + commaDelimetedTestNames.Replace(',', '|');
                }
            }               

            string filterArgument = $"--filter {argument}";

            // Create and start the process
            return await _processService.ExecuteProcessAsync(filterArgument, testFilePath);
        }



    }
}
