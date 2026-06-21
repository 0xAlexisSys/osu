// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Threading;
using osu.Game.Online.API.Requests.Responses;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Card
{
    [Cached]
    public partial class CardDetailsOverlayContainer : Container
    {
        public double HideDelay { get; set; } = 1000;

        protected override Container<Drawable> Content { get; }

        private readonly CardDetailsOverlay overlay;

        public CardDetailsOverlayContainer()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren =
            [
                Content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                },
                overlay = new CardDetailsOverlay
                {
                    Alpha = 0,
                }
            ];
        }

        private ScheduledDelegate? hideDelegate;

        public void ShowCardDetails(Drawable targetDrawable, APIBeatmap beatmap)
        {
            hideDelegate?.Cancel();
            hideDelegate = Scheduler.AddDelayed(overlay.Hide, HideDelay);

            overlay.TargetDrawable = targetDrawable;
            overlay.Beatmap.Value = beatmap;
            overlay.Show();
        }

        private partial class CardDetailsOverlay : VisibilityContainer
        {
            public readonly Bindable<APIBeatmap> Beatmap = new Bindable<APIBeatmap>();

            public Drawable? TargetDrawable;

            private Vector2 targetPosition => TargetDrawable is { } drawable
                ? Parent!.ToLocalSpace(drawable.ScreenSpaceDrawQuad.TopLeft) + new Vector2(-20, 0)
                // this results essentially a no-op when there's no valid target
                : Position;

            private readonly Vector2Spring position = new Vector2Spring
            {
                NaturalFrequency = 2f,
                Response = 0.25f,
                Damping = 0.85f
            };

            protected override void Update()
            {
                base.Update();
                Position = position.Update(Time.Elapsed, targetPosition);
            }

            protected override void PopIn()
            {
                this.FadeIn(300);
                position.Current = position.PreviousTarget = targetPosition;
            }

            protected override void PopOut() => this.FadeOut(300);
        }
    }
}
