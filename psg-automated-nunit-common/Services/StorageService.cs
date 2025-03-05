using Microsoft.Extensions.Logging;
using psg_automated_nunit_common.Contracts;
using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_common.Services
{
    public sealed class StorageService
    {

        private readonly List<IStorageService> _storageServices;
        private readonly ILogger<StorageService> _logger;

        private bool _enabled => _storageServices.Any();

        public StorageService(IEnumerable<IStorageService> storageServices,
                              ILogger<StorageService> logger)
        {
            _storageServices = storageServices.ToList();
            _logger = logger;
        }

        public async Task<List<TestResultDto>> GetAllAsync()
        {
            List<TestResultDto> results = [];

            if (!_enabled || _storageServices.Count == 0)
                return results;

            // only get from the first enabled entry, if more than one
            var service = _storageServices.Find(x => x.Enabled);

            if (service == null)
                return results;

            var data = await service.GetAllAsync();

            if (data == null)
                return results;

            results = data.ToList();

            return results;
        }

        public async Task SaveAllAsync(IEnumerable<TestResultDto> models)
        {
            // save to all storage mediums ...

            foreach (var service in _storageServices)
            {
                try
                {
                    await service.SaveAllAsync(models);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{message}", ex.Message);
                }
            }
        }

        public async Task ClearSavedResultsAsync()
        {
            // clear from all storage mediums ...

            foreach (var service in _storageServices)
            {
                try
                {
                    await service.ClearSavedResultsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{message}", ex.Message);
                }
            }
        }
    }
}
