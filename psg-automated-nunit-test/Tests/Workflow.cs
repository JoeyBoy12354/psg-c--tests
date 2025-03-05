using Microsoft.Playwright;
using Polly;
using Psg.Common.Registrations.Polly.Policies;
using psg_automated_nunit_shared.Attributes;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Contracts.Base;
using psg_automated_nunit_shared.Managers;
using psg_automated_nunit_shared.PageExtensions;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using psg_automated_nunit_test.Extensions;

namespace psg_automated_nunit_test.Tests
{

    // DO NOT ADD ANY PSG PACKAGES!!
    // JENKINS CANNOT CONNECT TO THE PSG PACKAGE SERVER TO DOWNLOAD THEM !!!!


    //[Parallelizable(ParallelScope.Fixtures)]
    [TestFixture, Order(9)]
    public sealed class Workflow : PsgTestBase
    {

        private readonly IAsyncPolicy _retryPolicy;
        private readonly TestConfiguration _testConfig;
        private readonly IOtpService _otpService;
        private readonly int _timeOutMs = 1000;//120_000;


        public Workflow() : base()
        {
            _retryPolicy = PollyPolicies.GetRetryPolicyAsync();
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }



        [Test]
        [Prod]
        public async Task Workflow_Create_adhoc_workflow()
        {
            // arrange
            Name = "Workflow - Create adhoc workflow";

            Description = "Assistant working on behalf of adviser to be able to create an old workflow type - ad hoc task and then be able to complete the workflow. DD-18809";

            context = await PlaywrightManager.LoginMyPracticeAndGetContextAsync();

            var page = await context.NewPageAsync();

            // act

            try
            {
                //await _retryPolicy.ExecuteAsync(async () =>
                //{
                    await page.GoToMyPracticeAsync(config, _otpService);


                    await page.GetByRole(AriaRole.Heading, new() { Name = $"{_testConfig.Name} {_testConfig.Surname} Logged in as" })
                                                                                                                      .ClickAsync(new() { Timeout = _timeOutMs });
                    await page.GetByText("Assistant").ClickAsync();
                    await Task.Delay(50);
                    await page.GetByRole(AriaRole.Heading, new() { Name = "Johan Psg Website Logged in" }).ClickAsync();
                    await Task.Delay(5000);
                    
                    // Check if #banner-menu-user exists
                    var bannerMenuUser = await page.Locator("#banner-menu-user").IsVisibleAsync();
                    if (bannerMenuUser)
                    {
                        await page.Locator("#banner-menu-user").GetByText("Working on behalf of All Users").ClickAsync(new() { Timeout = _timeOutMs });
                    }
                    else
                    {
                        await page.GetByText("Logged in as Assistant").ClickAsync();
                        await page.Locator("#banner-menu-user").GetByText("Working on behalf of All Users").ClickAsync(new() { Timeout = _timeOutMs });
                    }

                    await page.GetByText("Johan Support").ClickAsync();
                    await page.GetByRole(AriaRole.Heading, new() { Name = "Upcoming birthdays" }).ClickAsync();
                    await page.Locator("[id=\"__search\"]").ClickAsync();
                    await Task.Delay(50);
                    await page.Locator("[id=\"__search\"]").FillAsync("103045694082");
                    await page.GetByText("in Clients").ClickAsync();
                    await page.GetByRole(AriaRole.Row, new() { Name = "1 Client, Automation" }).Locator("path").ClickAsync();

                    //Dismiss Modal if present

                    bool isVisble = await TryWaitForLocatorVisible(page, page.Locator("#popup_1"));
                    if (isVisble)
                    {
                        await page.GetByRole(AriaRole.Button, new() { Name = "Close"}).IsVisibleAsync();
                        await page.GetByRole(AriaRole.Button, new() { Name = "Close" }).ClickAsync();
                    }

                    await page.Locator("li").Filter(new() { HasText = "Workflows" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Initiate new" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Use old workflows" }).ClickAsync();
                    await Task.Delay(50);
                    await page.GetByLabel("Workflow").SelectOptionAsync(new[] { "task" });
                    await Task.Delay(1000);
                    await page.GetByLabel("Service").SelectOptionAsync(new[] { "5" });
                    await Task.Delay(1000);
                    await page.GetByLabel("Note").ClickAsync();
                    await Task.Delay(1000);
                    await page.GetByLabel("Note").PressAsync("CapsLock");
                    await page.GetByLabel("Note").FillAsync("Testing creation of Adhoc task workflow");
                    await page.GetByRole(AriaRole.Button, new() { Name = "Submit >" }).ClickAsync();
                    await page.Locator("div").Filter(new() { HasTextRegex = new Regex("^Back$") }).Nth(2).ClickAsync();

                    bool searchForWorkflow = true;
                    while (searchForWorkflow)
                    {
                        //Wait for locator to become enabled
                        await Task.Delay(3000);
                        // Check if the element exists
                        var rowLocator = page.GetByRole(AriaRole.Row, new() { Name = "Investments Ad hoc task Testing creation of Adhoc task workflow Support, Johan None" }).Locator("path").First;

                        // Wait for the page to load and the element to be ready
                        var rowExists = await rowLocator.CountAsync() > 0;

                        if (rowExists)
                        {
                            var foundWorkflow = false;
                            try
                            {
                                foundWorkflow = await rowLocator.IsVisibleAsync();
                            }
                            catch (PlaywrightException e)
                            {
                                Console.WriteLine($"Error finding row: {e.Message}");
                                searchForWorkflow = false;
                            }

                            if (foundWorkflow)
                            {
                                // Perform actions
                                await rowLocator.ClickAsync();
                                await page.Locator("a").Filter(new() { HasText = "Complete workflow" }).ClickAsync();
                                await page.GetByRole(AriaRole.Button, new() { Name = "Ok" }).ClickAsync();
                                await page.Locator("div").Filter(new() { HasTextRegex = new Regex("^Back$") }).Nth(2).ClickAsync();
                                searchForWorkflow = false;
                            }
                        }
                        else
                        {
                            // Go to the next page if the workflow is not found
                            try
                            {
                                //Sometime 
                                await page.Locator("button[data-original-title='Next page']").ClickAsync();
                                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                            }
                            catch (PlaywrightException e)
                            {
                                Console.WriteLine($"Error navigating to next page: {e.Message}");
                                searchForWorkflow = false;
                            }
                        }
                    }
                //});
            }
            finally
            {
                await StoreScreenShotAsync(page);
                await context.CloseAsync();
            }
        }

        /// <summary>
        /// Waits for a locator to become visible within a specified timeout
        /// </summary>
        /// <param name="page">The Playwright Page</param>
        /// <param name="locator">The locator to wait for</param>
        /// <param name="timeoutInSeconds">Maximum wait time in seconds</param>
        /// <returns>True if locator becomes visible, False if timeout occurs</returns>
        public static async Task<bool> TryWaitForLocatorVisible(
            IPage page,
            ILocator locator,
            float timeoutInSeconds = 10)
        {
            try
            {
                // Wait for the locator with a specific timeout
                await locator.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = timeoutInSeconds * 1000 // Convert to milliseconds
                });
                return true;
            }
            catch (TimeoutException)
            {
                // Locator did not become visible within the timeout
                return false;
            }
        }
    }
}