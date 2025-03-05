using System.Diagnostics;

namespace Psg.Standardised.Api.Common.Models
{
    public abstract class MetaData
    {
    }

    public class MetaDataPaging : MetaData
    {
        public MetaDataPaging(PagingData pagingData)
            => PagingData = pagingData;

        public PagingData PagingData { get; set; }
    }

    public class MetaDataChange : MetaData
    {
        public MetaDataChange(Dictionary<string, string> changeData)
            => ChangeData = changeData;

        public Dictionary<string, string> ChangeData { get; set; }
    }

    public class MetaDataError : MetaData
    {
        public MetaDataError(IEnumerable<ErrorEntry> errorData)
            => ErrorData = errorData;

        public IEnumerable<ErrorEntry> ErrorData { get; set; }

        public class ErrorEntry
        {
            public ErrorEntry(string message, string stackTrace)
            {
                Message = message;
                StackTrace = stackTrace;
            }

            public string Message { get; set; }
            public string StackTrace { get; set; }
        }
    }

    public class MetaDataEvent : MetaData
    {
        public MetaDataEvent(Dictionary<string, string> eventData)
            => EventData = eventData;

        public Dictionary<string, string> EventData { get; set; }
    }

    public class MetaDataTrace : MetaData
    {
        public MetaDataTrace()
        {
            var traceId = Activity.Current?.TraceId.ToString();
            var spanId = Activity.Current?.SpanId.ToString();
            var parentId = Activity.Current?.ParentSpanId.ToString();

            TraceData = new()
            {
                { "TraceId", traceId },
                { "SpanId", spanId },
                { "ParentId", parentId }
            };
        }

        public MetaDataTrace(Dictionary<string, string> traceData)
            => TraceData = traceData;

        public Dictionary<string, string> TraceData { get; set; }
    }
}
