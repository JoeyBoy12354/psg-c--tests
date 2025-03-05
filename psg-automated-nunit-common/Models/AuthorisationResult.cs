namespace psg_automated_nunit_common.Models
{
    public sealed class AuthorisationResult
    {
        public bool Result { get; set; }
        public object? Error { get; set; }
        public object? Validation { get; set; }
        public List<MetaData>? MetaData { get; set; }
    }

    public class MetaData
    {
        public TraceData? TraceData { get; set; }
    }    

    public class TraceData
    {
        public string? TraceId { get; set; }
        public string? SpanId { get; set; }
        public string? ParentId { get; set; }
    }
}
