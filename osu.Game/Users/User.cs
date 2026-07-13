// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Realms;

namespace osu.Game.Users
{
    public class User : EmbeddedObject, IUser
    {
        public const string DEFAULT_AVATAR_PATH = @"Online/avatar-guest";
        public const string DEFAULT_PERSONAL_USERNAME = @"Player";
        public const int PERSONAL_USER_ID = 0;
        public const int OTHER_USER_ID = -1;
        public const int BOT_USER_ID = 1;

        public int ID { get; set; } = OTHER_USER_ID;
        public string Name { get; set; } = @"???";
        public string AvatarPath { get; set; } = DEFAULT_AVATAR_PATH;

        public override string ToString() => Name;
    }
}
