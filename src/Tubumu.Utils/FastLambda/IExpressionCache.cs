using System;
using System.Linq.Expressions;

namespace Tubumu.Utils.FastLambda
{
    /// <summary>
    /// IExpressionCache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IExpressionCache<T> where T : class
    {
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="key"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        T Get(Expression key, Func<Expression, T> creator);
    }
}
