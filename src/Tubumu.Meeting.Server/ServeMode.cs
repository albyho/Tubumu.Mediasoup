namespace Tubumu.Meeting.Server
{
    /// <summary>
    /// 服务器模式
    /// </summary>
    public enum ServeMode
    {
        /// <summary>
        /// 开放模式
        /// <para>1. 本用户进入后立即生产流。并且可消费其他用户已发布的流。</para>
        /// <para>2. 本用户可消费其他用户发布的新发布的流。</para>
        /// <para>3. 用户发布的流就算无人消费也不会自动停止流。</para>
        /// <para>4. 会议室的其他用户将被动消费。</para>
        /// </summary>
        Open,

        /// <summary>
        /// 拉取模式
        /// <para>1. 用户进入后，可选择拉取其他用户的流。</para>
        /// <para>2. 如果其他用户的流暂未生产则通知其生产，生产成功后通知本用户消费。</para>
        /// <para>3. 如果其他用户的流已经生产则本用户直接消费。</para>
        /// <para>4. 用户发布的流如果在一定时间内无人消费则自动停止流。</para>
        /// <para>5. 会议室的其他用户不会被动消费。</para>
        /// </summary>
        Pull,

        /// <summary>
        /// 邀请模式
        /// <para>1. 用户需会议室管理员邀请后才可发布流。管理员自己可任意发布流。</para>
        /// <para>2. 用户可申请发布流，管理员同意后邀请用户发布。</para>
        /// <para>3. 管理员可关闭任意用户的流。</para>
        /// <para>4. 用户的流断开后，如需继续发布需要再次申请或被邀请。</para>
        /// <para>5. 用户发布的流就算无人消费也不会自动停止流。</para>
        /// <para>6. 会议室的其他用户将被动消费。</para>
        /// </summary>
        Invite
    }
}
