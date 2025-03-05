using Newtonsoft.Json;
using NUnit.Framework.Interfaces;
using psg_automated_nunit_shared.Models;
using System.Text;

namespace psg_automated_nunit_shared.Helpers
{
    public static class ReportHelper
    {
        public static List<TestResultDto> GetDataPerStatus(IEnumerable<TestResultDto> results,
                                                            TestStatus status)
        {
            return results.Where(x => x.Status == status.ToString()).ToList();
        }

        public static string PrepareReport(IEnumerable<TestResultDto> results)
        {
            StringBuilder sb = new StringBuilder();

            var failed = GetDataPerStatus(results, TestStatus.Failed);
            var passed = GetDataPerStatus(results, TestStatus.Passed);
            var skipped = GetDataPerStatus(results, TestStatus.Skipped);

            var totalTests = results.Count();
            var allPassed = (passed.Count + skipped.Count) == totalTests;

            var msg = (allPassed && totalTests > 0) ? $"All Tests Passed!" : "";

            if (!string.IsNullOrWhiteSpace(msg))
            {
                sb.AppendLine(msg);
                sb.AppendLine("-------------------");
                sb.AppendLine();
                sb.AppendLine();
            }

            sb.AppendLine($"Total Tests:\t{totalTests}");
            sb.AppendLine("-------------------");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"Failed:\t{failed.Count}\tPassed:\t{passed.Count}\tSkipped:{skipped.Count}");
            sb.AppendLine("==============================================");
            sb.AppendLine();
            sb.AppendLine();

            AddDataToStringBuilderByStatus(sb, failed, "Failed");
            AddDataToStringBuilderByStatus(sb, passed, "Passed");
            AddDataToStringBuilderByStatus(sb, skipped, "Skipped");

            var data = sb.ToString();

            return data;
        }

        public static void AddDataToStringBuilderByStatus(StringBuilder sb,
                                                           List<TestResultDto> data,
                                                           string Status)
        {
            if (data.Count > 0)
            {
                var failedData = JsonConvert.SerializeObject(data, Formatting.Indented);

                sb.AppendLine($"{Status}: {data.Count}");
                sb.AppendLine("--------------");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(failedData);
                sb.AppendLine();
                sb.AppendLine();
            }
        }
    }
}
