using Microsoft.Playwright;
using NUnit.Framework;
using psg_automated_nunit_shared.Attributes;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.Managers;

namespace psg_automated_nunit_shared.Contracts.Base
{
    /// <summary>
    /// This class MUST be added to ALL tests.
    /// <br/> Contains methods common to all tests, including the setup and teardown methods
    /// </summary>
    public abstract class PsgTestBase
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public IBrowserContext? context;
        public TestConfiguration config;
        public ScreenshotManager screenshotManager;
       

        protected PsgTestBase()
        {          
        }
             

        // runs once before each test
        [SetUp]
        public void Setup()
        {  
         
            // Get the currently running test method
            var testContext = TestContext.CurrentContext;
            var testMethod = GetType().GetMethod(testContext.Test.MethodName ?? "");

            if (testMethod != null)
            {
                // Check if the test method has the QA attribute
                var hasQAAttribute = testMethod.GetCustomAttributes(typeof(QAAttribute), false).Any();

                // Check if the test method has the Prod attribute
                var hasProdAttribute = testMethod.GetCustomAttributes(typeof(ProdAttribute), false).Any();

                if (!hasQAAttribute && !hasProdAttribute)
                {
                    throw new ArgumentException($"Method {testMethod} missing [QA] or [Prod] attribute.");
                }
              
            }


            config = DependencyManager.GetRequiredService<TestConfiguration>();
            screenshotManager = DependencyManager.GetRequiredService<ScreenshotManager>();

            Name = "";
            Description = "";
        }

        // runs once after each test
        [TearDown]
        public async Task Teardown()
        {
            // Get the currently running test method
            var testContext = TestContext.CurrentContext;
            var testMethod = GetType().GetMethod(testContext.Test.MethodName ?? "");

            // check test name
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException($"Test {testMethod} must have a valid name.");
            }

            // Check if the test method has the Prod attribute
            var hasProdAttribute = testMethod?.GetCustomAttributes(typeof(ProdAttribute), false).Any() ?? false;


            if (hasProdAttribute)
            {
                await ResultsManager.AddResultAsync(TestContext.CurrentContext, screenshotManager, Name,Description, true);
            }
            else
            {
                // assume QA
                await ResultsManager.AddResultAsync(TestContext.CurrentContext, screenshotManager, Name,Description, false);
            }
           

            if (context != null)
            {
                await context.CloseAsync();
                await context.DisposeAsync();
            }
        }
     

        public async Task StoreScreenShotAsync(IPage page)
        {
            var key = KeyHelper.GetKey(TestContext.CurrentContext.Test.DisplayName, TestContext.CurrentContext.Test.MethodName);

            await screenshotManager.StoreScreenShotAsync(page, key);
        }
      


    }
}
