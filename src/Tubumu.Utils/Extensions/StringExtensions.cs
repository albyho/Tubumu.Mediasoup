using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.International.Converters.PinYinConverter;

namespace System
{
    /// <summary>
    /// StringExtensions
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Regex TagRegex = new("<[^<>]*>", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex GuidRegex = new(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex SpaceRegex = new(@"\s+", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex NonWordCharsRegex = new(@"[^\w]+", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex UrlRegex = new("(^|[^\\w'\"]|\\G)(?<uri>(?:https?|ftp)(?:&#58;|:)(?:&#47;&#47;|//)(?:[^./\\s'\"<)\\]]+\\.)+[^./\\s'\"<)\\]]+(?:(?:&#47;|/).*?)?)(?:[\\s\\.,\\)\\]'\"]?(?:\\s|\\.|\\)|\\]|,|<|$))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SubDirectoryRegex = new(@"^[a-zA-Z0-9-_]+(/[a-zA-Z0-9-_]+)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex VirtualDirectoryRegex = new(@"^~(/[a-zA-Z0-9-_]+)+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region Guid相关

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

        #endregion Guid相关

        #region 字符串截取

        /// <summary>
        /// Substr
        /// </summary>
        /// <param name="source"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string SubstringWithEllipsis(this string source, int len)
        {
            return source.Substring(len, "...");
        }

        /// <summary>
        /// Substr
        /// </summary>
        /// <param name="source"></param>
        /// <param name="len"></param>
        /// <param name="att"></param>
        /// <returns></returns>
        public static string Substring(this string source, int len, string att)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            att ??= string.Empty;

            var rChinese = new Regex(@"[\u4e00-\u9fa5]"); //验证中文
            var rEnglish = new Regex(@"^[A-Za-z0-9]+$");  //验证字母

            if (rChinese.IsMatch(source))
            {
                //中文
                return source.Length > len ? source[..len] + att : source;
            }
            else if (rEnglish.IsMatch(source))
            {
                //英文
                return source.Length > len * 2 ? source[..(len * 2)] + att : source;
            }
            return (source.Length > len) ? source[..len] + att : source;
        }

        #endregion 字符串截取

        #region 字符串空 / null 校验

        /// <summary>
        /// IsNullOrEmpty
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string? source)
        {
            return string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// IsNullOrWhiteSpace
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string? source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        /// <summary>
        /// NullOrWhiteSpaceReplace
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string NullOrWhiteSpaceReplace(this string? source, string newValue)
        {
            return !string.IsNullOrWhiteSpace(source) ? source : newValue;
        }

        /// <summary>
        /// NullOrEmptyReplace
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string NullOrEmptyReplace(this string? source, string newValue)
        {
            return !string.IsNullOrEmpty(source) ? source : newValue;
        }

        #endregion 字符串空 / null 校验

        #region 字符串格式化

        /// <summary>
        /// FormatWith
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, object arg0)
        {
            return string.Format(format, arg0);
        }

        /// <summary>
        /// FormatWith
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, object arg0, object arg1)
        {
            return string.Format(format, arg0, arg1);
        }

        /// <summary>
        /// FormatWith
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, object arg0, object arg1, object arg2)
        {
            return string.Format(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// FormatWith
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// FormatWith
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, format, args);
        }

        #endregion 字符串格式化

        #region 目录相关

        /// <summary>
        /// 确保目录为子目录
        /// </summary>
        /// <example>
        /// <para>~/目录名/ -> 目录名</para>
        /// <para>~/目录名/子目录名/ -> 目录名/子目录名</para>
        /// </example>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string EnsureSubFolder(this string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.StartsWith("~"))
            {
                path = path[1..];
            }

            string[] pathNames = path.Split(Path.DirectorySeparatorChar);
            path = string.Empty;

            foreach (string p in pathNames)
            {
                if (p != string.Empty)
                {
                    path += p + Path.DirectorySeparatorChar;
                }
            }
            if (path.EndsWith(Path.DirectorySeparatorChar))
            {
                path = path[0..^1];
            }
            return path;
        }

        /// <summary>
        /// 确保目录为根目录
        /// </summary>
        /// <example>
        /// <para>目录名/ -> ~/目录名</para>
        /// <para>/目录名/子目录名/ -> ~/目录名/子目录名</para>
        /// </example>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string EnsureVirtualDirectory(this string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!path.StartsWith($"~{Path.DirectorySeparatorChar}"))
            {
                if (!path.StartsWith(Path.DirectorySeparatorChar))
                {
                    path = Path.DirectorySeparatorChar + path;
                }

                if (!path.StartsWith("~"))
                {
                    path = "~" + path;
                }
            }
            if (path.EndsWith(Path.DirectorySeparatorChar))
            {
                path = path[0..^1];
            }
            return path;
        }

