// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FBS.RtpObserver
{

  using System;
  using System.Collections.Generic;
  using Google.FlatBuffers;
  using System.Text.Json.Serialization;

  public struct RemoveProducerRequest : IFlatbufferObject
  {
    private Table __p;
    public ByteBuffer ByteBuffer { get { return __p.bb; } }
    public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
    public static RemoveProducerRequest GetRootAsRemoveProducerRequest( ByteBuffer _bb ) { return GetRootAsRemoveProducerRequest( _bb, new RemoveProducerRequest() ); }
    public static RemoveProducerRequest GetRootAsRemoveProducerRequest( ByteBuffer _bb, RemoveProducerRequest obj ) { return (obj.__assign( _bb.GetInt( _bb.Position ) + _bb.Position, _bb )); }
    public void __init( int _i, ByteBuffer _bb ) { __p = new Table( _i, _bb ); }
    public RemoveProducerRequest __assign( int _i, ByteBuffer _bb ) { __init( _i, _bb ); return this; }

    public string ProducerId { get { int o = __p.__offset( 4 ); return o != 0 ? __p.__string( o + __p.bb_pos ) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetProducerIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
    public ArraySegment<byte>? GetProducerIdBytes() { return __p.__vector_as_arraysegment( 4 ); }
#endif
    public byte[] GetProducerIdArray() { return __p.__vector_as_array<byte>( 4 ); }

    public static Offset<FBS.RtpObserver.RemoveProducerRequest> CreateRemoveProducerRequest( FlatBufferBuilder builder,
        StringOffset producer_idOffset = default( StringOffset ) )
    {
      builder.StartTable( 1 );
      RemoveProducerRequest.AddProducerId( builder, producer_idOffset );
      return RemoveProducerRequest.EndRemoveProducerRequest( builder );
    }

    public static void StartRemoveProducerRequest( FlatBufferBuilder builder ) { builder.StartTable( 1 ); }
    public static void AddProducerId( FlatBufferBuilder builder, StringOffset producerIdOffset ) { builder.AddOffset( 0, producerIdOffset.Value, 0 ); }
    public static Offset<FBS.RtpObserver.RemoveProducerRequest> EndRemoveProducerRequest( FlatBufferBuilder builder )
    {
      int o = builder.EndTable();
      builder.Required( o, 4 );  // producer_id
      return new Offset<FBS.RtpObserver.RemoveProducerRequest>( o );
    }
    public RemoveProducerRequestT UnPack()
    {
      var _o = new RemoveProducerRequestT();
      this.UnPackTo( _o );
      return _o;
    }
    public void UnPackTo( RemoveProducerRequestT _o )
    {
      _o.ProducerId = this.ProducerId;
    }
    public static Offset<FBS.RtpObserver.RemoveProducerRequest> Pack( FlatBufferBuilder builder, RemoveProducerRequestT _o )
    {
      if ( _o == null )
        return default( Offset<FBS.RtpObserver.RemoveProducerRequest> );
      var _producer_id = _o.ProducerId == null ? default( StringOffset ) : builder.CreateString( _o.ProducerId );
      return CreateRemoveProducerRequest(
        builder,
        _producer_id );
    }
  }

  public class RemoveProducerRequestT
  {
    [JsonPropertyName( "producer_id" )]
    public string ProducerId { get; set; }

    public RemoveProducerRequestT()
    {
      this.ProducerId = null;
    }
  }


}
