using System;

namespace Tubumu.Libuv
{
    public interface IListener<TStream>
    {
        void Listen();

        event Action? Connection;

        TStream? Accept();
    }
}
