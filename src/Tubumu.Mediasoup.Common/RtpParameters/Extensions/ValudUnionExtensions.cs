using FBS.RtpParameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tubumu.Mediasoup
{
    public static class ValudUnionExtensions
    {
        public static bool IsIntegerType(this object value)
        {
            return value is byte || value is sbyte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong;
        }

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
            }
            else if(value is string stringValue)
            {
                if(stringValue.IsNullOrWhiteSpace())
                {
                    result.Type = Value.NONE;
                    result.Value_ = null;
                }
                else if(bool.TryParse(stringValue, out bool boolValue))
                {
                    result.Type = Value.Boolean;
                    result.Value_ = boolValue;
                }
                else
                {
                    result.Type = Value.String;
                    result.Value_ = stringValue;
                }
            }
            else if(IsIntegerType(value))
            {
                result.Type = Value.Integer32;
                result.Value_ = (int)value;
            }
            else if(value is IEnumerable<int> intEnumerableValue)
            {
                result.Type = Value.Integer32Array;
                result.Value_ = intEnumerableValue.ToArray(); // 转换为数组
            }
            else if(value is double || value is float)
            {
                result.Type = Value.Double;
                result.Value_ = Convert.ToDouble(value);
            }
            else if(value is bool x)
            {
                result.Type = Value.Boolean;
                result.Value_ = x;
            }
            else
            {
                // 对于无法处理的类型，可能需要进行适当的处理
                throw new ArgumentException($"Unsupported type for conversion: {value.GetType().FullName}");
            }

            return result;
        }
    }
}
