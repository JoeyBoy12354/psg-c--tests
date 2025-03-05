namespace Psg.Standardised.Api.Common.Converters
{
    public static class NumberConverters
    {
        public static decimal ConvertToDecimalIncludingScientificNotation(decimal income)
        {
            string incomeString = income.ToString();

            return ConvertToDecimalIncludingScientificNotation(incomeString);
        }

        public static decimal ConvertToDecimalIncludingScientificNotation(string incomeString)
        {
            if (incomeString.Contains("E+") 
               || incomeString.Contains("E-"))
            { 
              
                if (incomeString.Contains("E+"))   // very HUGE number
                {
                    var eIndex = incomeString.IndexOf("E+");

                    string incomePart = incomeString.Substring(0, eIndex);
                    incomePart = incomePart.Replace('.', ','); // South African number format              

                    int iDecimals = Convert.ToInt32(incomeString.Substring(eIndex + 2)); // Extract the exponent part

                    if (iDecimals > 0)
                    {
                        double originalIncome = Convert.ToDouble(incomePart);
                        originalIncome *= Math.Pow(10, -Math.Abs(iDecimals));
                        var sAppend = new string('0', Math.Abs(iDecimals));
                        incomeString = $"{originalIncome} {sAppend}";
                    }
                    else
                    {
                        var d = Convert.ToDouble(incomePart);
                        incomeString = string.Format("{0:N" + iDecimals + "}", d);
                    }
                }
                else if (incomeString.Contains("E-")) // very SMALL number
                {
                    var eIndex = incomeString.IndexOf("E-");

                    string incomePart = incomeString.Substring(0, eIndex);
                    incomePart = incomePart.Replace('.', ','); // South African number format              

                    int iDecimals = Convert.ToInt32(incomeString.Substring(eIndex + 2)); // Extract the exponent part

                    if (iDecimals > 0)
                    {
                        double originalIncome = Convert.ToDouble(incomePart);                     
                        var sAppend = new string('0', Math.Abs(iDecimals - 1));

                        if(originalIncome.ToString().Contains(","))
                        {
                            incomeString = $"0{sAppend}{originalIncome}";
                        }
                        else
                        {
                            incomeString = $"0,{sAppend}{originalIncome}";
                        }                       
                    }
                    else
                    {
                        var d = Convert.ToDouble(incomePart);
                        incomeString = string.Format("{0:N" + iDecimals + "}", d);
                    }
                }
            }

            return decimal.Parse(incomeString);
        }

        /// <summary>
        /// Converts to decimal from a string decimal and swaps '.' with ',' for ZA
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static decimal ConvertToDecimal(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return 0;

            number = number.Replace(".", ",");

            return Convert.ToDecimal(number);
        }

        public static T?[] ConvertToNullableArray<T>(T[] array) where T : struct
        {
            if (array == null)
            {
                return Array.Empty<T?>();
            }

            T?[] nullableArray = new T?[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                nullableArray[i] = array[i];
            }

            return nullableArray;
        }

        public static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                // Compute the Average
                double avg = values.Average();

                // Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                // Apply Bessel's correction
                ret = Math.Sqrt(sum / (count - 1));
            }
            return ret;
        }

        public static string GetElapsedFromMilliseconds(long ms)
        {
            if (ms == 0)
                return "0 ms";

            string elapsed = "";

            if(ms > 1000)
            {
                TimeSpan t = TimeSpan.FromMilliseconds(ms);

                if (ms > 1000 * 60 * 60)
                {
                    elapsed += $"{t.Hours}h ";
                }

                if (ms > 1000 * 60)
                {
                    elapsed += $"{t.Minutes}m ";
                }

                if (t.Seconds > 0)
                {
                    elapsed += $"{t.Seconds}s ";
                }
            }
            else
            {
                elapsed += $"{ms.ToString("# #### ####")}ms";
            }

            return elapsed.Trim();          
        }

    }
}
