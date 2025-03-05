using Microsoft.Playwright;
using Newtonsoft.Json;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.PageExtensions;

namespace psg_automated_nunit_shared.Managers
{
    /// <summary>
    /// Manages Playwright browser context. 
    /// <br/>Use <c>GetLoggedInBrowserContextAsync</c> to get an isolated loggedIn instance of a browser context.
    /// </summary>
    public static class PlaywrightManager
    {
        public static bool IsLoggedIn = false;     

        public static string LoginStoragePath = "";

        public static string FicaToken = "";
        public static bool hasFicaToken => !string.IsNullOrEmpty(FicaToken);

        private static IPlaywright? _playwright = null;
        private static IBrowser _browser = null!;
        private static List<IBrowserContext> _contexts = [];

        private static TestConfiguration? _config = null;
        private static string? _baseFolder = null;

        private const string _folder = "storage";
        private const string _loginFilename = "login.json";


        public static async Task SetupAsync()
        {
            if (_config == null)
            {
                _config = DependencyManager.GetRequiredService<TestConfiguration>();

                if(_config == null)
                    throw new Exception("_config is Null!");
                var json = JsonConvert.SerializeObject(_config);
                LogHelper.LogInfo(json);
            }

            if (_playwright == null)
            {
                _playwright = await Playwright.CreateAsync();
                
                if(_playwright == null)
                    throw new Exception("Playwright was not created is Setup!");                 

                _browser = await _playwright.Chromium.LaunchAsync(new()
                {
                    Headless = _config?.Headless

                });

                if(_browser == null)
                    throw new Exception("Browser was not created in Setup!");
            }

            if (_baseFolder == null)
            {
                _baseFolder = FileHelper.GetFolderPath(_folder);

                LoginStoragePath = Path.Combine(_baseFolder, _loginFilename);
            }
        }
        

        /// <summary>
        /// Gets a New isolated browser instance, use this if you want to login
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<IBrowserContext> GetNewBrowserContextAsync()
        {
            if (_playwright == null)
                throw new Exception("Playwright was not created!");

            if (_browser == null)
                throw new Exception("The Browser was not created!");

            var context = await _browser.NewContextAsync();

            _contexts.Add(context);

            return context;
        }

        /// <summary>
        /// Logs directly into myPractice and returns the context
        /// </summary>
        /// <returns></returns>
        public static async Task<IBrowserContext> LoginMyPracticeAndGetContextAsync()
        {
            var context = await GetNewBrowserContextAsync();

            var page = await context.NewPageAsync();

            await page.LoginMyPracticeAsync(_config!);

            return context;
        }


        // disabled for now

        ///// <summary>
        ///// Gets an isolated browser instance where the user is already logged in. Use this for all tests.
        ///// <br/> Context will be an empty page, so you you still have to naivigate to your starting page first
        ///// <br/> ex. <c>https://mypractice.psg.co.za/</c>
        ///// </summary>
        ///// <returns></returns>
        ///// <exception cref="Exception"></exception>
        //public static async Task<IBrowserContext> GetLoggedInBrowserContextAsync()
        //{
        //    if (_playwright == null)
        //        throw new Exception("Playwright was not created!");

        //    if (_browser == null)
        //        throw new Exception("The Browser was not created!");

        //    if (!IsLoggedIn)
        //        throw new Exception("The user is not loggin in! Please check if the GlobalLogin.cs executed correctly!");


        //    var context = await _browser.NewContextAsync(new()
        //    {
        //        StorageStatePath = LoginStoragePath,
        //        AcceptDownloads = true
        //    });

        //    _contexts.Add(context);

        //    return context;
        //}

        public static async ValueTask ClearContextsAsync()
        {
            foreach (var context in _contexts)
            {
                if (context != null)
                {
                    await context.DisposeAsync();
                }
            }

            _contexts = new();
        }

        public static async ValueTask TeardownAsync()
        {
            try
            {
                foreach (var context in _contexts)
                {
                    if (context != null)
                    {
                        await context.DisposeAsync();
                    }
                }

                if (_browser != null)
                {
                    await _browser.DisposeAsync();
                }

                if (_playwright != null)
                {
                    _playwright.Dispose();
                }

                IsLoggedIn = false;
            }
            catch (Exception ex) 
            { 
                LogHelper.LogError(ex);
            }          
        }

        public static void ClearLoginState()
        {
            ClearStorageState(LoginStoragePath);
        }


        public static async Task SaveLoginStateAsync(IBrowserContext context)
        {
            await SaveStorageStateAsync(context, LoginStoragePath);
        }

        public static async Task SaveStorageStateAsync(IBrowserContext context, string filePath)
        {
            await context.StorageStateAsync(new()
            {
                Path = filePath
            });
        }

        public static void ClearStorageState(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
