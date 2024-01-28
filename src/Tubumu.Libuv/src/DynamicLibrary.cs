using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    public abstract class DynamicLibrary
    {
        public static string Decorate(string name)
        {
            return UV.isUnix ? $"lib{name}.so" : $"{name}.dll";
        }

        public static DynamicLibrary Open(string name)
        {
            return UV.isUnix ? new LibuvDynamicLibrary(name) : new WindowsDynamicLibrary(name);
        }

        public static DynamicLibrary Open()
        {
            return UV.isUnix ? new LibuvDynamicLibrary() : new WindowsDynamicLibrary();
        }

        public abstract bool Closed { get; }

        public abstract void Close();

        public abstract bool TryGetSymbol(string name, out IntPtr pointer);

        public abstract IntPtr GetSymbol(string name);
    }

    internal class LibuvDynamicLibrary : DynamicLibrary
    {
        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_dlopen(IntPtr name, IntPtr handle);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_dlopen(string name, IntPtr handle);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void uv_dlclose(IntPtr handle);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_dlsym(IntPtr handle, string name, out IntPtr ptr);

        [DllImport(NativeMethods.Libuv)]
        internal static extern IntPtr uv_dlerror(IntPtr handle);

        [DllImport(NativeMethods.Libuv)]
        internal static extern IntPtr uv_dlerror_free(IntPtr handle);

        private IntPtr handle = IntPtr.Zero;

        public override bool Closed => handle == IntPtr.Zero;

        private void Check(int ret)
        {
            if(ret < 0)
            {
                throw new Exception(Marshal.PtrToStringAnsi(uv_dlerror(handle)));
            }
        }

        public LibuvDynamicLibrary()
        {
            handle = Marshal.AllocHGlobal(28);
            Check(uv_dlopen(IntPtr.Zero, handle));
        }

        public LibuvDynamicLibrary(string library)
        {
            Ensure.ArgumentNotNull(library, "library");

            handle = Marshal.AllocHGlobal(28);
            Check(uv_dlopen(library, handle));
        }

        public override void Close()
        {
            if(!Closed)
            {
                uv_dlclose(handle);
                handle = IntPtr.Zero;
            }
        }

        public override bool TryGetSymbol(string name, out IntPtr pointer)
        {
            return uv_dlsym(handle, name, out pointer) == 0;
        }

        public override IntPtr GetSymbol(string name)
        {
            return uv_dlsym(handle, name, out var ptr) < 0 ? throw new Exception(Marshal.PtrToStringAnsi(uv_dlerror(handle))) : ptr;
        }
    }

    internal class WindowsDynamicLibrary : DynamicLibrary
    {
        private IntPtr handle = IntPtr.Zero;

        public void Check(IntPtr ptr)
        {
            if(ptr == IntPtr.Zero)
            {
                throw new Exception();
            }

            handle = ptr;
        }

        public WindowsDynamicLibrary()
        {
            Check(LoadLibraryEx(IntPtr.Zero, IntPtr.Zero, LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH));
        }

        public WindowsDynamicLibrary(string name)
        {
            Ensure.ArgumentNotNull(name, "name");

            Check(LoadLibraryEx(name, IntPtr.Zero, LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH));
        }

        public override bool Closed => handle == IntPtr.Zero;

        public override void Close()
        {
            if(!Closed)
            {
                FreeLibrary(handle);
                handle = IntPtr.Zero;
            }
        }

        public override IntPtr GetSymbol(string name)
        {
            var ptr = GetProcAddress(handle, name);
            return ptr != IntPtr.Zero ? ptr : throw new Exception();
        }

        public override bool TryGetSymbol(string name, out IntPtr pointer)
        {
            pointer = GetProcAddress(handle, name);
            return pointer != IntPtr.Zero;
        }

        [Flags]
        public enum LoadLibraryFlags : uint
        {
            DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008,
        }

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryExW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(
            [MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
            IntPtr hFile,
            [MarshalAs(UnmanagedType.U4)] LoadLibraryFlags dwFlags);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryExW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(
            IntPtr lpFileName,
            IntPtr hFile,
            [MarshalAs(UnmanagedType.U4)] LoadLibraryFlags dwFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
    }
}
