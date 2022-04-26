using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Tubumu.Utils.FastReflection
{
    /// <summary>
    /// IMethodInvoker
    /// </summary>
    public interface IMethodInvoker
    {
        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object? Invoke(object instance, params object[]? parameters);
    }

    /// <summary>
    /// MethodInvoker
    /// </summary>
    public class MethodInvoker : IMethodInvoker
    {
        private readonly Func<object, object[]?, object?> _invoker;

        /// <summary>
        /// MethodInfo
        /// </summary>
        public MethodInfo MethodInfo { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="methodInfo"></param>
        public MethodInvoker(MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            MethodInfo = methodInfo;
            _invoker = CreateInvokeDelegate(methodInfo);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object? Invoke(object instance, params object[]? parameters)
        {
            return _invoker(instance, parameters);
        }

        private static Func<object, object[]?, object?> CreateInvokeDelegate(MethodInfo methodInfo)
        {
            // Target: ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)

            // parameters to execute
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            var parameterExpressions = new List<Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                // (Ti)parameters[i]
                BinaryExpression valueObj = Expression.ArrayIndex(
                    parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(
                    valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = methodInfo.IsStatic ? null :
                Expression.Convert(instanceParameter, methodInfo.ReflectedType!);

            // static invoke or ((TInstance)instance).Method
            var methodCall = Expression.Call(instanceCast, methodInfo, parameterExpressions);

            // ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)
            if (methodCall.Type == typeof(void))
            {
                var lambda = Expression.Lambda<Action<object, object[]?>>(
                        methodCall, instanceParameter, parametersParameter);

                Action<object, object[]?> execute = lambda.Compile();
                return (instance, parameters) =>
                {
                    execute(instance, parameters);
                    return null;
                };
            }
            else
            {
                var castMethodCall = Expression.Convert(methodCall, typeof(object));
                var lambda = Expression.Lambda<Func<object, object[]?, object?>>(
                    castMethodCall, instanceParameter, parametersParameter);

                return lambda.Compile();
            }
        }

        #region IMethodInvoker Members

        object? IMethodInvoker.Invoke(object instance, params object[]? parameters)
        {
            return Invoke(instance, parameters);
        }

        #endregion IMethodInvoker Members
    }
}
