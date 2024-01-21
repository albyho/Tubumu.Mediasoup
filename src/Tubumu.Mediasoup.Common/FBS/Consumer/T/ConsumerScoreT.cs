using System.Collections.Generic;

namespace FBS.Consumer
{
    public class ConsumerScoreT
    {
        public byte Score { get; set; }

        public byte ProducerScore { get; set; }

        public List<byte> ProducerScores { get; set; }
    }
}
