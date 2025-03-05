using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.Managers;
using psg_automated_nunit_shared.Models;
using psg_automated_nunit_shared.Refit;
using Refit;

namespace psg_automated_nunit_shared.Writers
{
    /// <summary>
    /// Writes test results to file.
    /// </summary>
    public class TestResultApiWriter : ITestResultWriter
    {
        public bool Enabled { get; set; }

        private readonly ApiWriterConfiguration _apiWriterConfiguration;
        private readonly ITestApi? _testApi;

        public TestResultApiWriter()
        {
            try
            {
                _apiWriterConfiguration = DependencyManager.GetRequiredService<ApiWriterConfiguration>() ?? new();
                Enabled = _apiWriterConfiguration.Enabled;

                _testApi = DependencyManager.GetService<ITestApi>();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, "Error in {Writer}", nameof(TestResultApiWriter));
            }
        }
       

        public async ValueTask SaveAsync(IEnumerable<TestResultDto> results)
        {
            try
            {
                if(_testApi == null)
                {                  
                    LogHelper.LogError("There was an error uploading the results to the api! TestApi interface was null!");
                    return;
                }

                LogHelper.LogInfo("Sending [{count}] results to API...", results.Count());

                var headers = new Dictionary<string, string>
                { 
                    { "X-Api-Secret", _apiWriterConfiguration.Secret }
                };

                // check logs
                var prodResults = results.Where(x => x.IsProd);
                var qaResults = results.Where(x => !x.IsProd);

                await SaveProdAsync(headers, prodResults);
                await SaveQaAsync(headers, qaResults);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }


        private async ValueTask SaveQaAsync(Dictionary<string, string> headers, IEnumerable<TestResultDto> results)
        {
            if (!results.Any())
                return;

            var response = await _testApi!.UploadResultsQaAsync(headers, results);

            VaidateResponse(response, results);
        }

        private async ValueTask SaveProdAsync(Dictionary<string, string> headers, IEnumerable<TestResultDto> results)
        {
            if (!results.Any())
                return;

            var response = await _testApi!.UploadResultsProdAsync(headers, results);

            VaidateResponse(response, results);
        }


        private void VaidateResponse(ApiResponse<string> response, IEnumerable<TestResultDto> results)
        {
            if (!response.IsSuccessStatusCode)
            {
                var path = response.RequestMessage?.RequestUri?.ToString();

                var rq = JsonConvert.SerializeObject(results);
                var error = "";

                var traceId = "";
                var spanId = "";
                var parentId = "";

                if (response.Error is ApiException ex)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(ex.Content))
                        {
                            var coreResult = JsonConvert.DeserializeObject<CoreResultModel>(ex.Content);

                            if (coreResult != null)
                            {
                                var meta = Array.Find(coreResult.MetaData, x => !string.IsNullOrWhiteSpace(x.TraceData?.TraceId));
                                traceId = meta?.TraceData.TraceId;
                                spanId = meta?.TraceData.SpanId;
                                parentId = meta?.TraceData.ParentId;
                            }
                        }

                    }
                    catch
                    {
                        // nothing here
                    }

                    error = JsonConvert.SerializeObject(ex.Content);

                }
                else
                {
                    error = JsonConvert.SerializeObject(response.Error);
                }

                if (!string.IsNullOrWhiteSpace(traceId))
                {
                    LogHelper.LogError("There was an error writing the results to Api! {message}, Path: {Path}, Request: {Request}, Error: {Error}, {TraceId} {SpanId} {ParentId}", response.Error?.Message, path, rq, error, traceId, spanId, parentId);

                }
                else
                {
                    LogHelper.LogError("There was an error writing the results to Api! {message}, Path: {Path}, Request: {Request}, Error: {Error}", response.Error?.Message, path, rq, error);

                }
            }

            var content = response.Content;

            LogHelper.LogInfo("API Content returned: [{content}]", content);
        }


      



    }
}
