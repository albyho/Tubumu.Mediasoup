using System;

namespace Tubumu.Libuv
{
    public interface IFileDescriptor
    {
        void Open(IntPtr socket);

        IntPtr FileDescriptor { get; }
    }
}
