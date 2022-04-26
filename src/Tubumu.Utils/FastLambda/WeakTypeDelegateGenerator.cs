using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tubumu.Utils.FastLambda
{
    /// <summary>
    /// WeakTypeDelegateGenerator
    /// </summary>
    public class WeakTypeDelegateGenerator : ExpressionVisitor
    {
        private List<ParameterExpression>? _parameters;

        /// <summary>
        /// Generate
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Delegate Generate(Expression exp)
        {
            _parameters = new List<ParameterExpression>();

            var body = this.Visit(exp);
            var lambda = Expression.Lambda(body, _parameters.ToArray());
            return lambda.Compile();
        }

        /// <summary>
        /// VisitConstant
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (_parameters == null)
            {
                throw new NullReferenceException(nameof(_parameters));
            }

            var p = Expression.Parameter(c.Type, "p" + _parameters.Count);
            _parameters.Add(p);
            return p;
        }
    }
}
