using System;
using System.Collections.Generic;

namespace Tubumu.H264ProfileLevelId
{
    /// <summary>
    /// H264ProfileLevelId utils.
    /// <para><see href="https://github.com/versatica/h264-profile-level-id"/></para>
    /// <para><see href="https://webrtc.googlesource.com/src/+/refs/heads/main/api/video_codecs/h264_profile_level_id.h"/></para>
    /// <para><see href="https://webrtc.googlesource.com/src/+/refs/heads/main/api/video_codecs/h264_profile_level_id.cc"/></para>
    /// <para><see href="https://webrtc.googlesource.com/src/+/refs/heads/main/api/video_codecs/test/h264_profile_level_id_unittest.cc"/></para>
    /// </summary>
    public static class Utils
    {
        // This is from https://tools.ietf.org/html/rfc6184#section-8.1.
        private static readonly ProfilePattern[] ProfilePatterns = new[]
        {
            new ProfilePattern(0x42, new BitPattern("x1xx0000"), Profile.ConstrainedBaseline),
            new ProfilePattern(0x4D, new BitPattern("1xxx0000"), Profile.ConstrainedBaseline),
            new ProfilePattern(0x58, new BitPattern("11xx0000"), Profile.ConstrainedBaseline),
            new ProfilePattern(0x42, new BitPattern("x0xx0000"), Profile.Baseline),
            new ProfilePattern(0x58, new BitPattern("10xx0000"), Profile.Baseline),
            new ProfilePattern(0x4D, new BitPattern("0x0x0000"), Profile.Main),
            new ProfilePattern(0x64, new BitPattern("00000000"), Profile.High),
            new ProfilePattern(0x64, new BitPattern("00001100"), Profile.ConstrainedHigh),
            new ProfilePattern(0xF4, new BitPattern("00000000"), Profile.PredictiveHigh444),
        };

        /// <summary>
        /// Parse profile level id that is represented as a string of 3 hex bytes.
        /// Nothing will be returned if the string is not a recognized H264 profile
        /// level id.
        /// </summary>
        /// <param name="str">profile-level-id value as a string of 3 hex bytes.</param>
        /// <returns>ProfileLevelId</returns>
        public static ProfileLevelId? ParseProfileLevelId(string str)
        {
            // For level_idc=11 and profile_idc=0x42, 0x4D, or 0x58, the constraint set3
            // flag specifies if level 1b or level 1.1 is used.
            const int constraintSet3Flag = 0x10;

            // The string should consist of 3 bytes in hexadecimal format.
            if (str.Length != 6)
            {
                return null;
            }

            var profileLevelIdNumeric = Convert.ToInt32(str, 16);

            if (profileLevelIdNumeric == 0)
            {
                return null;
            }

            // Separate into three bytes.
            var levelIdc = (Level)(profileLevelIdNumeric & 0xFF);
            var profileIop = (profileLevelIdNumeric >> 8) & 0xFF;
            var profileIdc = (profileLevelIdNumeric >> 16) & 0xFF;

            // Parse level based on level_idc and constraint set 3 flag.
            Level level;

            switch (levelIdc)
            {
                case Level.L1_1:
                {
                    level = (profileIop & constraintSet3Flag) != 0 ? Level.L1_b : Level.L1_1;

                    break;
                }

                case Level.L1:
                case Level.L1_2:
                case Level.L1_3:
                case Level.L2:
                case Level.L2_1:
                case Level.L2_2:
                case Level.L3:
                case Level.L3_1:
                case Level.L3_2:
                case Level.L4:
                case Level.L4_1:
                case Level.L4_2:
                case Level.L5:
                case Level.L5_1:
                case Level.L5_2:
                {
                    level = levelIdc;

                    break;
                }
                // Unrecognized level_idc.
                default:
                {
                    // NOTE: For testing.
                    //debug("ParseProfileLevelId() | Unrecognized level_idc:%s", level_idc);
                    return null;
                }
            }

            // Parse profile_idc/profile_iop into a Profile enum.
            foreach (var pattern in ProfilePatterns)
            {
                if (profileIdc == pattern.ProfileIdc && pattern.ProfileIop.IsMatch(profileIop))
                {
                    return new ProfileLevelId(pattern.Profile, level);
                }
            }

            // NOTE: For testing.
            //debug("ParseProfileLevelId() | Unrecognized profile_idc/profile_iop combination");
            return null;
        }

