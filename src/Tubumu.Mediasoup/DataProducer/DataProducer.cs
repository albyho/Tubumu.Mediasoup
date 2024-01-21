using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FBS.DataProducer;
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
        /// Close flag.
        /// </summary>
        private bool _closed;

        private readonly AsyncReaderWriterLock _closeLock = new();

        /// <summary>
        /// Paused flag.
        /// </summary>
        private bool _paused;

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
        /// <param name="paused"></param>
        /// <param name="appData"></param>
        public DataProducer(
            ILoggerFactory loggerFactory,
            DataProducerInternal @internal,
            DataProducerData data,
            IChannel channel,
            bool paused,
            Dictionary<string, object>? appData
        )
        {
            _logger = loggerFactory.CreateLogger<DataProducer>();

            _internal = @internal;
            Data = data;
            _channel = channel;
            _paused = paused;
            AppData = appData ?? new Dictionary<string, object>();

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the DataProducer.
        /// </summary>
        public async Task CloseAsync()
        {
            _logger.LogDebug("CloseAsync() | DataProducer:{DataProducerId}", DataProducerId);

            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                //_channel.OnNotification -= OnNotificationHandle;

                var closeDataProducerRequest = new FBS.Transport.CloseDataProducerRequestT
                {
                    DataProducerId = _internal.DataProducerId,
                };

                var closeDataProducerRequestOffset = FBS.Transport.CloseDataProducerRequest.Pack(_channel.BufferBuilder, closeDataProducerRequest);

                // Fire and forget
                _channel.RequestAsync(
                    FBS.Request.Method.TRANSPORT_CLOSE_DATAPRODUCER,
                    FBS.Request.Body.Transport_CloseDataProducerRequest,
                    closeDataProducerRequestOffset.Value,
                    _internal.TransportId
                    ).ContinueWithOnFaultedHandleLog(_logger);

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
            _logger.LogDebug("TransportClosedAsync() | DataProducer:{DataProducerId}", DataProducerId);

            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                //_channel.OnNotification -= OnNotificationHandle;

                Emit("transportclose");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Dump DataProducer.
        /// </summary>
        public async Task<DumpResponseT> DumpAsync()
        {
            _logger.LogDebug("DumpAsync() | DataProducer:{DataProducerId}", DataProducerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataProducer closed");
                }

                var response = await _channel.RequestAsync(
                    FBS.Request.Method.DATAPRODUCER_DUMP,
                    null,
                    null,
                    _internal.DataProducerId);

                /* Decode Response. */
                var data = response.Value.BodyAsDataProducer_DumpResponse().UnPack();
                return data;
            }
        }

        /// <summary>
        /// Get DataProducer stats. Return: DataProducerStat[]
        /// </summary>
        public async Task<GetStatsResponseT[]> GetStatsAsync()
        {
            _logger.LogDebug("GetStatsAsync() | DataProducer:{DataProducerId}", DataProducerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataProducer closed");
                }

                var response = await _channel.RequestAsync(
                    FBS.Request.Method.DATAPRODUCER_GET_STATS,
                    null,
                    null,
                    _internal.DataProducerId);

                /* Decode Response. */
                var data = response.Value.BodyAsDataProducer_GetStatsResponse().UnPack();
                return new[] { data };
            }
        }

        /// <summary>
        /// Pause the DataProducer.
        /// </summary>
        public async Task PauseAsync()
        {
            _logger.LogDebug("PauseAsync() | DataProducer:{DataProducerId}", DataProducerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataProducer closed");
                }

                /* Ignore Response. */
                _ = await _channel.RequestAsync(
                     FBS.Request.Method.DATACONSUMER_PAUSE,
                     null,
                     null,
                     _internal.DataProducerId);

                var wasPaused = _paused;

                _paused = true;

                // Emit observer event.
                if(!wasPaused)
                {
                    Observer.Emit("pause");
                }
            }
        }

        /// <summary>
        /// Resume the DataProducer.
        /// </summary>
        public async Task ResumeAsync()
        {
            _logger.LogDebug("ResumeAsync() | DataProducer:{DataProducerId}", DataProducerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                /* Ignore Response. */
                _ = await _channel.RequestAsync(
                     FBS.Request.Method.DATACONSUMER_RESUME,
                     null,
                     null,
                     _internal.DataProducerId);

                var wasPaused = _paused;

                _paused = false;

                // Emit observer event.
                if(wasPaused)
                {
                    Observer.Emit("resume");
                }
            }
        }

        /// <summary>
        /// Send data (just valid for DataProducers created on a DirectTransport).
        /// </summary>
        public async Task SendAsync(string message, uint? ppid, List<ushort>? subchannels, ushort? requiredSubchannel)
        {
            _logger.LogDebug("SendAsync() | DataProducer:{DataProducerId}", DataProducerId);

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

            ppid ??= !message.IsNullOrEmpty() ? 51u : 56u;

            // Ensure we honor PPIDs.
            if(ppid == 56)
            {
                message = " ";
            }

            await SendInternalAsync(Encoding.UTF8.GetBytes(message), ppid.Value, subchannels, requiredSubchannel);
        }

        /// <summary>
        /// Send data (just valid for DataProducers created on a DirectTransport).
        /// </summary>
        public async Task SendAsync(byte[] message, uint? ppid, List<ushort>? subchannels, ushort? requiredSubchannel)
        {
            _logger.LogDebug("SendAsync() | DataProducer:{DataProducerId}", DataProducerId);

            ppid ??= !message.IsNullOrEmpty() ? 53u : 57u;

            // Ensure we honor PPIDs.
            if(ppid == 57)
            {
                message = new byte[1];
            }

            await SendInternalAsync(message, ppid.Value, subchannels, requiredSubchannel);
        }

        private async Task SendInternalAsync(byte[] data, uint ppid, List<ushort>? subchannels, ushort? requiredSubchannel)
        {
            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataProducer closed");
                }

                var sendNotification = new SendNotificationT
                {
                    Ppid = ppid,
                    Data = data,
                    Subchannels = subchannels ?? new List<ushort>(0),
                    RequiredSubchannel = requiredSubchannel,
                };

                var sendNotificationOffset = SendNotification.Pack(_channel.BufferBuilder, sendNotification);

                // Fire and forget
                _channel.NotifyAsync(
                    FBS.Notification.Event.PRODUCER_SEND,
                    FBS.Notification.Body.DataProducer_SendNotification,
                    sendNotificationOffset.Value,
                    _internal.DataProducerId
                    ).ContinueWithOnFaultedHandleLog(_logger);
            }
        }

        #region Event Handlers

        private static void HandleWorkerNotifications()
        {
            // No need to subscribe to any event.
        }

        #endregion Event Handlers
    }
}
