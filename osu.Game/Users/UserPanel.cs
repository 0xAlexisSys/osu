// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays;
using osu.Game.Graphics.UserInterface;
using osu.Framework.Localisation;
using osu.Game.Graphics.Containers;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Chat;
using osu.Game.Users.Drawables;

namespace osu.Game.Users
{
    public abstract partial class UserPanel : OsuClickableContainer, IFilterable
    {
        public readonly APIUser User;

        /// <summary>
        /// Perform an action in addition to showing the user's profile.
        /// This should be used to perform auxiliary tasks and not as a primary action for clicking a user panel (to maintain a consistent UX).
        /// </summary>
        public new Action? Action;

        protected Action ViewProfile { get; private set; } = null!;

        protected Drawable Background { get; private set; } = null!;

        protected UserPanel(APIUser user)
            : base(HoverSampleSet.Button)
        {
            ArgumentNullException.ThrowIfNull(user);

            User = user;
        }

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        [Resolved]
        private ChannelManager? channelManager { get; set; }

        [Resolved]
        private ChatOverlay? chatOverlay { get; set; }

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

            var background = CreateBackground();
            if (background != null)
                Add(background);

            Add(CreateLayout());
        }

        // TODO: this whole api is messy. half these Create methods are expected to by the implementation and half are implictly called.

        protected abstract Drawable CreateLayout();

        /// <summary>
        /// Panel background container. Can be null if a panel doesn't want a background under it's layout
        /// </summary>
        protected virtual Drawable? CreateBackground() => Background = new UserCoverBackground
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            User = User
        };

        protected OsuSpriteText CreateUsername() => new OsuSpriteText
        {
            Font = OsuFont.GetFont(size: 16, weight: FontWeight.Bold),
            Shadow = false,
            Text = User.Username,
        };

        protected OsuSpriteText CreateRank() => new OsuSpriteText
        {
            Font = OsuFont.GetFont(size: 16, weight: FontWeight.SemiBold),
            Shadow = false,
            // We can't colour the properly because we don't have the required percentile data.

            Colour = Colours.BlueLighter,
            Text = (User.Rank?.Rank ?? User.Statistics.GlobalRank)?.ToLocalisableString("\\##,##0") ?? string.Empty,
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

        public bool FilteringActive { get; set; }
    }
}
