using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBS.Producer
{
    public class GetStatsResponseT
    {
        public List<FBS.RtpStream.StatsT> Stats { get; set; }
    }
}
