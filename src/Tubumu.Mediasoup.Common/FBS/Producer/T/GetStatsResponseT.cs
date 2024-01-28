using System.Collections.Generic;

namespace FBS.Producer
{
    public class GetStatsResponseT
    {
        public List<FBS.RtpStream.StatsT> Stats { get; set; }
    }
}
