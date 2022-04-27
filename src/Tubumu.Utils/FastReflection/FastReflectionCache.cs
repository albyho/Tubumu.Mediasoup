using System;
using System.Collections.Generic;

namespace Tubumu.Utils.FastReflection
{
    /// <summary>
    /// FastReflectionCache
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class FastReflectionCache<TKey, TValue> : IFastReflectionCache<TKey, TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> _cache = new();

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue? Get(TKey? key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            TValue? value;
            lock (key)
            {
                if (!_cache.TryGetValue(key, out value))
                {
                    value = Create(key);
                    _cache[key] = value;
                }
            }

            return value;
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected abstract TValue Create(TKey key);
    }
}
