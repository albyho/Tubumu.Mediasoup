using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace System
{
    /// <summary>
    /// Object 扩展方法
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
