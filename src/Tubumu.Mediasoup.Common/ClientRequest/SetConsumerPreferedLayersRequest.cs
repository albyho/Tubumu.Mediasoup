using FBS.Consumer;

namespace Tubumu.Mediasoup
{
    public class SetConsumerPreferedLayersRequest : SetPreferredLayersRequestT
    {
        public string ConsumerId { get; set; }
    }
}
