using HtmlTags;
using Microsoft.Extensions.Logging;
using psg_automated_nunit_common.Configurations;
using psg_automated_nunit_common.Contracts;
using psg_automated_nunit_common.Enums;
using psg_automated_nunit_common.Extensions;
using psg_automated_nunit_common.Managers;
using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_common.Writers
{
    public sealed class HtmlReportWriter : IReportWriter
    {
        private readonly ExternalResourceManager _externalResourceManager;
        private readonly ILogger<HtmlReportWriter> _logger;
        private readonly TestRunnerConfiguration _config;

        private const string _dateFormat = "yyyy-MM-dd";

        private const string _title = "MyPractice Test Results";


        public HtmlReportWriter(ILogger<HtmlReportWriter> logger,
                                ExternalResourceManager externalResourceManager,
                                TestRunnerConfiguration config)
        {
            _logger = logger;
            _externalResourceManager = externalResourceManager;
            _config = config;
        }


        public async Task GenerateReportAsync(List<TestResultDto> models)
        {
            try
            {
                string filePath = "wwwroot/testresults.html";

                var totalTests = models.Count;

                DateTime? lastRun = totalTests > 0 ? models.Max(x => x.Date) : null;

                var passed = models.Where(x => x.Status == TestStatus.Passed.ToString()).OrderBy(x => x.Key).ToList();
                var failed = models.Where(x => x.Status == TestStatus.Failed.ToString()).OrderBy(x => x.Key).ToList();

                // main html tag
                var html = new HtmlTag("html");

                CreateHead(html);

                CreateBody(totalTests,
                           lastRun,
                           passed,
                           failed,
                           html);

                string content = html.GetHtml(_config);

                await File.WriteAllTextAsync(filePath, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{message}", ex.Message);
            }
        }

        private void CreateHead(HtmlTag html)
        {
            // the styling must also into the head, but the HtmlTags package seems to mess it up
            // styling and scripts are added at the end with method 'html.GetHtml();'

            var head = html.Add("head");

            head.Add("title")
                .Text(_title);

            // <!-- Bootstrap CSS link -->
            head.Add("link")
                .Attr("rel", "stylesheet")
                .Attr("href", _externalResourceManager.GetLink("BootStrapCss"));

            //  <!-- Font Awesome CSS link -->
            head.Add("link")
                .Attr("rel", "stylesheet")
                .Attr("href", _externalResourceManager.GetLink("FontAwesomeCss"));
        }

        private void CreateBody(int totalTests,
                                DateTime? lastRun,
                                List<TestResultDto> passed,
                                List<TestResultDto> failed,
                                HtmlTag html)
        {
            var body = html.Add("body");

            var main_container = body.Add("div").AddClass("container mt-3"); // bootstrap container

            var test_summary = main_container.Add("div").AddClasses("test-summary");

            var main_header = test_summary.Add("div").Id("main_header")
                                                 .AddClasses("medium-space");

            main_header.Add("h1").Id("test_summary")
                        .Add("span")
                        .AddClasses("padding-right", "padding-left", "main-header-color")
                        .Text(_title);

            CreateSubHeader1(totalTests,
                             lastRun,
                             test_summary);

            CreateSubHeader2(passed, failed, test_summary);

            test_summary.Add("div").Id("space2").AddClasses("big-space");


            // list any failed records first
            ConstructRecordsWithHeader("Failed:",
                                               "test-header failed",
                                               "failed-color",
                                               failed,
                                               main_container);

            // passed records
            ConstructRecordsWithHeader("Passed:",
                                               "test-header passed",
                                               "passed-color",
                                               passed,
                                               main_container);


            // js scripts that need to be downloaded like jquery and bootstraps
            AddScripts(main_container);
        }


        private static void CreateSubHeader1(int totalTests,
                                             DateTime? lastRun,
                                             HtmlTag test_summary)
        {
            var lastRunDate = lastRun.ToDateOnly(_dateFormat) ?? "No Data";
            var lastRunTime = lastRun.ToTime() ?? "";         

            var sub_header_div = test_summary.Add("div").Id("sub_header_div1")
                                                    .AddClasses("box-border", "box-padding", "header-background");

            var row = sub_header_div.Add("div").Id("sub_header1")
                                                   .Add("div").AddClasses("container-fluid")
                                                   .Add("div").AddClasses("row");

            var col1 = row.Add("div").AddClasses("col-md-2");

            col1.Add("span").AddClasses("padding-right", "header-font-small", "sub-header-label")
                                           .Text("Total Tests:");        

            var totalColor = totalTests > 0 ? "passed-color" : "failed-color";

            var col2 = row.Add("div").AddClasses("col");

            col2.Add("span").AddClasses( "header-font-small", totalColor)
                                           .Text($"{totalTests}");

            var col3 = row.Add("div").AddClasses("col");

            col3.Add("span").Text($"Last run:").AddClasses("padding-right", "header-font-small", "sub-header-label");
            col3.Add("span").Text($"{lastRunDate}").AddClasses("header-date", "padding-right-small");
            col3.Add("span").Text($"{lastRunTime}").AddClasses("header-time");

            col3.Add("span").AddClasses("padding-right");
            var javascriptDate = lastRun != null ? lastRun.Value.ToString("yyyy-MM-ddTHH:mm:ss") : "";
            col3.Add("span").Id("header-how-long-ago-date").Text(javascriptDate)
                                                                           .AddClasses("hidden");
            col3.Add("span").Id("header-how-long-ago").AddClasses("header-how-long-ago");

        }

        private static void CreateSubHeader2(List<TestResultDto> passed, List<TestResultDto> failed, HtmlTag test_summary)
        {
            var sub_header2_div = test_summary.Add("div").Id("sub_header_div2")
                                                      .AddClasses("box-border", "box-padding", "header-background");

            var row = sub_header2_div.Add("div").Id("sub_header2")
                                                   .Add("div").AddClasses("container-fluid")
                                                   .Add("div").AddClasses("row align-items-center"); 

            var passedColor = passed.Count > 0 ? "passed-color" : "failed-color";

            var col1 = row.Add("div").AddClasses("col-md-2");

            col1.Add("span").AddClasses(passedColor, "sub-header-label2").Text("Passed:");

            var col2 = row.Add("div").AddClasses("col-md-2");

            col2.Add("span").AddClasses(passedColor, "sub-header-label2-value").Text($"{passed.Count}");

            var failedColor = failed.Count > 0 ? "failed-color" : "passed-color";

            var col3 = row.Add("div").AddClasses("col-md-1");

            col3.Add("span").AddClasses(failedColor, "sub-header-label2").Text("Failed:");

            var col4 = row.Add("div").AddClasses("col");

            col4.Add("span").AddClasses(failedColor, "sub-header-label2-value").Text($"{failed.Count}");
        }



        private static void ConstructRecordsWithHeader(string header,
                                                       string statusClass,
                                                       string colorClass,
                                                       List<TestResultDto> data,
                                                       HtmlTag container)
        {
            if (data.Count > 0)
            {
                var div = container.Add("div").Id("record_div");

                var group = $"{header.Trim(':')}_group";

                var id = $"{ header.Trim(':') }_header";

                var h = div.Add("h4").Id(id)
                                  .AddClasses("record-header", "box-border", "box-padding", "header-background")
                                  .AddClass(colorClass)
                                  .Attr("data-bs-toggle", "collapse")
                                  .Attr("data-bs-target", $"#{group}");

                h.Add("span").Id($"{group}-chevron").Add("i").AddClasses("fas fa-chevron-down");
                h.Add("span").Id("chevron-padding").AddClasses("padding-right-small");
                h.Add("span").AddClasses("record-header", "header-background").Text(header);
                h.Add("span").Text($"{data.Count}");


                ConstructRecords(group, statusClass, data, div);

                container.Add("div").AddClasses("big-space");

            }
        }

        private static void ConstructRecords(string group,
                                             string statusClass,
                                             List<TestResultDto> data,
                                             HtmlTag container)
        {
            var div = container.Add("div").Id(group).AddClasses("box-border collapse show");

            foreach (var model in data)
            {
                var test_container = div.Add("div").AddClasses("box-border");

                var toggleTest = test_container.Add("div")
                                                  .AddClass(statusClass)
                                                  .Attr("onclick", $"toggleTest('{model.Key}')");


                var container_fluid = toggleTest.Add("div")
                                                           .AddClasses("container-fluid"); 

                var row = container_fluid.Add("div")
                                                           .AddClasses("row");

                var col1 = row.Add("div").AddClasses("col");

                col1.Add("span")
                      .Add("h6")
                      .Text($"{model.Key}");

                col1.Add("span")
                      .Text($"{model.Date.ToDate(_dateFormat)}");

                if (!string.IsNullOrWhiteSpace(model.RunId))
                {
                    col1.Add("div").AddClasses("neutral-color")
                   .Text($"Run ID: {model.RunId}");
                }


                var col2 = row.Add("div").AddClasses("col-md-3 text-right");

                if (!string.IsNullOrWhiteSpace(model.Origin))
                {
                    col2.Add("span").AddClasses("neutral-color")
                   .Text($"Origin: {model.Origin}");
                }

                var col3 = row.Add("div").AddClasses("col-md-1 text-right");

                // Add a button that will send the testName
                if (!string.IsNullOrWhiteSpace(model.TestName))
                {
                    col3.Add("button").Id(model.TestName)
                        .AddClasses("btn", "btn-outline-primary", "btn-sm")
                        .Attr("onclick", $"callApi('{model.TestName}', event)")
                        .Text($"Run Test");
                }

             

                var p = test_container.Add("div")
                              .AddClass("test-content")
                              .Id($"{model.Key}")
                              .Add("p");

                if (!string.IsNullOrWhiteSpace(model.Message))
                {
                    p.Add("span").AddClasses("message-header").Text("Message");

                    p.Add("div").AddClasses("message-box-border", "message-box-padding")
                                        .Text($"{model.Message}");
                }

                if (!string.IsNullOrWhiteSpace(model.StackTrace))
                {
                    p.Add("span").AddClasses("message-header").Text("StackTrace");

                    p.Add("div").AddClasses("message-box-border", "message-box-padding")
                                        .Text($"{model.StackTrace}");
                }
            }
        }

        private void AddScripts(HtmlTag main_container)
        {
            main_container.Add("script").Attr("src", _externalResourceManager.GetLink("JQueryJs"));
            main_container.Add("script").Attr("src", _externalResourceManager.GetLink("BootStrapJs"));
            main_container.Add("script").Attr("src", _externalResourceManager.GetLink("FontAwesomeJs"));
        }

    }
}
