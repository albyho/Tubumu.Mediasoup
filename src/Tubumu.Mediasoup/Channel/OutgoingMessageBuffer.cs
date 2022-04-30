using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
	public class OutgoingMessageBuffer<T>
	{
		public ConcurrentQueue<T> Queue { get; } = new();

		public IntPtr Handle { get; set; }
    }
}
