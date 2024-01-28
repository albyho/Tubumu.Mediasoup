namespace System.Linq.Expressions
{
    /// <summary>
    /// Expression 扩展方法
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 获取表达式树种类型属性的字符串表示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static string PropertyName<T>(this Expression<Func<T, object>> expression)
        {
            return expression.Body.NodeType switch
            {
                ExpressionType.MemberAccess => ((MemberExpression)expression.Body).Member.Name,
                ExpressionType.Convert => ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member.Name,
                _ => throw new NotSupportedException(),
            };
        }
    }
}
