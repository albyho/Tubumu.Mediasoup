using System;
using System.Linq.Expressions;

namespace Tubumu.Utils.FastLambda
{
    /// <summary>
    /// CacheEvaluator
    /// </summary>
    public class CacheEvaluator : IEvaluator
    {
        private static readonly IExpressionCache<Delegate> s_cache = new HashedListCache<Delegate>();

        private readonly WeakTypeDelegateGenerator _delegateGenerator = new();
        private readonly ConstantExtractor _constantExtrator = new();

        private readonly IExpressionCache<Delegate> _cache;
        private readonly Func<Expression, Delegate> _creatorDelegate;

        /// <summary>
        /// Constructor
        /// </summary>
        public CacheEvaluator()
            : this(s_cache)
        { }

        /// <summary>
        /// CacheEvaluator
        /// </summary>
        /// <param name="cache"></param>
        public CacheEvaluator(IExpressionCache<Delegate> cache)
        {
            _cache = cache;
            _creatorDelegate = (key) => _delegateGenerator.Generate(key);
        }

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

            var parameters = _constantExtrator.Extract(exp);
            var func = _cache.Get(exp, _creatorDelegate);
            return func.DynamicInvoke(parameters.ToArray())!;
        }
    }
}
