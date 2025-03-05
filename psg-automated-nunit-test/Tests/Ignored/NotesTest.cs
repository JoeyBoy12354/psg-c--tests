using Microsoft.Playwright;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Extensions;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.Managers;

namespace psg_automated_nunit_test.Tests.Ignored
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public sealed class NotesTest
    {

        private IBrowserContext? _context;
        private TestConfiguration _config;
        private ScreenshotManager _screenshotManager;
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);



        [SetUp]
        [Ignore("Robot tests used")]
        public void Setup()
        {
            _config = DependencyManager.GetRequiredService<TestConfiguration>();
            _screenshotManager = DependencyManager.GetRequiredService<ScreenshotManager>();
        }



        [Test, Order(1)]
        [Ignore("Robot tests used.")]
        public async Task Notes_Create_And_Save()
        {
            // arrange         

            _context = await PlaywrightManager.LoginMyPracticeAndGetContextAsync();

            var page = await _context.NewPageAsync();

            // act

            try
            {
                await page.GotoAsync(_config.GetPageMyPsg("portfolio"));

                await page.GetByText("myPractice").ClickAsync();

                try
                {
                    await page.GetByRole(AriaRole.Button, new() { Name = "Close window" }).ClickAsync();
                }
                catch
                {
                    // message box was not there
                }

                // Search for User
                await page.Locator("[id=\"__search\"]").ClickAsync();

                await page.Locator("[id=\"__search\"]").FillAsync(_config.SearchName);

                await page.Locator("[id=\"__search\"]").PressAsync("Enter");

                await page.GetByText($"{_config.SearchName} in Clients").ClickAsync();


                // select user from list

                await page.GetByRole(AriaRole.Row, new() { Name = $"1 {_config.Surname}, {_config.Name}" }).Locator("svg").ClickAsync();


                // click on Notes / Tasks button in tab
                await page.GetByText("Notes / Tasks").ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { Name = "Add new" }).ClickAsync();

                await page.GetByLabel("Primary product").SelectOptionAsync(new[] { "3423280" });

                await page.GetByLabel("Interaction").SelectOptionAsync(new[] { "2" });

                await page.GetByLabel("Subject").ClickAsync();

                await page.GetByLabel("Subject").FillAsync("Save And Test Note");

                await page.GetByLabel("Subject").ClickAsync();

                await page.GetByLabel("Subject").FillAsync("Create and Save Note Test");

                await page.GetByRole(AriaRole.Heading, new() { Name = "Add noteJohan Psg Website (" }).ClickAsync();

                await page.GetByLabel("Comment").ClickAsync();

                await page.GetByLabel("Comment").FillAsync("Test Note");

                await page.GetByRole(AriaRole.Heading, new() { Name = "OptionsClick to show" }).Locator("small").ClickAsync();


                await page.Locator("#pec_followup").ClickAsync();

                await page.GetByLabel("Follow-up", new() { Exact = true }).SelectOptionAsync(new[] { "1" });

                var dateInput = page.Locator("#pec_date_followup");

                await dateInput.ClickAsync();

                // select the day from the calendar

                await page.GetByRole(AriaRole.Cell, new() { Name = $"{_date.Day}", Exact = true }).First.ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { Name = "Save new note", Exact = true }).ClickAsync();

                await page.GetByRole(AriaRole.Heading, new() { Name = "View noteCreate and Save Note" }).ClickAsync();



                // assert

                var dateCheckString = _date.ToString("yyyy-MM-dd");

                var result = page.GetByText($"{dateCheckString}", new() { Exact = true });

                await result.ClickAsync();

                await Assertions.Expect(result).ToBeVisibleAsync();



            }
            finally
            {
                await StoreScreenShotAsync(page);
                await _context.CloseAsync();
            }
        }

        [Test, Order(2)]
        [Ignore("Robot tests used.")]
        public async Task Notes_Amend_Follow_Up_Date()
        {
            // arrange         

            _context = await PlaywrightManager.LoginMyPracticeAndGetContextAsync();

            var page = await _context.NewPageAsync();

            // act

            try
            {
                await page.GotoAsync(_config.GetPageMyPsg("portfolio"));

                await page.GetByText("myPractice").ClickAsync();

                try
                {
                    await page.GetByRole(AriaRole.Button, new() { Name = "Close window" }).ClickAsync();
                }
                catch
                {
                    // message box was not there
                }

                // Search for User
                await page.Locator("[id=\"__search\"]").ClickAsync();

                await page.Locator("[id=\"__search\"]").FillAsync(_config.SearchName);

                await page.Locator("[id=\"__search\"]").PressAsync("Enter");

                await page.GetByText($"{_config.SearchName} in Clients").ClickAsync();


                // select user from list

                await page.GetByRole(AriaRole.Row, new() { Name = $"1 {_config.Surname}, {_config.Name}" }).Locator("svg").ClickAsync();


                // click on Notes / Tasks button in tab
                await page.GetByText("Notes / Tasks").ClickAsync();


                var dateString = _date.ToString("yyyy-MM-dd");

                await page.GetByRole(AriaRole.Row, new() { Name = $"{dateString}" }).Locator("svg").First.ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { Name = "Amend" }).ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { Name = "Amend", Exact = true }).ClickAsync();

                await page.GetByRole(AriaRole.Heading, new() { Name = "OptionsClick to show" }).Locator("small").ClickAsync();


                _date = _date.AddDays(1);

                await page.Locator("#pec_date_followup").ClickAsync();

                // use the arrow to select the next day
                await page.Locator("#pec_date_followup").PressAsync("ArrowRight");
                await page.Locator("[id=\"__pec_date_followupwrapper\"] div").First.ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { Name = "Save amendment" }).ClickAsync();


                // assert               

                var dateCheckString = _date.ToString("yyyy-MM-dd");

                var result = page.GetByText($"{dateCheckString}", new() { Exact = true });

                await result.ClickAsync();

                await Assertions.Expect(result).ToBeVisibleAsync();


            }
            finally
            {
                await StoreScreenShotAsync(page);
                await _context.CloseAsync();
            }
        }


        public async Task StoreScreenShotAsync(IPage page)
        {
            var key = KeyHelper.GetKey(TestContext.CurrentContext.Test.DisplayName, TestContext.CurrentContext.Test.MethodName);

            await _screenshotManager.StoreScreenShotAsync(page, key);
        }

        [TearDown]
        public async Task Teardown()
        {
            await ResultsManager.AddResultAsync(TestContext.CurrentContext, 
                                                              _screenshotManager,
                                                              "NotesTest",
                                                              "",
                                                              false);

            if (_context != null)
            {
                await _context.CloseAsync();
                await _context.DisposeAsync();
            }
        }

    }
}