        /// <summary>
        /// Returns canonical string representation as three hex bytes of the profile
        /// level id, or returns nothing for invalid profile level ids.
        /// </summary>
        public static string? ProfileLevelIdToString(ProfileLevelId profileLevelId)
        {
            // Handle special case level == 1b.
            if (profileLevelId.Level == Level.L1_b)
            {
                switch (profileLevelId.Profile)
                {
                    case Profile.ConstrainedBaseline:
                    {
                        return "42f00b";
                    }
                    case Profile.Baseline:
                    {
                        return "42100b";
                    }
                    case Profile.Main:
                    {
                        return "4d100b";
                    }
                    // Level 1_b is not allowed for other profiles.
                    default:
                    {
                        // NOTE: For testing.
                        //debug("ProfileLevelIdToString() | Level 1_b not is allowed for profile:%s", profileLevelId.profile);
                        return null;
                    }
                }
            }

            string profileIdcIopString;

            switch (profileLevelId.Profile)
            {
                case Profile.ConstrainedBaseline:
                {
                    profileIdcIopString = "42e0";
                    break;
                }
                case Profile.Baseline:
                {
                    profileIdcIopString = "4200";
                    break;
                }
                case Profile.Main:
                {
                    profileIdcIopString = "4d00";
                    break;
                }
                case Profile.ConstrainedHigh:
                {
                    profileIdcIopString = "640c";
                    break;
                }
                case Profile.High:
                {
                    profileIdcIopString = "6400";
                    break;
                }
                case Profile.PredictiveHigh444:
                {
                    profileIdcIopString = "f400";

                    break;
                }
                default:
                {
                    // NOTE: For testing.
                    //debug("ProfileLevelIdToString() | Unrecognized profile:%s", profileLevelId.profile);
                    return null;
                }
            }

            var levelStr = Convert.ToString((int)profileLevelId.Level, 16);

            if (levelStr.Length == 1)
            {
                levelStr = levelStr.PadLeft(2, '0');
            }

            return $"{profileIdcIopString}{levelStr}";
        }

        /// <summary>
        /// Returns a human friendly name for the given profile.
        /// </summary>
        public static string ProfileToString(this Profile profile)
        {
            return profile.ToString();
        }

        /// <summary>
        /// Returns a human friendly name for the given level.
        /// </summary>
        public static string LevelToString(this Level level)
        {
            // Level.L1_b => 1.b
            // Level.L1 => 1
            // Level.L1_1 => 1.1
            if (level == Level.L1_b)
            {
                return "1b";
            }

            var levelEnumString = level.ToString();
            return levelEnumString["Level.L".Length..].Replace('_', '.');
        }

        /// <summary>
        /// Parse profile level id that is represented as a string of 3 hex bytes
        /// contained in an SDP key-value map. A default profile level id will be
        /// returned if the profile-level-id key is missing. Nothing will be returned
        /// if the key is present but the string is invalid.
        /// </summary>
        public static ProfileLevelId? ParseSdpProfileLevelId(IDictionary<string, object> parameters)
        {
            return parameters.TryGetValue("profile-level-id", out var profileLevelId)
                ? ParseProfileLevelId(profileLevelId.ToString()!)
                : ProfileLevelId.DefaultProfileLevelId;
        }

        /// <summary>
        /// Returns true if the parameters have the same H264 profile, i.e. the same
        /// H264 profile (Baseline, High, etc.).
        /// </summary>
        public static bool IsSameProfile(IDictionary<string, object> params1, IDictionary<string, object> params2)
        {
            var profileLevelId1 = ParseSdpProfileLevelId(params1);
            var profileLevelId2 = ParseSdpProfileLevelId(params2);

            // Compare H264 profiles, but not levels.
            return profileLevelId1 != null
                && profileLevelId2 != null
                && profileLevelId1.Profile == profileLevelId2.Profile;
        }

