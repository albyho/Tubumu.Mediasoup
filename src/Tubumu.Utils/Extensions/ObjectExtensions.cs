using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Tubumu.Utils.FastReflection;

namespace System
{
    /// <summary>
    /// ObjectExtensions
    /// </summary>
    public static class ObjectExtensions
    {
        public static JsonSerializerOptions DefaultJsonSerializerOptions { get; }

        static ObjectExtensions()
        {
            DefaultJsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            DefaultJsonSerializerOptions.Converters.Add(new JsonStringEnumMemberConverter());
        }

        /// <summary>
        /// ToJson
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToJson(this object source)
        {
            return JsonSerializer.Serialize(source, DefaultJsonSerializerOptions);
        }

        /// <summary>
        /// FromJson
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T? FromJson<T>(string json) where T : class
        {
            return string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json, DefaultJsonSerializerOptions);
        }

        /// <summary>
        /// 创建一个新的类型的对象，并将现有对象的属性值赋给新对象相同名称的属性
        /// </summary>
        /// <typeparam name="T">新对象的类型</typeparam>
        /// <param name="source">现有对象</param>
        /// <returns>新的对象</returns>
        public static T? ToModel<T>(this object source) where T : new()
        {
            if (source == null)
            {
                return default;
            }

            var target = new T();

            return UpdateFrom(target, source);
        }

        /// <summary>
        /// 将目标对象的属性值赋给源对象相同名称的属性
        /// </summary>
        /// <typeparam name="T">泛型类型参数</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <returns>源对象</returns>
        public static T? UpdateFrom<T>(this T source, object target)
        {
            if (source == null)
            {
                return default;
            }

            if (target == null)
            {
                return source;
            }

            Type type = typeof(T);

            foreach (PropertyDescriptor targetPropertyDescriptor in TypeDescriptor.GetProperties(target))
            {
                var sourcePropertyInfo = type.GetProperty(targetPropertyDescriptor.Name, BindingFlags.Instance | BindingFlags.Public);
                if (sourcePropertyInfo != null && sourcePropertyInfo.CanWrite)
                {
                    var targetPropertyAccessor = new PropertyAccessor(sourcePropertyInfo);
                    var value = targetPropertyDescriptor.GetValue(target);
                    if (value != null)
                    {
                        if (sourcePropertyInfo.PropertyType.IsEnum)
                        {
                            targetPropertyAccessor.SetValue(source, Enum.ToObject(sourcePropertyInfo.PropertyType, value));
                        }
                        else
                        {
                            targetPropertyAccessor.SetValue(source, value);
                        }
                    }
                    else
                    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        targetPropertyAccessor.SetValue(source, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    }
                }
            }
            return source;
        }

        /// <summary>
        /// XML 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        public static T? FromXml<T>(string serializedObject) where T : class
        {
            return FromXml(typeof(T), serializedObject) as T;
        }

        /// <summary>
        /// XML 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        public static object? FromXml(this Type type, string serializedObject)
        {
            object? filledObject = null;
            if (!string.IsNullOrEmpty(serializedObject))
            {
                try
                {
                    var serializer = new XmlSerializer(type);
                    using var reader = new StringReader(serializedObject);
                    filledObject = serializer.Deserialize(reader);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"FromXml() | {ex.Message}");
                    filledObject = null;
                }
            }
            return filledObject;
        }

        /// <summary>
        /// XML 序列化
        /// </summary>
        /// <param name="source"></param>
        /// <param name="noneXsn"></param>
        /// <returns></returns>
        public static string ToXml(this object source, bool noneXsn = false)
        {
            var serializedObject = string.Empty;

            if (source != null)
            {
                var serializer = new XmlSerializer(source.GetType());

                if (noneXsn)
                {
                    var sb = new StringBuilder();

                    //去除xml version...
                    var settings = new XmlWriterSettings
                    {
                        Indent = true,
                        Encoding = Encoding.UTF8,
                        OmitXmlDeclaration = true, //Remove the <?xml version="1.0" encoding="utf-8"?>
                    };
                    var xmlWriter = XmlWriter.Create(sb, settings);

                    //去除默认命名空间
                    var xsn = new XmlSerializerNamespaces();
                    xsn.Add(string.Empty, string.Empty);

                    serializer.Serialize(xmlWriter, source, xsn);
                    return sb.ToString();
                }
                else
                {
                    using var writer = new StringWriter();
                    serializer.Serialize(writer, source);
                    return writer.ToString();
                }
            }
            return serializedObject;
        }

        /// <summary>
        /// 判断是否是 Json 数字或普通数字。
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsNumericType(this object o)
        {
            var jsonElement = o as JsonElement?;
            if (jsonElement != null)
            {
                return jsonElement.Value.ValueKind == JsonValueKind.Number;
            }

            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 判断是否是 Json 字符串或普通字符串。
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsStringType(this object o)
        {
            var jsonElement = o as JsonElement?;
            return jsonElement != null ? jsonElement.Value.ValueKind == JsonValueKind.String : o.GetType() == typeof(string);
        }
    }
}
