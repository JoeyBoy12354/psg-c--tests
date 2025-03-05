using Newtonsoft.Json;
using psg_automated_nunit_common.Configurations;
using psg_automated_nunit_common.Contracts;
using psg_automated_nunit_common.Helpers;
using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_common.Services
{
    public class FileStorageService : IStorageService
    {
        public bool Enabled { get; set; }

        private readonly FileStorageConfiguration _configuration;
        private readonly string _filePath;

        private const string fileName = "data.json";

        public FileStorageService(FileStorageConfiguration configuration)
        {
            _configuration = configuration;
            Enabled = _configuration.Enabled;

            var path = "TestResults";   //default

            if (!string.IsNullOrWhiteSpace(_configuration.Folder))
                path = _configuration.Folder;

            var baseFolder = FileHelper.GetFolderPath(path);
            _filePath = Path.Combine(baseFolder, fileName);

            if (!File.Exists(_filePath))
                File.Create(_filePath).Close();
        }


        public async Task<IEnumerable<TestResultDto>> GetAllAsync()
        {
            var data = await File.ReadAllTextAsync(_filePath);

            var results = JsonConvert.DeserializeObject<TestResultDto[]>(data) ?? [];

            return results;
        }

        public async Task SaveAllAsync(IEnumerable<TestResultDto> models)
        {
            if (!_configuration.Enabled)
                return;

            var data = JsonConvert.SerializeObject(models);

            await File.WriteAllTextAsync(_filePath, data);
        }

        public async Task ClearSavedResultsAsync()
        {
            if (!_configuration.Enabled)
                return;

            await File.WriteAllTextAsync(_filePath, "");
        }
    }
}
