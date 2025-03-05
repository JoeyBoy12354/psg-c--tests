using psg_automated_nunit_shared.Global;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.Managers;
using Serilog;

namespace psg_automated_nunit_test
{
    // DO NOT ADD ANY PSG PACKAGES!!
    // JENKINS CANNOT CONNECT TO THE PSG PACKAGE SERVER TO DOWNLOAD THEM !!!!


    [SetUpFixture]  
    public sealed class GlobalSetupAndTeardown
    {

        [OneTimeSetUp]
        public async Task RunBeforeAnyTestsAsync()
        {
            try
            {
                // Code to run once before any tests in the namespace
                // For example, initializing common resources, setting up connections, etc.

                await PlaywrightManager.SetupAsync();

               // global login disabled for now
               // await GlobalLogins.Create().SetupAsync();            

            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }


       [OneTimeTearDown]
        public async Task RunAfterAnyTestsAsync()
        {
            try
            {
                // Code to run once after all tests in the namespace have executed
                // For example, releasing resources, closing connections, etc.

                // save results
                await ResultsManager.SaveResultsAsync();

                await PlaywrightManager.TeardownAsync();
            }
            catch(Exception ex)
            {
                LogHelper.LogError(ex);
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }    

        }
    }
}
