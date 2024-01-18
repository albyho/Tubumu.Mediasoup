using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FBS.Consumer
{
    public class ConsumerTraceInfoUnionJsonConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(TraceInfoUnion) || typeToConvert == typeof(List<TraceInfoUnion>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(TraceInfoUnionConverterInner<>).MakeGenericType(typeToConvert),
                options
            )!;
        }

        private class TraceInfoUnionConverterInner<T> : JsonConverter<T>
        {
            private readonly JsonSerializerOptions _options;

            public TraceInfoUnionConverterInner(JsonSerializerOptions options)
            {
                _options = options;
            }

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotSupportedException();
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                if(typeof(T) == typeof(List<TraceInfoUnion>))
                {
                    JsonSerializer.Serialize(writer, value, _options);
                }
                else if(typeof(T) == typeof(TraceInfoUnion))
                {
                    var traceInfoUnion = value as TraceInfoUnion;
                    if(traceInfoUnion != null)
                    {
                        writer.WritePropertyName("info");
                        JsonSerializer.Serialize(writer, traceInfoUnion.Value, options);
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
