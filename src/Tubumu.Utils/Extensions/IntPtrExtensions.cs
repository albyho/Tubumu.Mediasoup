namespace System
{
    /// <summary>
    /// IntPtr 扩展方法
    /// </summary>
    public static class IntPtrExtensions
    {
        public static byte[] IntPtrToBytes(this IntPtr input)
        {
            return IntPtr.Size == sizeof(int) ? BitConverter.GetBytes((int)input) : BitConverter.GetBytes(input);
        }

        public static IntPtr BytesToIntPtr(this byte[] input)
        {
            return (IntPtr)(IntPtr.Size == sizeof(int) ? BitConverter.ToInt32(input) : BitConverter.ToInt64(input));
        }
    }
}
