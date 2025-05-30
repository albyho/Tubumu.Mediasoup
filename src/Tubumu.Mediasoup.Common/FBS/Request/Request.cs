// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.Request
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct Request : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static Request GetRootAsRequest(ByteBuffer _bb) { return GetRootAsRequest(_bb, new Request()); }
        public static Request GetRootAsRequest(ByteBuffer _bb, Request obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public static bool VerifyRequest(ByteBuffer _bb) { Google.FlatBuffers.Verifier verifier = new Google.FlatBuffers.Verifier(_bb); return verifier.VerifyBuffer("", false, RequestVerify.Verify); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public Request __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public uint Id { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
        public FBS.Request.Method Method { get { int o = __p.__offset(6); return o != 0 ? (FBS.Request.Method)__p.bb.Get(o + __p.bb_pos) : FBS.Request.Method.WORKER_CLOSE; } }
        public string HandlerId { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetHandlerIdBytes() { return __p.__vector_as_span<byte>(8, 1); }
#else
        public ArraySegment<byte>? GetHandlerIdBytes() { return __p.__vector_as_arraysegment(8); }
#endif
        public byte[] GetHandlerIdArray() { return __p.__vector_as_array<byte>(8); }
        public FBS.Request.Body BodyType { get { int o = __p.__offset(10); return o != 0 ? (FBS.Request.Body)__p.bb.Get(o + __p.bb_pos) : FBS.Request.Body.NONE; } }
        public TTable? Body<TTable>() where TTable : struct, IFlatbufferObject { int o = __p.__offset(12); return o != 0 ? (TTable?)__p.__union<TTable>(o + __p.bb_pos) : null; }
        public FBS.Worker.UpdateSettingsRequest BodyAsWorker_UpdateSettingsRequest() { return Body<FBS.Worker.UpdateSettingsRequest>().Value; }
        public FBS.Worker.CreateWebRtcServerRequest BodyAsWorker_CreateWebRtcServerRequest() { return Body<FBS.Worker.CreateWebRtcServerRequest>().Value; }
        public FBS.Worker.CloseWebRtcServerRequest BodyAsWorker_CloseWebRtcServerRequest() { return Body<FBS.Worker.CloseWebRtcServerRequest>().Value; }
        public FBS.Worker.CreateRouterRequest BodyAsWorker_CreateRouterRequest() { return Body<FBS.Worker.CreateRouterRequest>().Value; }
        public FBS.Worker.CloseRouterRequest BodyAsWorker_CloseRouterRequest() { return Body<FBS.Worker.CloseRouterRequest>().Value; }
        public FBS.Router.CreateWebRtcTransportRequest BodyAsRouter_CreateWebRtcTransportRequest() { return Body<FBS.Router.CreateWebRtcTransportRequest>().Value; }
        public FBS.Router.CreatePlainTransportRequest BodyAsRouter_CreatePlainTransportRequest() { return Body<FBS.Router.CreatePlainTransportRequest>().Value; }
        public FBS.Router.CreatePipeTransportRequest BodyAsRouter_CreatePipeTransportRequest() { return Body<FBS.Router.CreatePipeTransportRequest>().Value; }
        public FBS.Router.CreateDirectTransportRequest BodyAsRouter_CreateDirectTransportRequest() { return Body<FBS.Router.CreateDirectTransportRequest>().Value; }
        public FBS.Router.CreateActiveSpeakerObserverRequest BodyAsRouter_CreateActiveSpeakerObserverRequest() { return Body<FBS.Router.CreateActiveSpeakerObserverRequest>().Value; }
        public FBS.Router.CreateAudioLevelObserverRequest BodyAsRouter_CreateAudioLevelObserverRequest() { return Body<FBS.Router.CreateAudioLevelObserverRequest>().Value; }
        public FBS.Router.CloseTransportRequest BodyAsRouter_CloseTransportRequest() { return Body<FBS.Router.CloseTransportRequest>().Value; }
        public FBS.Router.CloseRtpObserverRequest BodyAsRouter_CloseRtpObserverRequest() { return Body<FBS.Router.CloseRtpObserverRequest>().Value; }
        public FBS.Transport.SetMaxIncomingBitrateRequest BodyAsTransport_SetMaxIncomingBitrateRequest() { return Body<FBS.Transport.SetMaxIncomingBitrateRequest>().Value; }
        public FBS.Transport.SetMaxOutgoingBitrateRequest BodyAsTransport_SetMaxOutgoingBitrateRequest() { return Body<FBS.Transport.SetMaxOutgoingBitrateRequest>().Value; }
        public FBS.Transport.SetMinOutgoingBitrateRequest BodyAsTransport_SetMinOutgoingBitrateRequest() { return Body<FBS.Transport.SetMinOutgoingBitrateRequest>().Value; }
        public FBS.Transport.ProduceRequest BodyAsTransport_ProduceRequest() { return Body<FBS.Transport.ProduceRequest>().Value; }
        public FBS.Transport.ConsumeRequest BodyAsTransport_ConsumeRequest() { return Body<FBS.Transport.ConsumeRequest>().Value; }
        public FBS.Transport.ProduceDataRequest BodyAsTransport_ProduceDataRequest() { return Body<FBS.Transport.ProduceDataRequest>().Value; }
        public FBS.Transport.ConsumeDataRequest BodyAsTransport_ConsumeDataRequest() { return Body<FBS.Transport.ConsumeDataRequest>().Value; }
        public FBS.Transport.EnableTraceEventRequest BodyAsTransport_EnableTraceEventRequest() { return Body<FBS.Transport.EnableTraceEventRequest>().Value; }
        public FBS.Transport.CloseProducerRequest BodyAsTransport_CloseProducerRequest() { return Body<FBS.Transport.CloseProducerRequest>().Value; }
        public FBS.Transport.CloseConsumerRequest BodyAsTransport_CloseConsumerRequest() { return Body<FBS.Transport.CloseConsumerRequest>().Value; }
        public FBS.Transport.CloseDataProducerRequest BodyAsTransport_CloseDataProducerRequest() { return Body<FBS.Transport.CloseDataProducerRequest>().Value; }
        public FBS.Transport.CloseDataConsumerRequest BodyAsTransport_CloseDataConsumerRequest() { return Body<FBS.Transport.CloseDataConsumerRequest>().Value; }
        public FBS.PlainTransport.ConnectRequest BodyAsPlainTransport_ConnectRequest() { return Body<FBS.PlainTransport.ConnectRequest>().Value; }
        public FBS.PipeTransport.ConnectRequest BodyAsPipeTransport_ConnectRequest() { return Body<FBS.PipeTransport.ConnectRequest>().Value; }
        public FBS.WebRtcTransport.ConnectRequest BodyAsWebRtcTransport_ConnectRequest() { return Body<FBS.WebRtcTransport.ConnectRequest>().Value; }
        public FBS.Producer.EnableTraceEventRequest BodyAsProducer_EnableTraceEventRequest() { return Body<FBS.Producer.EnableTraceEventRequest>().Value; }
        public FBS.Consumer.SetPreferredLayersRequest BodyAsConsumer_SetPreferredLayersRequest() { return Body<FBS.Consumer.SetPreferredLayersRequest>().Value; }
        public FBS.Consumer.SetPriorityRequest BodyAsConsumer_SetPriorityRequest() { return Body<FBS.Consumer.SetPriorityRequest>().Value; }
        public FBS.Consumer.EnableTraceEventRequest BodyAsConsumer_EnableTraceEventRequest() { return Body<FBS.Consumer.EnableTraceEventRequest>().Value; }
        public FBS.DataConsumer.SetBufferedAmountLowThresholdRequest BodyAsDataConsumer_SetBufferedAmountLowThresholdRequest() { return Body<FBS.DataConsumer.SetBufferedAmountLowThresholdRequest>().Value; }
        public FBS.DataConsumer.SendRequest BodyAsDataConsumer_SendRequest() { return Body<FBS.DataConsumer.SendRequest>().Value; }
        public FBS.DataConsumer.SetSubchannelsRequest BodyAsDataConsumer_SetSubchannelsRequest() { return Body<FBS.DataConsumer.SetSubchannelsRequest>().Value; }
        public FBS.DataConsumer.AddSubchannelRequest BodyAsDataConsumer_AddSubchannelRequest() { return Body<FBS.DataConsumer.AddSubchannelRequest>().Value; }
        public FBS.DataConsumer.RemoveSubchannelRequest BodyAsDataConsumer_RemoveSubchannelRequest() { return Body<FBS.DataConsumer.RemoveSubchannelRequest>().Value; }
        public FBS.RtpObserver.AddProducerRequest BodyAsRtpObserver_AddProducerRequest() { return Body<FBS.RtpObserver.AddProducerRequest>().Value; }
        public FBS.RtpObserver.RemoveProducerRequest BodyAsRtpObserver_RemoveProducerRequest() { return Body<FBS.RtpObserver.RemoveProducerRequest>().Value; }

        public static Offset<FBS.Request.Request> CreateRequest(FlatBufferBuilder builder,
            uint id = 0,
            FBS.Request.Method method = FBS.Request.Method.WORKER_CLOSE,
            StringOffset handler_idOffset = default(StringOffset),
            FBS.Request.Body body_type = FBS.Request.Body.NONE,
            int bodyOffset = 0)
        {
            builder.StartTable(5);
            Request.AddBody(builder, bodyOffset);
            Request.AddHandlerId(builder, handler_idOffset);
            Request.AddId(builder, id);
            Request.AddBodyType(builder, body_type);
            Request.AddMethod(builder, method);
            return Request.EndRequest(builder);
        }

        public static void StartRequest(FlatBufferBuilder builder) { builder.StartTable(5); }
        public static void AddId(FlatBufferBuilder builder, uint id) { builder.AddUint(0, id, 0); }
        public static void AddMethod(FlatBufferBuilder builder, FBS.Request.Method method) { builder.AddByte(1, (byte)method, 0); }
        public static void AddHandlerId(FlatBufferBuilder builder, StringOffset handlerIdOffset) { builder.AddOffset(2, handlerIdOffset.Value, 0); }
        public static void AddBodyType(FlatBufferBuilder builder, FBS.Request.Body bodyType) { builder.AddByte(3, (byte)bodyType, 0); }
        public static void AddBody(FlatBufferBuilder builder, int bodyOffset) { builder.AddOffset(4, bodyOffset, 0); }
        public static Offset<FBS.Request.Request> EndRequest(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 8);  // handler_id
            return new Offset<FBS.Request.Request>(o);
        }
        public static void FinishRequestBuffer(FlatBufferBuilder builder, Offset<FBS.Request.Request> offset) { builder.Finish(offset.Value); }
        public static void FinishSizePrefixedRequestBuffer(FlatBufferBuilder builder, Offset<FBS.Request.Request> offset) { builder.FinishSizePrefixed(offset.Value); }
        public RequestT UnPack()
        {
            var _o = new RequestT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(RequestT _o)
        {
            _o.Id = this.Id;
            _o.Method = this.Method;
            _o.HandlerId = this.HandlerId;
            _o.Body = new FBS.Request.BodyUnion();
            _o.Body.Type = this.BodyType;
            switch(this.BodyType)
            {
                default:
                    break;
                case FBS.Request.Body.Worker_UpdateSettingsRequest:
                    _o.Body.Value = this.Body<FBS.Worker.UpdateSettingsRequest>().HasValue ? this.Body<FBS.Worker.UpdateSettingsRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Worker_CreateWebRtcServerRequest:
                    _o.Body.Value = this.Body<FBS.Worker.CreateWebRtcServerRequest>().HasValue ? this.Body<FBS.Worker.CreateWebRtcServerRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Worker_CloseWebRtcServerRequest:
                    _o.Body.Value = this.Body<FBS.Worker.CloseWebRtcServerRequest>().HasValue ? this.Body<FBS.Worker.CloseWebRtcServerRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Worker_CreateRouterRequest:
                    _o.Body.Value = this.Body<FBS.Worker.CreateRouterRequest>().HasValue ? this.Body<FBS.Worker.CreateRouterRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Worker_CloseRouterRequest:
                    _o.Body.Value = this.Body<FBS.Worker.CloseRouterRequest>().HasValue ? this.Body<FBS.Worker.CloseRouterRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Router_CreateWebRtcTransportRequest:
                    _o.Body.Value = this.Body<FBS.Router.CreateWebRtcTransportRequest>().HasValue ? this.Body<FBS.Router.CreateWebRtcTransportRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Router_CreatePlainTransportRequest:
                    _o.Body.Value = this.Body<FBS.Router.CreatePlainTransportRequest>().HasValue ? this.Body<FBS.Router.CreatePlainTransportRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Router_CreatePipeTransportRequest:
                    _o.Body.Value = this.Body<FBS.Router.CreatePipeTransportRequest>().HasValue ? this.Body<FBS.Router.CreatePipeTransportRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Router_CreateDirectTransportRequest:
                    _o.Body.Value = this.Body<FBS.Router.CreateDirectTransportRequest>().HasValue ? this.Body<FBS.Router.CreateDirectTransportRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Router_CreateActiveSpeakerObserverRequest:
                    _o.Body.Value = this.Body<FBS.Router.CreateActiveSpeakerObserverRequest>().HasValue ? this.Body<FBS.Router.CreateActiveSpeakerObserverRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Router_CreateAudioLevelObserverRequest:
                    _o.Body.Value = this.Body<FBS.Router.CreateAudioLevelObserverRequest>().HasValue ? this.Body<FBS.Router.CreateAudioLevelObserverRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Router_CloseTransportRequest:
                    _o.Body.Value = this.Body<FBS.Router.CloseTransportRequest>().HasValue ? this.Body<FBS.Router.CloseTransportRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Router_CloseRtpObserverRequest:
                    _o.Body.Value = this.Body<FBS.Router.CloseRtpObserverRequest>().HasValue ? this.Body<FBS.Router.CloseRtpObserverRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Transport_SetMaxIncomingBitrateRequest:
                    _o.Body.Value = this.Body<FBS.Transport.SetMaxIncomingBitrateRequest>().HasValue ? this.Body<FBS.Transport.SetMaxIncomingBitrateRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Transport_SetMaxOutgoingBitrateRequest:
                    _o.Body.Value = this.Body<FBS.Transport.SetMaxOutgoingBitrateRequest>().HasValue ? this.Body<FBS.Transport.SetMaxOutgoingBitrateRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Transport_SetMinOutgoingBitrateRequest:
                    _o.Body.Value = this.Body<FBS.Transport.SetMinOutgoingBitrateRequest>().HasValue ? this.Body<FBS.Transport.SetMinOutgoingBitrateRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Transport_ProduceRequest:
                    _o.Body.Value = this.Body<FBS.Transport.ProduceRequest>().HasValue ? this.Body<FBS.Transport.ProduceRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Transport_ConsumeRequest:
                    _o.Body.Value = this.Body<FBS.Transport.ConsumeRequest>().HasValue ? this.Body<FBS.Transport.ConsumeRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Transport_ProduceDataRequest:
                    _o.Body.Value = this.Body<FBS.Transport.ProduceDataRequest>().HasValue ? this.Body<FBS.Transport.ProduceDataRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Transport_ConsumeDataRequest:
                    _o.Body.Value = this.Body<FBS.Transport.ConsumeDataRequest>().HasValue ? this.Body<FBS.Transport.ConsumeDataRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Transport_EnableTraceEventRequest:
                    _o.Body.Value = this.Body<FBS.Transport.EnableTraceEventRequest>().HasValue ? this.Body<FBS.Transport.EnableTraceEventRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Transport_CloseProducerRequest:
                    _o.Body.Value = this.Body<FBS.Transport.CloseProducerRequest>().HasValue ? this.Body<FBS.Transport.CloseProducerRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Transport_CloseConsumerRequest:
                    _o.Body.Value = this.Body<FBS.Transport.CloseConsumerRequest>().HasValue ? this.Body<FBS.Transport.CloseConsumerRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Transport_CloseDataProducerRequest:
                    _o.Body.Value = this.Body<FBS.Transport.CloseDataProducerRequest>().HasValue ? this.Body<FBS.Transport.CloseDataProducerRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Transport_CloseDataConsumerRequest:
                    _o.Body.Value = this.Body<FBS.Transport.CloseDataConsumerRequest>().HasValue ? this.Body<FBS.Transport.CloseDataConsumerRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.PlainTransport_ConnectRequest:
                    _o.Body.Value = this.Body<FBS.PlainTransport.ConnectRequest>().HasValue ? this.Body<FBS.PlainTransport.ConnectRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.PipeTransport_ConnectRequest:
                    _o.Body.Value = this.Body<FBS.PipeTransport.ConnectRequest>().HasValue ? this.Body<FBS.PipeTransport.ConnectRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.WebRtcTransport_ConnectRequest:
                    _o.Body.Value = this.Body<FBS.WebRtcTransport.ConnectRequest>().HasValue ? this.Body<FBS.WebRtcTransport.ConnectRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Producer_EnableTraceEventRequest:
                    _o.Body.Value = this.Body<FBS.Producer.EnableTraceEventRequest>().HasValue ? this.Body<FBS.Producer.EnableTraceEventRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Consumer_SetPreferredLayersRequest:
                    _o.Body.Value = this.Body<FBS.Consumer.SetPreferredLayersRequest>().HasValue ? this.Body<FBS.Consumer.SetPreferredLayersRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Consumer_SetPriorityRequest:
                    _o.Body.Value = this.Body<FBS.Consumer.SetPriorityRequest>().HasValue ? this.Body<FBS.Consumer.SetPriorityRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.Consumer_EnableTraceEventRequest:
                    _o.Body.Value = this.Body<FBS.Consumer.EnableTraceEventRequest>().HasValue ? this.Body<FBS.Consumer.EnableTraceEventRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.DataConsumer_SetBufferedAmountLowThresholdRequest:
                    _o.Body.Value = this.Body<FBS.DataConsumer.SetBufferedAmountLowThresholdRequest>().HasValue ? this.Body<FBS.DataConsumer.SetBufferedAmountLowThresholdRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.DataConsumer_SendRequest:
                    _o.Body.Value = this.Body<FBS.DataConsumer.SendRequest>().HasValue ? this.Body<FBS.DataConsumer.SendRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.DataConsumer_SetSubchannelsRequest:
                    _o.Body.Value = this.Body<FBS.DataConsumer.SetSubchannelsRequest>().HasValue ? this.Body<FBS.DataConsumer.SetSubchannelsRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.DataConsumer_AddSubchannelRequest:
                    _o.Body.Value = this.Body<FBS.DataConsumer.AddSubchannelRequest>().HasValue ? this.Body<FBS.DataConsumer.AddSubchannelRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.DataConsumer_RemoveSubchannelRequest:
                    _o.Body.Value = this.Body<FBS.DataConsumer.RemoveSubchannelRequest>().HasValue ? this.Body<FBS.DataConsumer.RemoveSubchannelRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.RtpObserver_AddProducerRequest:
                    _o.Body.Value = this.Body<FBS.RtpObserver.AddProducerRequest>().HasValue ? this.Body<FBS.RtpObserver.AddProducerRequest>().Value.UnPack() : null;
                    break;
                case FBS.Request.Body.RtpObserver_RemoveProducerRequest:
                    _o.Body.Value = this.Body<FBS.RtpObserver.RemoveProducerRequest>().HasValue ? this.Body<FBS.RtpObserver.RemoveProducerRequest>().Value.UnPack() : null;
                    break;
            }
        }
        public static Offset<FBS.Request.Request> Pack(FlatBufferBuilder builder, RequestT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Request.Request>);
            var _handler_id = _o.HandlerId == null ? default(StringOffset) : builder.CreateString(_o.HandlerId);
            var _body_type = _o.Body == null ? FBS.Request.Body.NONE : _o.Body.Type;
            var _body = _o.Body == null ? 0 : FBS.Request.BodyUnion.Pack(builder, _o.Body);
            return CreateRequest(
              builder,
              _o.Id,
              _o.Method,
              _handler_id,
              _body_type,
              _body);
        }
    }

    public class RequestT
    {
        public uint Id { get; set; }

        public FBS.Request.Method Method { get; set; }

        public string HandlerId { get; set; }

        private FBS.Request.Body BodyType
        {
            get
            {
                return this.Body != null ? this.Body.Type : FBS.Request.Body.NONE;
            }
            set
            {
                this.Body = new FBS.Request.BodyUnion();
                this.Body.Type = value;
            }
        }

        public FBS.Request.BodyUnion Body { get; set; }

        public RequestT()
        {
            this.Id = 0;
            this.Method = FBS.Request.Method.WORKER_CLOSE;
            this.HandlerId = null;
            this.Body = null;
        }

        public static RequestT DeserializeFromBinary(byte[] fbBuffer)
        {
            return Request.GetRootAsRequest(new ByteBuffer(fbBuffer)).UnPack();
        }
        public byte[] SerializeToBinary()
        {
            var fbb = new FlatBufferBuilder(0x10000);
            Request.FinishRequestBuffer(fbb, Request.Pack(fbb, this));
            return fbb.DataBuffer.ToSizedArray();
        }
    }


    static public class RequestVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyField(tablePos, 4 /*Id*/, 4 /*uint*/, 4, false)
              && verifier.VerifyField(tablePos, 6 /*Method*/, 1 /*FBS.Request.Method*/, 1, false)
              && verifier.VerifyString(tablePos, 8 /*HandlerId*/, true)
              && verifier.VerifyField(tablePos, 10 /*BodyType*/, 1 /*FBS.Request.Body*/, 1, false)
              && verifier.VerifyUnion(tablePos, 10, 12 /*Body*/, FBS.Request.BodyVerify.Verify, false)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
