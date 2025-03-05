using Serilog;
using Serilog.Events;
using SerilogTracing;

namespace Psg.Common.Registrations.Serilog
{

    //  https://docs.datalust.co/docs/serilogtracing


    /// <summary>
    /// Quickly setup a Serilog Trace in methods.
    /// <br/>
    /// <br/> More info at <see href="https://docs.datalust.co/docs/serilogtracing"/> 
    /// <br/>
    /// <br/> REMEMBER DISPOSE AFTER USE!
    /// </summary>
    public static class SerilogTraceHelper
    {
        /// <summary>
        /// Gets a Serilog Activity Trace logger.
        /// <br/>
        /// <br/> Only use this if you did not add  app.UseSerilogTracingPsg() to Program.cs.
        /// <br/>
        /// <br/> Meant to be discarded, so it can be assigned to a discard character.
        /// <br/>
        /// <br/> Use GetActivity() after this, to get a LoggerActivity that can measure the activity.
        /// <br/>
        /// <br/> REMEMBER TO DISPOSE IT!
        /// </summary>
        /// <returns></returns>
        public static IDisposable GetTraceToSharedLogger()
        {
            var logger = new ActivityListenerConfiguration()
                                       .Instrument.AspNetCoreRequests()
                                       .Instrument.SqlClientCommands()
                                       .TraceToSharedLogger();

            return logger;
        }

        /// <summary>
        /// Quickly starts a Serilog Trace Activity that logs to the Serilog shared Logger.
        /// <br/>
        /// <br/> Use activity.Complete() when activity completed successful,
        /// <br/> or  activity.Complete(LogEventLevel.Error, ex); when it failed ( usually in the catch block ).
        /// <br/>
        /// <br/> If you did not add app.UseSerilogTracingPsg(); in Program.cs,
        /// <br/> Make sure to use GetActivityTraceLogger before this.
        /// <br/>
        /// <br/> REMEMBER TO DISPOSE IT!
        /// </summary>
        public static LoggerActivity StartActivity(string messageTemplate, params object?[]? propertyValues)
        {
            var activity = Log.Logger.StartActivity(messageTemplate, propertyValues);

            return activity;
        }

        /// <summary>
        /// Quickly starts a Serilog Trace Activity that logs to the Serilog shared Logger.
        /// <br/>
        /// <br/> Use activity.Complete() when activity completed successful,
        /// <br/> or  activity.Complete(LogEventLevel.Error, ex); when it failed ( usually in the catch block ).
        /// <br/>
        /// <br/> If you did not add app.UseSerilogTracingPsg(); in Program.cs,
        /// <br/> Make sure to use GetActivityTraceLogger before this.
        /// <br/>
        /// <br/> REMEMBER TO DISPOSE IT!
        /// </summary>
        public static LoggerActivity StartActivity(LogEventLevel level, string messageTemplate, params object?[]? propertyValues)
        {
            var activity = Log.Logger.StartActivity(level, messageTemplate, propertyValues);

            return activity;
        }
    }
}
