using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Tubumu.Mediasoup
{
    public class ChannelNative : ChannelBase
    {
        private readonly OutgoingMessageBuffer<RequestMessage> _requestMessageQueue = new();

        public ChannelNative(ILogger<Channel> logger, int workerId) : base(logger, workerId)
        {

        }

        protected override void SendRequestMessage(RequestMessage requestMessage, Sent sent)
        {
            _requestMessageQueue.Queue.Enqueue(requestMessage);

            try
            {
                // Notify worker that there is something to read
                if (LibMediasoupWorkerNative.uv_async_send(_requestMessageQueue.Handle) != 0)
                {
                    _logger.LogError("uv_async_send call failed");
                    sent.Reject(new Exception("uv_async_send call failed"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("uv_async_send call failed");
                sent.Reject(ex);
            }
        }

        private RequestMessage? ProduceMessage(IntPtr message, IntPtr messageLen, IntPtr messageCtx, IntPtr handle)
        {
            _requestMessageQueue.Handle = handle;

            if (!_requestMessageQueue.Queue.TryDequeue(out var requestMessage))
            {
                return null;
            }

            var messageJson = requestMessage.ToJson();
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);
            if (messageBytes.Length > PayloadMaxLen)
            {
                throw new Exception("Channel request too big");
            }

            // message
            var messageBytesHandle = GCHandle.Alloc(messageBytes, GCHandleType.Pinned);
            var messagePtr = Marshal.UnsafeAddrOfPinnedArrayElement(messageBytes, 0);
            var temp = messagePtr.IntPtrToBytes();
            Marshal.Copy(temp, 0, message, temp.Length);

            // messageLen
            temp = BitConverter.GetBytes(messageBytes.Length);
            Marshal.Copy(temp, 0, messageLen, temp.Length);

            // messageCtx
            temp = GCHandle.ToIntPtr(messageBytesHandle).IntPtrToBytes();
            Marshal.Copy(temp, 0, messageCtx, temp.Length);
            return requestMessage;
        }

        #region P/Invoke Channel

        internal static readonly LibMediasoupWorkerNative.ChannelReadFreeFn OnChannelReadFree = (message, messageLen, messageCtx) =>
        {
            if (messageLen != 0)
            {
                var messageBytesHandle = GCHandle.FromIntPtr(messageCtx);
                messageBytesHandle.Free();
            }
        };

        internal static readonly LibMediasoupWorkerNative.ChannelReadFn OnChannelRead = (message, messageLen, messageCtx, handle, ctx) =>
        {
            var channel = (ChannelNative)GCHandle.FromIntPtr(ctx).Target!;
            var requestMessage = channel.ProduceMessage(message, messageLen, messageCtx, handle);
            return requestMessage == null ? null : OnChannelReadFree;
        };

        internal static readonly LibMediasoupWorkerNative.ChannelWriteFn OnChannelWrite = (message, messageLen, ctx) =>
        {
            var channel = (IChannel)GCHandle.FromIntPtr(ctx).Target!;
            channel.ProcessMessage(message);
        };

        #endregion
    }
}
