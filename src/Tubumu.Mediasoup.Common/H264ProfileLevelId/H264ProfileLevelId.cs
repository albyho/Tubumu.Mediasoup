using System;
using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// https://github.com/ibc/h264-profile-level-id
    /// </summary>
    public static class H264ProfileLevelId
    {
        public const int ProfileConstrainedBaseline = 1;
        public const int ProfileBaseline = 2;
        public const int ProfileMain = 3;
        public const int ProfileConstrainedHigh = 4;
        public const int ProfileHigh = 5;

        // All values are equal to ten times the level number, except level 1b which is
        // special.
        public const int Level1_b = 0;

        public const int Level1 = 10;
        public const int Level1_1 = 11;
        public const int Level1_2 = 12;
        public const int Level1_3 = 13;
        public const int Level2 = 20;
        public const int Level2_1 = 21;
        public const int Level2_2 = 22;
        public const int Level3 = 30;
        public const int Level3_1 = 31;
        public const int Level3_2 = 32;
        public const int Level4 = 40;
        public const int Level4_1 = 41;
        public const int Level4_2 = 42;
        public const int Level5 = 50;
        public const int Level5_1 = 51;
        public const int Level5_2 = 52;

        /// <summary>
        /// Default ProfileLevelId.
        ///
        /// TODO: The default should really be profile Baseline and level 1 according to
        /// the spec: https://tools.ietf.org/html/rfc6184#section-8.1. In order to not
        /// break backwards compatibility with older versions of WebRTC where external
        /// codecs don't have any parameters, use profile ConstrainedBaseline level 3_1
        /// instead. This workaround will only be done in an interim period to allow
        /// external clients to update their code.
        ///
        /// http://crbug/webrtc/6337.
        /// </summary>
        public static ProfileLevelId DefaultProfileLevelId { get; set; } = new ProfileLevelId(ProfileConstrainedBaseline, Level3_1);

        // For level_idc=11 and profile_idc=0x42, 0x4D, or 0x58, the constraint set3
        // flag specifies if level 1b or level 1.1 is used.
        public const int ConstraintSet3Flag = 0x10;

        public static readonly ProfilePattern[] ProfilePatterns = new ProfilePattern[]
        {
            new ProfilePattern(0x42, new BitPattern("x1xx0000"), ProfileConstrainedBaseline),
            new ProfilePattern(0x4D, new BitPattern("1xxx0000"), ProfileConstrainedBaseline),
            new ProfilePattern(0x58, new BitPattern("11xx0000"), ProfileConstrainedBaseline),
            new ProfilePattern(0x42, new BitPattern("x0xx0000"), ProfileBaseline),
            new ProfilePattern(0x58, new BitPattern("10xx0000"), ProfileBaseline),
            new ProfilePattern(0x4D, new BitPattern("0x0x0000"), ProfileMain),
            new ProfilePattern(0x64, new BitPattern("00000000"), ProfileHigh),
            new ProfilePattern(0x64, new BitPattern("00001100"), ProfileConstrainedHigh)
        };

        /// <summary>
        /// Parse profile level id that is represented as a string of 3 hex bytes.
        /// Nothing will be returned if the string is not a recognized H264 profile
        /// level id.
        ///
        /// @param {String} str - profile-level-id value as a string of 3 hex bytes.
        ///
        /// @returns {ProfileLevelId}
        /// </summary>
        public static ProfileLevelId? ParseProfileLevelId(string str)
        {
            // The string should consist of 3 bytes in hexadecimal format.
            if (str == null || str.Length != 6)
            {
                return null;
            }

            var profile_level_id_numeric = Convert.ToInt32(str, 16);

            if (profile_level_id_numeric == 0)
            {
                return null;
            }

            // Separate into three bytes.
            var level_idc = profile_level_id_numeric & 0xFF;
            var profile_iop = (profile_level_id_numeric >> 8) & 0xFF;
            var profile_idc = (profile_level_id_numeric >> 16) & 0xFF;

            // Parse level based on level_idc and constraint set 3 flag.
            int level;

            switch (level_idc)
            {
                case Level1_1:
                    {
                        level = (profile_iop & ConstraintSet3Flag) != 0 ? Level1_b : Level1_1;
                        break;
                    }
                case Level1:
                case Level1_2:
                case Level1_3:
                case Level2:
                case Level2_1:
                case Level2_2:
                case Level3:
                case Level3_1:
                case Level3_2:
                case Level4:
                case Level4_1:
                case Level4_2:
                case Level5:
                case Level5_1:
                case Level5_2:
                    {
                        level = level_idc;
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
                if (profile_idc == pattern.ProfileIdc && pattern.ProfileIop.IsMatch(profile_iop))
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
        ///
        /// @param {ProfileLevelId} profile_level_id
        ///
        /// @returns {String}
        /// </summary>
        public static string? ProfileLevelIdToString(ProfileLevelId profileLevelId)
        {
            // Handle special case level == 1b.
            if (profileLevelId.Level == Level1_b)
            {
                switch (profileLevelId.Profile)
                {
                    case ProfileConstrainedBaseline:
                        {
                            return "42f00b";
                        }
                    case ProfileBaseline:
                        {
                            return "42100b";
                        }
                    case ProfileMain:
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

            string profile_idc_iop_string;

            switch (profileLevelId.Profile)
            {
                case ProfileConstrainedBaseline:
                    {
                        profile_idc_iop_string = "42e0";
                        break;
                    }
                case ProfileBaseline:
                    {
                        profile_idc_iop_string = "4200";
                        break;
                    }
                case ProfileMain:
                    {
                        profile_idc_iop_string = "4d00";
                        break;
                    }
                case ProfileConstrainedHigh:
                    {
                        profile_idc_iop_string = "640c";
                        break;
                    }
                case ProfileHigh:
                    {
                        profile_idc_iop_string = "6400";
                        break;
                    }
                default:
                    {
                        // NOTE: For testing.
                        //debug("ProfileLevelIdToString() | Unrecognized profile:%s", profileLevelId.profile);
                        return null;
                    }
            }

            var levelStr = Convert.ToString(profileLevelId.Level, 16);

            if (levelStr.Length == 1)
            {
                levelStr = levelStr.PadLeft(2, '0');
            }

            return $"{profile_idc_iop_string}{levelStr}";
        }

        /// <summary>
        /// Parse profile level id that is represented as a string of 3 hex bytes
        /// contained in an SDP key-value map. A default profile level id will be
        /// returned if the profile-level-id key is missing. Nothing will be returned if
        /// the key is present but the string is invalid.
        ///
        /// @param {Object} [params={}] - Codec parameters object.
        ///
        /// @returns {ProfileLevelId}
        /// </summary>
        public static ProfileLevelId? ParseSdpProfileLevelId(IDictionary<string, object> parameters)
        {
            if (!parameters.TryGetValue("profile-level-id", out var profile_level_id))
            {
                return DefaultProfileLevelId;
            }

            return ParseProfileLevelId(profile_level_id.ToString()!);
        }

        /// <summary>
        /// Returns true if the parameters have the same H264 profile, i.e. the same
        /// H264 profile (Baseline, High, etc).
        ///
        /// @param {Object} [params1={}] - Codec parameters object.
        /// @param {Object} [params2={}] - Codec parameters object.
        ///
        /// @returns {Boolean}
        /// </summary>
        public static bool IsSameProfile(IDictionary<string, object> params1, IDictionary<string, object> params2)
        {
            var profile_level_id_1 = ParseSdpProfileLevelId(params1);
            var profile_level_id_2 = ParseSdpProfileLevelId(params2);

            // Compare H264 profiles, but not levels.
            return profile_level_id_1 != null && profile_level_id_2 != null && profile_level_id_1.Profile == profile_level_id_2.Profile;
        }

        /// <summary>
        /// Generate codec parameters that will be used as answer in an SDP negotiation
        /// based on local supported parameters and remote offered parameters. Both
        /// local_supported_params and remote_offered_params represent sendrecv media
        /// descriptions, i.e they are a mix of both encode and decode capabilities. In
        /// theory, when the profile in local_supported_params represent a strict superset
        /// of the profile in remote_offered_params, we could limit the profile in the
        /// answer to the profile in remote_offered_params.
        ///
        /// However, to simplify the code, each supported H264 profile should be listed
        /// explicitly in the list of local supported codecs, even if they are redundant.
        /// Then each local codec in the list should be tested one at a time against the
        /// remote codec, and only when the profiles are equal should this function be
        /// called. Therefore, this function does not need to handle profile intersection,
        /// and the profile of local_supported_params and remote_offered_params must be
        /// equal before calling this function. The parameters that are used when
        /// negotiating are the level part of profile-level-id and level-asymmetry-allowed.
        ///
        /// @param {Object} [local_supported_params={}]
        /// @param {Object} [remote_offered_params={}]
        ///
        /// @returns {String} Canonical string representation as three hex bytes of the
        /// profile level id, or null if no one of the params have profile-level-id.
        ///
        /// @throws {TypeError} If Profile mismatch or invalid params.
        /// </summary>
        public static string? GenerateProfileLevelIdForAnswer(IDictionary<string, object> local_supported_params, IDictionary<string, object> remote_offered_params)
        {
            // If both local and remote params do not contain profile-level-id, they are
            // both using the default profile. In this case, don"t return anything.
            if (!local_supported_params.TryGetValue("profile-level-id", out _) && !remote_offered_params.TryGetValue("profile-level-id", out _))
            {
                // NOTE: For testing.
                //debug("GenerateProfileLevelIdForAnswer() | No profile-level-id in local and remote params");
                return null;
            }

            // Parse profile-level-ids.
            var local_profile_level_id = ParseSdpProfileLevelId(local_supported_params);
            var remote_profile_level_id = ParseSdpProfileLevelId(remote_offered_params);

            // The local and remote codec must have valid and equal H264 Profiles.
            if (local_profile_level_id == null)
            {
                throw new Exception("invalid local_profile_level_id");
            }

            if (remote_profile_level_id == null)
            {
                throw new Exception("invalid remote_profile_level_id");
            }

            if (local_profile_level_id.Profile != remote_profile_level_id.Profile)
            {
                throw new Exception("H264 Profile mismatch");
            }

            // Parse level information.
            var level_asymmetry_allowed = IsLevelAsymmetryAllowed(local_supported_params) && IsLevelAsymmetryAllowed(remote_offered_params);

            var local_level = local_profile_level_id.Level;
            var remote_level = remote_profile_level_id.Level;
            var min_level = MinLevel(local_level, remote_level);

            // Determine answer level. When level asymmetry is not allowed, level upgrade
            // is not allowed, i.e., the level in the answer must be equal to or lower
            // than the level in the offer.
            var answer_level = level_asymmetry_allowed ? local_level : min_level;

            //debug("GenerateProfileLevelIdForAnswer() | Result: [Profile:%s, Level:%s]", local_profile_level_id.profile, answer_level);

            // Return the resulting profile-level-id for the answer parameters.
            return ProfileLevelIdToString(new ProfileLevelId(local_profile_level_id.Profile, answer_level));
        }

        #region Private Methods

        // Compare H264 levels and handle the level 1b case.
        private static bool IsLessLevel(int a, int b)
        {
            if (a == Level1_b)
            {
                return b != Level1 && b != Level1_b;
            }

            if (b == Level1_b)
            {
                return a != Level1;
            }

            return a < b;
        }

        private static int MinLevel(int a, int b)
        {
            return IsLessLevel(a, b) ? a : b;
        }

        private static bool IsLevelAsymmetryAllowed(IDictionary<string, object> parameters)
        {
            return parameters.TryGetValue("level-asymmetry-allowed", out var level_asymmetry_allowed) && level_asymmetry_allowed.ToString() == "1";
        }

        #endregion Private Methods
    }
}
