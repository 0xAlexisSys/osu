// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Resources.Localisation.Web;
using osuTK;

namespace osu.Game.Overlays.BeatmapSet.Buttons
{
    public partial class FavouriteButton : HeaderButton
    {
        public readonly Bindable<APIBeatmapSet> BeatmapSet = new Bindable<APIBeatmapSet>();

        private readonly BindableBool favourited = new BindableBool();

        private LoadingLayer loading;

        [Resolved]
        private RealmAccess realm { get; set; }

        public override LocalisableString TooltipText
        {
            get
            {
                if (!Enabled.Value) return string.Empty;

                return favourited.Value ? BeatmapsetsStrings.ShowDetailsUnfavourite : BeatmapsetsStrings.ShowDetailsFavourite;
            }
        }

        private IDisposable realmSubscription;

        [BackgroundDependencyLoader(true)]
        private void load()
        {
            SpriteIcon icon;

            AddRange(new Drawable[]
            {
                icon = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Regular.Heart,
                    Size = new Vector2(18),
                    Shadow = false,
                },
                loading = new LoadingLayer(true, false),
            });

            Action = () =>
            {
                if (BeatmapSet.Value == null)
                    return;

                loading.Show();

                realm.Write(r =>
                {
                    var set = r.All<BeatmapSetInfo>().FirstOrDefault(s => s.OnlineID == BeatmapSet.Value.OnlineID);

                    if (set != null)
                        set.HasFavourited = !set.HasFavourited;
                });

                loading.Hide();
            };

            favourited.BindValueChanged(favourited => icon.Icon = favourited.NewValue ? FontAwesome.Solid.Heart : FontAwesome.Regular.Heart, true);

            // must be run after setting the Action to ensure correct enabled state (setting an Action forces a button to be enabled).
            BeatmapSet.BindValueChanged(setInfo =>
            {
                realmSubscription?.Dispose();
                realmSubscription = null;

                if (setInfo.NewValue != null)
                {
                    realmSubscription = realm.RegisterForNotifications(r => r.All<BeatmapSetInfo>().Where(s => s.OnlineID == setInfo.NewValue.OnlineID), (sender, _) =>
                    {
                        var set = sender.FirstOrDefault();

                        if (set != null)
                            favourited.Value = set.HasFavourited;
                    });

                    favourited.Value = realm.Run(r => r.All<BeatmapSetInfo>().FirstOrDefault(s => s.OnlineID == setInfo.NewValue.OnlineID)?.HasFavourited ?? false);
                }
                else
                {
                    favourited.Value = false;
                }

                updateEnabled();
            }, true);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            realmSubscription?.Dispose();
        }

        private void updateEnabled() => Enabled.Value = BeatmapSet.Value != null;

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            Width = DrawHeight;
        }
    }
}
