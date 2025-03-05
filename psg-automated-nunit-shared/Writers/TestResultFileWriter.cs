using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.Managers;
using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_shared.Writers
{
    /// <summary>
    /// Writes test results to file.
    /// </summary>
    public class TestResultFileWriter : ITestResultWriter
    {
        public bool Enabled { get; set; }

        private readonly FileWriterConfiguration _fileWriterConfiguration;
        private readonly string _baseFolder;

        public TestResultFileWriter()
        {
            try
            {
                _fileWriterConfiguration = DependencyManager.GetRequiredService<FileWriterConfiguration>();
                Enabled = _fileWriterConfiguration.Enabled;

                var path = "TestResults";   //default

                if (!string.IsNullOrWhiteSpace(_fileWriterConfiguration.Folder))
                    path = _fileWriterConfiguration.Folder;

                _baseFolder = FileHelper.GetFolderPath(path);

                LogHelper.LogInfo("Writing logs to {Folder}", _baseFolder);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, "Error in {Writer}", nameof(TestResultFileWriter));
            }
          
        }
       

        public async ValueTask SaveAsync(IEnumerable<TestResultDto> results)
        {
            try
            {
                var fileName = Path.Combine(_baseFolder, $"test_results.txt");

                var data = ReportHelper.PrepareReport(results);

                await File.WriteAllTextAsync(fileName, data);   

            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }

        
    }
}
