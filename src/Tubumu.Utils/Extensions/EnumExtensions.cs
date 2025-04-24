using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace System
{
    /// <summary>
    /// 枚举扩展方法
    /// </summary>
    public static class EnumExtensions
    {
        #region DescriptionAttribute

        /// <summary>
        /// 根据 Description 获取枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="description">枚举描述</param>
        /// <returns>枚举</returns>
        public static T GetValueByDescription<T>(this string description)
            where T : Enum
        {
            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                var curDesc = field.GetDescriptAttributes();
                if (curDesc?.Length > 0)
                {
                    if (curDesc[0].Description == description)
                    {
                        return (T)field.GetValue(null)!;
                    }
                }
                else
                {
                    if (field.Name == description)
                    {
                        return (T)field.GetValue(null)!;
                    }
                }
            }

            throw new ArgumentException("未能找到对应的枚举.", nameof(description));
        }

        /// <summary>
        /// 获取字段 Description
        /// </summary>
        /// <param name="fieldInfo">FieldInfo</param>
        /// <returns>DescriptionAttribute[] </returns>
        private static DescriptionAttribute[]? GetDescriptAttributes(this FieldInfo fieldInfo)
        {
            return fieldInfo != null
                ? (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)
                : null;
        }

        /// <summary>
        /// 根据枚举值，获取枚举的 DescriptionAttribute 的 Description 或字符串表示。
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>DisplayName</returns>
        public static string GetDescription(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var enumName = Enum.GetName(type, enumValue) ?? throw new NotSupportedException();

            var attribute = type.GetField(enumName)!.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            if (attribute != null)
            {
                var displayName = ((DisplayAttribute)attribute).Name;
                if (displayName != null)
                {
                    return displayName;
                }
            }

            return enumValue.ToString();
        }

        /// <summary>
        /// 根据枚举的类型，获取枚举的 DescriptionAttribute 的 Description 形成的字典。
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="type">枚举类型</param>
        /// <returns>枚举值与对应的 DisplayAttribute 的 Name 形成的字典</returns>
        public static IEnumerable<KeyValuePair<T, string>> GetDescriptionMap<T>(this Type type)
            where T : Enum
        {
            return from e in Enum.GetValues(type).Cast<T>() select new KeyValuePair<T, string>(e, e.GetDescription());
        }

        /// <summary>
        /// 根据枚举的类型，获取枚举的 DisplayAttribute 的 Name 形成的字典。(非扩展方法)
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <returns>枚举值与对应的 DisplayAttribute 的 Name 形成的字典</returns>
        public static IEnumerable<KeyValuePair<T, string>> GetDescriptionMap<T>()
            where T : Enum
        {
            return GetDescriptionMap<T>(typeof(T));
        }

        #endregion DescriptionAttribute

        #region DisplayAttribute

        /// <summary>
        /// 根据枚举值，获取枚举的 DisplayAttribute 的 Name。
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>DisplayName</returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var enumName = Enum.GetName(type, enumValue) ?? throw new NotSupportedException();

            var attribute = type.GetField(enumName)!.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (attribute != null)
            {
                var displayName = ((DisplayAttribute)attribute).Name;
                if (displayName != null)
                {
                    return displayName;
                }
            }

            return enumValue.ToString();
        }

        /// <summary>
        /// 根据枚举的类型，获取枚举的 DisplayAttribute 的 Name 形成的字典。
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="type">枚举类型</param>
        /// <returns>枚举值与对应的 DisplayAttribute 的 Name 形成的字典</returns>
        public static IEnumerable<KeyValuePair<T, string>> GetDisaplayNameMap<T>(this Type type)
            where T : Enum
        {
            return from e in Enum.GetValues(type).Cast<T>() select new KeyValuePair<T, string>(e, e.GetDisplayName());
        }

        /// <summary>
        /// 根据枚举的类型，获取枚举的 DisplayAttribute 的 Name 形成的字典。(非扩展方法)
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <returns>枚举值与对应的 DisplayAttribute 的 Name 形成的字典</returns>
        public static IEnumerable<KeyValuePair<T, string>> GetDisaplayNameMap<T>()
            where T : Enum
        {
            return GetDisaplayNameMap<T>(typeof(T));
        }

        #endregion DisplayAttribute

        #region EnumMemberAttribute

        /// <summary>
        /// 根据枚举值，获取枚举的 EnumMemberAttribute 的 Value。
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>DisplayName</returns>
        public static string GetEnumMemberValue(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var enumName = Enum.GetName(type, enumValue) ?? throw new NotSupportedException();

            var attribute = type.GetField(enumName)!.GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault();
            if (attribute != null)
            {
                var value = ((EnumMemberAttribute)attribute).Value;
                if (value != null)
                {
                    return value;
                }
            }

            return enumValue.ToString();
        }

        /// <summary>
        /// 根据枚举的类型，获取枚举的 EnumMemberAttribute 的 Value 形成的字典。
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="type">枚举类型</param>
        /// <returns>枚举值与对应的 EnumMemberAttribute 的 Value 形成的字典</returns>
        public static IEnumerable<KeyValuePair<T, string>> GetEnumMemberValueMap<T>(this Type type)
            where T : Enum
        {
            return from e in Enum.GetValues(type).Cast<T>() select new KeyValuePair<T, string>(e, e.GetEnumMemberValue());
        }

        /// <summary>
        /// 根据枚举的类型，获取枚举的 EnumMemberAttribute 的 Value 形成的字典。(非扩展方法)
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <returns>枚举值与对应的 EnumMemberAttribute 的 Value 形成的字典</returns>
        public static IEnumerable<KeyValuePair<T, string>> GetEnumMemberValueMap<T>()
            where T : Enum
        {
            return GetDisaplayNameMap<T>(typeof(T));
        }

        #endregion EnumMemberAttribute

        #region RawConstantValue

        /// <summary>
        /// 获取枚举的 Int32 数字值。
        /// </summary>
        public static int GetInt32(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var enumName = Enum.GetName(type, enumValue)!;
            var enumFieldInfo = type.GetField(enumName);
            return (int)enumFieldInfo!.GetRawConstantValue()!;
        }

        #endregion RawConstantValue
    }
}
