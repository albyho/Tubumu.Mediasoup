using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Tubumu.Utils.FastReflection
{
    /// <summary>
    /// IFieldAccessor
    /// </summary>
    public interface IFieldAccessor
    {
        /// <summary>
        /// GetValue
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        object GetValue(object instance);
    }

    /// <summary>
    /// FieldAccessor
    /// </summary>
    public class FieldAccessor : IFieldAccessor
    {
        private Func<object, object> _getter;

        /// <summary>
        /// FieldInfo
        /// </summary>
        public FieldInfo FieldInfo { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldInfo"></param>
        public FieldAccessor(FieldInfo fieldInfo)
        {
            FieldInfo = fieldInfo;
            _getter = InitializeGet(fieldInfo);
        }

        private Func<object, object> InitializeGet(FieldInfo fieldInfo)
        {
            // target: (object)(((TInstance)instance).Field)

            // preparing parameter, object type
            var instance = Expression.Parameter(typeof(object), "instance");

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = fieldInfo.IsStatic ? null :
                Expression.Convert(instance, fieldInfo.ReflectedType!);

            // ((TInstance)instance).Property
            var fieldAccess = Expression.Field(instanceCast, fieldInfo);

            // (object)(((TInstance)instance).Property)
            var castFieldValue = Expression.Convert(fieldAccess, typeof(object));

            // Lambda expression
            var lambda = Expression.Lambda<Func<object, object>>(castFieldValue, instance);

            return lambda.Compile();
        }

        /// <summary>
        /// GetValue
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public object GetValue(object instance)
        {
            return _getter(instance);
        }

        #region IFieldAccessor Members

        object IFieldAccessor.GetValue(object instance)
        {
            return GetValue(instance);
        }

        #endregion IFieldAccessor Members
    }
}
