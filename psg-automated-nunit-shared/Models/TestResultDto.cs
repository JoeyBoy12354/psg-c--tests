using System.ComponentModel.DataAnnotations.Schema;

namespace psg_automated_nunit_shared.Models
{
    /// <summary>
    /// record to save test results, for saving later.
    /// </summary>
    public sealed class TestResultDto : TestResultsBase
    {
        public string? Type { get; init; } = "Automated";
        public string? Key { get; init; }
        public string? Status { get; set; }
        public string? TestSuite { get; init; }
        public string? TestName { get; init; }
        public string? Description { get; set; }
        public string? Message { get; init; }
        public string? StackTrace { get; init; }
        public DateTime? Date { get; set; }

        public string? Origin { get; set; } = "CSharp";
        public string? RunId { get; init; }

        [NotMapped]
        public bool IsProd { get; set; } = false;
    }
}
