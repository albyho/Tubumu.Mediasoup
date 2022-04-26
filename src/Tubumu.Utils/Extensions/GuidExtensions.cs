using System;

namespace Tubumu.Utils.Extensions
{
    /// <summary>
    /// GuidExtensions
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// IsNullOrEmpty
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this Guid? source)
        {
            return source == null || source.Value == Guid.Empty;
        }
    }
}
