using Microsoft.Playwright;
using NUnit.Framework.Interfaces;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_shared.Managers
{
    /// <summary>
    /// Manages Screenshots.
    /// </summary>
    public sealed class ScreenshotManager
    {
        private readonly TestConfiguration _config;
        private readonly string _baseFolder = "";
        private volatile int _count = 0;

        private readonly Dictionary<string, ScreenShot?> _screenshots = [];


        public ScreenshotManager()
        {
            _config = DependencyManager.GetRequiredService<TestConfiguration>();

            if (_config.Screenshots)
            {
                var path = "screenshots";   //default

                if (!string.IsNullOrWhiteSpace(_config.ScreenshotPath))
                    path = _config.ScreenshotPath;

                _baseFolder = FileHelper.GetFolderPath(path);
            }
        }

        /// <summary>
        /// Store a screenshot in memory. When the test has completed, it should be Saved to disk, 
        /// <br/> using method <c>SaveScreenshotsAsync</c>, in the test's <c>Teardown</c> method.      
        /// </summary>     
        /// <returns></returns>
        public async Task StoreScreenShotAsync(IPage page, string? key)
        {
            if (!_config.Screenshots || string.IsNullOrWhiteSpace(key))
                return;

            var imageData = await page.ScreenshotAsync(new()
            {
                FullPage = true,
            });

            var screenshot = new ScreenShot(key, imageData);

            _screenshots[key] = screenshot;
        }

        /// <summary>
        /// Saves screenshots stored in memory, to disk. If key is not found, method will just return.
        /// <br/> The key will be used as filename. The test status will be appended to the filename.
        /// </summary>     
        /// <returns></returns>
        public async Task SaveScreenshotsAsync(string? key, TestStatus status)
        {
            if (!_config.Screenshots || string.IsNullOrWhiteSpace(key))
                return;

            try
            {
                var screenShot = GetScreenShot(key);

                if (screenShot == null)
                    return;

                var imageData = screenShot.ImageData;

                if (imageData == null)
                    return;

                _count++;

                string filePath;

                if (_config.ErrorScreenshotsOnly && status != TestStatus.Failed)
                {
                    return;
                }

                var statusString = status.ToString();

                if (status == TestStatus.Failed)
                    statusString = statusString.ToUpper();

                var fileName = $"{_count}_{key}_{statusString}.png";

                filePath = Path.Combine(_baseFolder, fileName);

                await File.WriteAllBytesAsync(filePath, imageData);

            }
            catch(Exception ex)
            {
                LogHelper.LogError(ex);
            }
            finally
            {
                ClearScreenShot(key);
            }
        }

        public ScreenShot? GetScreenShot(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (!_screenshots.TryGetValue(key, out var value))
                return null;

            return value;
        }

        public void ClearScreenShot(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (!_screenshots.ContainsKey(key))
                return;

            _screenshots[key] = null;
        }

    }
}
