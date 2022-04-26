using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Tubumu.Utils.FastLambda
{
    /// <summary>
    /// DelegateGenerator
    /// </summary>
    public class DelegateGenerator : ExpressionVisitor
    {
        private static readonly MethodInfo IndexerInfo = typeof(List<object>).GetMethod("get_Item")!;

        private int _parameterCount;
        private ParameterExpression? _parametersExpression;

        /// <summary>
        /// Generate
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Func<List<object>, object> Generate(Expression exp)
        {
            _parameterCount = 0;
            _parametersExpression =
                Expression.Parameter(typeof(List<object>), "parameters");

            var body = this.Visit(exp); // normalize
            if (body.Type != typeof(object))
            {
                body = Expression.Convert(body, typeof(object));
            }

            var lambda = Expression.Lambda<Func<List<object>, object>>(body, _parametersExpression);
            return lambda.Compile();
        }

        /// <summary>
        /// VisitConstant
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            if(_parametersExpression == null)
            {
                throw new NullReferenceException(nameof(_parametersExpression));
            }

            Expression exp = Expression.Call(
                _parametersExpression,
                IndexerInfo,
                Expression.Constant(_parameterCount++));
            return c.Type == typeof(object) ? exp : Expression.Convert(exp, c.Type);
        }
    }
}
