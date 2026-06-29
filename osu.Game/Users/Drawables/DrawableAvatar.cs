// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Users.Drawables
{
    [LongRunningLoad]
    public partial class DrawableAvatar : Sprite
    {
        private readonly User? user;

        /// <summary>
        /// A simple avatar sprite for the specified user.
        /// </summary>
        /// <param name="user">The user. A null value will get a placeholder avatar.</param>
        public DrawableAvatar(User? user = null)
        {
            this.user = user;
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            RelativeSizeAxes = Axes.Both;
            FillMode = FillMode.Fit;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            Texture = textures.Get(user switch
            {
                { ID: User.PERSONAL_USER_ID } => user.AvatarPath,
                { ID: User.BOT_USER_ID or User.OTHER_USER_ID } or null => @"Online/avatar-guest",
                _ => $@"https://a.ppy.sh/{user.ID}",
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            this.FadeInFromZero(300, Easing.OutQuint);
        }
    }
}
