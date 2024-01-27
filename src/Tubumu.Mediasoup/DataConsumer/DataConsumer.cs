using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FBS.DataConsumer;
using FBS.Notification;
using FBS.Request;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace Tubumu.Mediasoup
{
    public class DataConsumer : EventEmitter
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<DataConsumer> _logger;

        /// <summary>
        /// Internal data.
        /// </summary>
        private readonly DataConsumerInternal _internal;

        /// <summary>
        /// DataConsumer id.
        /// </summary>
        public string DataConsumerId => _internal.DataConsumerId;

        /// <summary>
        /// DataConsumer data.
        /// </summary>
        public DataConsumerData Data { get; set; }

        /// <summary>
        /// Channel instance.
        /// </summary>
        private readonly IChannel _channel;

        /// <summary>
        /// Close flag
        /// </summary>
        private bool _closed;

        private readonly AsyncReaderWriterLock _closeLock = new();

        /// <summary>
        /// Paused flag.
        /// </summary>
        private bool _paused;

        /// <summary>
        /// Associated DataProducer paused flag.
        /// </summary>
        private bool _dataProducerPaused;

        /// <summary>
        /// Subchannels subscribed to.
        /// </summary>
        private List<ushort> _subchannels;

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
        /// <para>@emits dataproducerclose</para>
        /// <para>@emits message - (message: Buffer, ppid: number)</para>
        /// <para>@emits sctpsendbufferfull</para>
        /// <para>@emits bufferedamountlow - (bufferedAmount: number)</para>
        /// <para>@emits @close</para>
        /// <para>@emits @dataproducerclose</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits pause</para>
        /// <para>@emits resume</para>
        /// </summary>
        public DataConsumer(
            ILoggerFactory loggerFactory,
            DataConsumerInternal internal_,
            DataConsumerData data,
            IChannel channel,
            bool paused,
            bool dataProducerPaused,
            List<ushort> subchannels,
            Dictionary<string, object>? appData
        )
        {
            _logger = loggerFactory.CreateLogger<DataConsumer>();

            _internal = internal_;
            Data = data;
            _channel = channel;
            _paused = paused;
            _dataProducerPaused = dataProducerPaused;
            _subchannels = subchannels;
            AppData = appData ?? new Dictionary<string, object>();

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the DataConsumer.
        /// </summary>
        public async Task CloseAsync()
        {
            _logger.LogDebug("CloseAsync() | DataConsumer:{DataConsumerId}", DataConsumerId);

            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                _channel.OnNotification -= OnNotificationHandle;

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var closeDataConsumerRequest = new FBS.Transport.CloseDataConsumerRequestT
                {
                    DataConsumerId = _internal.DataConsumerId,
                };

                var closeDataConsumerRequestOffset = FBS.Transport.CloseDataConsumerRequest.Pack(bufferBuilder, closeDataConsumerRequest);

                // Fire and forget
                _channel.RequestAsync(
                    bufferBuilder,
                    Method.TRANSPORT_CLOSE_DATACONSUMER,
                    FBS.Request.Body.Transport_CloseDataConsumerRequest,
                    closeDataConsumerRequestOffset.Value,
                    _internal.TransportId
                    ).ContinueWithOnFaultedHandleLog(_logger);

                Emit("@close");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Transport was closed.
        /// </summary>
        public async Task TransportClosedAsync()
        {
            _logger.LogDebug("TransportClosedAsync() | DataConsumer:{DataConsumerId}", DataConsumerId);

            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _closed = true;

                // Remove notification subscriptions.
                _channel.OnNotification -= OnNotificationHandle;

                Emit("transportclose");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Dump DataConsumer.
        /// </summary>
        public async Task<DumpResponseT> DumpAsync()
        {
            _logger.LogDebug("DumpAsync() | DataConsumer:{DataConsumerId}", DataConsumerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var response = await _channel.RequestAsync(bufferBuilder, Method.DATACONSUMER_DUMP,
                    null,
                    null,
                    _internal.DataConsumerId);

                /* Decode Response. */
                var data = response.Value.BodyAsDataConsumer_DumpResponse().UnPack();
                return data;
            }
        }

        /// <summary>
        /// Get DataConsumer stats. Return: DataConsumerStat[]
        /// </summary>
        public async Task<GetStatsResponseT[]> GetStatsAsync()
        {
            _logger.LogDebug("GetStatsAsync() | DataConsumer:{DataConsumerId}", DataConsumerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var response = await _channel.RequestAsync(bufferBuilder, Method.DATACONSUMER_GET_STATS,
                    null,
                    null,
                    _internal.DataConsumerId);

                // Decode Response
                var data = response.Value.BodyAsDataConsumer_GetStatsResponse().UnPack();

                return new[] { data };
            }
        }

        /// <summary>
        /// Pause the DataConsumer.
        /// </summary>
        public async Task PauseAsync()
        {
            _logger.LogDebug("PauseAsync() | DataConsumer:{DataConsumerId}", DataConsumerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                /* Ignore Response. */
                _ = await _channel.RequestAsync(bufferBuilder, Method.DATACONSUMER_PAUSE,
                     null,
                     null,
                     _internal.DataConsumerId);

                var wasPaused = _paused;

                _paused = true;

                // Emit observer event.
                if(!wasPaused && !_dataProducerPaused)
                {
                    Observer.Emit("pause");
                }
            }
        }

        /// <summary>
        /// Resume the DataConsumer.
        /// </summary>
        public async Task ResumeAsync()
        {
            _logger.LogDebug("ResumeAsync() | DataConsumer:{DataConsumerId}", DataConsumerId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                await _channel.RequestAsync(bufferBuilder, Method.DATACONSUMER_PAUSE,
                    null,
                    null,
                    _internal.DataConsumerId);

                var wasPaused = _paused;

                _paused = false;

                // Emit observer event.
                if(wasPaused && !_dataProducerPaused)
                {
                    Observer.Emit("resume");
                }
            }
        }

        /// <summary>
        /// Set buffered amount low threshold.
        /// </summary>
        public async Task SetBufferedAmountLowThresholdAsync(uint threshold)
        {
            _logger.LogDebug("SetBufferedAmountLowThreshold() | Threshold:{threshold}", threshold);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var setBufferedAmountLowThresholdRequest = new SetBufferedAmountLowThresholdRequestT
                {
                    Threshold = threshold
                };

                var setBufferedAmountLowThresholdRequestOffset = SetBufferedAmountLowThresholdRequest.Pack(bufferBuilder, setBufferedAmountLowThresholdRequest);

                // Fire and forget
                _channel.RequestAsync(
                    bufferBuilder,
                    Method.DATACONSUMER_SET_BUFFERED_AMOUNT_LOW_THRESHOLD,
                    FBS.Request.Body.DataConsumer_SetBufferedAmountLowThresholdRequest,
                    setBufferedAmountLowThresholdRequestOffset.Value,
                    _internal.DataConsumerId
                    ).ContinueWithOnFaultedHandleLog(_logger);
            }
        }

        /// <summary>
        /// Send data (just valid for DataProducers created on a DirectTransport).
        /// </summary>
        public async Task SendAsync(string message, uint? ppid)
        {
            _logger.LogDebug("SendAsync() | DataConsumer:{DataConsumerId}", DataConsumerId);

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

            await SendInternalAsync(Encoding.UTF8.GetBytes(message), ppid.Value);
        }

        /// <summary>
        /// Send data (just valid for DataProducers created on a DirectTransport).
        /// </summary>
        public async Task SendAsync(byte[] message, uint? ppid)
        {
            _logger.LogDebug("SendAsync() | DataConsumer:{DataConsumerId}", DataConsumerId);

            ppid ??= !message.IsNullOrEmpty() ? 53u : 57u;

            // Ensure we honor PPIDs.
            if(ppid == 57)
            {
                message = new byte[1];
            }

            await SendInternalAsync(message, ppid.Value);
        }

        private async Task SendInternalAsync(byte[] data, uint ppid)
        {
            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var sendRequest = new SendRequestT
                {
                    Ppid = ppid,
                    Data = data
                };

                var sendRequestOffset = SendRequest.Pack(bufferBuilder, sendRequest);

                // Fire and forget
                _channel.RequestAsync(bufferBuilder, Method.DATACONSUMER_SEND,
                    FBS.Request.Body.DataConsumer_SendRequest,
                    sendRequestOffset.Value,
                    _internal.DataConsumerId
                    ).ContinueWithOnFaultedHandleLog(_logger);
            }
        }

        /// <summary>
        /// Get buffered amount size.
        /// </summary>
        public async Task<uint> GetBufferedAmountAsync()
        {
            _logger.LogDebug("GetBufferedAmountAsync()");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var response = await _channel.RequestAsync(bufferBuilder, Method.DATACONSUMER_GET_BUFFERED_AMOUNT,
                    null,
                    null,
                    _internal.DataConsumerId);

                /* Decode Response. */
                var data = response.Value.BodyAsDataConsumer_GetBufferedAmountResponse().UnPack();
                return data.BufferedAmount;
            }
        }

        /// <summary>
        /// Set subchannels.
        /// </summary>
        public async Task SetSubchannelsAsync(List<ushort> subchannels)
        {
            _logger.LogDebug("SetSubchannelsAsync()");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var setSubchannelsRequest = new SetSubchannelsRequestT
                {
                    Subchannels = subchannels
                };

                var setSubchannelsRequestOffset = SetSubchannelsRequest.Pack(bufferBuilder, setSubchannelsRequest);

                var response = await _channel.RequestAsync(bufferBuilder, Method.DATACONSUMER_SET_SUBCHANNELS,
                     FBS.Request.Body.DataConsumer_SetSubchannelsRequest,
                     setSubchannelsRequestOffset.Value,
                     _internal.DataConsumerId);

                /* Decode Response. */
                var data = response.Value.BodyAsDataConsumer_SetSubchannelsResponse().UnPack();
                // Update subchannels.
                _subchannels = data.Subchannels;
            }
        }

        /// <summary>
        /// Add a subchannel.
        /// </summary>
        public async Task AddSubchannelAsync(ushort subchannel)
        {
            _logger.LogDebug("AddSubchannelAsync()");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var addSubchannelsRequest = new AddSubchannelRequestT
                {
                    Subchannel = subchannel
                };

                var addSubchannelRequestOffset = AddSubchannelRequest.Pack(bufferBuilder, addSubchannelsRequest);

                var response = await _channel.RequestAsync(bufferBuilder, Method.DATACONSUMER_ADD_SUBCHANNEL,
                     FBS.Request.Body.DataConsumer_AddSubchannelRequest,
                     addSubchannelRequestOffset.Value,
                     _internal.DataConsumerId);

                /* Decode Response. */
                var data = response.Value.BodyAsDataConsumer_AddSubchannelResponse().UnPack();
                // Update subchannels.
                _subchannels = data.Subchannels;
            }
        }

        /// <summary>
        /// Remove a subchannel.
        /// </summary>
        public async Task RemoveSubchannelAsync(ushort subchannel)
        {
            _logger.LogDebug("RemoveSubchannelAsync()");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("DataConsumer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var removeSubchannelsRequest = new RemoveSubchannelRequestT
                {
                    Subchannel = subchannel
                };

                var removeSubchannelRequestOffset = RemoveSubchannelRequest.Pack(bufferBuilder, removeSubchannelsRequest);

                var response = await _channel.RequestAsync(bufferBuilder, Method.DATACONSUMER_REMOVE_SUBCHANNEL,
                     FBS.Request.Body.DataConsumer_RemoveSubchannelRequest,
                     removeSubchannelRequestOffset.Value,
                     _internal.DataConsumerId);

                /* Decode Response. */
                var data = response.Value.BodyAsDataConsumer_AddSubchannelResponse().UnPack();
                // Update subchannels.
                _subchannels = data.Subchannels;
            }
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            _channel.OnNotification += OnNotificationHandle;
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void OnNotificationHandle(string handlerId, Event @event, Notification notification)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            if(handlerId != DataConsumerId)
            {
                return;
            }

            switch(@event)
            {
                case Event.DATACONSUMER_DATAPRODUCER_CLOSE:
                    {
                        using(await _closeLock.WriteLockAsync())
                        {
                            if(_closed)
                            {
                                break;
                            }

                            _closed = true;

                            // Remove notification subscriptions.
                            _channel.OnNotification -= OnNotificationHandle;

                            Emit("@dataproducerclose");
                            Emit("dataproducerclose");

                            // Emit observer event.
                            Observer.Emit("close");
                        }

                        break;
                    }
                case Event.DATACONSUMER_DATAPRODUCER_PAUSE:
                    {
                        if(_dataProducerPaused)
                        {
                            break;
                        }

                        _dataProducerPaused = true;

                        Emit("dataproducerpause");

                        // Emit observer event.
                        if(!_paused)
                        {
                            Observer.Emit("pause");
                        }

                        break;
                    }

                case Event.DATACONSUMER_DATAPRODUCER_RESUME:
                    {
                        if(!_dataProducerPaused)
                        {
                            break;
                        }

                        _dataProducerPaused = false;

                        Emit("dataproducerresume");

                        // Emit observer event.
                        if(!_paused)
                        {
                            Observer.Emit("resume");
                        }

                        break;
                    }
                case Event.DATACONSUMER_SCTP_SENDBUFFER_FULL:
                    {
                        Emit("sctpsendbufferfull");

                        break;
                    }
                case Event.DATACONSUMER_BUFFERED_AMOUNT_LOW:
                    {
                        var bufferedAmountLowNotification = notification.BodyAsDataConsumer_BufferedAmountLowNotification().UnPack();

                        Emit("bufferedamountlow", bufferedAmountLowNotification.BufferedAmount);

                        break;
                    }
                case Event.DATACONSUMER_MESSAGE:
                    {
                        var messageNotification = notification.BodyAsDataConsumer_MessageNotification().UnPack();
                        Emit("message", messageNotification);

                        break;
                    }
                default:
                    {
                        _logger.LogError("OnNotificationHandle() | Ignoring unknown event \"{@event}\" in channel listener", @event);
                        break;
                    }
            }
        }

        #endregion Event Handlers
    }
}
