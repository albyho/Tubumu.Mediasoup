using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tubumu.Utils.Utilities
{
    /// <summary>
    /// StructHelper
    /// </summary>
    public class StructHelper
    {
        /// <summary>
        /// 结构体转 Byte 数组
        /// </summary>
        /// <param name="structObj">要转换的结构体</param>
        /// <returns>转换后的 Byte 数组</returns>
        public static byte[] StructToBytes<T>(T structObj) where T : struct
        {
            // 得到结构体的大小
            var size = Marshal.SizeOf(structObj);
            // 创建 byte 数组
            var bytes = new byte[size];
            // 分配结构体大小的内存空间
            var structPtr = Marshal.AllocHGlobal(size);
            // 将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            // 从内存空间拷到 byte 数组
            Marshal.Copy(structPtr, bytes, 0, size);
            // 释放内存空间
            Marshal.FreeHGlobal(structPtr);
            // 返回 byte 数组
            return bytes;
        }

        /// <summary>
        /// Byte 数组转结构体
        /// </summary>
        /// <param name="bytes">Byte 数组</param>
        /// <returns>转换后的结构体</returns>
        public static T BytesToStuct<T>(byte[] bytes) where T : struct
        {
            var type = typeof(T);
            // 得到结构体的大小
            var size = Marshal.SizeOf(type);
            // byte 数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                throw new ArgumentException("bytes 的长度不足", nameof(bytes));
                // 返回空
                //return default(T);
            }
            // 分配结构体大小的内存空间
            var structPtr = Marshal.AllocHGlobal(size);
            // 将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            // 将内存空间转换为目标结构体
            var obj = Marshal.PtrToStructure(structPtr, type)!;
            // 释放内存空间
            Marshal.FreeHGlobal(structPtr);
            // 返回结构体
            return (T)obj;
        }

        /// <summary>
        /// Byte 数组转结构体
        /// </summary>
        /// <param name="bytes">Byte 数组</param>
        /// <param name="offset"></param>
        /// <returns>转换后的结构体</returns>
        public static T BytesToStuct<T>(byte[] bytes, int offset) where T : struct
        {
            var type = typeof(T);
            // 得到结构体的大小
            var size = Marshal.SizeOf(type);
            // byte 数组长度小于结构体的大小
            if (size > bytes.Length - offset)
            {
                throw new ArgumentException("bytes 的长度不足", nameof(bytes));
                // 返回空
                //return default(T);
            }
            // 分配结构体大小的内存空间
            var structPtr = Marshal.AllocHGlobal(size);
            // 将 byte 数组拷到分配好的内存空间
            Marshal.Copy(bytes, offset, structPtr, size);
            // 将内存空间转换为目标结构体
            var obj = Marshal.PtrToStructure(structPtr, type)!;
            // 释放内存空间
            Marshal.FreeHGlobal(structPtr);
            // 返回结构体
            return (T)obj;
        }

        /// <summary>
        /// 字节序列转16进制
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string ByteArray2String(IEnumerable<byte> buffer)
        {
            var sb = new StringBuilder();
            foreach (byte item in buffer)
            {
                sb.AppendFormat("{0:X2}", item);
            }
            return sb.ToString();
        }
    }
}
