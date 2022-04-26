using System.Reflection;

namespace Tubumu.Utils.FastReflection
{
    /// <summary>
    /// ConstructorInvokerFactory
    /// </summary>
    public class ConstructorInvokerFactory : IFastReflectionFactory<ConstructorInfo, IConstructorInvoker>
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IConstructorInvoker Create(ConstructorInfo key)
        {
            return new ConstructorInvoker(key);
        }

        #region IFastReflectionFactory<ConstructorInfo,IConstructorInvoker> Members

        IConstructorInvoker IFastReflectionFactory<ConstructorInfo, IConstructorInvoker>.Create(ConstructorInfo key)
        {
            return this.Create(key);
        }

        #endregion IFastReflectionFactory<ConstructorInfo,IConstructorInvoker> Members
    }
}
