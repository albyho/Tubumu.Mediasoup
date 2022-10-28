using System.Runtime.CompilerServices;

namespace System.Threading
{
    /// <summary>
    /// Interlocked 扩展方法
    /// </summary>
    public class InterlockedExtensions
    {
        /// <summary>
        /// unsigned equivalent of <see cref="Interlocked.Increment(ref int)"/>
        /// </summary>
        public static uint Increment(ref uint location)
        {
            int incrementedSigned = Interlocked.Increment(ref Unsafe.As<uint, int>(ref location));
            return Unsafe.As<int, uint>(ref incrementedSigned);
        }

        /// <summary>
        /// unsigned equivalent of <see cref="Interlocked.Increment(ref long)"/>
        /// </summary>
        public static ulong Increment(ref ulong location)
        {
            var incrementedSigned = Interlocked.Increment(ref Unsafe.As<ulong, long>(ref location));
            return Unsafe.As<long, ulong>(ref incrementedSigned);
        }
    }
}
