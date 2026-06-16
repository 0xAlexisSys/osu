// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Database;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Resources.Localisation.Web;

namespace osu.Game.Beatmaps.Drawables.Cards.Buttons
{
    public partial class FavouriteButton : BeatmapCardIconButton, IHasCurrentValue<BeatmapSetFavouriteState>
    {
        private readonly BindableWithCurrent<BeatmapSetFavouriteState> current;

        public Bindable<BeatmapSetFavouriteState> Current
        {
            get => current.Current;
            set => current.Current = value;
        }

        private readonly APIBeatmapSet beatmapSet;

        [Resolved]
        private RealmAccess realm { get; set; } = null!;

        public FavouriteButton(APIBeatmapSet beatmapSet)
        {
            current = new BindableWithCurrent<BeatmapSetFavouriteState>(new BeatmapSetFavouriteState(beatmapSet.HasFavourited, 0));
            this.beatmapSet = beatmapSet;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            realmSubscription = realm.RegisterForNotifications(
                r => r.All<BeatmapSetInfo>().Where(s => s.OnlineID == beatmapSet.OnlineID),
                (sender, _) =>
                {
                    var set = sender.FirstOrDefault();

                    if (set != null)
                        current.Value = new BeatmapSetFavouriteState(set.HasFavourited, 0);
                });

            current.Value = new BeatmapSetFavouriteState(realm.Run(r => r.All<BeatmapSetInfo>().FirstOrDefault(s => s.OnlineID == beatmapSet.OnlineID)?.HasFavourited ?? false), 0);
        }

        private IDisposable? realmSubscription;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Action = toggleFavouriteStatus;
            current.BindValueChanged(_ => updateState(), true);
        }

        private void toggleFavouriteStatus()
        {
            realm.Write(r =>
            {
                var set = r.All<BeatmapSetInfo>().FirstOrDefault(s => s.OnlineID == beatmapSet.OnlineID);

                if (set != null)
                    set.HasFavourited = !set.HasFavourited;
            });
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            realmSubscription?.Dispose();
        }

        private void updateState()
        {
            if (current.Value.Favourited)
            {
                Icon.Icon = FontAwesome.Solid.Heart;
                TooltipText = BeatmapsetsStrings.ShowDetailsUnfavourite;
            }
            else
            {
                Icon.Icon = FontAwesome.Regular.Heart;
                TooltipText = BeatmapsetsStrings.ShowDetailsFavourite;
            }
        }
    }
}