        /// <summary>
        /// <para>
        /// Generate codec parameters that will be used as answer in an SDP negotiation
        /// based on local supported parameters and remote offered parameters. Both
        /// local_supported_params and remote_offered_params represent sendrecv media
        /// descriptions, i.e. they are a mix of both encode and decode capabilities. In
        /// theory, when the profile in local_supported_params represent a strict superset
        /// of the profile in remote_offered_params, we could limit the profile in the
        /// answer to the profile in remote_offered_params.
        /// </para>
        /// <para>
        /// However, to simplify the code, each supported H264 profile should be listed
        /// explicitly in the list of local supported codecs, even if they are redundant.
        /// Then each local codec in the list should be tested one at a time against the
        /// remote codec, and only when the profiles are equal should this function be
        /// called. Therefore, this function does not need to handle profile intersection,
        /// and the profile of local_supported_params and remote_offered_params must be
        /// equal before calling this function. The parameters that are used when
        /// negotiating are the level part of profile-level-id and level-asymmetry-allowed.
        /// </para>
        /// </summary>
        public static string? GenerateProfileLevelIdForAnswer(
            IDictionary<string, object> localSupportedParams,
            IDictionary<string, object> remoteOfferedParams
        )
        {
            // If both local and remote params do not contain profile-level-id, they are
            // both using the default profile. In this case, don"t return anything.
            if (
                !localSupportedParams.TryGetValue("profile-level-id", out _)
                && !remoteOfferedParams.TryGetValue("profile-level-id", out _)
            )
            {
                // NOTE: For testing.
                //debug("GenerateProfileLevelIdForAnswer() | No profile-level-id in local and remote params");
                return null;
            }

            // Parse profile-level-ids.
            var localProfileLevelId = ParseSdpProfileLevelId(localSupportedParams);
            var remoteProfileLevelId = ParseSdpProfileLevelId(remoteOfferedParams);

            // The local and remote codec must have valid and equal H264 Profiles.
            if (localProfileLevelId == null)
            {
                throw new Exception("invalid local_profile_level_id");
            }

            if (remoteProfileLevelId == null)
            {
                throw new Exception("invalid remote_profile_level_id");
            }

            if (localProfileLevelId.Profile != remoteProfileLevelId.Profile)
            {
                throw new Exception("H264 Profile mismatch");
            }

            // Parse level information.
            var levelAsymmetryAllowed =
                IsLevelAsymmetryAllowed(localSupportedParams) && IsLevelAsymmetryAllowed(remoteOfferedParams);

            var localLevel = localProfileLevelId.Level;
            var remoteLevel = remoteProfileLevelId.Level;
            var minLevel = MinLevel(localLevel, remoteLevel);

            // Determine answer level. When level asymmetry is not allowed, level upgrade
            // is not allowed, i.e., the level in the answer must be equal to or lower
            // than the level in the offer.
            var answerLevel = levelAsymmetryAllowed ? localLevel : minLevel;

            //debug("GenerateProfileLevelIdForAnswer() | Result: [Profile:%s, Level:%s]", local_profile_level_id.profile, answer_level);

            // Return the resulting profile-level-id for the answer parameters.
            return ProfileLevelIdToString(new ProfileLevelId(localProfileLevelId.Profile, answerLevel));
        }

        #region Private Methods

        // Compare H264 levels and handle the level 1b case.
        private static bool IsLessLevel(Level a, Level b)
        {
            if (a == Level.L1_b)
            {
                return b != Level.L1 && b != Level.L1_b;
            }

            if (b == Level.L1_b)
            {
                return a != Level.L1;
            }

            return a < b;
        }

        private static Level MinLevel(Level a, Level b)
        {
            return IsLessLevel(a, b) ? a : b;
        }

        private static bool IsLevelAsymmetryAllowed(IDictionary<string, object> parameters)
        {
            return parameters.TryGetValue("level-asymmetry-allowed", out var levelAsymmetryAllowed)
                && levelAsymmetryAllowed.ToString() == "1";
        }

        #endregion Private Methods
    }
}
