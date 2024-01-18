using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace Tubumu.Mediasoup
{
    public class DataProducer : EventEmitter
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<DataProducer> _logger;

        /// <summary>
        /// Whether the DataProducer is closed.
        /// </summary>
        private bool _closed;
        private readonly AsyncReaderWriterLock _closeLock = new();

        /// <summary>
        /// Internal data.
        /// </summary>
        private readonly DataProducerInternal _internal;

        /// <summary>
        /// DataProducer id.
        /// </summary>
        public string DataProducerId => _internal.DataProducerId;

        /// <summary>
        /// DataProducer data.
        /// </summary>
        public DataProducerData Data { get; }

        /// <summary>
        /// Channel instance.
        /// </summary>
        private readonly IChannel _channel;

        /// <summary>
        /// App custom data.
        /// </summary>
        public Dictionary<string, object> AppData { get; }

        /// <summary>
        /// Observer instance.
        /// </summary>
        public EventEmitter Observer { get; } = new EventEmitter();

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits transportclose</para>
        /// <para>@emits @close</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="@internal"></param>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        /// <param name="payloadChannel"></param>
        /// <param name="appData"></param>
        public DataProducer(
            ILoggerFactory loggerFactory,
            DataProducerInternal @internal,
            DataProducerData data,
            IChannel channel,
            Dictionary<string, object>? appData
        )
        {
            _logger = loggerFactory.CreateLogger<DataProducer>();

            _internal = @internal;
            Data = data;
            _channel = channel;
            AppData = appData ?? new Dictionary<string, object>();

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the DataProducer.
        /// </summary>
        public async Task CloseAsync()
        {
            _logger.LogDebug($"CloseAsync() | DataProducer:{DataProducerId}");

            using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                //_channel.OnNotification -= OnNotificationHandle;

                var reqData = new { DataProducerId = _internal.DataProducerId };

                // Fire and forget
                _channel
                    .RequestAsync(MethodId.TRANSPORT_CLOSE_DATA_PRODUCER, _internal.TransportId, reqData)
                    .ContinueWithOnFaultedHandleLog(_logger);

                Emit("close");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Transport was closed.
        /// </summary>
        public async Task TransportClosedAsync()
        {
            _logger.LogDebug($"TransportClosedAsync() | DataProducer:{DataProducerId}");

            using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                //_channel.OnNotification -= OnNotificationHandle;
                //_payloadChannel.OnNotification -= OnPayloadChannelMessage;

                Emit("transportclose");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Dump DataProducer.
        /// </summary>
        public async Task<string> DumpAsync()
        {
            _logger.LogDebug($"DumpAsync() | DataProducer:{DataProducerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("DataProducer closed");
                }

                return (await _channel.RequestAsync(MethodId.DATA_PRODUCER_DUMP, _internal.DataProducerId))!;
            }
        }

        /// <summary>
        /// Get DataProducer stats. Return: DataProducerStat[]
        /// </summary>
        public async Task<string> GetStatsAsync()
        {
            _logger.LogDebug($"GetStatsAsync() | DataProducer:{DataProducerId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("DataProducer closed");
                }

                return (await _channel.RequestAsync(MethodId.DATA_PRODUCER_GET_STATS, _internal.DataProducerId))!;
            }
        }

        /// <summary>
        /// Send data (just valid for DataProducers created on a DirectTransport).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ppid"></param>
        /// <returns></returns>
        public async Task SendAsync(string message, int? ppid)
        {
            _logger.LogDebug($"SendAsync() | DataProducer:{DataProducerId}");

            /*
             * +-------------------------------+----------+
             * | Value                         | SCTP     |
             * |                               | PPID     |
             * +-------------------------------+----------+
             * | WebRTC String                 | 51       |
             * | WebRTC Binary Partial         | 52       |
             * | (Deprecated)                  |          |
             * | WebRTC Binary                 | 53       |
             * | WebRTC String Partial         | 54       |
             * | (Deprecated)                  |          |
             * | WebRTC String Empty           | 56       |
             * | WebRTC Binary Empty           | 57       |
             * +-------------------------------+----------+
             */

            ppid ??= !message.IsNullOrEmpty() ? 51 : 56;

            // Ensure we honor PPIDs.
            if (ppid == 56)
            {
                message = " ";
            }

            var notifyData = ppid.Value.ToString();

            await SendInternalAsync(notifyData, Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// Send data (just valid for DataProducers created on a DirectTransport).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ppid"></param>
        /// <returns></returns>
        public async Task SendAsync(byte[] message, int? ppid)
        {
            _logger.LogDebug($"SendAsync() | DataProducer:{DataProducerId}");

            ppid ??= !message.IsNullOrEmpty() ? 53 : 57;

            // Ensure we honor PPIDs.
            if (ppid == 57)
            {
                message = new byte[1];
            }

            var notifyData = ppid.Value.ToString();

            await SendInternalAsync(notifyData, message);
        }

        private async Task SendInternalAsync(string notifyData, byte[] message)
        {
            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("DataProducer closed");
                }

                await _payloadChannel.NotifyAsync("dataProducer.send", _internal.DataProducerId, notifyData, message);
            }
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            // No need to subscribe to any event.
        }

        #endregion Event Handlers
    }
}
