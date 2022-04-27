using System;
using System.Collections.Generic;

namespace Tubumu.Utils.Utilities
{
    public class QueueBuffer
    {
        private Queue<ArraySegment<byte>> Segments { get; set; }

        public int Length { get; private set; }

        public void Enqueue(ArraySegment<byte> data)
        {
            Segments.Enqueue(data);
            Length += data.Count;
        }

        public ArraySegment<byte> Dequeue()
        {
            if (Segments.TryDequeue(out var item))
            {
                Length -= item.Count;
            }
            return item;
        }

        public ArraySegment<byte> Peek()
        {
            var item = Segments.Peek();
            return item;
        }

        public QueueBuffer()
        {
            Segments = new Queue<ArraySegment<byte>>();
            Length = 0;
        }
    }
}
