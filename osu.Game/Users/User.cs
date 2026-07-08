// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using Realms;

namespace osu.Game.Users
{
    public class User : EmbeddedObject
    {
        public const string DEFAULT_AVATAR_PATH = @"Online/avatar-guest";
        public const string DEFAULT_PERSONAL_USERNAME = @"Player";
        public const int PERSONAL_USER_ID = 0;
        public const int OTHER_USER_ID = -1;
        public const int BOT_USER_ID = 1;

        public int ID { get; set; } = OTHER_USER_ID;
        public string Name { get; set; } = @"???";
        public string AvatarPath { get; set; } = DEFAULT_AVATAR_PATH;

        public UserStatistics Statistics
        {
            get => field ??= new UserStatistics();
            set;
        }

        [Ignored]
        public Dictionary<string, UserStatistics>? RulesetsStatistics { get; set; }

        public override string ToString() => Name;
    }
}