        /// <summary>
        /// 判断目录是否为子目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns><c>true</c>是子目录；<c>false</c>不是子目录</returns>
        public static bool IsSubDirectory(this string path)
        {
            return !path.IsNullOrWhiteSpace() && SubDirectoryRegex.IsMatch(path);
        }

        /// <summary>
        /// 判断目录是否为虚拟目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns><c>true</c>是虚拟目录；<c>false</c>不是虚拟目录</returns>
        public static bool IsVirtualDirectory(this string path)
        {
            return !path.IsNullOrWhiteSpace() && VirtualDirectoryRegex.IsMatch(path);
        }

        #endregion 目录相关

        /// <summary>
        /// 字符串重复
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="times">重复次数</param>
        /// <returns></returns>
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
        public static string? ToNullableString<T>(this T source) where T : class
        {
            return source?.ToString();
        }

        /// <summary>
        /// 如果源对象为 null ，则返回 string.Empty ，否则返回其 ToString 方法返回值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToEmptyableString<T>(this T? source) where T : class
        {
            return source != null ? source.ToString()! : string.Empty;
        }

        /// <summary>
        /// WithUrl
        /// </summary>
        /// <param name="source"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string? WithUrl(this string source, string url)
        {
            return source == null || url == null ? null : $"<a href=\"{url}\">{source}</a>";
        }

        #region 拼音

        /// <summary>
        /// ConvertToPinYin
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ConvertToPinYin(this string source)
        {
            var pinYin = "";
            foreach (var item in source)
            {
                if (ChineseChar.IsValidChar(item))
                {
                    var cc = new ChineseChar(item);
                    //PYstr += string.Join("", cc.Pinyins.ToArray());
                    pinYin += cc.Pinyins[0][0..^1].ToLowerInvariant();
                    //PYstr += cc.Pinyins[0].Substring(0, cc.Pinyins[0].Length - 1).Substring(0, 1).ToLower();
                }
                else
                {
                    pinYin += item.ToString();
                }
            }
            return pinYin;
        }

        /// <summary>
        /// ConvertToPinYinPY
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Tuple<string, string> ConvertToPinYinPY(this string source)
        {
            var pinYin = "";
            var py = "";
            foreach (char item in source)
            {
                if (ChineseChar.IsValidChar(item))
                {
                    var cc = new ChineseChar(item);
                    var pinYinString = cc.Pinyins[0][0..^1].ToLowerInvariant();
                    pinYin += pinYinString;
                    py += pinYinString[..1];
                }
                else
                {
                    var charString = item.ToString();
                    pinYin += charString;
                    py += charString;
                }
            }
            return new Tuple<string, string>(pinYin, py);
        }

        /// <summary>
        /// ConvertToPY
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ConvertToPY(this string source)
        {
            string py = "";
            foreach (char item in source)
            {
                if (ChineseChar.IsValidChar(item))
                {
                    var cc = new ChineseChar(item);
                    py += cc.Pinyins[0][..1].ToLowerInvariant();
                }
                else
                {
                    var charString = item.ToString();
                    py += charString;
                }
            }
            return py;
        }

        #endregion 拼音
    }
}
