using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FBS.RtpParameters;
using SixLabors.ImageSharp.Metadata.Profiles.Iptc;

namespace Tubumu.Mediasoup
{
    public static class ValudUnionExtensions
    {
        public static object? UnPackValueUnion(this ValueUnion value)
        {
            return value.Type switch
            {
                Value.Boolean => value.AsBoolean(),
                Value.Integer32 => value.AsInteger32(),
                Value.Double => value.AsDouble(),
                Value.String => value.AsString(),
                Value.Integer32Array => value.AsInteger32Array(),
                Value.NONE => null,
                _ => throw new ArgumentException($"Unsupported type for conversion: {value.Type}"),
            };
        }

        public static ValueUnion ConvertToValueUnion(this object value)
        {
            var result = new ValueUnion();

            if(value == null)
            {
                result.Type = Value.NONE;
                result.Value_ = null;
                return result;
            }

            if(value is JsonElement element)
            {
                switch(element.ValueKind)
                {
                    case JsonValueKind.Number:
                        {
                            if(element.TryGetInt32(out int intValue))
                            {
                                result.Type = Value.Integer32;
                                result.Value_ = new Integer32T
                                {
                                    Value = intValue,
                                };
                                return result;
                            }
                            if(element.TryGetDouble(out double doubleValue))
                            {
                                result.Type = Value.Double;
                                result.Value_ = new DoubleT
                                {
                                    Value = intValue,
                                };
                                return result;
                            }
                            if(element.TryGetSingle(out float floatValue))
                            {
                                result.Type = Value.Double;
                                result.Value_ = new DoubleT
                                {
                                    Value = intValue,
                                };
                                return result;
                            }
                        }
                        break;
                    case JsonValueKind.String:
                        {
                            result.Type = Value.String;
                            result.Value_ = new StringT
                            {
                                Value = element.ToString(),
                            };
                            return result;
                        }
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        {
                            result.Type = Value.Boolean;
                            result.Value_ = new BooleanT
                            {
                                Value = element.GetBoolean() ? (byte)1 : (byte)0
                            };
                            return result;
                        }
                    case JsonValueKind.Array:
                        {
                            var integer32Array = new Integer32T[element.GetArrayLength()];
                            var index = 0;
                            foreach(var itemElement in element.EnumerateArray())
                            {
                                if(element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out int intValue))
                                {
                                    integer32Array[index++] = new Integer32T
                                    {
                                        Value = intValue,
                                    };
                                }
                                else
                                {
                                    throw new ArgumentException($"Unsupported type for conversion: {itemElement.GetType().FullName}");
                                }
                            }

                            result.Type = Value.Integer32Array;
                            result.Value_ = integer32Array;
                            return result;
                        }
                    default:
                        // 对于无法处理的类型，可能需要进行适当的处理
                        throw new ArgumentException($"Unsupported type for conversion: {value.GetType().FullName}");
                }
            }

            if(value is string stringValue)
            {
                if(stringValue.IsNullOrWhiteSpace())
                {
                    result.Type = Value.NONE;
                    result.Value_ = null;
                    return result;
                }
                else if(bool.TryParse(stringValue, out bool boolValue))
                {
                    result.Type = Value.Boolean;
                    result.Value_ = new BooleanT
                    {
                        Value = boolValue ? (byte)1 : (byte)0
                    };
                    return result;
                }
                else
                {
                    result.Type = Value.String;
                    result.Value_ = new StringT
                    {
                        Value = stringValue,
                    };
                    return result;
                }
            }

            if(IsInteger32Type(value))
            {
                result.Type = Value.Integer32;
                result.Value_ = new Integer32T
                {
                    Value = Convert.ToInt32(value),
                };
                return result;
            }

            if(value is IEnumerable<int> intEnumerableValue)
            {
                result.Type = Value.Integer32Array;
                result.Value_ = intEnumerableValue.Select(m=> new Integer32T
                {
                    Value = (int)value,
                }).ToArray();
                return result;
            }

            if(value is double || value is float)
            {
                result.Type = Value.Double;
                result.Value_ = new DoubleT
                {
                    Value = Convert.ToDouble(value),
                };
                result.Value_ = Convert.ToDouble(value);
                return result;
            }

            if(value is bool x)
            {
                result.Type = Value.Boolean;
                result.Value_ = new BooleanT
                {
                    Value = x ? (byte)1 : (byte)0
                };
                return result;
            }

            // 对于无法处理的类型，可能需要进行适当的处理
            throw new ArgumentException($"Unsupported type for conversion: {value.GetType().FullName}");
        }

        public static bool IsInteger32Type(this object value)
        {
            // byte sbyte short ushort int uint long ulong
            return value is byte || value is sbyte || value is short || value is ushort || value is int;
        }
    }
}
