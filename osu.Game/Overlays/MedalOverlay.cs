// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Game.Graphics.Containers;
using osu.Game.Input.Bindings;
using osu.Game.Medals;
using osu.Game.Screens.Play;
using osu.Game.Screens.Ranking;
using osu.Game.Screens.Ranking.Expanded.Accuracy;
using osu.Game.Screens.Select;

namespace osu.Game.Overlays
{
    public partial class MedalOverlay : OsuFocusedOverlayContainer
    {
        public override bool IsPresent => base.IsPresent || Scheduler.HasPendingTasks;

        protected override string? PopInSampleName => null;
        protected override string? PopOutSampleName => null;

        private readonly Container<Drawable> medalContainer;
        private MedalAnimation? currentMedalDisplay;
        private bool scheduledShow;
        private readonly Queue<MedalAnimation> queuedMedalDisplays = [];

        [Resolved]
        private OsuGame game { get; set; } = null!;

        [Resolved]
        private MedalManager medalManager { get; set; } = null!;

        public MedalOverlay()
        {
            RelativeSizeAxes = Axes.Both;
            Child = medalContainer = new Container { RelativeSizeAxes = Axes.Both };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            game.ScreenStack.ScreenPushed += onScreenPushed;
            medalManager.MedalUnlocked += onMedalUnlocked;
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            game.ScreenStack.ScreenPushed -= onScreenPushed;
            medalManager.MedalUnlocked -= onMedalUnlocked;
        }

        public override void Hide()
        {
            // don't allow hiding the overlay via any method other than our own.
        }

        public override bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
        {
            if (e.Action == GlobalAction.Back)
            {
                progressDisplayByUser();
                return true;
            }

            return base.OnPressed(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            progressDisplayByUser();
            return true;
        }

        protected override void PopIn() => this.FadeIn();

        protected override void PopOut() => this.FadeOut();

        private void onScreenPushed(IScreen lastScreen, IScreen newScreen)
        {
            if (!queuedMedalDisplays.TryPeek(out _))
            {
                switch (lastScreen)
                {
                    case GameplayPlayer when newScreen is ResultsScreen:
                        scheduleShow(AccuracyCircle.TOTAL_DURATION + 1000.0d);
                        break;

                    case PlayerLoader when newScreen is SongSelect:
                        scheduleShow(500.0d);
                        break;
                }
            }
        }

        private void onMedalUnlocked(Medal medal)
        {
            Logger.Log($@"Queueing medal ""{medal.Slug}"" to display");

            Schedule(() => LoadComponentAsync(new MedalAnimation(medal), m =>
            {
                queuedMedalDisplays.Enqueue(m);
                if (game.ScreenStack.CurrentScreen is not (ResultsScreen or PlayerLoader)) showNextMedal();
            }));
        }

        private void progressDisplayByUser()
        {
            // Dismissing may sometimes play out the medal animation rather than immediately dismissing.
            if (currentMedalDisplay?.Dismiss() == false)
                return;

            currentMedalDisplay = null;
            showNextMedal();
        }

        private void showNextMedal()
        {
            // If already displayed, keep displaying medals regardless of activation mode changes.
            if (OverlayActivationMode.Value != OverlayActivation.All && State.Value == Visibility.Hidden)
                return;

            // A medal is already displaying.
            if (currentMedalDisplay is not null)
                return;

            if (queuedMedalDisplays.TryDequeue(out currentMedalDisplay))
            {
                Logger.Log($@"Displaying medal ""{currentMedalDisplay.Medal.Slug}""");
                medalContainer.Add(currentMedalDisplay);
                Show();
            }
            else if (State.Value == Visibility.Visible)
            {
                Logger.Log(@"All queued medals have been displayed, hiding overlay!");
                base.Hide();
            }
        }

        private void scheduleShow(double delayTime)
        {
            if (!scheduledShow)
            {
                scheduledShow = true;
                Scheduler.AddDelayed(() =>
                {
                    showNextMedal();
                    scheduledShow = false;
                }, delayTime);
            }
        }
    }
}
