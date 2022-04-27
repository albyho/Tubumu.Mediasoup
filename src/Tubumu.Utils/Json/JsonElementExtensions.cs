using System.Text.Json;

namespace Tubumu.Utils.Json
{
    public static class JsonElementExtensions
    {
        public static JsonElement? GetNullableJsonElement(this JsonElement jsonElement, string propertyName)
        {
            if (jsonElement.TryGetProperty(propertyName, out var value))
            {
                return value;
            }
            return null;
        }

        public static bool? GetNullableBool(this JsonElement jsonElement)
        {
            return jsonElement.ValueKind == JsonValueKind.Null ? null : jsonElement.GetBoolean();
        }

        public static uint? GetNullableUInt32(this JsonElement jsonElement)
        {
            return jsonElement.TryGetUInt32(out var uintValue) ? uintValue : null;
        }
    }
}
