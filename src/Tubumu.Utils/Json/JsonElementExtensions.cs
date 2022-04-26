using System.Text.Json;

namespace Tubumu.Utils.Json
{
    public static class JsonElementExtensions
    {
        public static JsonElement? GetNullableJsonElement(this JsonElement jsonElement, string propertyName)
        {
            if(jsonElement.TryGetProperty(propertyName, out var value))
            {
                return value;
            }
            return null;
        }

        public static bool? GetNullableBool(this JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Null) return null;
            return jsonElement.GetBoolean();
        }

        public static uint? GetNullableUInt32(this JsonElement jsonElement)
        {
            if (jsonElement.TryGetUInt32(out var uintValue))
            {
                return uintValue;
            }
            return null;
        }
    }
}
