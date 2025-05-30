// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FBS.Message
{

    public enum Body : byte
    {
        NONE = 0,
        Request = 1,
        Response = 2,
        Notification = 3,
        Log = 4,
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
        public FBS.Request.RequestT AsRequest() { return this.As<FBS.Request.RequestT>(); }
        public static BodyUnion FromRequest(FBS.Request.RequestT _request) { return new BodyUnion { Type = Body.Request, Value = _request }; }
        public FBS.Response.ResponseT AsResponse() { return this.As<FBS.Response.ResponseT>(); }
        public static BodyUnion FromResponse(FBS.Response.ResponseT _response) { return new BodyUnion { Type = Body.Response, Value = _response }; }
        public FBS.Notification.NotificationT AsNotification() { return this.As<FBS.Notification.NotificationT>(); }
        public static BodyUnion FromNotification(FBS.Notification.NotificationT _notification) { return new BodyUnion { Type = Body.Notification, Value = _notification }; }
        public FBS.Log.LogT AsLog() { return this.As<FBS.Log.LogT>(); }
        public static BodyUnion FromLog(FBS.Log.LogT _log) { return new BodyUnion { Type = Body.Log, Value = _log }; }

        public static int Pack(Google.FlatBuffers.FlatBufferBuilder builder, BodyUnion _o)
        {
            switch(_o.Type)
            {
                default:
                    return 0;
                case Body.Request:
                    return FBS.Request.Request.Pack(builder, _o.AsRequest()).Value;
                case Body.Response:
                    return FBS.Response.Response.Pack(builder, _o.AsResponse()).Value;
                case Body.Notification:
                    return FBS.Notification.Notification.Pack(builder, _o.AsNotification()).Value;
                case Body.Log:
                    return FBS.Log.Log.Pack(builder, _o.AsLog()).Value;
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
                case Body.Request:
                    result = FBS.Request.RequestVerify.Verify(verifier, tablePos);
                    break;
                case Body.Response:
                    result = FBS.Response.ResponseVerify.Verify(verifier, tablePos);
                    break;
                case Body.Notification:
                    result = FBS.Notification.NotificationVerify.Verify(verifier, tablePos);
                    break;
                case Body.Log:
                    result = FBS.Log.LogVerify.Verify(verifier, tablePos);
                    break;
                default:
                    result = true;
                    break;
            }
            return result;
        }
    }


}
