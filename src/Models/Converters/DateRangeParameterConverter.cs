using System;
using Nervestaple.EntityFrameworkCore.Models.Parameters;
using Nervestaple.WebApi.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nervestaple.WebApi.Models.converters {

    /// <summary>
    /// Provides a converter that accepts a JSON object and returns a new
    /// DateRangeParameter instance.
    /// </summary>
    public class DateRangeParameterJsonConverter : JsonConverter {

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(DateRangeParameter);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null) return null;
            if (reader.TokenType != JsonToken.StartObject) return null;
            var jObject = JObject.Load(reader);

            try {
                var parameters = jObject.ToObject<DateRangeParameter>();
                return parameters;
            } catch (Exception exception) {
                throw new ParseException(
                    "Could not parse the provided JSON, it should be in the format of a DateRangeParameter instance",
                    exception);
            }
        }

        // we can't convert from an object to a JSON object
        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}