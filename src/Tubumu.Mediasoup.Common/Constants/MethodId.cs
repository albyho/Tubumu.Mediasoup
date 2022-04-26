using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    public enum MethodId
    {
        [EnumMember(Value = "worker.dump")]
        WORKER_DUMP = 1,

        [EnumMember(Value = "worker.getResourceUsage")]
        WORKER_GET_RESOURCE_USAGE,

        [EnumMember(Value = "worker.updateSettings")]
        WORKER_UPDATE_SETTINGS,

        [EnumMember(Value = "worker.createRouter")]
        WORKER_CREATE_ROUTER,

        [EnumMember(Value = "router.close")]
        ROUTER_CLOSE,

        [EnumMember(Value = "router.dump")]
        ROUTER_DUMP,

        [EnumMember(Value = "router.createWebRtcTransport")]
        ROUTER_CREATE_WEBRTC_TRANSPORT,

        [EnumMember(Value = "router.createPlainTransport")]
        ROUTER_CREATE_PLAIN_TRANSPORT,

        [EnumMember(Value = "router.createPipeTransport")]
        ROUTER_CREATE_PIPE_TRANSPORT,

        [EnumMember(Value = "router.createDirectTransport")]
        ROUTER_CREATE_DIRECT_TRANSPORT,

        [EnumMember(Value = "router.createActiveSpeakerObserver")]
        ROUTER_CREATE_ACTIVE_SPEAKER_OBSERVER,

        [EnumMember(Value = "router.createAudioLevelObserver")]
        ROUTER_CREATE_AUDIO_LEVEL_OBSERVER,

        [EnumMember(Value = "transport.close")]
        TRANSPORT_CLOSE,

        [EnumMember(Value = "transport.dump")]
        TRANSPORT_DUMP,

        [EnumMember(Value = "transport.getStats")]
        TRANSPORT_GET_STATS,

        [EnumMember(Value = "transport.connect")]
        TRANSPORT_CONNECT,

        [EnumMember(Value = "transport.setMaxIncomingBitrate")]
        TRANSPORT_SET_MAX_INCOMING_BITRATE,

        [EnumMember(Value = "transport.setMaxOutgoingBitrate")]
        TRANSPORT_SET_MAX_OUTGOING_BITRATE,

        [EnumMember(Value = "transport.restartIce")]
        TRANSPORT_RESTART_ICE,

        [EnumMember(Value = "transport.produce")]
        TRANSPORT_PRODUCE,

        [EnumMember(Value = "transport.consume")]
        TRANSPORT_CONSUME,

        [EnumMember(Value = "transport.produceData")]
        TRANSPORT_PRODUCE_DATA,

        [EnumMember(Value = "transport.consumeData")]
        TRANSPORT_CONSUME_DATA,

        [EnumMember(Value = "transport.enableTraceEvent")]
        TRANSPORT_ENABLE_TRACE_EVENT,

        [EnumMember(Value = "producer.close")]
        PRODUCER_CLOSE,

        [EnumMember(Value = "producer.dump")]
        PRODUCER_DUMP,

        [EnumMember(Value = "producer.getStats")]
        PRODUCER_GET_STATS,

        [EnumMember(Value = "producer.pause")]
        PRODUCER_PAUSE,

        [EnumMember(Value = "producer.resume")]
        PRODUCER_RESUME,

        [EnumMember(Value = "producer.enableTraceEvent")]
        PRODUCER_ENABLE_TRACE_EVENT,

        [EnumMember(Value = "consumer.close")]
        CONSUMER_CLOSE,

        [EnumMember(Value = "consumer.dump")]
        CONSUMER_DUMP,

        [EnumMember(Value = "consumer.getStats")]
        CONSUMER_GET_STATS,

        [EnumMember(Value = "consumer.pause")]
        CONSUMER_PAUSE,

        [EnumMember(Value = "consumer.resume")]
        CONSUMER_RESUME,

        [EnumMember(Value = "consumer.setPreferredLayers")]
        CONSUMER_SET_PREFERRED_LAYERS,

        [EnumMember(Value = "consumer.setPriority")]
        CONSUMER_SET_PRIORITY,

        [EnumMember(Value = "consumer.requestKeyFrame")]
        CONSUMER_REQUEST_KEY_FRAME,

        [EnumMember(Value = "consumer.enableTraceEvent")]
        CONSUMER_ENABLE_TRACE_EVENT,

        [EnumMember(Value = "dataProducer.close")]
        DATA_PRODUCER_CLOSE,

        [EnumMember(Value = "dataProducer.dump")]
        DATA_PRODUCER_DUMP,

        [EnumMember(Value = "dataProducer.getStats")]
        DATA_PRODUCER_GET_STATS,

        [EnumMember(Value = "dataConsumer.dump")]
        DATA_CONSUMER_CLOSE,

        [EnumMember(Value = "dataConsumer.close")]
        DATA_CONSUMER_DUMP,

        [EnumMember(Value = "dataConsumer.getStats")]
        DATA_CONSUMER_GET_STATS,

        [EnumMember(Value = "dataConsumer.getBufferedAmount")]
        DATA_CONSUMER_GET_BUFFERED_AMOUNT,

        [EnumMember(Value = "dataConsumer.setBufferedAmountLowThreshold")]
        DATA_CONSUMER_SET_BUFFERED_AMOUNT_LOW_THRESHOLD,

        [EnumMember(Value = "rtpObserver.close")]
        RTP_OBSERVER_CLOSE,

        [EnumMember(Value = "rtpObserver.pause")]
        RTP_OBSERVER_PAUSE,

        [EnumMember(Value = "rtpObserver.resume")]
        RTP_OBSERVER_RESUME,

        [EnumMember(Value = "rtpObserver.addProducer")]
        RTP_OBSERVER_ADD_PRODUCER,

        [EnumMember(Value = "rtpObserver.removeProducer")]
        RTP_OBSERVER_REMOVE_PRODUCER
    }
}
