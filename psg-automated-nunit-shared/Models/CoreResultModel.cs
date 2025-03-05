namespace psg_automated_nunit_shared.Models
{

    public class CoreResultModel
    {
        public object Result { get; set; }
        public string Error { get; set; }
        public object Validation { get; set; }
        public Metadata[] MetaData { get; set; }
    }

    public class Metadata
    {
        public Errordata[] ErrorData { get; set; }
        public Tracedata TraceData { get; set; }
    }

    public class Tracedata
    {
        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string ParentId { get; set; }
    }

    public class Errordata
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
