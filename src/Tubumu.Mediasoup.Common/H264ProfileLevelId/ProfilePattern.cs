namespace Tubumu.Mediasoup
{
    public class ProfilePattern
    {
        public int ProfileIdc { get; }
        public BitPattern ProfileIop { get; }
        public int Profile { get; }

        public ProfilePattern(int profile_idc, BitPattern profile_iop, int profile)
        {
            ProfileIdc = profile_idc;
            ProfileIop = profile_iop;
            Profile = profile;
        }
    }
}
