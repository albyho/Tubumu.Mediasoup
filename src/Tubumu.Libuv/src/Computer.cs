using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    internal unsafe struct uv_cpu_times_t
    {
        public ulong user;
        public ulong nice;
        public ulong sys;
        public ulong idle;
        public ulong irq;
    }

    internal unsafe struct uv_cpu_info_t
    {
        public IntPtr model;
        public int speed;
        public uv_cpu_times_t cpu_times;
    }

    public class CpuTimes
    {
        internal CpuTimes(uv_cpu_times_t times)
        {
            User = times.user;
            Nice = times.nice;
            System = times.sys;
            Idle = times.idle;
            IRQ = times.irq;
        }

        public ulong User { get; protected set; }
        public ulong Nice { get; protected set; }
        public ulong System { get; protected set; }
        public ulong Idle { get; protected set; }
        public ulong IRQ { get; protected set; }
    }

    public unsafe class CpuInformation
    {
        internal CpuInformation(uv_cpu_info_t* info)
        {
            Name = Marshal.PtrToStringAnsi(info->model)!;
            Speed = info->speed;
            Times = new CpuTimes(info->cpu_times);
        }

        public string Name { get; protected set; }
        public int Speed { get; protected set; }
        public CpuTimes Times { get; protected set; }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_cpu_info(out IntPtr info, out int count);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void uv_free_cpu_info(IntPtr info, int count);

        internal static CpuInformation[] GetInfo()
        {
            var r = uv_cpu_info(out var info, out var count);
            Ensure.Success(r);

            var ret = new CpuInformation[count];

            for (int i = 0; i < count; i++)
            {
                var cpuinfo = (uv_cpu_info_t*)(info.ToInt64() + (i * sizeof(uv_cpu_info_t)));
                ret[i] = new CpuInformation(cpuinfo);
            }

            uv_free_cpu_info(info, count);

            return ret;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct uv_interface_address_t
    {
        public IntPtr name;
        public fixed byte phys_addr[6];
        public int is_internal;
        public sockaddr_in6 sockaddr;
        public sockaddr_in6 netmask;
    }

    public unsafe class NetworkInterface
    {
        internal NetworkInterface(uv_interface_address_t* iface)
        {
            Name = Marshal.PtrToStringAnsi(iface->name)!;
            Internal = iface->is_internal != 0;
            byte[] phys_addr = new byte[6];
            for (int i = 0; i < phys_addr.Length; i++)
            {
                phys_addr[i] = iface->phys_addr[i];
            }
            PhysicalAddress = new PhysicalAddress(phys_addr);
            Address = UV.GetIPEndPoint(new IntPtr(&iface->sockaddr), false).Address;
            Netmask = UV.GetIPEndPoint(new IntPtr(&iface->netmask), false).Address;
        }

        public string Name { get; protected set; }
        public bool Internal { get; protected set; }
        public PhysicalAddress PhysicalAddress { get; protected set; }
        public IPAddress Address { get; protected set; }
        public IPAddress Netmask { get; protected set; }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_interface_addresses(out IntPtr address, out int count);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void uv_free_interface_addresses(IntPtr address, int count);

        internal static NetworkInterface[] GetInterfaces()
        {
            var r = uv_interface_addresses(out var interfaces, out var count);
            Ensure.Success(r);

            NetworkInterface[] ret = new NetworkInterface[count];

            for (int i = 0; i < count; i++)
            {
                uv_interface_address_t* iface = (uv_interface_address_t*)(
                    interfaces.ToInt64() + (i * sizeof(uv_interface_address_t))
                );
                ret[i] = new NetworkInterface(iface);
            }

            uv_free_interface_addresses(interfaces, count);
            return ret;
        }
    }

    public unsafe class LoadAverage
    {
        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void uv_loadavg(IntPtr avg);

        internal LoadAverage()
        {
            IntPtr ptr = Marshal.AllocHGlobal(sizeof(double) * 3);
            uv_loadavg(ptr);
            Last = *(double*)ptr;
            Five = *(double*)(ptr.ToInt64() + sizeof(double));
            Fifteen = *(double*)(ptr.ToInt64() + (sizeof(double) * 2));
            Marshal.FreeHGlobal(ptr);
        }

        public double Last { get; protected set; }
        public double Five { get; protected set; }
        public double Fifteen { get; protected set; }
    }

    public static class Computer
    {
        public static class Memory
        {
            [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
            internal static extern long uv_get_free_memory();

            public static long Free => uv_get_free_memory();

            [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
            internal static extern long uv_get_total_memory();

            public static long Total => uv_get_total_memory();

            public static long Used => Total - Free;
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong uv_hrtime();

        public static ulong HighResolutionTime => uv_hrtime();

        public static LoadAverage Load => new();

        public static NetworkInterface[] NetworkInterfaces => NetworkInterface.GetInterfaces();

        public static CpuInformation[] CpuInfo => CpuInformation.GetInfo();

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_uptime(out double uptime);

        public static double Uptime
        {
            get
            {
                Ensure.Success(uv_uptime(out var uptime));
                return uptime;
            }
        }
    }
}
