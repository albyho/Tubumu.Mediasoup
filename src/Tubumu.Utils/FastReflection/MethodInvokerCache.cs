using System.Reflection;

namespace Tubumu.Utils.FastReflection
{
    /// <summary>
    /// MethodInvokerCache
    /// </summary>
    public class MethodInvokerCache : FastReflectionCache<MethodInfo, IMethodInvoker>
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override IMethodInvoker Create(MethodInfo key)
        {
            return FastReflectionFactories.MethodInvokerFactory.Create(key);
        }
    }
}
