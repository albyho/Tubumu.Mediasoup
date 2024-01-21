using System.Collections.Generic;
using FBS.RtpParameters;
using Tubumu.Mediasoup;

namespace Tubumu.Meeting.Server
{
    #region 业务类通知

    /// <summary>
    /// 用户进入房间，通知其他用户
    /// </summary>
    public class PeerJoinRoomNotification
    {
        public Peer Peer { get; set; }
    }

    /// <summary>
    /// 用户离开房间，通知其他用户
    /// </summary>
    public class PeerLeaveRoomNotification
    {
        /// <summary>
        /// PeerId
        /// </summary>
        public string PeerId { get; set; }
    }

    /// <summary>
    /// 产生新消费者，通知客户端进行消费。客户端可根据该通知的信息创建本地消费者。
    /// </summary>
    public class NewConsumerNotification
    {
        /// <summary>
        /// 生产者的 PeerId
        /// </summary>
        public string ProducerPeerId { get; set; }

        /// <summary>
        /// 媒体类型，音频或视频
        /// </summary>
        public MediaKind Kind { get; set; }

        /// <summary>
        /// 生产者 Id
        /// </summary>
        public string ProducerId { get; set; }

        /// <summary>
        /// 消费者 Id
        /// </summary>
        public string ConsumerId { get; set; }

        /// <summary>
        /// Rtp 参数
        /// </summary>
        public Mediasoup.RtpParameters RtpParameters { get; set; }

        /// <summary>
        /// 消费者类型，如 SVC, Simulcast 等。
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 生产者的 AppData
        /// </summary>
        public Dictionary<string, object>? ProducerAppData { get; set; }

        /// <summary>
        /// 生产者是否暂停中
        /// </summary>
        public bool ProducerPaused { get; set; }
    }

    /// <summary>
    /// Pull 或 Invite 模式下，请求用户生产源。
    /// </summary>
    public class ProduceSourcesNotification
    {
        public HashSet<string> Sources { get; set; }
    }

    /// <summary>
    /// Invite 模式下，管理员请出发言后，用户收到的通知。
    /// </summary>
    public class CloseSourcesNotification
    {
        public HashSet<string> Sources { get; set; }
    }

    /// <summary>
    /// Invite 模式下，用户向管理员申请生产，管理员收到的通知。
    /// </summary>
    public class RequestProduceNotification
    {
        /// <summary>
        /// 申请者 PeerId
        /// </summary>
        public string PeerId { get; set; }

        /// <summary>
        /// 申请生产源
        /// </summary>
        public HashSet<string> Sources { get; set; }
    }

    /// <summary>
    /// 用户 AppData 改变后，通知其他用户。
    /// </summary>
    public class PeerAppDataChangedNotification
    {
        /// <summary>
        /// PeerId
        /// </summary>
        public string PeerId { get; set; }

        /// <summary>
        /// AppData
        /// </summary>
        public Dictionary<string, object> AppData { get; set; }
    }

    /// <summary>
    /// 文本消息通知。
    /// </summary>
    public class NewMessageNotification
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }

    #endregion

    #region Consumer

    /// <summary>
    /// 消费者通知
    /// </summary>
    public class ConsumerNotificationBase
    {
        /// <summary>
        /// 消费者 Id
        /// </summary>
        public string ProducerPeerId { get; set; }

        /// <summary>
        /// 媒体类型
        /// </summary>
        public MediaKind Kind { get; set; }

        /// <summary>
        /// 消费者 Id
        /// </summary>
        public string ConsumerId { get; set; }
    }

    /// <summary>
    /// 消费者分值通知
    /// </summary>
    public class ConsumerScoreNotification : ConsumerNotificationBase
    {
        /// <summary>
        /// 分值
        /// </summary>
        public object? Score { get; set; } // 透传
    }

    /// <summary>
    /// 消费者关闭通知
    /// </summary>
    public class ConsumerClosedNotification : ConsumerNotificationBase
    {
    }

    /// <summary>
    /// 消费者暂停通知
    /// </summary>
    public class ConsumerPausedNotification : ConsumerNotificationBase
    {
    }

    /// <summary>
    /// 消费者恢复通知
    /// </summary>
    public class ConsumerResumedNotification : ConsumerNotificationBase
    {
    }

    /// <summary>
    /// 消费者 Layers 改变通知
    /// </summary>
    public class ConsumerLayersChangedNotification : ConsumerNotificationBase
    {
        /// <summary>
        /// Layers
        /// </summary>
        public object? Layers { get; set; } // 透传
    }

    #endregion

    #region Producer

    /// <summary>
    /// 生产者通知
    /// </summary>
    public class ProducerNotificationBase
    {
        /// <summary>
        /// 生产者 Id
        /// </summary>
        public string ProducerId { get; set; }
    }

    /// <summary>
    /// 生产者分值通知
    /// </summary>
    public class ProducerScoreNotification : ProducerNotificationBase
    {
        /// <summary>
        /// 生产者分值
        /// </summary>
        public object? Score { get; set; } // 透传
    }

    /// <summary>
    /// 生产者视频方向改变通知
    /// </summary>
    public class ProducerVideoOrientationChangedNotification : ProducerNotificationBase
    {
        /// <summary>
        /// 视频方向
        /// </summary>
        public object? VideoOrientation { get; set; } // 透传
    }

    /// <summary>
    /// 生产者关闭通知
    /// </summary>
    public class ProducerClosedNotification : ProducerNotificationBase
    {

    }

    #endregion
}
