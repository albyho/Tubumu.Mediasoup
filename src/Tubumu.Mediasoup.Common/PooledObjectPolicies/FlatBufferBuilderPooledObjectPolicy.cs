using Google.FlatBuffers;
using Microsoft.Extensions.ObjectPool;

namespace Tubumu.Mediasoup
{
    public class FlatBufferBuilderPooledObjectPolicy : IPooledObjectPolicy<FlatBufferBuilder>
    {
        private readonly int _initialSize;

        public FlatBufferBuilderPooledObjectPolicy(int initialSize)
        {
            _initialSize = initialSize;
        }

        public FlatBufferBuilder Create()
        {
            return new FlatBufferBuilder(_initialSize);
        }

        public bool Return(FlatBufferBuilder obj)
        {
            return true;
        }
    }
}
