using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Talifun.Commander.Command.Esb.Serialization
{
    public class StringDecimalConverter : JsonConverter
    {
        private const NumberStyles stringDecimalStyle = NumberStyles.Number;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("This converter is not writing decimal values, just reading them");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return new Decimal(0);
            if (reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Float)
                return Convert.ToDecimal(reader.Value, CultureInfo.InvariantCulture);
            Decimal result;
            if (reader.TokenType == JsonToken.String && Decimal.TryParse((string)reader.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                return result;
            throw new JsonReaderException(string.Format(CultureInfo.InvariantCulture, "Error reading decimal. Expected a number but got {0}.", new[]
      {
        (object) reader.TokenType
      }));
        }

        public override bool CanConvert(Type objectType)
        {
            if (!(objectType == typeof(Decimal)))
                return objectType == typeof(Decimal?);
            return true;
        }
    }
}
