using System.Buffers;
using System.Buffers.Text;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// 处理整数和 JSON 数据类型不匹配的问题
    /// </summary>
    public class NumberToStringJsonConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if(reader.TokenType != JsonTokenType.Number && type == typeof(string))
            {
                return reader.GetString()!;
            }

            var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;

            if(Utf8Parser.TryParse(span, out long longNumber, out var longBytesConsumed) && span.Length == longBytesConsumed)
            {
                return longNumber.ToString();
            }

            if(Utf8Parser.TryParse(span, out ulong ulongNumber, out var ulongBytesConsumed) && span.Length == ulongBytesConsumed)
            {
                return ulongNumber.ToString();
            }

            var data = reader.GetString();

            throw new InvalidOperationException($"'{data}' is not a correct expected value!")
            {
                Source = "NumberToStringJsonConverter"
            };
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
