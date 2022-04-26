namespace Tubumu.Utils.FastReflection
{
    /// <summary>
    /// IFastReflectionFactory
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IFastReflectionFactory<TKey, TValue>
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue Create(TKey key);
    }
}
