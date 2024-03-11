namespace System
{
    /// <summary>
    /// IntPtr 扩展方法
    /// </summary>
    [Literal($"{nameof(Tubumu)}.{nameof(Tubumu.Templates)}.{nameof(IntPtrExtensions)}")]
    internal static class IntPtrExtensions
    {
        public static byte[] IntPtrToBytes(this IntPtr input)
        {
            return IntPtr.Size == sizeof(int)
                ? BitConverter.GetBytes((int)input)
                : BitConverter.GetBytes((long)input);
        }

        public static IntPtr BytesToIntPtr(this byte[] input)
        {
            return (IntPtr)(IntPtr.Size == sizeof(int)
                ? BitConverter.ToInt32(input, 0)
                : BitConverter.ToInt64(input, 0));
        }
    }
}
