// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using System.Text.Json.Serialization;

namespace FBS.Worker
{

    using global::System;
    using global::System.Collections.Generic;
    using global::Google.FlatBuffers;

    public struct ChannelMessageHandlers : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
        public static ChannelMessageHandlers GetRootAsChannelMessageHandlers(ByteBuffer _bb) { return GetRootAsChannelMessageHandlers(_bb, new ChannelMessageHandlers()); }
        public static ChannelMessageHandlers GetRootAsChannelMessageHandlers(ByteBuffer _bb, ChannelMessageHandlers obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public ChannelMessageHandlers __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public string ChannelRequestHandlers(int j) { int o = __p.__offset(4); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
        public int ChannelRequestHandlersLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }
        public string ChannelNotificationHandlers(int j) { int o = __p.__offset(6); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
        public int ChannelNotificationHandlersLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }

        public static Offset<FBS.Worker.ChannelMessageHandlers> CreateChannelMessageHandlers(FlatBufferBuilder builder,
            VectorOffset channel_request_handlersOffset = default(VectorOffset),
            VectorOffset channel_notification_handlersOffset = default(VectorOffset))
        {
            builder.StartTable(2);
            ChannelMessageHandlers.AddChannelNotificationHandlers(builder, channel_notification_handlersOffset);
            ChannelMessageHandlers.AddChannelRequestHandlers(builder, channel_request_handlersOffset);
            return ChannelMessageHandlers.EndChannelMessageHandlers(builder);
        }

        public static void StartChannelMessageHandlers(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddChannelRequestHandlers(FlatBufferBuilder builder, VectorOffset channelRequestHandlersOffset) { builder.AddOffset(0, channelRequestHandlersOffset.Value, 0); }
        public static VectorOffset CreateChannelRequestHandlersVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateChannelRequestHandlersVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateChannelRequestHandlersVectorBlock(FlatBufferBuilder builder, ArraySegment<StringOffset> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateChannelRequestHandlersVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<StringOffset>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartChannelRequestHandlersVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static void AddChannelNotificationHandlers(FlatBufferBuilder builder, VectorOffset channelNotificationHandlersOffset) { builder.AddOffset(1, channelNotificationHandlersOffset.Value, 0); }
        public static VectorOffset CreateChannelNotificationHandlersVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for(int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
        public static VectorOffset CreateChannelNotificationHandlersVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateChannelNotificationHandlersVectorBlock(FlatBufferBuilder builder, ArraySegment<StringOffset> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
        public static VectorOffset CreateChannelNotificationHandlersVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<StringOffset>(dataPtr, sizeInBytes); return builder.EndVector(); }
        public static void StartChannelNotificationHandlersVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
        public static Offset<FBS.Worker.ChannelMessageHandlers> EndChannelMessageHandlers(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            builder.Required(o, 4);  // channel_request_handlers
            builder.Required(o, 6);  // channel_notification_handlers
            return new Offset<FBS.Worker.ChannelMessageHandlers>(o);
        }
        public ChannelMessageHandlersT UnPack()
        {
            var _o = new ChannelMessageHandlersT();
            this.UnPackTo(_o);
            return _o;
        }
        public void UnPackTo(ChannelMessageHandlersT _o)
        {
            _o.ChannelRequestHandlers = new List<string>();
            for(var _j = 0; _j < this.ChannelRequestHandlersLength; ++_j)
            { _o.ChannelRequestHandlers.Add(this.ChannelRequestHandlers(_j)); }
            _o.ChannelNotificationHandlers = new List<string>();
            for(var _j = 0; _j < this.ChannelNotificationHandlersLength; ++_j)
            { _o.ChannelNotificationHandlers.Add(this.ChannelNotificationHandlers(_j)); }
        }
        public static Offset<FBS.Worker.ChannelMessageHandlers> Pack(FlatBufferBuilder builder, ChannelMessageHandlersT _o)
        {
            if(_o == null)
                return default(Offset<FBS.Worker.ChannelMessageHandlers>);
            var _channel_request_handlers = default(VectorOffset);
            if(_o.ChannelRequestHandlers != null)
            {
                var __channel_request_handlers = new StringOffset[_o.ChannelRequestHandlers.Count];
                for(var _j = 0; _j < __channel_request_handlers.Length; ++_j)
                { __channel_request_handlers[_j] = builder.CreateString(_o.ChannelRequestHandlers[_j]); }
                _channel_request_handlers = CreateChannelRequestHandlersVector(builder, __channel_request_handlers);
            }
            var _channel_notification_handlers = default(VectorOffset);
            if(_o.ChannelNotificationHandlers != null)
            {
                var __channel_notification_handlers = new StringOffset[_o.ChannelNotificationHandlers.Count];
                for(var _j = 0; _j < __channel_notification_handlers.Length; ++_j)
                { __channel_notification_handlers[_j] = builder.CreateString(_o.ChannelNotificationHandlers[_j]); }
                _channel_notification_handlers = CreateChannelNotificationHandlersVector(builder, __channel_notification_handlers);
            }
            return CreateChannelMessageHandlers(
              builder,
              _channel_request_handlers,
              _channel_notification_handlers);
        }
    }

    public class ChannelMessageHandlersT
    {
        public List<string> ChannelRequestHandlers { get; set; }

        public List<string> ChannelNotificationHandlers { get; set; }

        public ChannelMessageHandlersT()
        {
            this.ChannelRequestHandlers = null;
            this.ChannelNotificationHandlers = null;
        }
    }


    static public class ChannelMessageHandlersVerify
    {
        static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
        {
            return verifier.VerifyTableStart(tablePos)
              && verifier.VerifyVectorOfStrings(tablePos, 4 /*ChannelRequestHandlers*/, true)
              && verifier.VerifyVectorOfStrings(tablePos, 6 /*ChannelNotificationHandlers*/, true)
              && verifier.VerifyTableEnd(tablePos);
        }
    }

}
