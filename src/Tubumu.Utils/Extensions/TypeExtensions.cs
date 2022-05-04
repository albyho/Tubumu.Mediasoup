using System.ComponentModel;
using System.Linq.Expressions;

namespace System
{
    /// <summary>
    /// TypeExtensions
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 获取类型属性的字符串表示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string PropertyName<T>(Expression<Func<T, object>> expression)
        {
            switch (expression.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression.Body).Member.Name;

                case ExpressionType.Convert:
                    return ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member.Name;

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 判断类型是否是复合类型
        /// <para>string , int 等从 string 转化而来的类型为简单类型，其他为复合类型</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsComplexType(this Type type)
        {
            return !TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
        }
    }
}
