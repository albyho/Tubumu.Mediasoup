using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FBS.PipeTransport;
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
            if(Data.SctpState.HasValue)
            {
                Data.SctpState = SctpState.Closed;
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
        /// Provide the PipeTransport remote parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task ConnectAsync(object parameters)
        {
            _logger.LogDebug("ConnectAsync()");

            if(parameters is not PipeTransportConnectParameters connectParameters)
            {
                throw new Exception($"{nameof(parameters)} type is not PipeTransportConnectParameters");
            }

            await ConnectAsync(connectParameters);
        }

        /// <summary>
        /// Provide the PipeTransport remote parameters.
        /// </summary>
        /// <param name="pipeTransportConnectParameters"></param>
        /// <returns></returns>
        public async Task ConnectAsync(PipeTransportConnectParameters pipeTransportConnectParameters)
        {
            using(await CloseLock.ReadLockAsync())
            {
                if(Closed)
                {
                    throw new InvalidStateException("Transport closed");
                }

                var reqData = pipeTransportConnectParameters;
                var resData = await Channel.RequestAsync(MethodId.TRANSPORT_CONNECT, Internal.TransportId, reqData);
                var responseData = JsonSerializer.Deserialize<PipeTransportConnectResponseData>(
                    resData!,
                    ObjectExtensions.DefaultJsonSerializerOptions
                )!;

                // Update data.
                Data.Tuple = responseData.Tuple;
            }
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

            var producer = await GetProducerById(consumerOptions.ProducerId);
            if(producer == null)
            {
                throw new Exception($"Producer with id {consumerOptions.ProducerId} not found");
            }

            // This may throw.
            var rtpParameters = ORTC.GetPipeConsumerRtpParameters(producer.Data.ConsumableRtpParameters, Data.Rtx);
            var reqData = new
            {
                ConsumerId = Guid.NewGuid().ToString(),
                producer.Data.Kind,
                RtpParameters = rtpParameters,
                Type = ConsumerType.Pipe,
                ConsumableRtpEncodings = producer.Data.ConsumableRtpParameters.Encodings,
            };

            var resData = await Channel.RequestAsync(MethodId.TRANSPORT_CONSUME, Internal.TransportId, reqData);
            var responseData = JsonSerializer.Deserialize<TransportConsumeResponseData>(
                resData!,
                ObjectExtensions.DefaultJsonSerializerOptions
            )!;

            var data = new ConsumerData(consumerOptions.ProducerId, producer.Data.Kind, rtpParameters, ConsumerType.Pipe);

            // 在 Node.js 实现中， 创建 Consumer 对象时没提供 score 和 preferredLayers 参数，且 score = { score: 10, producerScore: 10 }。
            var consumer = new Consumer(
                _loggerFactory,
                new ConsumerInternal(Internal.RouterId, Internal.TransportId, reqData.ConsumerId),
                data,
                Channel,
                PayloadChannel,
                AppData,
                responseData.Paused,
                responseData.ProducerPaused,
                responseData.Score,
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

        private void OnNotificationHandle(string targetId, string @event, string? data)
        {
            if(targetId != Internal.TransportId)
            {
                return;
            }

            switch(@event)
            {
                case "sctpstatechange":
                    {
                        var notification = JsonSerializer.Deserialize<TransportSctpStateChangeNotificationData>(
                            data!,
                            ObjectExtensions.DefaultJsonSerializerOptions
                        )!;
                        Data.SctpState = notification.SctpState;

                        Emit("sctpstatechange", Data.SctpState);

                        // Emit observer event.
                        Observer.Emit("sctpstatechange", Data.SctpState);

                        break;
                    }

                case "trace":
                    {
                        var trace = JsonSerializer.Deserialize<TransportTraceEventData>(
                            data!,
                            ObjectExtensions.DefaultJsonSerializerOptions
                        )!;

                        Emit("trace", trace);

                        // Emit observer event.
                        Observer.Emit("trace", trace);

                        break;
                    }

                default:
                    {
                        _logger.LogError($"OnNotificationHandle() | Ignoring unknown event{@event}");
                        break;
                    }
            }
        }

        #endregion Event Handlers
    }
}
