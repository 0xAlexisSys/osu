// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays;
using osu.Game.Graphics.UserInterface;
using osu.Framework.Localisation;
using osu.Game.Graphics.Containers;
using osu.Game.Users.Drawables;

namespace osu.Game.Users
{
    public abstract partial class UserPanel : OsuClickableContainer
    {
        public readonly User User;

        /// <summary>
        /// Perform an action in addition to showing the user's profile.
        /// This should be used to perform auxiliary tasks and not as a primary action for clicking a user panel (to maintain a consistent UX).
        /// </summary>
        public new Action? Action;

        protected Action ViewProfile { get; private set; } = null!;

        protected Drawable Background { get; private set; } = null!;

        protected UserPanel(User user)
            : base(HoverSampleSet.Button)
        {
            ArgumentNullException.ThrowIfNull(user);

            User = user;
        }

        [Resolved]
        protected OverlayColourProvider? ColourProvider { get; private set; }

        [Resolved]
        protected OsuColour Colours { get; private set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            Masking = true;

            Add(new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ColourProvider?.Background5 ?? Colours.Gray1
            });

            Add(CreateLayout());
        }

        // TODO: this whole api is messy. half these Create methods are expected to by the implementation and half are implictly called.

        protected abstract Drawable CreateLayout();

        protected OsuSpriteText CreateUsername() => new OsuSpriteText
        {
            Font = OsuFont.GetFont(size: 16, weight: FontWeight.Bold),
            Shadow = false,
            Text = User.Username,
        };

        protected UpdateableAvatar CreateAvatar() => new UpdateableAvatar(User, false);

        public IEnumerable<LocalisableString> FilterTerms => [User.Username];

        public bool MatchingFilter
        {
            set
            {
                if (value)
                    Show();
                else
                    Hide();
            }
        }
    }
}
