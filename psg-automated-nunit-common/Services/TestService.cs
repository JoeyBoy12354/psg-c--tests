using Microsoft.Extensions.Caching.Memory;
using Psg.Standardised.Api.Common.Configurations;
using Psg.Standardised.Api.Common.Services;
using psg_automated_nunit_common.Constants;
using psg_automated_nunit_common.Contracts;
using psg_automated_nunit_common.Extensions;
using psg_automated_nunit_common.Models;
using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_common.Services
{
    public sealed class TestService : ITestService
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly CacheService _cacheService;
        private readonly MemoryCacheEntryOptions _options;
        private readonly StorageService _storageService;
        private readonly IReportService _reportService;

        public TestService(CacheService cacheService,
                           TestCacheConfiguration config,
                           IAuthorisationService authorisationService,
                           StorageService storageManager,
                           IReportService reportService)
        {
            _cacheService = cacheService;
            _options = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(config.AbsoluteExpirationHours),
                Size = 1
            };
            _authorisationService = authorisationService;
            _storageService = storageManager;
            _reportService = reportService;
        }

        public async Task AddAsync(IEnumerable<TestResultDto> models)
        {
            await _authorisationService.CheckAuthorisationAsync();

            var cache = AddCache(models);

            //save full cache to storage
            await _storageService.SaveAllAsync(cache);

            // genrate report
            await _reportService.GenerateReportAsync(cache);
        }

        private List<TestResultDto> AddCache(IEnumerable<TestResultDto> models)
        {
            var cache = _cacheService.GetTestResults();

            foreach (var model in models)
            {
                // add or update cache
                cache[model.Key] = model;
            }

            _cacheService.Add(TestResultsKey.Value, cache, _options);

            // return full cache as list
            var data = cache.Select(x => x.Value).ToList();

            return data;
        }



        public async Task<TestResults> GetResultsAsync()
        {
            await _authorisationService.CheckAuthorisationAsync();

            TestResults results = new();

            var cache = _cacheService.GetTestResults()?.Select(x => x.Value).ToList() ?? [];

            results.Total = cache.Count;
            results.Passed = cache.Count(x => x.Status == "Passed");
            results.Failed = cache.Count(x => x.Status == "Failed");

            return results;
        }

        public async Task ReloadCacheAsync()
        {
            var data = await _storageService.GetAllAsync();
            AddCache(data);

            // generate report
            await _reportService.GenerateReportAsync(data);
        }

        public void ClearCache()
        {
            Dictionary<string, TestResultDto> emptyData = [];
            _cacheService.Add(TestResultsKey.Value, emptyData, _options);
        }

        public async Task ClearSavedResultsAsync()
        {
            await _storageService.ClearSavedResultsAsync();
        }
    }
}
