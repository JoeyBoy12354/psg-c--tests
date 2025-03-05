using Microsoft.Extensions.Logging;
using psg_automated_nunit_common.Configurations;

namespace psg_automated_nunit_common.Services
{
    public class TestFileProvider
    {
        public string TestFilePath => _testFilePath ?? "";

        private string _testFilePath;

        private readonly ILogger<TestFileProvider> _logger;
        private readonly TestRunnerConfiguration _testConfiguration;

        public TestFileProvider(ILogger<TestFileProvider> logger,
                               TestRunnerConfiguration testConfiguration)
        {
            _logger = logger;
            _testConfiguration = testConfiguration;

            SetFile();
        }

        private void SetFile()
        {
            if (string.IsNullOrWhiteSpace(_testConfiguration.TestDllPath))
            {
                _logger.LogError("No TestDllPath was specified in the TestConfiguration!");
                return;
            }

            _logger.LogInformation("TestDllPath was specified in the TestConfiguration: {file}", _testConfiguration.TestDllPath);

            if (!File.Exists(_testConfiguration.TestDllPath))
            {
                _logger.LogError("An Invalid TestDllPath was specified in the TestConfiguration!");
                return;
            }

            _testFilePath = _testConfiguration.TestDllPath;
            _logger.LogInformation("TestDllPath set to: {file}", _testConfiguration.TestDllPath);

        }      

    }
}
