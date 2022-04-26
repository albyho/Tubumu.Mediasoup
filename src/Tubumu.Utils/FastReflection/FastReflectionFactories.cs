using System.Reflection;

namespace Tubumu.Utils.FastReflection
{
    /// <summary>
    /// FastReflectionFactories
    /// </summary>
    public static class FastReflectionFactories
    {
        /// <summary>
        /// Static constructor
        /// </summary>
        static FastReflectionFactories()
        {
            MethodInvokerFactory = new MethodInvokerFactory();
            PropertyAccessorFactory = new PropertyAccessorFactory();
            FieldAccessorFactory = new FieldAccessorFactory();
            ConstructorInvokerFactory = new ConstructorInvokerFactory();
        }

        /// <summary>
        /// MethodInvokerFactory
        /// </summary>
        public static IFastReflectionFactory<MethodInfo, IMethodInvoker> MethodInvokerFactory { get; set; }

        /// <summary>
        /// PropertyAccessorFactory
        /// </summary>
        public static IFastReflectionFactory<PropertyInfo, IPropertyAccessor> PropertyAccessorFactory { get; set; }

        /// <summary>
        /// FieldAccessorFactory
        /// </summary>
        public static IFastReflectionFactory<FieldInfo, IFieldAccessor> FieldAccessorFactory { get; set; }

        /// <summary>
        /// ConstructorInvokerFactory
        /// </summary>
        public static IFastReflectionFactory<ConstructorInfo, IConstructorInvoker> ConstructorInvokerFactory { get; set; }
    }
}
