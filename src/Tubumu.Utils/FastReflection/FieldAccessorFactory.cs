using System.Reflection;

namespace Tubumu.Utils.FastReflection
{
    /// <summary>
    /// FieldAccessorFactory
    /// </summary>
    public class FieldAccessorFactory : IFastReflectionFactory<FieldInfo, IFieldAccessor>
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IFieldAccessor Create(FieldInfo key)
        {
            return new FieldAccessor(key);
        }

        #region IFastReflectionFactory<FieldInfo,IFieldAccessor> Members

        IFieldAccessor IFastReflectionFactory<FieldInfo, IFieldAccessor>.Create(FieldInfo key)
        {
            return Create(key);
        }

        #endregion IFastReflectionFactory<FieldInfo,IFieldAccessor> Members
    }
}
