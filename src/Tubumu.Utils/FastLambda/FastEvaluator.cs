using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tubumu.Utils.FastLambda
{
    /// <summary>
    /// FastEvaluator
    /// </summary>
    public class FastEvaluator : IEvaluator
    {
        private static readonly IExpressionCache<Func<List<object>, object>> Cache =
            new HashedListCache<Func<List<object>, object>>();

        private readonly DelegateGenerator _delegateGenerator = new DelegateGenerator();
        private readonly ConstantExtractor _constantExtrator = new ConstantExtractor();

        private readonly IExpressionCache<Func<List<object>, object>> _cache;
        private readonly Func<Expression, Func<List<object>, object>> _creatorDelegate;

        /// <summary>
        /// Constructor
        /// </summary>
        public FastEvaluator()
            : this(Cache)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cache"></param>
        public FastEvaluator(IExpressionCache<Func<List<object>, object>> cache)
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
            return func(parameters);
        }
    }
}
