namespace FBS.SctpParameters
{
    public class SctpStreamParametersT
    {
        public ushort StreamId { get; set; }

        public bool? Ordered { get; set; }

        public ushort? MaxPacketLifeTime { get; set; }

        public ushort? MaxRetransmits { get; set; }
    }
}
