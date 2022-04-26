using System.Reflection;

namespace Tubumu.Utils.FastReflection
{
    /// <summary>
    /// FastReflectionExtensions
    /// </summary>
    public static class FastReflectionExtensions
    {
        /// <summary>
        /// FastInvoke
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="instance"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object? FastInvoke(this MethodInfo methodInfo, object instance, params object[] parameters)
        {
            return FastReflectionCaches.MethodInvokerCache.Get(methodInfo)!.Invoke(instance, parameters);
        }

        /// <summary>
        /// FastSetValue
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        public static void FastSetValue(this PropertyInfo propertyInfo, object instance, object value)
        {
            FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo)!.SetValue(instance, value);
        }

        /// <summary>
        /// FastGetValue
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object FastGetValue(this PropertyInfo propertyInfo, object instance)
        {
            return FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo)!.GetValue(instance);
        }

        /// <summary>
        /// FastGetValue
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object FastGetValue(this FieldInfo fieldInfo, object instance)
        {
            return FastReflectionCaches.FieldAccessorCache.Get(fieldInfo)!.GetValue(instance);
        }

        /// <summary>
        /// FastInvoke
        /// </summary>
        /// <param name="constructorInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object FastInvoke(this ConstructorInfo constructorInfo, params object[] parameters)
        {
            return FastReflectionCaches.ConstructorInvokerCache.Get(constructorInfo)!.Invoke(parameters);
        }
    }
}
