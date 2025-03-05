using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.Managers;
using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_shared.Writers
{
    /// <summary>
    /// Writes test results to SEQ.
    /// <br/> SEQ logs can be used to send emails.
    /// </summary>
    public class TestResultLogWriter : ITestResultWriter
    {
        public bool Enabled { get; set; }      

        private readonly LogWriterConfiguration? _fileLogConfiguration;
      
      

        public TestResultLogWriter()
        {
            try
            {
                _fileLogConfiguration = DependencyManager.GetRequiredService<LogWriterConfiguration>();
                Enabled = _fileLogConfiguration.Enabled;               
            }
            catch(Exception ex) 
            {
                LogHelper.LogError(ex, "Error in {Writer}", nameof(TestResultLogWriter));
            }                
        }

       

        public async ValueTask SaveAsync(IEnumerable<TestResultDto> results)
        {
            try
            {
                foreach (var result in results) 
                {
                    if(result.Status == "Passed")
                    {
                        LogHelper.LogInfo("{Type} test Passed: [{Test}], Status: {Status}", result.Type, result.TestName, result.Status);
                        continue;
                    }

                    LogHelper.LogError("{Type} test Failed: [{Test}], Status: {Status}", result.Type, result.TestName, result.Status);
                }

                await Task.CompletedTask;

            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }

        
    }
}
