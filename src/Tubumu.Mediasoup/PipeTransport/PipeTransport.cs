using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
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
            IChannel channel,
            IPayloadChannel payloadChannel,
            Dictionary<string, object>? appData,
            Func<RtpCapabilities> getRouterRtpCapabilities,
            Func<string, Task<Producer?>> getProducerById,
            Func<string, Task<DataProducer?>> getDataProducerById,
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
            catch(Exception ex)
            {
                _logger.LogError(ex, "CloseAsync()");
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
            catch(Exception ex)
            {
                _logger.LogError(ex, "RouterClosedAsync()");
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

            return parameters is PipeTransportConnectParameters connectParameters
                ? ConnectAsync(connectParameters)
                : throw new Exception($"{nameof(parameters)} type is not PipTransportConnectParameters");
        }

        /// <summary>
        /// Provide the PipeTransport remote parameters.
        /// </summary>
        /// <param name="pipeTransportConnectParameters"></param>
        /// <returns></returns>
        public async Task ConnectAsync(PipeTransportConnectParameters pipeTransportConnectParameters)
        {
            var reqData = pipeTransportConnectParameters;
            var resData = await Channel.RequestAsync(MethodId.TRANSPORT_CONNECT, Internal, reqData);
            var responseData = JsonSerializer.Deserialize<PipeTransportConnectResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions)!;

            // Update data.
            Tuple = responseData.Tuple;
        }

        /// <summary>
        /// Create a Consumer.
        /// </summary>
        /// <param name="consumerOptions">注意：由于强类型的原因，这里使用的是 ConsumerOptions 类而不是 PipConsumerOptions 类</param>
        /// <returns></returns>
        public override async Task<Consumer> ConsumeAsync(ConsumerOptions consumerOptions)
        {
            _logger.LogDebug("ConsumeAsync()");

            if (consumerOptions.ProducerId.IsNullOrWhiteSpace())
            {
                throw new Exception("missing producerId");
            }

            var producer = await GetProducerById(consumerOptions.ProducerId);
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

            var resData = await Channel.RequestAsync(MethodId.TRANSPORT_CONSUME, @internal, reqData);
            var responseData = JsonSerializer.Deserialize<TransportConsumeResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions)!;

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

            consumer.On("@close", async (_, _) =>
                {
                    await ConsumersLock.WaitAsync();
                    try
                    {
                        Consumers.Remove(consumer.ConsumerId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "@close");
                    }
                    finally
                    {
                        ConsumersLock.Set();
                    }
                });
            consumer.On("@producerclose", async (_, _) =>
            {
                await ConsumersLock.WaitAsync();
                try
                {
                    Consumers.Remove(consumer.ConsumerId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "@producerclose");
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
            Channel.MessageEvent += OnChannelMessage;
        }

        private void OnChannelMessage(string targetId, string @event, string? data)
        {
            if (targetId != Internal.TransportId)
            {
                return;
            }

            switch (@event)
            {
                case "sctpstatechange":
                    {
                        var notification = JsonSerializer.Deserialize<TransportSctpStateChangeNotificationData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;
                        SctpState = notification.SctpState;

                        Emit("sctpstatechange", SctpState);

                        // Emit observer event.
                        Observer.Emit("sctpstatechange", SctpState);

                        break;
                    }

                case "trace":
                    {
                        var trace = JsonSerializer.Deserialize<TransportTraceEventData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;

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
