// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Users.Drawables
{
    [LongRunningLoad]
    public partial class DrawableAvatar : Sprite
    {
        [CanBeNull]
        private readonly User user;

        /// <summary>
        /// A simple, non-interactable avatar sprite for the specified user.
        /// </summary>
        /// <param name="user">The user. A null value will get a placeholder avatar.</param>
        public DrawableAvatar(User user = null)
        {
            if (user is not null)
                this.user = user as User;
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            RelativeSizeAxes = Axes.Both;
            FillMode = FillMode.Fit;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Texture = textures.Get(user is not null && user.ID == User.PERSONAL_USER_ID ? user.AvatarPath : @"Online/avatar-guest");
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            this.FadeInFromZero(300, Easing.OutQuint);
        }
    }
}
