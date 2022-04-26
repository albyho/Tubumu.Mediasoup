using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tubumu.Utils.FastLambda
{
    /// <summary>
    /// PartialEvaluatorBase
    /// </summary>
    public abstract class PartialEvaluatorBase : ExpressionVisitor
    {
        private readonly IEvaluator _evaluator;
        private HashSet<Expression>? _candidates;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="evaluator"></param>
        protected PartialEvaluatorBase(IEvaluator evaluator)
        {
            _evaluator = evaluator;
        }

        /// <summary>
        /// Eval
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Expression? Eval(Expression exp)
        {
            _candidates = new Nominator().Nominate(exp);
            return _candidates.Count > 0 ? this.Visit(exp) : exp;
        }

        /// <summary>
        /// Visit
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected override Expression Visit(Expression exp)
        {
            if (_candidates == null)
            {
                throw new NullReferenceException(nameof(_candidates));
            }

            if (_candidates.Contains(exp))
            {
                return exp.NodeType == ExpressionType.Constant ? exp :
                    Expression.Constant(_evaluator.Eval(exp), exp.Type);
            }

            return base.Visit(exp);
        }

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        private class Nominator : ExpressionVisitor
        {
            private readonly Func<Expression, bool> _fnCanBeEvaluated;
            private HashSet<Expression>? _candidates;
            private bool _cannotBeEvaluated;

            public Nominator()
                : this(CanBeEvaluatedLocally)
            { }

            public Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                _fnCanBeEvaluated = fnCanBeEvaluated ?? CanBeEvaluatedLocally;
            }

            private static bool CanBeEvaluatedLocally(Expression exp)
            {
                return exp.NodeType != ExpressionType.Parameter;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                _candidates = new HashSet<Expression>();
                this.Visit(expression);
                return _candidates;
            }

            protected override Expression Visit(Expression expression)
            {
                if (_candidates == null)
                {
                    throw new NullReferenceException(nameof(_candidates));
                }

                bool saveCannotBeEvaluated = _cannotBeEvaluated;
                _cannotBeEvaluated = false;

                base.Visit(expression);

                if (!_cannotBeEvaluated)
                {
                    if (_fnCanBeEvaluated(expression))
                    {
                        _candidates.Add(expression);
                    }
                    else
                    {
                        _cannotBeEvaluated = true;
                    }
                }

                _cannotBeEvaluated |= saveCannotBeEvaluated;

                return expression;
            }
        }
    }
}
