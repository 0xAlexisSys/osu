// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Users
{
    public interface IUser
    {
        int ID { get; set; }
        string Name { get; set; }
        string AvatarPath { get; set; }
    }
}
