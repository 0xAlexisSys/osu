// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using Realms;

namespace osu.Game.Users
{
    public class User : EmbeddedObject
    {
        public const string DEFAULT_PERSONAL_USERNAME = @"Player";
        public const int PERSONAL_USER_ID = 0;
        public const int OTHER_USER_ID = -1;
        public const int BOT_USER_ID = 1;

        public int ID { get; set; } = OTHER_USER_ID;
        public string Username { get; set; } = @"???";
        public string AvatarPath { get; set; } = @"Online/avatar-guest";

        private UserStatistics? statistics;

        /// <summary>
        /// User statistics for the requested ruleset (in the case of a <see cref="GetUserRequest"/>).
        /// </summary>
        /// <remarks>
        /// This returns null when accessed from <see cref="User"/>. Use <see cref="LocalUserStatisticsProvider"/> instead.
        /// </remarks>
        public UserStatistics Statistics
        {
            get => statistics ??= new UserStatistics();
            set => statistics = value;
        }

        /// <summary>
        /// All user statistics per ruleset's short name (in the case of a <see cref="GetUsersRequest"/> or <see cref="GetMeRequest"/> response).
        /// Otherwise empty. Can be altered for testing purposes.
        /// </summary>
        // todo: this should likely be moved to a separate UserCompact class at some point.
        [Ignored]
        public Dictionary<string, UserStatistics>? RulesetsStatistics { get; set; }

        public override string ToString() => Username;
    }
}
