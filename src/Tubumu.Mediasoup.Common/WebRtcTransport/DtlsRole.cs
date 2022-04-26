using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    public enum DtlsRole
    {
        [EnumMember(Value = "auto")]
        Auto,

        [EnumMember(Value = "client")]
        Client,

        [EnumMember(Value = "server")]
        Server
    }
}
