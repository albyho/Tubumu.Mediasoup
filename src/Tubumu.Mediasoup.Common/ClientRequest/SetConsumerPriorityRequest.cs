using FBS.Consumer;

namespace Tubumu.Mediasoup
{
    public class SetConsumerPriorityRequest : SetPriorityRequestT
    {
        public string ConsumerId { get; set; }
    }
}
