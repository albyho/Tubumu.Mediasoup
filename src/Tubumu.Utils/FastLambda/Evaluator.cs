using System;
using System.Linq.Expressions;

namespace Tubumu.Utils.FastLambda
{
    /// <summary>
    /// Evaluator
    /// </summary>
    public class Evaluator : IEvaluator
    {
        /// <summary>
        /// Eval
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public object Eval(Expression exp)
        {
            if (exp.NodeType == ExpressionType.Constant)
            {
                return ((ConstantExpression)exp).Value!;
            }

            LambdaExpression lambda = Expression.Lambda(exp);
            Delegate fn = lambda.Compile();

            return fn.DynamicInvoke(null)!;
        }
    }
}
