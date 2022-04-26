using System.Text.RegularExpressions;
using Tubumu.Utils.Extensions;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// 扩展模式
    /// <para>https://w3c.github.io/webrtc-svc/</para>
    /// </summary>
    public class ScalabilityMode
    {
        private static readonly Regex ScalabilityModeRegex = new Regex("^[LS]([1-9]\\d{0,1})T([1-9]\\d{0,1})(_KEY)?.*", RegexOptions.Compiled);

        /// <summary>
        /// 空间层
        /// </summary>
        public int SpatialLayers { get; set; }

        /// <summary>
        /// 时间层
        /// </summary>
        public int TemporalLayers { get; set; }

        /// <summary>
        /// 是否包含 "_KEY"
        /// </summary>
        public bool Ksvc { get; set; }

        /// <summary>
        /// Parses the given scalabilityMode string according to the rules in webrtc-svc.
        /// </summary>
        /// <param name="scalabilityMode"></param>
        /// <returns></returns>
        public static ScalabilityMode Parse(string scalabilityMode)
        {
            var match = ScalabilityModeRegex.Match(scalabilityMode);
            var result = new ScalabilityMode();
            if (match.Success)
            {
                result.SpatialLayers = int.Parse(match.Groups[1].Value);
                result.TemporalLayers = int.Parse(match.Groups[2].Value);
                result.Ksvc = match.Groups.Count >= 4 && !match.Groups[3].Value.IsNullOrWhiteSpace();
            }
            else
            {
                result.SpatialLayers = 1;
                result.TemporalLayers = 1;
                result.Ksvc = false;
            }
            return result;
        }
    }
}
