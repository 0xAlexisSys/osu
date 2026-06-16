// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Resources.Localisation.Web;
using osuTK;

namespace osu.Game.Screens.Ranking
{
    public partial class FavouriteButton : GrayButton
    {
        private IDisposable? realmSubscription;
        private readonly BeatmapSetInfo beatmapSet;

        [Resolved]
        private RealmAccess realm { get; set; } = null!;

        [Resolved]
        private OsuColour colours { get; set; } = null!;

        public FavouriteButton(BeatmapSetInfo set)
            : base(FontAwesome.Regular.Heart)
        {
            beatmapSet = set;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(75, 30);
            Action = toggleFavouriteStatus;

            realmSubscription = realm.RegisterForNotifications(r => r.All<BeatmapSetInfo>().Where(s => s.ID == beatmapSet.ID), (sender, _) =>
            {
                if (sender.Count > 0)
                    updateVisualState(sender[0]);
            });
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            realmSubscription?.Dispose();
            realmSubscription = null;
        }

        private void toggleFavouriteStatus()
        {
            realm.Write(r =>
            {
                var set = r.Find<BeatmapSetInfo>(beatmapSet.ID);
                if (set != null)
                    set.HasFavourited = !set.HasFavourited;
            });
        }

        private void updateVisualState(BeatmapSetInfo? targetBeatmapSet)
        {
            if (targetBeatmapSet?.HasFavourited == true)
            {
                Background.Colour = colours.Green;
                Icon.Icon = FontAwesome.Solid.Heart;
                TooltipText = BeatmapsetsStrings.ShowDetailsUnfavourite;
            }
            else
            {
                Background.Colour = colours.Gray4;
                Icon.Icon = FontAwesome.Regular.Heart;
                TooltipText = BeatmapsetsStrings.ShowDetailsFavourite;
            }
        }
    }
}
