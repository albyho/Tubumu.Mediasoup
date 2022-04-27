using System.Linq.Expressions;

namespace Tubumu.Utils.FastLambda
{
    /// <summary>
    /// ExpressionHasher
    /// </summary>
    public class ExpressionHasher : ExpressionVisitor
    {
        /// <summary>
        /// Hash
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int Hash(Expression exp)
        {
            HashCode = 0;
            Visit(exp);
            return HashCode;
        }

        /// <summary>
        /// HashCode
        /// </summary>
        public int HashCode { get; protected set; }

        /// <summary>
        /// Hash
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual ExpressionHasher Hash(int value)
        {
            unchecked { HashCode += value; }
            return this;
        }

        /// <summary>
        /// Hash
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual ExpressionHasher Hash(bool value)
        {
            unchecked { HashCode += value ? 1 : 0; }
            return this;
        }

        private static readonly object NullValue = new();

        /// <summary>
        /// Hash
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual ExpressionHasher Hash(object value)
        {
            value ??= NullValue;
            unchecked { HashCode += value.GetHashCode(); }
            return this;
        }

        /// <summary>
        /// Visit
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected override Expression Visit(Expression exp)
        {
            Hash((int)exp.NodeType).Hash(exp.Type);
            return base.Visit(exp);
        }

        /// <summary>
        /// VisitBinary
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            Hash(b.IsLifted).Hash(b.IsLiftedToNull).Hash(b.Method!);
            return base.VisitBinary(b);
        }

        /// <summary>
        /// VisitBinding
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected override MemberBinding VisitBinding(MemberBinding binding)
        {
            Hash(binding.BindingType).Hash(binding.Member);
            return base.VisitBinding(binding);
        }

        /// <summary>
        /// VisitConstant
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            Hash(c.Value!);
            return base.VisitConstant(c);
        }

        /// <summary>
        /// VisitElementInitializer
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        protected override ElementInit VisitElementInitializer(ElementInit initializer)
        {
            Hash(initializer.AddMethod);
            return base.VisitElementInitializer(initializer);
        }

        /// <summary>
        /// VisitLambda
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        protected override Expression VisitLambda(LambdaExpression lambda)
        {
            foreach (var p in lambda.Parameters)
            {
                VisitParameter(p);
            }

            return base.VisitLambda(lambda);
        }

        /// <summary>
        /// VisitMemberAccess
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            Hash(m.Member);
            return base.VisitMemberAccess(m);
        }

        /// <summary>
        /// VisitMethodCall
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            Hash(m.Method);
            return base.VisitMethodCall(m);
        }

        /// <summary>
        /// VisitNew
        /// </summary>
        /// <param name="nex"></param>
        /// <returns></returns>
        protected override NewExpression VisitNew(NewExpression nex)
        {
            Hash(nex.Constructor!);
            if (nex.Members != null)
            {
                foreach (var m in nex.Members)
                {
                    Hash(m);
                }
            }

            return base.VisitNew(nex);
        }

        /// <summary>
        /// VisitParameter
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            Hash(p.Name!);
            return base.VisitParameter(p);
        }

        /// <summary>
        /// VisitTypeIs
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Hash(b.TypeOperand);
            return base.VisitTypeIs(b);
        }

        /// <summary>
        /// VisitUnary
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression u)
        {
            Hash(u.IsLifted).Hash(u.IsLiftedToNull).Hash(u.Method!);
            return base.VisitUnary(u);
        }
    }
}
