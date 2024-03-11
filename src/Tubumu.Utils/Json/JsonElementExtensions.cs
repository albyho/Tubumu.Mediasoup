using System;
using System.Text.Json;

namespace Tubumu.Utils.Json
{
    /// <summary>
    /// JsonElementExtensions 扩展方法
    /// </summary>
    internal static class JsonElementExtensions
    {
        public static JsonElement? GetNullableJsonElement(this JsonElement jsonElement, string propertyName)
        {
            return jsonElement.TryGetProperty(propertyName, out var value) ? value : null;
        }

        public static bool? GetNullableBool(this JsonElement jsonElement)
        {
            return jsonElement.ValueKind == JsonValueKind.Null ? null : jsonElement.GetBoolean();
        }

        public static Guid? GetNullableGuid(this JsonElement jsonElement)
        {
            return jsonElement.TryGetGuid(out var guidValue) ? guidValue : null;
        }

        public static byte? GetNullableByte(this JsonElement jsonElement)
        {
            return jsonElement.TryGetByte(out var byteValue) ? byteValue : null;
        }

        public static sbyte? GetNullableSByte(this JsonElement jsonElement)
        {
            return jsonElement.TryGetSByte(out var sbyteValue) ? sbyteValue : null;
        }

        public static ushort? GetNullableUInt16(this JsonElement jsonElement)
        {
            return jsonElement.TryGetUInt16(out var ushortValue) ? ushortValue : null;
        }

        public static short? GetNullableInt16(this JsonElement jsonElement)
        {
            return jsonElement.TryGetInt16(out var shortValue) ? shortValue : null;
        }

        public static uint? GetNullableUInt32(this JsonElement jsonElement)
        {
            return jsonElement.TryGetUInt32(out var uintValue) ? uintValue : null;
        }

        public static int? GetNullableInt32(this JsonElement jsonElement)
        {
            return jsonElement.TryGetInt32(out var intValue) ? intValue : null;
        }

        public static ulong? GetNullableUInt64(this JsonElement jsonElement)
        {
            return jsonElement.TryGetUInt64(out var uint64Value) ? uint64Value : null;
        }

        public static long? GetNullableInt64(this JsonElement jsonElement)
        {
            return jsonElement.TryGetInt64(out var int64Value) ? int64Value : null;
        }

        public static float? GetNullableSingle(this JsonElement jsonElement)
        {
            return jsonElement.TryGetSingle(out var singleValue) ? singleValue : null;
        }

        public static double? GetNullableDouble(this JsonElement jsonElement)
        {
            return jsonElement.TryGetUInt64(out var doubleValue) ? doubleValue : null;
        }

        public static decimal? GetNullableDecimal(this JsonElement jsonElement)
        {
            return jsonElement.TryGetDecimal(out var decimalValue) ? decimalValue : null;
        }

        public static DateTime? GetNullableDateTime(this JsonElement jsonElement)
        {
            return jsonElement.TryGetDateTime(out var dateTimeValue) ? dateTimeValue : null;
        }
    }
}
