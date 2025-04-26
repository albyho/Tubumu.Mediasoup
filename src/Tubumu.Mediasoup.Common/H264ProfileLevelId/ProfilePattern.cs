namespace Tubumu.H264ProfileLevelId
{
    /// <summary>
    ///  Class for converting between profile_idc/profile_iop to Profile.
    /// </summary>
    public class ProfilePattern
    {
        public int ProfileIdc { get; }

        public BitPattern ProfileIop { get; }

        public Profile Profile { get; }

        public ProfilePattern(int profileIdc, BitPattern profileIop, Profile profile)
        {
            ProfileIdc = profileIdc;
            ProfileIop = profileIop;
            Profile = profile;
        }
    }
}
