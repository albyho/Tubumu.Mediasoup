using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tubumu.Utils
{
    public class ByteSegmentBuffer
    {
        private readonly List<ArraySegment<byte>> _segments;

        public int Count { get; private set; }

        public ByteSegmentBuffer()
        {
            _segments = new List<ArraySegment<byte>>();
            Count = 0;
        }

        public void Write(ArraySegment<byte> data)
        {
            _segments.Add(data);
            Count += data.Count;
        }

        public byte[] Read(int count, int offset = 0, bool peek = false)
        {
            if(count < 1)
            {
                throw new ArgumentException($"`{nameof(count)}` must be greater than or equal to 1");
            }

            if(offset < 0)
            {
                throw new ArgumentException($"`{nameof(offset)}` cannot be negative");
            }

            if(count + offset > Count)
            {
                throw new InvalidOperationException("Not enough data available in buffer to read");
            }

            var segmentsToRemove = new List<ArraySegment<byte>>();

            // Skip segments based on offset
            var peekBytes = 0;
            var readSegmentIndex = 0;
            var readSegmentOffset = 0;

            for(var segmentIndex = 0; segmentIndex < _segments.Count; segmentIndex++)
            {
                var segment = _segments[segmentIndex];
                if(segmentIndex == 0)
                {
                    // 如果第一段已经有足够的数据来适应 offset。
                    if(segment.Count > offset)
                    {
                        readSegmentIndex = segmentIndex;
                        readSegmentOffset = offset;
                        break;
                    }
                    else
                    {
                        peekBytes += segment.Count;
                        if(!peek)
                        {
                            segmentsToRemove.Add(segment);
                        }
                    }
                }
                else
                {
                    // 如果非第一段已经有足够的数据来适应 offset。
                    if(peekBytes + segment.Count > offset)
                    {
                        readSegmentIndex = segmentIndex;
                        readSegmentOffset = peekBytes + segment.Count - offset;
                        break;
                    }
                    else
                    {
                        peekBytes += segment.Count;
                        if(!peek)
                        {
                            segmentsToRemove.Add(segment);
                        }
                    }
                }
            }

            // Read data from segments
            var data = new byte[count];
            int dataWriteOffset = 0;
            int bytesToRead = count;

            do
            {
                var segment = _segments[readSegmentIndex];
                // 如果读取第一段，则 readSegmentOffset 可能导致偏移。
                int segmentBytesToRead = Math.Min(segment.Count - readSegmentOffset, bytesToRead);

                Array.Copy(segment.Array!, segment.Offset + readSegmentOffset, data, dataWriteOffset, segmentBytesToRead);

                dataWriteOffset += segmentBytesToRead;
                bytesToRead -= segmentBytesToRead;

                if(!peek)
                {
                    // 如果当前 segment 还有数据，说明数据已经足额读取。新建一个 ArraySegment<byte> 对象。
                    if(segment.Count - readSegmentOffset > segmentBytesToRead)
                    {
                        // For testing
                        Debug.Assert(bytesToRead == 0);
                        _segments[readSegmentIndex] = new ArraySegment<byte>(segment.Array!, segment.Offset + segmentBytesToRead, segment.Count - segmentBytesToRead);
                    }
                    else
                    {
                        segmentsToRemove.Add(segment);
                    }
                }

                readSegmentIndex++;
                // 只有第一段可能导致偏移。
                readSegmentOffset = 0;
            } while(bytesToRead > 0);

            if(!peek)
            {
                _segments.RemoveAll(m => segmentsToRemove.Contains(m));
                Count -= count;
            }

            return data;
        }
    }
}
