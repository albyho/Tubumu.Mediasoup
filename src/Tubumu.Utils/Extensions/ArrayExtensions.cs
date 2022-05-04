namespace System
{
    /// <summary>
    /// ArrayExtensions
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// 获取子数组
        /// </summary>
        /// <typeparam name="T">泛型类型参数</typeparam>
        /// <param name="sourceArray">源数组</param>
        /// <param name="length">获取的长度</param>
        /// <returns>子数组</returns>
        public static T[] SubArray<T>(this T[] sourceArray, int length)
        {
            return SubArray(sourceArray, length);
        }

        /// <summary>
        /// 获取子数组
        /// </summary>
        /// <typeparam name="T">泛型类型参数</typeparam>
        /// <param name="sourceArray">源数组</param>
        /// <param name="length">获取的长度</param>
        /// <returns>子数组</returns>
        public static T[] SubArray<T>(this T[] sourceArray, long length)
        {
            ValidateParamters(sourceArray, length);
            var result = new T[length];
            Array.Copy(sourceArray, result, length);
            return result;
        }

        #region Private Methods

        private static void ValidateParamters<T>(T[] sourceArray, long length)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException(nameof(sourceArray));
            }

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "length 不能小于或等于零");
            }

            if (sourceArray.Length < length)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "length 不能大于 sourceArray 中的元素数");
            }
        }

        #endregion
    }
}
