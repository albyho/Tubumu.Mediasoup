using System.Collections.ObjectModel;
using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// 枚举器扩展方法
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 对枚举器的每个元素执行指定的操作
        /// </summary>
        /// <typeparam name="T">枚举器类型参数</typeparam>
        /// <param name="source">枚举器</param>
        /// <param name="action">要对枚举器的每个元素执行的委托</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null || action == null)
            {
                return;
            }

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// 指示指定的枚举器是否为 null 或没有任何元素
        /// </summary>
        /// <typeparam name="T">枚举器类型参数</typeparam>
        /// <param name="source">要测试的枚举器</param>
        /// <returns>true:枚举器是null或者没有任何元素 false:枚举器不为null并且包含至少一个元素</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        {
            return source?.Any() != true;
        }

        /// <summary>
        /// 判断指定的集合是否为 null 或没有任何元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static bool IsNullOrEmpty<T>(this ICollection<T>? source)
        {
            return source == null || source.Count == 0;
        }

        /// <summary>
        /// 判断指定的数组是否为 null 或没有任何元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static bool IsNullOrEmpty<T>(this T[]? source)
        {
            return source == null || source.Length == 0;
        }

        /// <summary>
        /// 对 String 型序列的每个元素进行字符串替换操作
        /// </summary>
        /// <param name="source">源序列</param>
        /// <param name="oldValue">查找字符串</param>
        /// <param name="newValue">替换字符串</param>
        /// <returns>新的String型序列</returns>
        public static IEnumerable<string> Replace(this IEnumerable<string> source, string oldValue, string newValue)
        {
            return source.Select(format => format.Replace(oldValue, newValue));
        }

        /// <summary>
        /// 将序列转化为 ReadOnlyCollection
        /// </summary>
        /// <typeparam name="T">类型参数</typeparam>
        /// <param name="source">源序列</param>
        ///
        public static IList<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            return new ReadOnlyCollection<T>(source.ToList());
        }

        /// <summary>
        /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
        /// <para><see cref="https://github.com/aspnetboilerplate/aspnetboilerplate/blob/e0ded5d8702f389aa1f5947d3446f16aec845287/src/Abp/Collections/Extensions/EnumerableExtensions.cs"/></para>
        /// </summary>
        /// <param name="source">Enumerable to apply filtering</param>
        /// <param name="condition">A boolean value</param>
        /// <param name="predicate">Predicate to filter the enumerable</param>
        /// <returns>Filtered or not filtered enumerable based on <paramref name="condition"/></returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
        /// <para><see cref="https://github.com/aspnetboilerplate/aspnetboilerplate/blob/e0ded5d8702f389aa1f5947d3446f16aec845287/src/Abp/Collections/Extensions/EnumerableExtensions.cs"/></para>
        /// </summary>
        /// <param name="source">Enumerable to apply filtering</param>
        /// <param name="condition">A boolean value</param>
        /// <param name="predicate">Predicate to filter the enumerable</param>
        /// <returns>Filtered or not filtered enumerable based on <paramref name="condition"/></returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, int, bool> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        /// Concatenates the members of a collection, using the specified separator between each member.
        /// This is a shortcut for string.Join(...)
        /// <para><see cref="https://github.com/aspnetboilerplate/aspnetboilerplate/blob/e0ded5d8702f389aa1f5947d3446f16aec845287/src/Abp/Collections/Extensions/EnumerableExtensions.cs"/></para>
        /// </summary>
        /// <param name="source">A collection that contains the objects to concatenate.</param>
        /// <param name="separator">The string to use as a separator. separator is included in the returned string only if values has more than one element.</param>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <returns>A string that consists of the members of values delimited by the separator string. If values has no members, the method returns System.String.Empty.</returns>
        public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join(separator, source);
        }
    }
}
