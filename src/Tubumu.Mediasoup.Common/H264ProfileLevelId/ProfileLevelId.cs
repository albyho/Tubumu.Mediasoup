namespace Tubumu.H264ProfileLevelId
{
    /// <summary>
    /// Represents a parsed h264 profile-level-id value.
    /// </summary>
    public class ProfileLevelId
    {
        /// <summary>
        /// <para>Default ProfileLevelId.</para>
        /// <para>
        /// TODO: The default should really be profile Baseline and level 1 according to
        /// the spec: https://tools.ietf.org/html/rfc6184#section-8.1. In order to not
        /// break backwards compatibility with older versions of WebRTC where external
        /// codecs don't have any parameters, use profile ConstrainedBaseline level 3_1
        /// instead. This workaround will only be done in an interim period to allow
        /// external clients to update their code.
        /// </para>
        /// <para>http://crbug/webrtc/6337.</para>
        /// </summary>
        public static readonly ProfileLevelId DefaultProfileLevelId = new(Profile.ConstrainedBaseline, Level.L3_1);

        public Profile Profile { get; }

        public Level Level { get; }

        public ProfileLevelId(Profile profile, Level level)
        {
            Profile = profile;
            Level = level;
        }
    }
}
