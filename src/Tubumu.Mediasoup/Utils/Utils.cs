using System;

namespace Tubumu.Mediasoup
{
    public static class Utils
    {
        private static readonly Random _random = new();

        /// <summary>
        /// 生成 100000000 - 999999999 的随机数
        /// </summary>
        /// <returns></returns>
        public static uint GenerateRandomNumber()
        {
            return (uint)_random.Next(100_000_000, 1_000_000_000);
        }
    }
}
