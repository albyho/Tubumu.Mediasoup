namespace Tubumu.Utils.FastLambda
{
    /// <summary>
    /// PartialEvaluator
    /// </summary>
    public class PartialEvaluator : PartialEvaluatorBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PartialEvaluator()
            : base(new Evaluator())
        {
        }
    }
}
