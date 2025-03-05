using Microsoft.Extensions.Logging;
using psg_automated_nunit_common.Contracts;
using psg_automated_nunit_common.Models;
using System.Diagnostics;

namespace psg_automated_nunit_common.Services
{
    public class DotNetProcessService : IProcessService
    {
        private readonly ILogger<DotNetProcessService> _logger;
        private readonly TestFileProvider _testFileProvider;

        public DotNetProcessService(ILogger<DotNetProcessService> logger,
                                      TestFileProvider testFileProvider)
        {
            _logger = logger;
            _testFileProvider = testFileProvider;
        }

        public async Task<TestRunnerResult> ExecuteProcessAsync(string argument, string? testFilePath)
        {
            string path;

            if(!string.IsNullOrEmpty(testFilePath)) 
            {
                _logger.LogInformation("Custom TestFilePath received on API Call: {path}", testFilePath);
                path = testFilePath;
            }
            else
            {
                path = _testFileProvider.TestFilePath;
            }

            if(!File.Exists(path)) 
            { 
                throw new FileNotFoundException(path);
            }

            // Create process start info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"test \"{path}\" {argument}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };           

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();

                // Read the output and error streams (optional)
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                await process.WaitForExitAsync();

                // Display output and error if needed
                _logger.LogInformation("Output: {message}", output);

                if (!string.IsNullOrWhiteSpace(error))
                {
                    _logger.LogError("Error: {message}", error);
                }

                // Check the exit code to see if the command was successful
                int exitCode = process.ExitCode;

                _logger.LogInformation("Exit Code:{code} ", exitCode);

                return new TestRunnerResult { Output = output, Error = error };
            }
        }

    }
}
