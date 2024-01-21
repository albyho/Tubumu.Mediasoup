using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FBS.Notification;
using FBS.PipeTransport;
using FBS.Request;
using FBS.Transport;
using Microsoft.Extensions.Logging;

namespace Tubumu.Mediasoup
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

        /// <summary>
        /// PipeTransport data.
        /// </summary>
        public DumpResponseT Data { get; }

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
        /// <param name="@internal"></param>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        /// <param name="appData"></param>
        /// <param name="getRouterRtpCapabilities"></param>
        /// <param name="getProducerById"></param>
        /// <param name="getDataProducerById"></param>
        public PipeTransport(
            ILoggerFactory loggerFactory,
            TransportInternal @internal,
            DumpResponseT data,
            IChannel channel,
            Dictionary<string, object>? appData,
            Func<RtpCapabilities> getRouterRtpCapabilities,
            Func<string, Task<Producer?>> getProducerById,
            Func<string, Task<DataProducer?>> getDataProducerById
        )
            : base(
                loggerFactory,
                @internal,
                data.Base,
                channel,
                appData,
                getRouterRtpCapabilities,
                getProducerById,
                getDataProducerById
            )
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<PipeTransport>();

            Data = data;

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the PipeTransport.
        /// </summary>
        protected override Task OnCloseAsync()
        {
            if(Data.Base.SctpState.HasValue)
            {
                Data.Base.SctpState = FBS.SctpAssociation.SctpState.CLOSED;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Router was closed.
        /// </summary>
        protected override Task OnRouterClosedAsync()
        {
            return OnCloseAsync();
        }

        /// <summary>
        /// Dump Transport.
        /// </summary>
        protected override async Task<object> OnDumpAsync()
        {
            var response = await Channel.RequestAsync(Method.TRANSPORT_DUMP, null, null, Internal.TransportId);
            var data = response.Value.BodyAsPipeTransport_DumpResponse().UnPack();
            return data;
        }

        /// <summary>
        /// Get Transport stats.
        /// </summary>
        protected override async Task<object[]> OnGetStatsAsync()
        {
            var response = await Channel.RequestAsync(Method.TRANSPORT_GET_STATS, null, null, Internal.TransportId);
            var data = response.Value.BodyAsPipeTransport_GetStatsResponse().UnPack();
            return new[] { data };
        }

        /// <summary>
        /// Provide the PipeTransport remote parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected override async Task OnConnectAsync(object parameters)
        {
            _logger.LogDebug("OnConnectAsync() | PipeTransport:{TransportId}", TransportId);

            if(parameters is not ConnectRequestT connectRequestT)
            {
                throw new Exception($"{nameof(parameters)} type is not FBS.PipeTransport.ConnectRequestT");
            }

            var connectRequestOffset = ConnectRequest.Pack(Channel.BufferBuilder, connectRequestT);

            var response = await Channel.RequestAsync(
                 FBS.Request.Method.PIPETRANSPORT_CONNECT,
                 FBS.Request.Body.PipeTransport_ConnectRequest,
                 connectRequestOffset.Value,
                 Internal.TransportId);

            /* Decode Response. */
            var data = response.Value.BodyAsPipeTransport_ConnectResponse().UnPack();

            // Update data.
            Data.Tuple = data.Tuple;
        }

        /// <summary>
        /// Create a Consumer.
        /// </summary>
        /// <param name="consumerOptions">注意：由于强类型的原因，这里使用的是 ConsumerOptions 类而不是 PipConsumerOptions 类</param>
        /// <returns></returns>
        public override async Task<Consumer> ConsumeAsync(ConsumerOptions consumerOptions)
        {
            _logger.LogDebug("ConsumeAsync()");

            if(consumerOptions.ProducerId.IsNullOrWhiteSpace())
            {
                throw new Exception("missing producerId");
            }

            var producer = await GetProducerById(consumerOptions.ProducerId) ?? throw new Exception($"Producer with id {consumerOptions.ProducerId} not found");

            // This may throw.
            var rtpParameters = ORTC.GetPipeConsumerRtpParameters(producer.Data.ConsumableRtpParameters, Data.Rtx);

            var consumerId = Guid.NewGuid().ToString();

            var consumeRequest = new ConsumeRequestT
            {
                ProducerId = consumerOptions.ProducerId,
                ConsumerId = consumerId,
                Kind = producer.Data.Kind,
                RtpParameters = rtpParameters.SerializeRtpParameters(),
                Type = FBS.RtpParameters.Type.PIPE,
                ConsumableRtpEncodings = producer.Data.ConsumableRtpParameters.Encodings,
            };

            var consumeRequestOffset = ConsumeRequest.Pack(Channel.BufferBuilder, consumeRequest);

            var response = await Channel.RequestAsync(
                 FBS.Request.Method.TRANSPORT_CONSUME,
                 FBS.Request.Body.Transport_ConsumeRequest,
                 consumeRequestOffset.Value,
                 Internal.TransportId);

            /* Decode Response. */
            var responseData = response.Value.BodyAsTransport_ConsumeResponse().UnPack();

            var consumerData = new ConsumerData
            {
                ProducerId = consumerOptions.ProducerId,
                Kind = producer.Data.Kind,
                RtpParameters = rtpParameters,
                Type = producer.Data.Type,
            };

            var score = new FBS.Consumer.ConsumerScoreT
            {
                Score = 10,
                ProducerScore = 10,
                ProducerScores = new List<byte>(0)
            };

            var consumer = new Consumer(
                _loggerFactory,
                new ConsumerInternal(Internal.RouterId, Internal.TransportId, consumerId),
                consumerData,
                Channel,
                AppData,
                responseData.Paused,
                responseData.ProducerPaused,
                score, // Not `responseData.Score`
                responseData.PreferredLayers
            );

            consumer.On(
                    "@close",
                    async (_, _) =>
                    {
                        await ConsumersLock.WaitAsync();
                        try
                        {
                            Consumers.Remove(consumer.ConsumerId);
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError(ex, "@close");
                        }
                        finally
                        {
                            ConsumersLock.Set();
                        }
                    }
                );
            consumer.On(
                "@producerclose",
                async (_, _) =>
                {
                    await ConsumersLock.WaitAsync();
                    try
                    {
                        Consumers.Remove(consumer.ConsumerId);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "@producerclose");
                    }
                    finally
                    {
                        ConsumersLock.Set();
                    }
                }
            );

            await ConsumersLock.WaitAsync();
            try
            {
                Consumers[consumer.ConsumerId] = consumer;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "ConsumeAsync()");
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
            Channel.OnNotification += OnNotificationHandle;
        }

        private void OnNotificationHandle(string handlerId, Event @event, Notification notification)
        {
            if(handlerId != Internal.TransportId)
            {
                return;
            }

            switch(@event)
            {
                case Event.TRANSPORT_SCTP_STATE_CHANGE:
                    {
                        var sctpStateChangeNotification = notification.BodyAsTransport_SctpStateChangeNotification().UnPack();

                        Data.Base.SctpState = sctpStateChangeNotification.SctpState;

                        Emit("sctpstatechange", Data.Base.SctpState);

                        // Emit observer event.
                        Observer.Emit("sctpstatechange", Data.Base.SctpState);

                        break;
                    }
                case Event.TRANSPORT_TRACE:
                    {
                        var traceNotification = notification.BodyAsTransport_TraceNotification().UnPack();

                        Emit("trace", traceNotification);

                        // Emit observer event.
                        Observer.Emit("trace", traceNotification);

                        break;
                    }
                default:
                    {
                        _logger.LogError("OnNotificationHandle() | PipeTransport:{TransportId} Ignoring unknown event:{@event}", TransportId, @event);
                        break;
                    }
            }
        }

        #endregion Event Handlers
    }
}
