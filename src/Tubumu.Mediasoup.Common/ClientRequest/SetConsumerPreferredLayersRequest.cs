using FBS.Consumer;

namespace Tubumu.Mediasoup
{
    public class SetConsumerPreferredLayersRequest : SetPreferredLayersRequestT
    {
        public string ConsumerId { get; set; }
    }
}
