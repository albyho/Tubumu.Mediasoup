namespace Tubumu.Mediasoup
{
    public class PipeToRouterResult
    {
        /// <summary>
        /// The Consumer created in the current Router.
        /// </summary>
        public Consumer? PipeConsumer { get; set; }

        /// <summary>
        /// The Producer created in the target Router.
        /// </summary>
        public Producer? PipeProducer { get; set; }

        /// <summary>
        /// The DataConsumer created in the current Router.
        /// </summary>
        public DataConsumer? PipeDataConsumer { get; set; }

        /// <summary>
        /// The DataProducer created in the target Router.
        /// </summary>
        public DataProducer? PipeDataProducer { get; set; }
    }
}
