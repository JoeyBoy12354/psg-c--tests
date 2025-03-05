namespace psg_automated_nunit_common.Models
{
    public sealed record TestRunnerResult
    {
        public string Output { get; init; }
        public string Error { get; init; }

        public bool IsSucces => string.IsNullOrWhiteSpace(Error);
        public bool IsFailure => !IsSucces;
    }
}
