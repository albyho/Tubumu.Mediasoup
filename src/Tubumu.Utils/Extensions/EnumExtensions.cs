using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace Tubumu.Utils.Extensions
{
    /// <summary>
    /// EnumExtensions
    /// </summary>
    public static class EnumExtensions
    {
        #region DisplayAttribute

        /// <summary>
        /// 根据枚举值，获取枚举的 DisplayAttribute 的 Name。
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>DisplayName</returns>
        public static string GetDisplayName<T>(this T enumValue) where T: Enum
        {
            var type = enumValue.GetType();
            var enumName = Enum.GetName(type, enumValue);
            if (enumName == null)
            {
                throw new NotSupportedException("GetEnumDisplayName");
            }

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
        public static IEnumerable<KeyValuePair<T, string>> GetDisaplayNameMap<T>(this Type type) where T: Enum
        {
            return from e in Enum.GetValues(type).Cast<T>()
                   select new KeyValuePair<T, string?>(e, e.GetDisplayName());
        }

        /// <summary>
        /// 根据枚举的类型，获取枚举的 DisplayAttribute 的 Name 形成的字典。(非扩展方法)
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <returns>枚举值与对应的 DisplayAttribute 的 Name 形成的字典</returns>
        public static IEnumerable<KeyValuePair<T, string>> GetDisaplayNameMap<T>() where T: Enum
        {
            return GetDisaplayNameMap<T>(typeof(T));
        }

        #endregion

        #region EnumMemberAttribute

        /// <summary>
        /// 根据枚举值，获取枚举的 EnumMemberAttribute 的 Value。
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>DisplayName</returns>
        public static string GetEnumMemberValue<T>(this T enumValue) where T : Enum
        {
            var type = enumValue.GetType();
            var enumName = Enum.GetName(type, enumValue);
            if (enumName == null)
            {
                throw new NotSupportedException("GetEnumMemberValue");
            }

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
        public static IEnumerable<KeyValuePair<T, string>> GetEnumMemberValueMap<T>(this Type type) where T : Enum
        {
            return from e in Enum.GetValues(type).Cast<T>()
                   select new KeyValuePair<T, string?>(e, e.GetEnumMemberValue());
        }

        /// <summary>
        /// 根据枚举的类型，获取枚举的 EnumMemberAttribute 的 Value 形成的字典。(非扩展方法)
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <returns>枚举值与对应的 EnumMemberAttribute 的 Value 形成的字典</returns>
        public static IEnumerable<KeyValuePair<T, string>> GetEnumMemberValueMap<T>() where T : Enum
        {
            return GetDisaplayNameMap<T>(typeof(T));
        }

        #endregion

        #region RawConstantValue

        /// <summary>
        /// 获取枚举的 Int32 数字值。
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static int GetInt32<T>(this T enumValue) where T: Enum
        {
            var type = enumValue.GetType();
            var enumName = Enum.GetName(type, enumValue)!;
            var enumFieldInfo = type.GetField(enumName)!;
            return (int)enumFieldInfo.GetRawConstantValue()!;
        }

        #endregion
    }

}
