using Microsoft.Playwright;

namespace psg_automated_nunit_shared.PageExtensions
{
    public static class IPageExtensions
    {
        private static readonly int _timeOutMs = 120_000;

        /// <summary>
        /// Checks if a locator is in a List when the Find box was used.
        /// <br/> Waits until <paramref name="maxWaitSeconds"/> or if list is empty.
        /// </summary>  
        public static async Task<bool> IsFoundInListAsync(this IPage page,
                                                          ILocator locatorToBeFound,
                                                          int maxWaitSeconds = 10)
        {
            var locatorPageLoaded = page.GetByRole(AriaRole.Cell, new() { Name = "There are no items that match" });

            var result = await page.IsFoundAsync(locatorToBeFound, locatorPageLoaded, maxWaitSeconds);

            return result;


        }

        /// <summary>
        /// Returns true if Locator is Visible.
        /// <br/> Waits until page is loaded.
        /// <br/> locatorPageLoaded is a locator that indicates the page is loaded, if the locatorToBeFound is not found.
        /// <br/> ex. locatorPageLoaded can be something to indicate an empty list
        /// </summary>     
        public static async Task<bool> IsFoundAsync(this IPage page,
                                                    ILocator locatorToBeFound,
                                                    ILocator locatorPageLoaded,
                                                    int maxWaitSeconds = 10)
        {         

            for (int i = 0; i < maxWaitSeconds; i++)
            {
                if (await locatorToBeFound.IsVisibleAsync())
                {
                    //locator found
                    return true;
                }

                if (await locatorPageLoaded.IsVisibleAsync())
                {
                    //page loaded and no records found
                    return false;
                }

                await Task.Delay(1000);
            }

            return false;
        }

    }
}
