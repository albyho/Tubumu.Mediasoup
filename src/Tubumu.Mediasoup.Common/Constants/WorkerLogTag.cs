using System.Runtime.Serialization;

namespace Tubumu.Mediasoup
{
    public enum WorkerLogTag
    {
        /// <summary>
        /// Logs about software/library versions, configuration and process information.
        /// </summary>
        [EnumMember(Value = "info")]
        Info,

        /// <summary>
        /// Logs about ICE.
        /// </summary>
        [EnumMember(Value = "ice")]
        Ice,

        /// <summary>
        /// Logs about DTLS.
        /// </summary>
        [EnumMember(Value = "dtls")]
        Dtls,

        /// <summary>
        /// Logs about RTP.
        /// </summary>
        [EnumMember(Value = "rtp")]
        Rtp,

        /// <summary>
        /// Logs about SRTP encryption/decryption.
        /// </summary>
        [EnumMember(Value = "srtp")]
        Srtp,

        /// <summary>
        /// Logs about RTCP.
        /// </summary>
        [EnumMember(Value = "rtcp")]
        Rtcp,

        /// <summary>
        /// Logs about RTP retransmission, including NACK/PLI/FIR.
        /// </summary>
        [EnumMember(Value = "rtx")]
        Rtx,

        /// <summary>
        /// Logs about transport bandwidth estimation.
        /// </summary>
        [EnumMember(Value = "bwe")]
        Bwe,

        /// <summary>
        /// Logs related to the scores of Producers and Consumers.
        /// </summary>
        [EnumMember(Value = "score")]
        Score,

        /// <summary>
        /// Logs about video simulcast.
        /// </summary>
        [EnumMember(Value = "simulcast")]
        Simulcast,

        /// <summary>
        /// Logs about video SVC.
        /// </summary>
        [EnumMember(Value = "svc")]
        Svc,

        /// <summary>
        /// Logs about SCTP (DataChannel).
        /// </summary>
        [EnumMember(Value = "sctp")]
        Sctp,

        /// <summary>
        /// Logs about messages (can be SCTP messages or direct messages).
        /// </summary>
        [EnumMember(Value = "message")]
        Message,
    }
}
