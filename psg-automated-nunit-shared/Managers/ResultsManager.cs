using NUnit.Framework;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Global;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_shared.Managers
{
    /// <summary>
    /// Manages test results. Use the <c>AddResult</c> method to add test results, in the <c>Teardown</c> method of each test.
    /// </summary>
    public static class ResultsManager
    {
        private static readonly TestConfiguration? _config = DependencyManager.GetRequiredService<TestConfiguration>();
        private static bool _saveResults => _config?.SaveConfiguration?.SaveResults ?? false;

        private static readonly Dictionary<string, TestResultDto> _results = [];

        /// <summary>
        /// Adds test results.
        /// </summary>
        /// <returns></returns>
        public static async Task AddResultAsync(TestContext CurrentContext,
                                                ScreenshotManager screenshotManager,
                                                string name,
                                                string description,
                                                bool isProd)
        {
            if (!_saveResults)
                return;

            // remove underscores with Humanize
            var displayName = CurrentContext.Test.DisplayName;
            var testName = CurrentContext.Test.MethodName;

            var key = (!string.IsNullOrWhiteSpace(name)) ? name : KeyHelper.GetKey(displayName, testName);
            var status = CurrentContext.Result.Outcome.Status;

            var result = CurrentContext.Result;

            var record = new TestResultDto()
            {
                Key = key,
                Status = result.Outcome.Status.ToString(),
                TestName = testName,
                Description = description,
                Message = !string.IsNullOrWhiteSpace(result.Message) ? result.Message : null,
                StackTrace = !string.IsNullOrWhiteSpace(result.StackTrace) ? result.StackTrace : null,
                Date = DateTime.Now,

                Ip = GlobalHostData.Ip,
                Host = GlobalHostData.Host,
                User = GlobalHostData.User,

                IsProd = isProd
            };

            _results.TryAdd(key, record);

         

            await screenshotManager.SaveScreenshotsAsync(key, status);
        }

        public static List<TestResultDto> GetResults()
        {
            if (_results == null)
                return [];

            return _results.Select(x => x.Value).ToList();
        }

        /// <summary>
        /// Saves the results, if the <c>SaveResults</c> flag in the <c>SaveConfiguration</c> section in the appsettings is set to true.
        /// <br/><c>SaveConfiguration</c> section must contain at least 1 valid configured writer.
        /// <br/> Results will be saved to all valid configured writers.
        /// </summary>
        /// <returns></returns>
        public static async Task SaveResultsAsync()
        {
            try
            {
                if (!_saveResults)
                    return;

                var results = GetResults();

                // Get a list of configured writers
                var writers = DependencyManager.GetServices<ITestResultWriter>();

                foreach (var writer in writers.Where(x => x.Enabled))
                {
                    await writer.SaveAsync(results);
                }
            }
            catch (Exception ex) 
            { 
                LogHelper.LogError(ex);
            }
           
        }
    }
}
