using Newtonsoft.Json;
using System.Globalization;

namespace Psg.Standardised.Api.Common.Converters
{
    public class DateOnlyConverterNs : Newtonsoft.Json.JsonConverter<DateOnly>
    {
        public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
            => DateOnly.ParseExact(reader.Value.ToString(), "yyyy-MM-dd");

        public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
            => writer.WriteValue(value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }

    public class NullableDateOnlyConverterNs : Newtonsoft.Json.JsonConverter<DateOnly?>
    {
        public override DateOnly? ReadJson(JsonReader reader, Type objectType, DateOnly? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            return DateOnly.ParseExact(reader.Value.ToString(), "yyyy-MM-dd");
        }

        public override void WriteJson(JsonWriter writer, DateOnly? value, JsonSerializer serializer)
        {
            if (value.HasValue)
            {
                writer.WriteValue(value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}
