namespace psg_automated_nunit_common.Extensions
{
    public static class StringExtensions
    { 
        /// <summary>
        /// Takes a long string and cuts it into an array of smaller strings for display
        /// Cuts if off after the word
        /// </summary>      
        /// <returns></returns>
        public static List<string> GetFormattedStringsForDisplay(this string stringInput, int cutoffLength = 156) 
        { 
            var indx = stringInput.GetCutoffIndex(cutoffLength);

            if(indx == 0)
                return new() { stringInput.Trim() };

            var str1 = stringInput.Substring(0, indx);

            var remainingStr = stringInput.Substring(indx).Trim();

            var strList = remainingStr.GetFormattedStringsForDisplay(cutoffLength);

            List<string> list = [str1.Trim(), .. strList];

            return list;
        }

        public static int GetCutoffIndex(this string str, int cutoffLength)
        {
            if (cutoffLength == 0)
                return 0;

            if (str.Length <= cutoffLength)
                return 0;

            var indx = str.IndexOf(' ', cutoffLength);

            if (indx > -1)
                return indx;

            return 0;
        }

        /// <summary>
        /// Converts encoded HTML special characters back to readable form.
        /// Ex. str = str.Sanitise();
        /// </summary>
        /// <param name="str"></param>
        public static string Sanitise(this string str)
        {
            str = str.Replace("&amp;", "&");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&gt;", ">");
            str = str.Replace("nbsp;", " ");          

            return str;
        }
    }
}
