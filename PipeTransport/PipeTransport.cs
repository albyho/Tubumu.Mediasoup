using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tubumu.Core.Extensions;

namespace TubumuMeeting.Mediasoup
{
    public class PipeTransport : Transport
    {
        /// <summary>
        /// Logger factory for create logger.
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<PipeTransport> _logger;

        #region PipeTransport data.

        public TransportTuple Tuple { get; private set; }

        public bool Rtx { get; private set; }

        public SrtpParameters? SrtpParameters { get; private set; }

        #endregion PipeTransport data.

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits sctpstatechange - (sctpState: SctpState)</para>
        /// <para>@emits trace - (trace: TransportTraceEventData)</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits newproducer - (producer: Producer)</para>
        /// <para>@emits newconsumer - (consumer: Consumer)</para>
        /// <para>@emits newdataproducer - (dataProducer: DataProducer)</para>
        /// <para>@emits newdataconsumer - (dataConsumer: DataConsumer)</para>
        /// <para>@emits sctpstatechange - (sctpState: SctpState)</para>
        /// <para>@emits trace - (trace: TransportTraceEventData)</para>
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="transportInternalData"></param>
        /// <param name="sctpParameters"></param>
        /// <param name="sctpState"></param>
        /// <param name="channel"></param>
        /// <param name="payloadChannel"></param>
        /// <param name="appData"></param>
        /// <param name="getRouterRtpCapabilities"></param>
        /// <param name="getProducerById"></param>
        /// <param name="getDataProducerById"></param>
        public PipeTransport(ILoggerFactory loggerFactory,
            TransportInternalData transportInternalData,
            SctpParameters? sctpParameters,
            SctpState? sctpState,
            Channel channel,
            PayloadChannel payloadChannel,
            Dictionary<string, object>? appData,
            Func<RtpCapabilities> getRouterRtpCapabilities,
            Func<string, Producer?> getProducerById,
            Func<string, DataProducer?> getDataProducerById,
            TransportTuple tuple,
            bool rtx,
            SrtpParameters? srtpParameters
            ) : base(loggerFactory, transportInternalData, sctpParameters, sctpState, channel, payloadChannel, appData, getRouterRtpCapabilities, getProducerById, getDataProducerById)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<PipeTransport>();

            // Data
            Tuple = tuple;
            Rtx = rtx;
            SrtpParameters = srtpParameters;

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the PipeTransport.
        /// </summary>
        public override async Task CloseAsync()
        {
            if (Closed)
            {
                return;
            }

            await CloseLock.WaitAsync();
            try
            {
                if (Closed)
                {
                    return;
                }

                if (SctpState.HasValue)
                {
                    SctpState = Mediasoup.SctpState.Closed;
                }

                await base.CloseAsync();
            }
            finally
            {
                CloseLock.Set();
            }
        }

        /// <summary>
        /// Router was closed.
        /// </summary>
        public override async Task RouterClosedAsync()
        {
            if (Closed)
            {
                return;
            }

            await CloseLock.WaitAsync();
            try
            {
                if (Closed)
                {
                    return;
                }

                if (SctpState.HasValue)
                {
                    SctpState = Mediasoup.SctpState.Closed;
                }

                await base.RouterClosedAsync();
            }
            finally
            {
                CloseLock.Set();
            }
        }

        /// <summary>
        /// Provide the PipeTransport remote parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override Task ConnectAsync(object parameters)
        {
            _logger.LogDebug("ConnectAsync()");

            if (!(parameters is PipeTransportConnectParameters connectParameters))
            {
                throw new Exception($"{nameof(parameters)} type is not PipTransportConnectParameters");
            }
            return ConnectAsync(connectParameters);
        }

        /// <summary>
        /// Provide the PipeTransport remote parameters.
        /// </summary>
        /// <param name="pipeTransportConnectParameters"></param>
        /// <returns></returns>
        public async Task ConnectAsync(PipeTransportConnectParameters pipeTransportConnectParameters)
        {
            var reqData = pipeTransportConnectParameters;

            var status = await Channel.RequestAsync(MethodId.TRANSPORT_CONNECT, Internal, reqData);
            var responseData = JsonConvert.DeserializeObject<PipeTransportConnectResponseData>(status!);

            // Update data.
            Tuple = responseData.Tuple;
        }

