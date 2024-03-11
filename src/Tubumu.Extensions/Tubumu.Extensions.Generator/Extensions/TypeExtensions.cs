using System.ComponentModel;

namespace System
{
    /// <summary>
    /// Type 扩展方法
    /// </summary>
    [Literal($"{nameof(Tubumu)}.{nameof(Tubumu.Templates)}.{nameof(TypeExtensions)}")]
    internal static class TypeExtensions
    {
        /// <summary>
        /// 判断类型是否是复合类型
        /// <para>string , int 等从 string 转化而来的类型为简单类型，其他为复合类型</para>
        /// </summary>
        public static bool IsComplexType(this Type type)
        {
            return !TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
        }
    }
}
