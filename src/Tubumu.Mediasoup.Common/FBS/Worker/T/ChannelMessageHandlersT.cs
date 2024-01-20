using System.Collections.Generic;

namespace FBS.Worker
{
    public class ChannelMessageHandlersT
    {
        public List<string> ChannelRequestHandlers { get; set; }
        public List<string> ChannelNotificationHandlers { get; set; }
    }
}
