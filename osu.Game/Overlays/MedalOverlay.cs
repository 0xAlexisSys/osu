// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Game.Graphics.Containers;
using osu.Game.Input.Bindings;
using osu.Game.Medals;

namespace osu.Game.Overlays
{
    public partial class MedalOverlay : OsuFocusedOverlayContainer
    {
        protected override string? PopInSampleName => null;
        protected override string? PopOutSampleName => null;

        public override bool IsPresent => base.IsPresent || Scheduler.HasPendingTasks;

        protected override void PopIn() => this.FadeIn();

        protected override void PopOut() => this.FadeOut();

        private readonly Queue<MedalAnimation> queuedMedals = new Queue<MedalAnimation>();

        [Resolved]
        private MedalEvaluator medalEvaluator { get; set; } = null!;

        private Container<Drawable> medalContainer = null!;
        private MedalAnimation? currentMedalDisplay;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            medalEvaluator.MedalUnlocked += handleMedal;

            Add(medalContainer = new Container
            {
                RelativeSizeAxes = Axes.Both
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            OverlayActivationMode.BindValueChanged(_ => showNextMedal(), true);
        }

        public override void Hide()
        {
            // don't allow hiding the overlay via any method other than our own.
        }

        private void handleMedal(Medal medal)
        {
            var medalAnimation = new MedalAnimation(medal);

            Logger.Log($"Queueing medal unlock for \"{medal.DisplayName}\" ({queuedMedals.Count} to display)");

            Schedule(() => LoadComponentAsync(medalAnimation, m =>
            {
                queuedMedals.Enqueue(m);
                showNextMedal();
            }));
        }

        protected override bool OnClick(ClickEvent e)
        {
            progressDisplayByUser();
            return true;
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
            if (currentMedalDisplay != null)
                return;

            if (queuedMedals.TryDequeue(out currentMedalDisplay))
            {
                Logger.Log($"Displaying \"{currentMedalDisplay.Medal.DisplayName}\"");
                medalContainer.Add(currentMedalDisplay);
                Show();
            }
            else if (State.Value == Visibility.Visible)
            {
                Logger.Log("All queued medals have been displayed, hiding overlay!");
                base.Hide();
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            medalEvaluator.MedalUnlocked -= handleMedal;

            base.Dispose(isDisposing);
        }
    }
}
