namespace Tubumu.Mediasoup
{
    public class MediasoupStartupSettings
    {
        public string MediasoupVersion { get; set; }

        public bool WorkerInProcess { get; set; }

        public string WorkerPath { get; set; }

        public int? NumberOfWorkers { get; set; }
    }
}
