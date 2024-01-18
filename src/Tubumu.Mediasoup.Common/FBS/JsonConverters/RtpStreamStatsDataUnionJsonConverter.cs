using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FBS.RtpStream
{
    public class RtpStreamStatsDataUnionJsonConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(StatsDataUnion) || typeToConvert == typeof(List<StatsDataUnion>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(StatsDataUnionConverterInner<>).MakeGenericType(typeToConvert),
                options
            )!;
        }

        private class StatsDataUnionConverterInner<T> : JsonConverter<T>
        {
            private readonly JsonSerializerOptions _options;

            public StatsDataUnionConverterInner(JsonSerializerOptions options)
            {
                _options = options;
            }

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotSupportedException();
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                if(typeof(T) == typeof(List<StatsDataUnion>))
                {
                    JsonSerializer.Serialize(writer, value, _options);
                }
                else if(typeof(T) == typeof(StatsDataUnion))
                {
                    var StatsDataUnion = value as StatsDataUnion;
                    if(StatsDataUnion != null)
                    {
                        writer.WritePropertyName("info");
                        JsonSerializer.Serialize(writer, StatsDataUnion.Value, options);
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
