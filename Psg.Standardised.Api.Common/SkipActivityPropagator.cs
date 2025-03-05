using System.Diagnostics;

namespace Psg.Standardised.Api.Common
{
    public sealed class SkipActivityPropagator : DistributedContextPropagator
    {
        private readonly DistributedContextPropagator _originalPropagator = Current;
        private readonly List<string> _skippedActivities;

        public SkipActivityPropagator(params string[] skippedActivities)
        {
            _skippedActivities = new List<string>(skippedActivities);
        }

        public override IReadOnlyCollection<string> Fields => _originalPropagator.Fields;

        public override void Inject(Activity activity, object carrier, PropagatorSetterCallback setter)
        {
            if (activity?.OperationName != null
                && _skippedActivities.Contains(activity.OperationName))
            {
                activity = activity.Parent;
            }

            _originalPropagator.Inject(activity, carrier, setter);
        }

        public override void ExtractTraceIdAndState(object carrier, PropagatorGetterCallback getter, out string traceId, out string traceState) =>
            _originalPropagator.ExtractTraceIdAndState(carrier, getter, out traceId, out traceState);

        public override IEnumerable<KeyValuePair<string, string>> ExtractBaggage(object carrier, PropagatorGetterCallback getter) =>
            _originalPropagator.ExtractBaggage(carrier, getter);
    }
}
