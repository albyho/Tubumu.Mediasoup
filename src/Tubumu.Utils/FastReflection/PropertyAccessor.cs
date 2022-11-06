using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Tubumu.Utils.FastReflection
{
    /// <summary>
    /// IPropertyAccessor
    /// </summary>
    public interface IPropertyAccessor
    {
        /// <summary>
        /// GetValue
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        object GetValue(object instance);

        /// <summary>
        /// SetValue
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        void SetValue(object instance, object value);
    }

    /// <summary>
    /// PropertyAccessor
    /// </summary>
    public class PropertyAccessor : IPropertyAccessor
    {
        private Func<object, object>? _getter;
        private MethodInvoker? _setMethodInvoker;

        /// <summary>
        /// PropertyInfo
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="propertyInfo"></param>
        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            InitializeGet(propertyInfo);
            InitializeSet(propertyInfo);
        }

        private void InitializeGet(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead)
            {
                return;
            }

            // Target: (object)(((TInstance)instance).Property)

            // preparing parameter, object type
            var instance = Expression.Parameter(typeof(object), "instance");

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = propertyInfo.GetGetMethod(true)!.IsStatic ? null :
                Expression.Convert(instance, propertyInfo.ReflectedType!);

            // ((TInstance)instance).Property
            var propertyAccess = Expression.Property(instanceCast, propertyInfo);

            // (object)(((TInstance)instance).Property)
            var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));

            // Lambda expression
            var lambda = Expression.Lambda<Func<object, object>>(castPropertyValue, instance);

            _getter = lambda.Compile();
        }

        private void InitializeSet(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite)
            {
                return;
            }

            _setMethodInvoker = new MethodInvoker(propertyInfo.GetSetMethod(true)!);
        }

        /// <summary>
        /// GetValue
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public object GetValue(object o)
        {
            return _getter == null ? throw new NotSupportedException("Get method is not defined for this property.") : _getter!(o);
        }

        /// <summary>
        /// SetValue
        /// </summary>
        /// <param name="o"></param>
        /// <param name="value"></param>
        public void SetValue(object o, object value)
        {
            if (_setMethodInvoker == null)
            {
                throw new NotSupportedException("Set method is not defined for this property.");
            }

            _setMethodInvoker!.Invoke(o, new object[] { value });
        }

        #region IPropertyAccessor Members

        object IPropertyAccessor.GetValue(object instance)
        {
            return GetValue(instance);
        }

        void IPropertyAccessor.SetValue(object instance, object value)
        {
            SetValue(instance, value);
        }

        #endregion IPropertyAccessor Members
    }
}
