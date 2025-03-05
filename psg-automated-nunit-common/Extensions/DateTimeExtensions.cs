namespace psg_automated_nunit_common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string? ToDate(this DateTime? date, string dateFormat)
        {
            if (date == null)
                return null;

            return date.Value.ToDate(dateFormat);
        }

        public static string ToDate(this DateTime date, string dateFormat)
        {
            var dateOnly = DateOnly.FromDateTime(date);
            var dateOnlyString = dateOnly.ToString(dateFormat);

            var h = date.Hour.ToString().PadLeft(2, '0');
            var m = date.Minute.ToString().PadLeft(2, '0');
            var s = date.Second.ToString().PadLeft(2, '0');

            var dateString = $"{dateOnlyString} {h}h{m}:{s}";

            return dateString;
        }

        public static string? ToDateOnly(this DateTime? date, string dateFormat)
        {
            if (date == null)
                return null;          

            return date.Value.ToDateOnly(dateFormat);
        }

        public static string ToDateOnly(this DateTime date, string dateFormat)
        {
            var dateOnly = DateOnly.FromDateTime(date);
            var dateOnlyString = dateOnly.ToString(dateFormat);

            var dateString = $"{dateOnlyString}";

            return dateString;
        }

        public static string? ToTime(this DateTime? date)
        {
            if(date == null) 
                return null;

            return date.Value.ToTime();
        }

        public static string ToTime(this DateTime date)
        {
            var h = date.Hour.ToString().PadLeft(2,'0');
            var m = date.Minute.ToString().PadLeft(2, '0');
            var s = date.Second.ToString().PadLeft(2, '0');

            var time = $"{h}h{m}:{s}";

            return time;
        }


        public static string HowLongAgo(this DateTime? date)
        {
            if (date == null)
                return "";

            return date.Value.HowLongAgo();
        }

        public static string HowLongAgo(this DateTime date)
        {
            var now = DateTime.Now;       

            var result = now - date;

            if (result.TotalDays > 365)
            {
                var yearsAgo = now.Year - date.Year;

                if (yearsAgo > 1)
                {
                    return $"About {yearsAgo} years ago.";
                }

                return "About a year ago.";
            }


            var months = (int)result.TotalDays / 30;

            if (months == 1)
                return $"About a month ago.";

            if (months > 0)
                return $"About {months} months ago.";

            if (result.TotalDays > 30)
                return $"About a month ago.";

            if (result.TotalDays >= 2)
                return $"About {(int) result.TotalDays} days ago.";

            var hour = date.Hour;

            var daysAgo = now.Day - date.Day;

            if (daysAgo == 1)
            {
                if (hour == 0)
                    return "Yesterday at midnight.";

                if (hour >= 0 && hour <= 5)
                    return "Yesterday early morning.";

                if (hour >= 6 && hour <= 11)
                    return "Yesterday morning.";

                if (hour == 12)
                    return "Yesterday at noon.";

                if (hour >= 12 && hour <= 18)
                    return "Yesterday afternoon.";

                if (hour >= 18 && hour <= 23)
                    return "Last night.";
            }

            if (hour == 0)
                return "Midnight.";

            if (hour >= 0 && hour <= 5)
                return "Early morning today.";

            if (hour >= 6 && hour <= 11)
                return "This morning.";

            if (hour == 12)
                return "Noon.";

            if (hour >= 12 && hour <= 18)
                return "This afternoon.";

            if (hour >= 18 && hour <= 23)
                return "Tonight.";

            return "";
        }


    }
}