        /// <summary>
        /// Create a Consumer.
        /// </summary>
        /// <param name="consumerOptions"></param>
        /// <returns></returns>
        public override async Task<Consumer> ConsumeAsync(ConsumerOptions consumerOptions)
        {
            _logger.LogDebug("ConsumeAsync()");


            if (consumerOptions.ProducerId.IsNullOrWhiteSpace())
            {
                throw new Exception("missing producerId");
            }

            var producer = GetProducerById(consumerOptions.ProducerId);
            if (producer == null)
            {
                throw new Exception($"Producer with id {consumerOptions.ProducerId} not found");
            }

            // This may throw.
            var rtpParameters = ORTC.GetPipeConsumerRtpParameters(producer.ConsumableRtpParameters, Rtx);

            var @internal = new ConsumerInternalData
            (
                Internal.RouterId,
                Internal.TransportId,
                consumerOptions.ProducerId,
                Guid.NewGuid().ToString()
            );

            var reqData = new
            {
                producer.Kind,
                RtpParameters = rtpParameters,
                Type = ConsumerType.Pipe,
                ConsumableRtpEncodings = producer.ConsumableRtpParameters.Encodings,
            };

            var status = await Channel.RequestAsync(MethodId.TRANSPORT_CONSUME, @internal, reqData);
            var responseData = JsonConvert.DeserializeObject<TransportConsumeResponseData>(status!);

            var data = new
            {
                producer.Kind,
                RtpParameters = rtpParameters,
                Type = ConsumerType.Pipe,
            };

            // 在 Node.js 实现中， 创建 Consumer 对象时没提供 score 和 preferredLayers 参数，且 score = { score: 10, producerScore: 10 }。
            var consumer = new Consumer(_loggerFactory,
                @internal,
                data.Kind,
                data.RtpParameters,
                data.Type,
                Channel,
                PayloadChannel,
                AppData,
                responseData.Paused,
                responseData.ProducerPaused,
                responseData.Score,
                responseData.PreferredLayers);

            consumer.On("@close", async _ =>
                {
                    await ConsumersLock.WaitAsync();
                    try
                    {
                        Consumers.Remove(consumer.ConsumerId);
                    }
                    finally
                    {
                        ConsumersLock.Set();
                    }
                });
            consumer.On("@producerclose", async _ =>
            {
                await ConsumersLock.WaitAsync();
                try
                {
                    Consumers.Remove(consumer.ConsumerId);
                }
                finally
                {
                    ConsumersLock.Set();
                }
            });

            await ConsumersLock.WaitAsync();
            try
            {
                Consumers[consumer.ConsumerId] = consumer;
            }
            finally
            {
                ConsumersLock.Set();
            }

            // Emit observer event.
            Observer.Emit("newconsumer", consumer);

            return consumer;
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            Channel.MessageEvent += OnChannelMessage;
        }

        private void OnChannelMessage(string targetId, string @event, string data)
        {
            if (targetId != Internal.TransportId)
            {
                return;
            }

            switch (@event)
            {
                case "sctpstatechange":
                    {
                        var notification = JsonConvert.DeserializeObject<TransportSctpStateChangeNotificationData>(data);
                        SctpState = notification.SctpState;

                        Emit("sctpstatechange", SctpState);

                        // Emit observer event.
                        Observer.Emit("sctpstatechange", SctpState);

                        break;
                    }

                case "trace":
                    {
                        var trace = JsonConvert.DeserializeObject<TransportTraceEventData>(data);

                        Emit("trace", trace);

                        // Emit observer event.
                        Observer.Emit("trace", trace);

                        break;
                    }

                default:
                    {
                        _logger.LogError($"OnChannelMessage() | Ignoring unknown event{@event}");
                        break;
                    }
            }
        }

        #endregion Event Handlers
    }
}
