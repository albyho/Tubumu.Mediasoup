// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FBS.Request
{

    public enum Body : byte
    {
        NONE = 0,
        Worker_UpdateSettingsRequest = 1,
        Worker_CreateWebRtcServerRequest = 2,
        Worker_CloseWebRtcServerRequest = 3,
        Worker_CreateRouterRequest = 4,
        Worker_CloseRouterRequest = 5,
        Router_CreateWebRtcTransportRequest = 6,
        Router_CreatePlainTransportRequest = 7,
        Router_CreatePipeTransportRequest = 8,
        Router_CreateDirectTransportRequest = 9,
        Router_CreateActiveSpeakerObserverRequest = 10,
        Router_CreateAudioLevelObserverRequest = 11,
        Router_CloseTransportRequest = 12,
        Router_CloseRtpObserverRequest = 13,
        Transport_SetMaxIncomingBitrateRequest = 14,
        Transport_SetMaxOutgoingBitrateRequest = 15,
        Transport_SetMinOutgoingBitrateRequest = 16,
        Transport_ProduceRequest = 17,
        Transport_ConsumeRequest = 18,
        Transport_ProduceDataRequest = 19,
        Transport_ConsumeDataRequest = 20,
        Transport_EnableTraceEventRequest = 21,
        Transport_CloseProducerRequest = 22,
        Transport_CloseConsumerRequest = 23,
        Transport_CloseDataProducerRequest = 24,
        Transport_CloseDataConsumerRequest = 25,
        PlainTransport_ConnectRequest = 26,
        PipeTransport_ConnectRequest = 27,
        WebRtcTransport_ConnectRequest = 28,
        Producer_EnableTraceEventRequest = 29,
        Consumer_SetPreferredLayersRequest = 30,
        Consumer_SetPriorityRequest = 31,
        Consumer_EnableTraceEventRequest = 32,
        DataConsumer_SetBufferedAmountLowThresholdRequest = 33,
        DataConsumer_SendRequest = 34,
        DataConsumer_SetSubchannelsRequest = 35,
        DataConsumer_AddSubchannelRequest = 36,
        DataConsumer_RemoveSubchannelRequest = 37,
        RtpObserver_AddProducerRequest = 38,
        RtpObserver_RemoveProducerRequest = 39,
    };

    public class BodyUnion
    {
        public Body Type { get; set; }
        public object Value { get; set; }

        public BodyUnion()
        {
            this.Type = Body.NONE;
            this.Value = null;
        }

        public T As<T>() where T : class { return this.Value as T; }
        public FBS.Worker.UpdateSettingsRequestT AsWorker_UpdateSettingsRequest() { return this.As<FBS.Worker.UpdateSettingsRequestT>(); }
        public static BodyUnion FromWorker_UpdateSettingsRequest(FBS.Worker.UpdateSettingsRequestT _worker_updatesettingsrequest) { return new BodyUnion { Type = Body.Worker_UpdateSettingsRequest, Value = _worker_updatesettingsrequest }; }
        public FBS.Worker.CreateWebRtcServerRequestT AsWorker_CreateWebRtcServerRequest() { return this.As<FBS.Worker.CreateWebRtcServerRequestT>(); }
        public static BodyUnion FromWorker_CreateWebRtcServerRequest(FBS.Worker.CreateWebRtcServerRequestT _worker_createwebrtcserverrequest) { return new BodyUnion { Type = Body.Worker_CreateWebRtcServerRequest, Value = _worker_createwebrtcserverrequest }; }
        public FBS.Worker.CloseWebRtcServerRequestT AsWorker_CloseWebRtcServerRequest() { return this.As<FBS.Worker.CloseWebRtcServerRequestT>(); }
        public static BodyUnion FromWorker_CloseWebRtcServerRequest(FBS.Worker.CloseWebRtcServerRequestT _worker_closewebrtcserverrequest) { return new BodyUnion { Type = Body.Worker_CloseWebRtcServerRequest, Value = _worker_closewebrtcserverrequest }; }
        public FBS.Worker.CreateRouterRequestT AsWorker_CreateRouterRequest() { return this.As<FBS.Worker.CreateRouterRequestT>(); }
        public static BodyUnion FromWorker_CreateRouterRequest(FBS.Worker.CreateRouterRequestT _worker_createrouterrequest) { return new BodyUnion { Type = Body.Worker_CreateRouterRequest, Value = _worker_createrouterrequest }; }
        public FBS.Worker.CloseRouterRequestT AsWorker_CloseRouterRequest() { return this.As<FBS.Worker.CloseRouterRequestT>(); }
        public static BodyUnion FromWorker_CloseRouterRequest(FBS.Worker.CloseRouterRequestT _worker_closerouterrequest) { return new BodyUnion { Type = Body.Worker_CloseRouterRequest, Value = _worker_closerouterrequest }; }
        public FBS.Router.CreateWebRtcTransportRequestT AsRouter_CreateWebRtcTransportRequest() { return this.As<FBS.Router.CreateWebRtcTransportRequestT>(); }
        public static BodyUnion FromRouter_CreateWebRtcTransportRequest(FBS.Router.CreateWebRtcTransportRequestT _router_createwebrtctransportrequest) { return new BodyUnion { Type = Body.Router_CreateWebRtcTransportRequest, Value = _router_createwebrtctransportrequest }; }
        public FBS.Router.CreatePlainTransportRequestT AsRouter_CreatePlainTransportRequest() { return this.As<FBS.Router.CreatePlainTransportRequestT>(); }
        public static BodyUnion FromRouter_CreatePlainTransportRequest(FBS.Router.CreatePlainTransportRequestT _router_createplaintransportrequest) { return new BodyUnion { Type = Body.Router_CreatePlainTransportRequest, Value = _router_createplaintransportrequest }; }
        public FBS.Router.CreatePipeTransportRequestT AsRouter_CreatePipeTransportRequest() { return this.As<FBS.Router.CreatePipeTransportRequestT>(); }
        public static BodyUnion FromRouter_CreatePipeTransportRequest(FBS.Router.CreatePipeTransportRequestT _router_createpipetransportrequest) { return new BodyUnion { Type = Body.Router_CreatePipeTransportRequest, Value = _router_createpipetransportrequest }; }
        public FBS.Router.CreateDirectTransportRequestT AsRouter_CreateDirectTransportRequest() { return this.As<FBS.Router.CreateDirectTransportRequestT>(); }
        public static BodyUnion FromRouter_CreateDirectTransportRequest(FBS.Router.CreateDirectTransportRequestT _router_createdirecttransportrequest) { return new BodyUnion { Type = Body.Router_CreateDirectTransportRequest, Value = _router_createdirecttransportrequest }; }
        public FBS.Router.CreateActiveSpeakerObserverRequestT AsRouter_CreateActiveSpeakerObserverRequest() { return this.As<FBS.Router.CreateActiveSpeakerObserverRequestT>(); }
        public static BodyUnion FromRouter_CreateActiveSpeakerObserverRequest(FBS.Router.CreateActiveSpeakerObserverRequestT _router_createactivespeakerobserverrequest) { return new BodyUnion { Type = Body.Router_CreateActiveSpeakerObserverRequest, Value = _router_createactivespeakerobserverrequest }; }
        public FBS.Router.CreateAudioLevelObserverRequestT AsRouter_CreateAudioLevelObserverRequest() { return this.As<FBS.Router.CreateAudioLevelObserverRequestT>(); }
        public static BodyUnion FromRouter_CreateAudioLevelObserverRequest(FBS.Router.CreateAudioLevelObserverRequestT _router_createaudiolevelobserverrequest) { return new BodyUnion { Type = Body.Router_CreateAudioLevelObserverRequest, Value = _router_createaudiolevelobserverrequest }; }
        public FBS.Router.CloseTransportRequestT AsRouter_CloseTransportRequest() { return this.As<FBS.Router.CloseTransportRequestT>(); }
        public static BodyUnion FromRouter_CloseTransportRequest(FBS.Router.CloseTransportRequestT _router_closetransportrequest) { return new BodyUnion { Type = Body.Router_CloseTransportRequest, Value = _router_closetransportrequest }; }
        public FBS.Router.CloseRtpObserverRequestT AsRouter_CloseRtpObserverRequest() { return this.As<FBS.Router.CloseRtpObserverRequestT>(); }
        public static BodyUnion FromRouter_CloseRtpObserverRequest(FBS.Router.CloseRtpObserverRequestT _router_closertpobserverrequest) { return new BodyUnion { Type = Body.Router_CloseRtpObserverRequest, Value = _router_closertpobserverrequest }; }
        public FBS.Transport.SetMaxIncomingBitrateRequestT AsTransport_SetMaxIncomingBitrateRequest() { return this.As<FBS.Transport.SetMaxIncomingBitrateRequestT>(); }
        public static BodyUnion FromTransport_SetMaxIncomingBitrateRequest(FBS.Transport.SetMaxIncomingBitrateRequestT _transport_setmaxincomingbitraterequest) { return new BodyUnion { Type = Body.Transport_SetMaxIncomingBitrateRequest, Value = _transport_setmaxincomingbitraterequest }; }
        public FBS.Transport.SetMaxOutgoingBitrateRequestT AsTransport_SetMaxOutgoingBitrateRequest() { return this.As<FBS.Transport.SetMaxOutgoingBitrateRequestT>(); }
        public static BodyUnion FromTransport_SetMaxOutgoingBitrateRequest(FBS.Transport.SetMaxOutgoingBitrateRequestT _transport_setmaxoutgoingbitraterequest) { return new BodyUnion { Type = Body.Transport_SetMaxOutgoingBitrateRequest, Value = _transport_setmaxoutgoingbitraterequest }; }
        public FBS.Transport.SetMinOutgoingBitrateRequestT AsTransport_SetMinOutgoingBitrateRequest() { return this.As<FBS.Transport.SetMinOutgoingBitrateRequestT>(); }
        public static BodyUnion FromTransport_SetMinOutgoingBitrateRequest(FBS.Transport.SetMinOutgoingBitrateRequestT _transport_setminoutgoingbitraterequest) { return new BodyUnion { Type = Body.Transport_SetMinOutgoingBitrateRequest, Value = _transport_setminoutgoingbitraterequest }; }
        public FBS.Transport.ProduceRequestT AsTransport_ProduceRequest() { return this.As<FBS.Transport.ProduceRequestT>(); }
        public static BodyUnion FromTransport_ProduceRequest(FBS.Transport.ProduceRequestT _transport_producerequest) { return new BodyUnion { Type = Body.Transport_ProduceRequest, Value = _transport_producerequest }; }
        public FBS.Transport.ConsumeRequestT AsTransport_ConsumeRequest() { return this.As<FBS.Transport.ConsumeRequestT>(); }
        public static BodyUnion FromTransport_ConsumeRequest(FBS.Transport.ConsumeRequestT _transport_consumerequest) { return new BodyUnion { Type = Body.Transport_ConsumeRequest, Value = _transport_consumerequest }; }
        public FBS.Transport.ProduceDataRequestT AsTransport_ProduceDataRequest() { return this.As<FBS.Transport.ProduceDataRequestT>(); }
        public static BodyUnion FromTransport_ProduceDataRequest(FBS.Transport.ProduceDataRequestT _transport_producedatarequest) { return new BodyUnion { Type = Body.Transport_ProduceDataRequest, Value = _transport_producedatarequest }; }
        public FBS.Transport.ConsumeDataRequestT AsTransport_ConsumeDataRequest() { return this.As<FBS.Transport.ConsumeDataRequestT>(); }
        public static BodyUnion FromTransport_ConsumeDataRequest(FBS.Transport.ConsumeDataRequestT _transport_consumedatarequest) { return new BodyUnion { Type = Body.Transport_ConsumeDataRequest, Value = _transport_consumedatarequest }; }
        public FBS.Transport.EnableTraceEventRequestT AsTransport_EnableTraceEventRequest() { return this.As<FBS.Transport.EnableTraceEventRequestT>(); }
        public static BodyUnion FromTransport_EnableTraceEventRequest(FBS.Transport.EnableTraceEventRequestT _transport_enabletraceeventrequest) { return new BodyUnion { Type = Body.Transport_EnableTraceEventRequest, Value = _transport_enabletraceeventrequest }; }
        public FBS.Transport.CloseProducerRequestT AsTransport_CloseProducerRequest() { return this.As<FBS.Transport.CloseProducerRequestT>(); }
        public static BodyUnion FromTransport_CloseProducerRequest(FBS.Transport.CloseProducerRequestT _transport_closeproducerrequest) { return new BodyUnion { Type = Body.Transport_CloseProducerRequest, Value = _transport_closeproducerrequest }; }
        public FBS.Transport.CloseConsumerRequestT AsTransport_CloseConsumerRequest() { return this.As<FBS.Transport.CloseConsumerRequestT>(); }
        public static BodyUnion FromTransport_CloseConsumerRequest(FBS.Transport.CloseConsumerRequestT _transport_closeconsumerrequest) { return new BodyUnion { Type = Body.Transport_CloseConsumerRequest, Value = _transport_closeconsumerrequest }; }
        public FBS.Transport.CloseDataProducerRequestT AsTransport_CloseDataProducerRequest() { return this.As<FBS.Transport.CloseDataProducerRequestT>(); }
        public static BodyUnion FromTransport_CloseDataProducerRequest(FBS.Transport.CloseDataProducerRequestT _transport_closedataproducerrequest) { return new BodyUnion { Type = Body.Transport_CloseDataProducerRequest, Value = _transport_closedataproducerrequest }; }
        public FBS.Transport.CloseDataConsumerRequestT AsTransport_CloseDataConsumerRequest() { return this.As<FBS.Transport.CloseDataConsumerRequestT>(); }
        public static BodyUnion FromTransport_CloseDataConsumerRequest(FBS.Transport.CloseDataConsumerRequestT _transport_closedataconsumerrequest) { return new BodyUnion { Type = Body.Transport_CloseDataConsumerRequest, Value = _transport_closedataconsumerrequest }; }
        public FBS.PlainTransport.ConnectRequestT AsPlainTransport_ConnectRequest() { return this.As<FBS.PlainTransport.ConnectRequestT>(); }
        public static BodyUnion FromPlainTransport_ConnectRequest(FBS.PlainTransport.ConnectRequestT _plaintransport_connectrequest) { return new BodyUnion { Type = Body.PlainTransport_ConnectRequest, Value = _plaintransport_connectrequest }; }
        public FBS.PipeTransport.ConnectRequestT AsPipeTransport_ConnectRequest() { return this.As<FBS.PipeTransport.ConnectRequestT>(); }
        public static BodyUnion FromPipeTransport_ConnectRequest(FBS.PipeTransport.ConnectRequestT _pipetransport_connectrequest) { return new BodyUnion { Type = Body.PipeTransport_ConnectRequest, Value = _pipetransport_connectrequest }; }
        public FBS.WebRtcTransport.ConnectRequestT AsWebRtcTransport_ConnectRequest() { return this.As<FBS.WebRtcTransport.ConnectRequestT>(); }
        public static BodyUnion FromWebRtcTransport_ConnectRequest(FBS.WebRtcTransport.ConnectRequestT _webrtctransport_connectrequest) { return new BodyUnion { Type = Body.WebRtcTransport_ConnectRequest, Value = _webrtctransport_connectrequest }; }
        public FBS.Producer.EnableTraceEventRequestT AsProducer_EnableTraceEventRequest() { return this.As<FBS.Producer.EnableTraceEventRequestT>(); }
        public static BodyUnion FromProducer_EnableTraceEventRequest(FBS.Producer.EnableTraceEventRequestT _producer_enabletraceeventrequest) { return new BodyUnion { Type = Body.Producer_EnableTraceEventRequest, Value = _producer_enabletraceeventrequest }; }
        public FBS.Consumer.SetPreferredLayersRequestT AsConsumer_SetPreferredLayersRequest() { return this.As<FBS.Consumer.SetPreferredLayersRequestT>(); }
        public static BodyUnion FromConsumer_SetPreferredLayersRequest(FBS.Consumer.SetPreferredLayersRequestT _consumer_setpreferredlayersrequest) { return new BodyUnion { Type = Body.Consumer_SetPreferredLayersRequest, Value = _consumer_setpreferredlayersrequest }; }
        public FBS.Consumer.SetPriorityRequestT AsConsumer_SetPriorityRequest() { return this.As<FBS.Consumer.SetPriorityRequestT>(); }
        public static BodyUnion FromConsumer_SetPriorityRequest(FBS.Consumer.SetPriorityRequestT _consumer_setpriorityrequest) { return new BodyUnion { Type = Body.Consumer_SetPriorityRequest, Value = _consumer_setpriorityrequest }; }
        public FBS.Consumer.EnableTraceEventRequestT AsConsumer_EnableTraceEventRequest() { return this.As<FBS.Consumer.EnableTraceEventRequestT>(); }
        public static BodyUnion FromConsumer_EnableTraceEventRequest(FBS.Consumer.EnableTraceEventRequestT _consumer_enabletraceeventrequest) { return new BodyUnion { Type = Body.Consumer_EnableTraceEventRequest, Value = _consumer_enabletraceeventrequest }; }
        public FBS.DataConsumer.SetBufferedAmountLowThresholdRequestT AsDataConsumer_SetBufferedAmountLowThresholdRequest() { return this.As<FBS.DataConsumer.SetBufferedAmountLowThresholdRequestT>(); }
        public static BodyUnion FromDataConsumer_SetBufferedAmountLowThresholdRequest(FBS.DataConsumer.SetBufferedAmountLowThresholdRequestT _dataconsumer_setbufferedamountlowthresholdrequest) { return new BodyUnion { Type = Body.DataConsumer_SetBufferedAmountLowThresholdRequest, Value = _dataconsumer_setbufferedamountlowthresholdrequest }; }
        public FBS.DataConsumer.SendRequestT AsDataConsumer_SendRequest() { return this.As<FBS.DataConsumer.SendRequestT>(); }
        public static BodyUnion FromDataConsumer_SendRequest(FBS.DataConsumer.SendRequestT _dataconsumer_sendrequest) { return new BodyUnion { Type = Body.DataConsumer_SendRequest, Value = _dataconsumer_sendrequest }; }
        public FBS.DataConsumer.SetSubchannelsRequestT AsDataConsumer_SetSubchannelsRequest() { return this.As<FBS.DataConsumer.SetSubchannelsRequestT>(); }
        public static BodyUnion FromDataConsumer_SetSubchannelsRequest(FBS.DataConsumer.SetSubchannelsRequestT _dataconsumer_setsubchannelsrequest) { return new BodyUnion { Type = Body.DataConsumer_SetSubchannelsRequest, Value = _dataconsumer_setsubchannelsrequest }; }
        public FBS.DataConsumer.AddSubchannelRequestT AsDataConsumer_AddSubchannelRequest() { return this.As<FBS.DataConsumer.AddSubchannelRequestT>(); }
        public static BodyUnion FromDataConsumer_AddSubchannelRequest(FBS.DataConsumer.AddSubchannelRequestT _dataconsumer_addsubchannelrequest) { return new BodyUnion { Type = Body.DataConsumer_AddSubchannelRequest, Value = _dataconsumer_addsubchannelrequest }; }
        public FBS.DataConsumer.RemoveSubchannelRequestT AsDataConsumer_RemoveSubchannelRequest() { return this.As<FBS.DataConsumer.RemoveSubchannelRequestT>(); }
        public static BodyUnion FromDataConsumer_RemoveSubchannelRequest(FBS.DataConsumer.RemoveSubchannelRequestT _dataconsumer_removesubchannelrequest) { return new BodyUnion { Type = Body.DataConsumer_RemoveSubchannelRequest, Value = _dataconsumer_removesubchannelrequest }; }
        public FBS.RtpObserver.AddProducerRequestT AsRtpObserver_AddProducerRequest() { return this.As<FBS.RtpObserver.AddProducerRequestT>(); }
        public static BodyUnion FromRtpObserver_AddProducerRequest(FBS.RtpObserver.AddProducerRequestT _rtpobserver_addproducerrequest) { return new BodyUnion { Type = Body.RtpObserver_AddProducerRequest, Value = _rtpobserver_addproducerrequest }; }
        public FBS.RtpObserver.RemoveProducerRequestT AsRtpObserver_RemoveProducerRequest() { return this.As<FBS.RtpObserver.RemoveProducerRequestT>(); }
        public static BodyUnion FromRtpObserver_RemoveProducerRequest(FBS.RtpObserver.RemoveProducerRequestT _rtpobserver_removeproducerrequest) { return new BodyUnion { Type = Body.RtpObserver_RemoveProducerRequest, Value = _rtpobserver_removeproducerrequest }; }

        public static int Pack(Google.FlatBuffers.FlatBufferBuilder builder, BodyUnion _o)
        {
            switch(_o.Type)
            {
                default:
                    return 0;
                case Body.Worker_UpdateSettingsRequest:
                    return FBS.Worker.UpdateSettingsRequest.Pack(builder, _o.AsWorker_UpdateSettingsRequest()).Value;
                case Body.Worker_CreateWebRtcServerRequest:
                    return FBS.Worker.CreateWebRtcServerRequest.Pack(builder, _o.AsWorker_CreateWebRtcServerRequest()).Value;
                case Body.Worker_CloseWebRtcServerRequest:
                    return FBS.Worker.CloseWebRtcServerRequest.Pack(builder, _o.AsWorker_CloseWebRtcServerRequest()).Value;
                case Body.Worker_CreateRouterRequest:
                    return FBS.Worker.CreateRouterRequest.Pack(builder, _o.AsWorker_CreateRouterRequest()).Value;
                case Body.Worker_CloseRouterRequest:
                    return FBS.Worker.CloseRouterRequest.Pack(builder, _o.AsWorker_CloseRouterRequest()).Value;
                case Body.Router_CreateWebRtcTransportRequest:
                    return FBS.Router.CreateWebRtcTransportRequest.Pack(builder, _o.AsRouter_CreateWebRtcTransportRequest()).Value;
                case Body.Router_CreatePlainTransportRequest:
                    return FBS.Router.CreatePlainTransportRequest.Pack(builder, _o.AsRouter_CreatePlainTransportRequest()).Value;
                case Body.Router_CreatePipeTransportRequest:
                    return FBS.Router.CreatePipeTransportRequest.Pack(builder, _o.AsRouter_CreatePipeTransportRequest()).Value;
                case Body.Router_CreateDirectTransportRequest:
                    return FBS.Router.CreateDirectTransportRequest.Pack(builder, _o.AsRouter_CreateDirectTransportRequest()).Value;
                case Body.Router_CreateActiveSpeakerObserverRequest:
                    return FBS.Router.CreateActiveSpeakerObserverRequest.Pack(builder, _o.AsRouter_CreateActiveSpeakerObserverRequest()).Value;
                case Body.Router_CreateAudioLevelObserverRequest:
                    return FBS.Router.CreateAudioLevelObserverRequest.Pack(builder, _o.AsRouter_CreateAudioLevelObserverRequest()).Value;
                case Body.Router_CloseTransportRequest:
                    return FBS.Router.CloseTransportRequest.Pack(builder, _o.AsRouter_CloseTransportRequest()).Value;
                case Body.Router_CloseRtpObserverRequest:
                    return FBS.Router.CloseRtpObserverRequest.Pack(builder, _o.AsRouter_CloseRtpObserverRequest()).Value;
                case Body.Transport_SetMaxIncomingBitrateRequest:
                    return FBS.Transport.SetMaxIncomingBitrateRequest.Pack(builder, _o.AsTransport_SetMaxIncomingBitrateRequest()).Value;
                case Body.Transport_SetMaxOutgoingBitrateRequest:
                    return FBS.Transport.SetMaxOutgoingBitrateRequest.Pack(builder, _o.AsTransport_SetMaxOutgoingBitrateRequest()).Value;
                case Body.Transport_SetMinOutgoingBitrateRequest:
                    return FBS.Transport.SetMinOutgoingBitrateRequest.Pack(builder, _o.AsTransport_SetMinOutgoingBitrateRequest()).Value;
                case Body.Transport_ProduceRequest:
                    return FBS.Transport.ProduceRequest.Pack(builder, _o.AsTransport_ProduceRequest()).Value;
                case Body.Transport_ConsumeRequest:
                    return FBS.Transport.ConsumeRequest.Pack(builder, _o.AsTransport_ConsumeRequest()).Value;
                case Body.Transport_ProduceDataRequest:
                    return FBS.Transport.ProduceDataRequest.Pack(builder, _o.AsTransport_ProduceDataRequest()).Value;
                case Body.Transport_ConsumeDataRequest:
                    return FBS.Transport.ConsumeDataRequest.Pack(builder, _o.AsTransport_ConsumeDataRequest()).Value;
                case Body.Transport_EnableTraceEventRequest:
                    return FBS.Transport.EnableTraceEventRequest.Pack(builder, _o.AsTransport_EnableTraceEventRequest()).Value;
                case Body.Transport_CloseProducerRequest:
                    return FBS.Transport.CloseProducerRequest.Pack(builder, _o.AsTransport_CloseProducerRequest()).Value;
                case Body.Transport_CloseConsumerRequest:
                    return FBS.Transport.CloseConsumerRequest.Pack(builder, _o.AsTransport_CloseConsumerRequest()).Value;
                case Body.Transport_CloseDataProducerRequest:
                    return FBS.Transport.CloseDataProducerRequest.Pack(builder, _o.AsTransport_CloseDataProducerRequest()).Value;
                case Body.Transport_CloseDataConsumerRequest:
                    return FBS.Transport.CloseDataConsumerRequest.Pack(builder, _o.AsTransport_CloseDataConsumerRequest()).Value;
                case Body.PlainTransport_ConnectRequest:
                    return FBS.PlainTransport.ConnectRequest.Pack(builder, _o.AsPlainTransport_ConnectRequest()).Value;
                case Body.PipeTransport_ConnectRequest:
                    return FBS.PipeTransport.ConnectRequest.Pack(builder, _o.AsPipeTransport_ConnectRequest()).Value;
                case Body.WebRtcTransport_ConnectRequest:
                    return FBS.WebRtcTransport.ConnectRequest.Pack(builder, _o.AsWebRtcTransport_ConnectRequest()).Value;
                case Body.Producer_EnableTraceEventRequest:
                    return FBS.Producer.EnableTraceEventRequest.Pack(builder, _o.AsProducer_EnableTraceEventRequest()).Value;
                case Body.Consumer_SetPreferredLayersRequest:
                    return FBS.Consumer.SetPreferredLayersRequest.Pack(builder, _o.AsConsumer_SetPreferredLayersRequest()).Value;
                case Body.Consumer_SetPriorityRequest:
                    return FBS.Consumer.SetPriorityRequest.Pack(builder, _o.AsConsumer_SetPriorityRequest()).Value;
                case Body.Consumer_EnableTraceEventRequest:
                    return FBS.Consumer.EnableTraceEventRequest.Pack(builder, _o.AsConsumer_EnableTraceEventRequest()).Value;
                case Body.DataConsumer_SetBufferedAmountLowThresholdRequest:
                    return FBS.DataConsumer.SetBufferedAmountLowThresholdRequest.Pack(builder, _o.AsDataConsumer_SetBufferedAmountLowThresholdRequest()).Value;
                case Body.DataConsumer_SendRequest:
                    return FBS.DataConsumer.SendRequest.Pack(builder, _o.AsDataConsumer_SendRequest()).Value;
                case Body.DataConsumer_SetSubchannelsRequest:
                    return FBS.DataConsumer.SetSubchannelsRequest.Pack(builder, _o.AsDataConsumer_SetSubchannelsRequest()).Value;
                case Body.DataConsumer_AddSubchannelRequest:
                    return FBS.DataConsumer.AddSubchannelRequest.Pack(builder, _o.AsDataConsumer_AddSubchannelRequest()).Value;
                case Body.DataConsumer_RemoveSubchannelRequest:
                    return FBS.DataConsumer.RemoveSubchannelRequest.Pack(builder, _o.AsDataConsumer_RemoveSubchannelRequest()).Value;
                case Body.RtpObserver_AddProducerRequest:
                    return FBS.RtpObserver.AddProducerRequest.Pack(builder, _o.AsRtpObserver_AddProducerRequest()).Value;
                case Body.RtpObserver_RemoveProducerRequest:
                    return FBS.RtpObserver.RemoveProducerRequest.Pack(builder, _o.AsRtpObserver_RemoveProducerRequest()).Value;
            }
        }
    }



    static public class BodyVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, byte typeId, uint tablePos)
        {
            bool result = true;
            switch((Body)typeId)
            {
                case Body.Worker_UpdateSettingsRequest:
                    result = FBS.Worker.UpdateSettingsRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Worker_CreateWebRtcServerRequest:
                    result = FBS.Worker.CreateWebRtcServerRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Worker_CloseWebRtcServerRequest:
                    result = FBS.Worker.CloseWebRtcServerRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Worker_CreateRouterRequest:
                    result = FBS.Worker.CreateRouterRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Worker_CloseRouterRequest:
                    result = FBS.Worker.CloseRouterRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Router_CreateWebRtcTransportRequest:
                    result = FBS.Router.CreateWebRtcTransportRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Router_CreatePlainTransportRequest:
                    result = FBS.Router.CreatePlainTransportRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Router_CreatePipeTransportRequest:
                    result = FBS.Router.CreatePipeTransportRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Router_CreateDirectTransportRequest:
                    result = FBS.Router.CreateDirectTransportRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Router_CreateActiveSpeakerObserverRequest:
                    result = FBS.Router.CreateActiveSpeakerObserverRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Router_CreateAudioLevelObserverRequest:
                    result = FBS.Router.CreateAudioLevelObserverRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Router_CloseTransportRequest:
                    result = FBS.Router.CloseTransportRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Router_CloseRtpObserverRequest:
                    result = FBS.Router.CloseRtpObserverRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Transport_SetMaxIncomingBitrateRequest:
                    result = FBS.Transport.SetMaxIncomingBitrateRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Transport_SetMaxOutgoingBitrateRequest:
                    result = FBS.Transport.SetMaxOutgoingBitrateRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Transport_SetMinOutgoingBitrateRequest:
                    result = FBS.Transport.SetMinOutgoingBitrateRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Transport_ProduceRequest:
                    result = FBS.Transport.ProduceRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Transport_ConsumeRequest:
                    result = FBS.Transport.ConsumeRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Transport_ProduceDataRequest:
                    result = FBS.Transport.ProduceDataRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Transport_ConsumeDataRequest:
                    result = FBS.Transport.ConsumeDataRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Transport_EnableTraceEventRequest:
                    result = FBS.Transport.EnableTraceEventRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Transport_CloseProducerRequest:
                    result = FBS.Transport.CloseProducerRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Transport_CloseConsumerRequest:
                    result = FBS.Transport.CloseConsumerRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Transport_CloseDataProducerRequest:
                    result = FBS.Transport.CloseDataProducerRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Transport_CloseDataConsumerRequest:
                    result = FBS.Transport.CloseDataConsumerRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.PlainTransport_ConnectRequest:
                    result = FBS.PlainTransport.ConnectRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.PipeTransport_ConnectRequest:
                    result = FBS.PipeTransport.ConnectRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.WebRtcTransport_ConnectRequest:
                    result = FBS.WebRtcTransport.ConnectRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Producer_EnableTraceEventRequest:
                    result = FBS.Producer.EnableTraceEventRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Consumer_SetPreferredLayersRequest:
                    result = FBS.Consumer.SetPreferredLayersRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Consumer_SetPriorityRequest:
                    result = FBS.Consumer.SetPriorityRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Consumer_EnableTraceEventRequest:
                    result = FBS.Consumer.EnableTraceEventRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.DataConsumer_SetBufferedAmountLowThresholdRequest:
                    result = FBS.DataConsumer.SetBufferedAmountLowThresholdRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.DataConsumer_SendRequest:
                    result = FBS.DataConsumer.SendRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.DataConsumer_SetSubchannelsRequest:
                    result = FBS.DataConsumer.SetSubchannelsRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.DataConsumer_AddSubchannelRequest:
                    result = FBS.DataConsumer.AddSubchannelRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.DataConsumer_RemoveSubchannelRequest:
                    result = FBS.DataConsumer.RemoveSubchannelRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.RtpObserver_AddProducerRequest:
                    result = FBS.RtpObserver.AddProducerRequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.RtpObserver_RemoveProducerRequest:
                    result = FBS.RtpObserver.RemoveProducerRequestVerify.Verify(verifier, tablePos);
                    break;
                default:
                    result = true;
                    break;
            }
            return result;
        }
    }


}
