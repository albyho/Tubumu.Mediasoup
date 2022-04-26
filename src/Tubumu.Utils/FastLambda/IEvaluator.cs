using System.Linq.Expressions;

namespace Tubumu.Utils.FastLambda
{
    /// <summary>
    /// IEvaluator
    /// </summary>
    public interface IEvaluator
    {
        /// <summary>
        /// Eval
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        object Eval(Expression exp);
    }
}
