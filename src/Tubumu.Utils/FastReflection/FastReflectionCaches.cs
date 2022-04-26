using System.Reflection;

namespace Tubumu.Utils.FastReflection
{
    /// <summary>
    /// FastReflectionCaches
    /// </summary>
    public static class FastReflectionCaches
    {
        /// <summary>
        /// Static constructor
        /// </summary>
        static FastReflectionCaches()
        {
            MethodInvokerCache = new MethodInvokerCache();
            PropertyAccessorCache = new PropertyAccessorCache();
            FieldAccessorCache = new FieldAccessorCache();
            ConstructorInvokerCache = new ConstructorInvokerCache();
        }

        /// <summary>
        /// MethodInvokerCache
        /// </summary>
        public static IFastReflectionCache<MethodInfo, IMethodInvoker> MethodInvokerCache { get; set; }

        /// <summary>
        /// PropertyAccessorCache
        /// </summary>
        public static IFastReflectionCache<PropertyInfo, IPropertyAccessor> PropertyAccessorCache { get; set; }

        /// <summary>
        /// FieldAccessorCache
        /// </summary>
        public static IFastReflectionCache<FieldInfo, IFieldAccessor> FieldAccessorCache { get; set; }

        /// <summary>
        /// ConstructorInvokerCache
        /// </summary>
        public static IFastReflectionCache<ConstructorInfo, IConstructorInvoker> ConstructorInvokerCache { get; set; }
    }
}
