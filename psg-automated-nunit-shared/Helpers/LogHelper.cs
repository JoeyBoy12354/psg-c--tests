using Serilog;

namespace psg_automated_nunit_shared.Helpers
{
    public static class LogHelper
    {
        public static void LogInfo(string messageTemplate)
        {
            Log.Logger.Information(messageTemplate);
        }

        public static void LogInfo(string messageTemplate, params object?[]? propertyValues)
        {
            Log.Logger.Information(messageTemplate, propertyValues);
        }


        public static void LogError(Exception ex)
        {
            Log.Logger.Error(ex, "{message}");
        }

        public static void LogError(string messageTemplate, params object?[]? propertyValues)
        {
            Log.Logger.Error(messageTemplate, propertyValues);
        }

        public static void LogError(Exception ex, string messageTemplate, params object?[]? propertyValues)
        {
            Log.Logger.Error(ex, messageTemplate, propertyValues);
        }
    }
}
