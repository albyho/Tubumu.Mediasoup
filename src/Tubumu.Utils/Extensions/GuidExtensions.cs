using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// Guid 扩展方法
    /// </summary>
    public static class GuidExtensions
    {
        private static readonly Regex GuidRegex = new(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// IsNullOrEmpty
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this Guid? source)
        {
            return source == null || source.Value == Guid.Empty;
        }

        /// <summary>
        /// 校验字符串是否是 Guid 格式
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGuid(this string source)
        {
            return !source.IsNullOrWhiteSpace() && GuidRegex.IsMatch(source);
        }

        /// <summary>
        /// 字符串转换为Guid
        /// </summary>
        /// <param name="source"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool GuidTryParse(this string source, out Guid result)
        {
            if (source.IsNullOrWhiteSpace())
            {
                result = Guid.Empty;
                return false;
            }

            try
            {
                result = new Guid(source);
                return true;
            }
            catch (FormatException)
            {
                result = Guid.Empty;
                return false;
            }
            catch (OverflowException)
            {
                result = Guid.Empty;
                return false;
            }
            catch
            {
                result = Guid.Empty;
                return false;
            }
        }
    }
}
