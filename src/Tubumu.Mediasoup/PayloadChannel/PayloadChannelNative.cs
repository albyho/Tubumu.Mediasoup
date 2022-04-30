using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tubumu.Utils.Extensions;
using Tubumu.Utils.Extensions.Object;
using Tubumu.Utils.Json;
using ObjectExtensions = Tubumu.Utils.Extensions.Object.ObjectExtensions;

namespace Tubumu.Mediasoup
{
    public class PayloadChannelNative : PayloadChannelBase
    {
        #region Events

        public override event Action<string, string, NotifyData, ArraySegment<byte>>? MessageEvent;

        #endregion Events

        private OutgoingMessageBuffer<RequestMessage> _requestMessageQueue = new();

        public PayloadChannelNative(ILogger<PayloadChannel> logger, int workerId):base(logger, workerId)
        {

        }

        protected override void SendNotification(RequestMessage notification)
        {
            _requestMessageQueue.Queue.Enqueue(notification);

            try
            {
                // Notify worker that there is something to read
                if (LibMediasoupWorkerNative.uv_async_send(_requestMessageQueue.Handle) != 0)
                {
                    _logger.LogError("uv_async_send call failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "uv_async_send call failed");
            }
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
                _logger.LogError(ex, "uv_async_send call failed");
                sent.Reject(ex);
            }
        }

        private RequestMessage? ProduceMessage(IntPtr message, IntPtr messageLen, IntPtr messageCtx,
            IntPtr payload, IntPtr payloadLen, IntPtr payloadCapacity,
            IntPtr handle)
        {
            _requestMessageQueue.Handle = handle;

            if (!_requestMessageQueue.Queue.TryDequeue(out var requestMessage))
            {
                return null;
            }

            var requestMessageJson = _requestMessageQueue.ToJson();
            var requestMessageBytes = Encoding.UTF8.GetBytes(requestMessageJson);

            if (requestMessageBytes.Length > MessageMaxLen)
            {
                throw new Exception("PayloadChannel message too big");
            }
            else if (requestMessage.Payload != null && requestMessage.Payload.Length > PayloadMaxLen)
            {
                throw new Exception("PayloadChannel payload too big");
            }

            // message
            var messageBytesHandle = GCHandle.Alloc(requestMessageBytes, GCHandleType.Pinned);
            var messagePtr = Marshal.UnsafeAddrOfPinnedArrayElement(requestMessageBytes, 0);
            var temp = messagePtr.IntPtrToBytes();
            Marshal.Copy(temp, 0, message, temp.Length);

            // messageLen
            temp = BitConverter.GetBytes(requestMessageBytes.Length);
            Marshal.Copy(temp, 0, messageLen, temp.Length);

            // messageCtx
            temp = GCHandle.ToIntPtr(messageBytesHandle).IntPtrToBytes();
            Marshal.Copy(temp, 0, messageCtx, temp.Length);

            // payload
            if (requestMessage.Payload != null)
            {
                // payload
                var payloadBytesHandle = GCHandle.Alloc(requestMessage.Payload, GCHandleType.Pinned);
                var payloadBytesPtr = Marshal.UnsafeAddrOfPinnedArrayElement(requestMessage.Payload, 0);
                temp = payloadBytesPtr.IntPtrToBytes();
                Marshal.Copy(temp, 0, payload, temp.Length);

                // payloadLen
                temp = BitConverter.GetBytes(requestMessage.Payload.Length);
                Marshal.Copy(temp, 0, payloadLen, temp.Length);

                // payloadCapacity
                temp = GCHandle.ToIntPtr(payloadBytesHandle).IntPtrToBytes();
                Marshal.Copy(temp, 0, payloadCapacity, temp.Length);
            }
            else
            {
                // payload
                temp = IntPtr.Zero.IntPtrToBytes();
                Marshal.Copy(temp, 0, payload, temp.Length);

                // payloadLen
                temp = BitConverter.GetBytes(0);
                Marshal.Copy(temp, 0, payloadLen, temp.Length);

                // payloadCapacity
                temp = IntPtr.Zero.IntPtrToBytes();
                Marshal.Copy(temp, 0, payloadCapacity, temp.Length);
            }

            return requestMessage;
        }

        public override void Process(string message, byte[] payload)
        {
            var jsonDocument = JsonDocument.Parse(message);
            var msg = jsonDocument.RootElement;
            var id = msg.GetNullableJsonElement("id")?.GetNullableUInt32();
            var accepted = msg.GetNullableJsonElement("accepted")?.GetNullableBool();
            // targetId 可能是 Number 或 String。不能使用 GetString()，否则可能报错：Cannot get the value of a token type 'Number' as a string"
            var targetId = msg.GetNullableJsonElement("targetId")?.ToString();
            var @event = msg.GetNullableJsonElement("event")?.GetString();
            var error = msg.GetNullableJsonElement("error")?.GetString();
            var reason = msg.GetNullableJsonElement("reason")?.GetString();
            var data = msg.GetNullableJsonElement("data")?.GetString();

            // If a response, retrieve its associated request.
            if (id.HasValue && id.Value >= 0)
            {
                if (!_sents.TryGetValue(id.Value, out var sent))
                {
                    _logger.LogError($"ProcessData() | Worker[{_workerId}] Received response does not match any sent request [id:{id}]");

                    return;
                }

                if (accepted.HasValue && accepted.Value)
                {
                    _logger.LogDebug($"ProcessData() | Worker[{_workerId}] Request succeed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]");

                    sent.Resolve?.Invoke(data);
                }
                else if (!error.IsNullOrWhiteSpace())
                {
                    // 在 Node.js 实现中，error 的值可能是 "Error" 或 "TypeError"。
                    _logger.LogWarning($"ProcessData() | Worker[{_workerId}] Request failed [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]: {reason}");

                    sent.Reject?.Invoke(new Exception(reason));
                }
                else
                {
                    _logger.LogError($"ProcessData() | Worker[{_workerId}] Received response is not accepted nor rejected [method:{sent.RequestMessage.Method}, id:{sent.RequestMessage.Id}]");
                }
            }
            // If a notification emit it to the corresponding entity.
            else if (!targetId.IsNullOrWhiteSpace() && !@event.IsNullOrWhiteSpace())
            {
                var notifyData = JsonSerializer.Deserialize<NotifyData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;
                _ongoingNotification = new OngoingNotification
                {
                    TargetId = targetId!,
                    Event = @event!,
                    Data = notifyData,
                };
            }
            else
            {
                _logger.LogError($"ProcessData() | Worker[{_workerId}] Received data is not a notification nor a response");
                return;
            }

            if (_ongoingNotification != null)
            {
                // Emit the corresponding event.
                MessageEvent?.Invoke(_ongoingNotification.TargetId, _ongoingNotification.Event, _ongoingNotification.Data, new ArraySegment<byte>(payload));

                // Unset ongoing notification.
                _ongoingNotification = null;
            }
        }

        #region P/Invoke PayloadChannel

        internal static readonly LibMediasoupWorkerNative.PayloadChannelReadFreeFn OnPayloadChannelReadFree = (message, messageLen, messageCtx) =>
        {
            if (messageLen != 0)
            {
                var messageBytesHandle = GCHandle.FromIntPtr(messageCtx);
                messageBytesHandle.Free();
            }
        };

        internal static readonly LibMediasoupWorkerNative.PayloadChannelReadFn OnPayloadChannelRead = (message, messageLen, messageCtx,
            payload, payloadLen, payloadCapacity,
            handle, ctx) =>
        {
            var payloadChannel = (PayloadChannelNative)GCHandle.FromIntPtr(ctx).Target!;
            var requestMessage = payloadChannel.ProduceMessage(message, messageLen, messageCtx,
                payload, payloadLen, payloadCapacity,
                handle);
            if (requestMessage == null)
            {
                return null;
            }
            return OnPayloadChannelReadFree;
        };

        internal static readonly LibMediasoupWorkerNative.PayloadChannelWriteFn OnPayloadchannelWrite = (message, messageLen, payload, payloadLen, ctx) =>
        {
            var handle = GCHandle.FromIntPtr(ctx);
            var payloadChannel = (IPayloadChannel)handle.Target!;

            var payloadBytes = new byte[payloadLen];
            Marshal.Copy(payload, payloadBytes, 0, (int)payloadLen);
            payloadChannel.Process(message, payloadBytes);
        };

        #endregion
    }
}
