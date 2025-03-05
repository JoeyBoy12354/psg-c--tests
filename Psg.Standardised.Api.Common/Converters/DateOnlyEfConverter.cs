using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Psg.Standardised.Api.Common.Converters
{
    public class DateOnlyEfConverter : ValueConverter<DateOnly, DateTime>
    {
        /// <summary>
        /// Creates a new instance of this converter.
        /// </summary>
        public DateOnlyEfConverter() : base(
            d => d.ToDateTime(TimeOnly.MinValue),
            d => DateOnly.FromDateTime(d))
        { }
    }
}
