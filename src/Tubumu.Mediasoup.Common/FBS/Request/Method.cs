// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FBS.Request
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Method : byte
    {
        [EnumMember(Value = "worker.close")]
        WORKER_CLOSE = 0,
        [EnumMember(Value = "worker.dump")]
        WORKER_DUMP = 1,
        [EnumMember(Value = "worker.getResourceUsage")]
        WORKER_GET_RESOURCE_USAGE = 2,
        [EnumMember(Value = "worker.updateSettings")]
        WORKER_UPDATE_SETTINGS = 3,
        [EnumMember(Value = "worker.createWebRtcServer")]
        WORKER_CREATE_WEBRTCSERVER = 4,
        [EnumMember(Value = "worker.createRouter")]
        WORKER_CREATE_ROUTER = 5,
        [EnumMember(Value = "worker.closeWebRtcServer")]
        WORKER_WEBRTCSERVER_CLOSE = 6,
        [EnumMember(Value = "worker.closeRouter")]
        WORKER_CLOSE_ROUTER = 7,
        [EnumMember(Value = "webRtcServer.dump")]
        WEBRTCSERVER_DUMP = 8,
        [EnumMember(Value = "router.dump")]
        ROUTER_DUMP = 9,
        [EnumMember(Value = "router.createWebRtcTransport")]
        ROUTER_CREATE_WEBRTCTRANSPORT = 10,
        [EnumMember(Value = "router.createWebRtcTransportWithServer")]
        ROUTER_CREATE_WEBRTCTRANSPORT_WITH_SERVER = 11,
        [EnumMember(Value = "router.createPlainTransport")]
        ROUTER_CREATE_PLAINTRANSPORT = 12,
        [EnumMember(Value = "router.createPipeTransport")]
        ROUTER_CREATE_PIPETRANSPORT = 13,
        [EnumMember(Value = "router.createDirectTransport")]
        ROUTER_CREATE_DIRECTTRANSPORT = 14,
        [EnumMember(Value = "router.closeTransport")]
        ROUTER_CLOSE_TRANSPORT = 15,
        [EnumMember(Value = "router.createActiveSpeakerObserver")]
        ROUTER_CREATE_ACTIVESPEAKEROBSERVER = 16,
        [EnumMember(Value = "router.createAudioLevelObserver")]
        ROUTER_CREATE_AUDIOLEVELOBSERVER = 17,
        [EnumMember(Value = "router.closeRtpObserver")]
        ROUTER_CLOSE_RTPOBSERVER = 18,
        [EnumMember(Value = "transport.dump")]
        TRANSPORT_DUMP = 19,
        [EnumMember(Value = "transport.getStats")]
        TRANSPORT_GET_STATS = 20,
        [EnumMember(Value = "transport.connect")]
        TRANSPORT_CONNECT = 21,
        [EnumMember(Value = "transport.setMaxIncomingBitrate")]
        TRANSPORT_SET_MAX_INCOMING_BITRATE = 22,
        [EnumMember(Value = "transport.setMaxOutgoingBitrate")]
        TRANSPORT_SET_MAX_OUTGOING_BITRATE = 23,
        [EnumMember(Value = "transport.setMinOutgoingBitrate")]
        TRANSPORT_SET_MIN_OUTGOING_BITRATE = 24,
        [EnumMember(Value = "transport.restartIce")]
        TRANSPORT_RESTART_ICE = 25,
        [EnumMember(Value = "transport.produce")]
        TRANSPORT_PRODUCE = 26,
        [EnumMember(Value = "transport.produceData")]
        TRANSPORT_PRODUCE_DATA = 27,
        [EnumMember(Value = "transport.consume")]
        TRANSPORT_CONSUME = 28,
        [EnumMember(Value = "transport.consumeData")]
        TRANSPORT_CONSUME_DATA = 29,
        [EnumMember(Value = "transport.enableTraceEvent")]
        TRANSPORT_ENABLE_TRACE_EVENT = 30,
        [EnumMember(Value = "transport.closeProducer")]
        TRANSPORT_CLOSE_PRODUCER = 31,
        [EnumMember(Value = "transport.closeConsumer")]
        TRANSPORT_CLOSE_CONSUMER = 32,
        [EnumMember(Value = "transport.closeDataProducer")]
        TRANSPORT_CLOSE_DATAPRODUCER = 33,
        [EnumMember(Value = "transport.closeDataConsumer")]
        TRANSPORT_CLOSE_DATACONSUMER = 34,
        [EnumMember(Value = "plainTransport.connect")]
        PLAINTRANSPORT_CONNECT = 35,
        [EnumMember(Value = "pipeTransport.connect")]
        PIPETRANSPORT_CONNECT = 36,
        [EnumMember(Value = "webRtcTransport.connect")]
        WEBRTCTRANSPORT_CONNECT = 37,
        [EnumMember(Value = "producer.dump")]
        PRODUCER_DUMP = 38,
        [EnumMember(Value = "producer.getStats")]
        PRODUCER_GET_STATS = 39,
        [EnumMember(Value = "producer.pause")]
        PRODUCER_PAUSE = 40,
        [EnumMember(Value = "producer.resume")]
        PRODUCER_RESUME = 41,
        [EnumMember(Value = "producer.enableTraceEvent")]
        PRODUCER_ENABLE_TRACE_EVENT = 42,
        [EnumMember(Value = "consumer.dump")]
        CONSUMER_DUMP = 43,
        [EnumMember(Value = "consumer.getStats")]
        CONSUMER_GET_STATS = 44,
        [EnumMember(Value = "consumer.pause")]
        CONSUMER_PAUSE = 45,
        [EnumMember(Value = "consumer.resume")]
        CONSUMER_RESUME = 46,
        [EnumMember(Value = "consumer.setPreferredLayers")]
        CONSUMER_SET_PREFERRED_LAYERS = 47,
        [EnumMember(Value = "consumer.setPriority")]
        CONSUMER_SET_PRIORITY = 48,
        [EnumMember(Value = "consumer.requestKeyFrame")]
        CONSUMER_REQUEST_KEY_FRAME = 49,
        [EnumMember(Value = "consumer.enableTraceEvent")]
        CONSUMER_ENABLE_TRACE_EVENT = 50,
        [EnumMember(Value = "dataProducer.dump")]
        DATAPRODUCER_DUMP = 51,
        [EnumMember(Value = "dataProducer.getStats")]
        DATAPRODUCER_GET_STATS = 52,
        [EnumMember(Value = "dataProducer.pause")]
        DATAPRODUCER_PAUSE = 53,
        [EnumMember(Value = "dataProducer.resume")]
        DATAPRODUCER_RESUME = 54,
        [EnumMember(Value = "dataConsumer.dump")]
        DATACONSUMER_DUMP = 55,
        [EnumMember(Value = "dataConsumer.getStats")]
        DATACONSUMER_GET_STATS = 56,
        [EnumMember(Value = "dataConsumer.pause")]
        DATACONSUMER_PAUSE = 57,
        [EnumMember(Value = "dataConsumer.resume")]
        DATACONSUMER_RESUME = 58,
        [EnumMember(Value = "dataConsumer.getBufferedAmount")]
        DATACONSUMER_GET_BUFFERED_AMOUNT = 59,
        [EnumMember(Value = "dataConsumer.setBufferedAmountLowThreshold")]
        DATACONSUMER_SET_BUFFERED_AMOUNT_LOW_THRESHOLD = 60,
        [EnumMember(Value = "dataConsumer.send")]
        DATACONSUMER_SEND = 61,
        [EnumMember(Value = "dataConsumer.setSubchannels")]
        DATACONSUMER_SET_SUBCHANNELS = 62,
        [EnumMember(Value = "dataConsumer.addSubchannel")]
        DATACONSUMER_ADD_SUBCHANNEL = 63,
        [EnumMember(Value = "dataConsumer.removeSubchannel")]
        DATACONSUMER_REMOVE_SUBCHANNEL = 64,
        [EnumMember(Value = "rtpObserver.pause")]
        RTPOBSERVER_PAUSE = 65,
        [EnumMember(Value = "rtpObserver.resume")]
        RTPOBSERVER_RESUME = 66,
        [EnumMember(Value = "rtpObserver.addProducer")]
        RTPOBSERVER_ADD_PRODUCER = 67,
        [EnumMember(Value = "rtpObserver.removeProducer")]
        RTPOBSERVER_REMOVE_PRODUCER = 68,
    }
}
