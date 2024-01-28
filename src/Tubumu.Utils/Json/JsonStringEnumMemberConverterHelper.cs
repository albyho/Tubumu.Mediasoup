using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Text.Json.Serialization
{
    internal class JsonStringEnumMemberConverterHelper<TEnum>
    where TEnum : struct, Enum
    {
        private class EnumInfo
        {
            public string Name { get; set; }

            public TEnum EnumValue { get; set; }

            public ulong RawValue { get; set; }

            public EnumInfo(string name, TEnum enumValue, ulong rawValue)
            {
                Name = name;
                EnumValue = enumValue;
                RawValue = rawValue;
            }
        }

        private const BindingFlags EnumBindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

#if NETSTANDARD2_0
		private static readonly string[] _split = new string[] { ", " };
#endif

        private readonly bool _allowIntegerValues;
        private readonly Type _enumType;
        private readonly TypeCode _enumTypeCode;
        private readonly bool _isFlags;
        private readonly Dictionary<TEnum, EnumInfo> _rawToTransformed;
        private readonly Dictionary<string, EnumInfo> _transformedToRaw;

        public JsonStringEnumMemberConverterHelper(JsonNamingPolicy? namingPolicy, bool allowIntegerValues)
        {
            _allowIntegerValues = allowIntegerValues;
            _enumType = typeof(TEnum);
            _enumTypeCode = Type.GetTypeCode(_enumType);
            _isFlags = _enumType.IsDefined(typeof(FlagsAttribute), true);

            string[] builtInNames = _enumType.GetEnumNames();
            Array builtInValues = _enumType.GetEnumValues();

            _rawToTransformed = new Dictionary<TEnum, EnumInfo>();
            _transformedToRaw = new Dictionary<string, EnumInfo>();

            for(int i = 0; i < builtInNames.Length; i++)
            {
                Enum? enumValue = (Enum?)builtInValues.GetValue(i);
                if(enumValue == null)
                {
                    continue;
                }

                ulong rawValue = GetEnumValue(enumValue);

                string name = builtInNames[i];
                FieldInfo field = _enumType.GetField(name, EnumBindings)!;
                EnumMemberAttribute? enumMemberAttribute = field.GetCustomAttribute<EnumMemberAttribute>(true);
                string transformedName = enumMemberAttribute?.Value ?? namingPolicy?.ConvertName(name) ?? name;

                //if (enumValue is not TEnum typedValue)
                //	throw new NotSupportedException();
                var typedValue = (TEnum)enumValue;

                _rawToTransformed[typedValue] = new EnumInfo(transformedName, typedValue, rawValue);
                _transformedToRaw[transformedName] = new EnumInfo(name, typedValue, rawValue);
            }
        }

        public TEnum Read(ref Utf8JsonReader reader)
        {
            JsonTokenType token = reader.TokenType;

            if(token == JsonTokenType.String)
            {
                string enumString = reader.GetString()!;

                // Case sensitive search attempted first.
                if(_transformedToRaw.TryGetValue(enumString, out EnumInfo? enumInfo))
                {
                    return enumInfo.EnumValue;
                }

                if(_isFlags)
                {
                    ulong calculatedValue = 0;
#if NETSTANDARD2_0
					string[] flagValues = enumString.Split(_split, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
#else
                    string[] flagValues = enumString.Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
#endif
                    foreach(string flagValue in flagValues)
                    {
                        // Case sensitive search attempted first.
                        if(_transformedToRaw.TryGetValue(flagValue, out enumInfo))
                        {
                            calculatedValue |= enumInfo.RawValue;
                        }
                        else
                        {
                            // Case insensitive search attempted second.
                            bool matched = false;
                            foreach(KeyValuePair<string, EnumInfo> enumItem in _transformedToRaw)
                            {
                                if(string.Equals(enumItem.Key, flagValue, StringComparison.OrdinalIgnoreCase))
                                {
                                    calculatedValue |= enumItem.Value.RawValue;
                                    matched = true;
                                    break;
                                }
                            }

                            if(!matched)
                            {
                                //throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(_EnumType, flagValue);
                                throw new Exception("DeserializeUnableToConvertValue");
                            }
                        }
                    }

                    TEnum enumValue = (TEnum)Enum.ToObject(_enumType, calculatedValue);
                    if(_transformedToRaw.Count < 64)
                    {
                        _transformedToRaw[enumString] = new EnumInfo(enumString, enumValue, calculatedValue);
                    }

                    return enumValue;
                }

                // Case insensitive search attempted second.
                foreach(KeyValuePair<string, EnumInfo> enumItem in _transformedToRaw)
                {
                    if(string.Equals(enumItem.Key, enumString, StringComparison.OrdinalIgnoreCase))
                    {
                        return enumItem.Value.EnumValue;
                    }
                }

                //throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(_EnumType, enumString);
                throw new Exception("DeserializeUnableToConvertValue");
            }

            if(token != JsonTokenType.Number || !_allowIntegerValues)
            {
                //throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(_EnumType);
                throw new Exception("DeserializeUnableToConvertValue");
            }

            switch(_enumTypeCode)
            {
                case TypeCode.Int32:
                    if(reader.TryGetInt32(out int int32))
                    {
                        return (TEnum)Enum.ToObject(_enumType, int32);
                    }

                    break;
                case TypeCode.Int64:
                    if(reader.TryGetInt64(out long int64))
                    {
                        return (TEnum)Enum.ToObject(_enumType, int64);
                    }

                    break;
                case TypeCode.Int16:
                    if(reader.TryGetInt16(out short int16))
                    {
                        return (TEnum)Enum.ToObject(_enumType, int16);
                    }

                    break;
                case TypeCode.Byte:
                    if(reader.TryGetByte(out byte ubyte8))
                    {
                        return (TEnum)Enum.ToObject(_enumType, ubyte8);
                    }

                    break;
                case TypeCode.UInt32:
                    if(reader.TryGetUInt32(out uint uint32))
                    {
                        return (TEnum)Enum.ToObject(_enumType, uint32);
                    }

                    break;
                case TypeCode.UInt64:
                    if(reader.TryGetUInt64(out ulong uint64))
                    {
                        return (TEnum)Enum.ToObject(_enumType, uint64);
                    }

                    break;
                case TypeCode.UInt16:
                    if(reader.TryGetUInt16(out ushort uint16))
                    {
                        return (TEnum)Enum.ToObject(_enumType, uint16);
                    }

                    break;
                case TypeCode.SByte:
                    if(reader.TryGetSByte(out sbyte byte8))
                    {
                        return (TEnum)Enum.ToObject(_enumType, byte8);
                    }

                    break;
                case TypeCode.Empty:
                    break;
                case TypeCode.Object:
                    break;
                case TypeCode.DBNull:
                    break;
                case TypeCode.Boolean:
                    break;
                case TypeCode.Char:
                    break;
                case TypeCode.Single:
                    break;
                case TypeCode.Double:
                    break;
                case TypeCode.Decimal:
                    break;
                case TypeCode.DateTime:
                    break;
                case TypeCode.String:
                    break;
            }

            //throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(_EnumType);
            throw new Exception("DeserializeUnableToConvertValue");
        }

        public void Write(Utf8JsonWriter writer, TEnum value)
        {
            if(_rawToTransformed.TryGetValue(value, out EnumInfo? enumInfo))
            {
                writer.WriteStringValue(enumInfo.Name);
                return;
            }

            var rawValue = GetEnumValue(value);

            if(_isFlags)
            {
                ulong calculatedValue = 0;

                var Builder = new StringBuilder();
                foreach(KeyValuePair<TEnum, EnumInfo> enumItem in _rawToTransformed)
                {
                    enumInfo = enumItem.Value;
                    if(!value.HasFlag(enumInfo.EnumValue)
                        || enumInfo.RawValue == 0) // Definitions with 'None' should hit the cache case.
                    {
                        continue;
                    }

                    // Track the value to make sure all bits are represented.
                    calculatedValue |= enumInfo.RawValue;

                    if(Builder.Length > 0)
                    {
                        Builder.Append(", ");
                    }

                    Builder.Append(enumInfo.Name);
                }

                if(calculatedValue == rawValue)
                {
                    string finalName = Builder.ToString();
                    if(_rawToTransformed.Count < 64)
                    {
                        _rawToTransformed[value] = new EnumInfo(finalName, value, rawValue);
                    }

                    writer.WriteStringValue(finalName);
                    return;
                }
            }

            if(!_allowIntegerValues)
            {
                throw new JsonException($"Enum type {_enumType} does not have a mapping for integer value '{rawValue.ToString(CultureInfo.CurrentCulture)}'.");
            }

            switch(_enumTypeCode)
            {
                case TypeCode.Int32:
                    writer.WriteNumberValue((int)rawValue);
                    break;
                case TypeCode.Int64:
                    writer.WriteNumberValue((long)rawValue);
                    break;
                case TypeCode.Int16:
                    writer.WriteNumberValue((short)rawValue);
                    break;
                case TypeCode.Byte:
                    writer.WriteNumberValue((byte)rawValue);
                    break;
                case TypeCode.UInt32:
                    writer.WriteNumberValue((uint)rawValue);
                    break;
                case TypeCode.UInt64:
                    writer.WriteNumberValue(rawValue);
                    break;
                case TypeCode.UInt16:
                    writer.WriteNumberValue((ushort)rawValue);
                    break;
                case TypeCode.SByte:
                    writer.WriteNumberValue((sbyte)rawValue);
                    break;
                case TypeCode.Empty:
                    break;
                case TypeCode.Object:
                    break;
                case TypeCode.DBNull:
                    break;
                case TypeCode.Boolean:
                    break;
                case TypeCode.Char:
                    break;
                case TypeCode.Single:
                    break;
                case TypeCode.Double:
                    break;
                case TypeCode.Decimal:
                    break;
                case TypeCode.DateTime:
                    break;
                case TypeCode.String:
                    break;
                default:
                    throw new JsonException(); // GetEnumValue should have already thrown.
            }
        }

        private ulong GetEnumValue(object value)
        {
            return _enumTypeCode switch
            {
                TypeCode.Int32 => (ulong)(int)value,
                TypeCode.Int64 => (ulong)(long)value,
                TypeCode.Int16 => (ulong)(short)value,
                TypeCode.Byte => (byte)value,
                TypeCode.UInt32 => (uint)value,
                TypeCode.UInt64 => (ulong)value,
                TypeCode.UInt16 => (ushort)value,
                TypeCode.SByte => (ulong)(sbyte)value,
                TypeCode.Empty => throw new NotImplementedException(),
                TypeCode.Object => throw new NotImplementedException(),
                TypeCode.DBNull => throw new NotImplementedException(),
                TypeCode.Boolean => throw new NotImplementedException(),
                TypeCode.Char => throw new NotImplementedException(),
                TypeCode.Single => throw new NotImplementedException(),
                TypeCode.Double => throw new NotImplementedException(),
                TypeCode.Decimal => throw new NotImplementedException(),
                TypeCode.DateTime => throw new NotImplementedException(),
                TypeCode.String => throw new NotImplementedException(),
                _ => throw new NotSupportedException($"Enum '{value}' of {_enumTypeCode} type is not supported."),
            };
        }
    }
}
