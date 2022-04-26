using System.Reflection;

namespace Tubumu.Utils.FastReflection
{
    /// <summary>
    /// FieldAccessorCache
    /// </summary>
    public class FieldAccessorCache : FastReflectionCache<FieldInfo, IFieldAccessor>
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override IFieldAccessor Create(FieldInfo key)
        {
            return FastReflectionFactories.FieldAccessorFactory.Create(key);
        }
    }
}
