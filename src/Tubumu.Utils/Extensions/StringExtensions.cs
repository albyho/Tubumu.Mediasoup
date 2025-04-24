using System.Text;

namespace System
{
    /// <summary>
    /// String 扩展方法
    /// </summary>
    public static class StringExtensions
    {
        #region 字符串空 / null 校验

        /// <summary>
        /// IsNullOrEmpty
        /// </summary>
        public static bool IsNullOrEmpty(this string? source)
        {
            return string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// IsNullOrWhiteSpace
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string? source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        /// <summary>
        /// NullOrWhiteSpaceReplace
        /// </summary>
        public static string NullOrWhiteSpaceReplace(this string? source, string newValue)
        {
            return !string.IsNullOrWhiteSpace(source) ? source : newValue;
        }

        /// <summary>
        /// NullOrEmptyReplace
        /// </summary>
        public static string NullOrEmptyReplace(this string? source, string newValue)
        {
            return !string.IsNullOrEmpty(source) ? source : newValue;
        }

        #endregion 字符串空 / null 校验

        #region 字符串格式化

        /// <summary>
        /// FormatWith
        /// </summary>
        public static string FormatWith(this string format, object arg0)
        {
            return string.Format(format, arg0);
        }

        /// <summary>
        /// FormatWith
        /// </summary>
        public static string FormatWith(this string format, object arg0, object arg1)
        {
            return string.Format(format, arg0, arg1);
        }

        /// <summary>
        /// FormatWith
        /// </summary>
        public static string FormatWith(this string format, object arg0, object arg1, object arg2)
        {
            return string.Format(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// FormatWith
        /// </summary>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// FormatWith
        /// </summary>
        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, format, args);
        }

        #endregion 字符串格式化

        /// <summary>
        /// 字符串重复
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="times">重复次数</param>
        public static string Repeat(this string source, int times)
        {
            if (string.IsNullOrEmpty(source) || times <= 0)
            {
                return source;
            }

            var sb = new StringBuilder();
            while (times > 0)
            {
                sb.Append(source);
                times--;
            }

            return sb.ToString();
        }

        /// <summary>
        /// 如果源对象为 null ，则返回 null ，否则返回其 ToString 方法返回值
        /// </summary>
        /// <param name="source">源对象</param>
        /// <returns>字符串</returns>
        public static string? ToNullableString<T>(this T? source)
            where T : class
        {
            return source?.ToString();
        }

        /// <summary>
        /// 如果源对象为 null ，则返回 string.Empty ，否则返回其 ToString 方法返回值
        /// </summary>
        public static string ToEmptyableString<T>(this T? source)
            where T : class
        {
            return source != null ? source.ToString()! : string.Empty;
        }
    }
}
