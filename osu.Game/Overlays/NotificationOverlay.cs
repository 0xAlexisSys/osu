// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osu.Framework.Threading;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays.Notifications;
using osu.Game.Resources.Localisation.Web;
using osuTK;
using osuTK.Graphics;
using NotificationsStrings = osu.Game.Localisation.NotificationsStrings;

namespace osu.Game.Overlays
{
    public partial class NotificationOverlay : OsuFocusedOverlayContainer, INamedOverlayComponent, INotificationOverlay
    {
        public const float WIDTH = 320;
        public const float TRANSITION_LENGTH = 600;

        public override bool PropagatePositionalInputSubTree => base.PropagatePositionalInputSubTree || toastTray.IsDisplayingToasts;

        public IconUsage Icon => OsuIcon.Notification;
        public LocalisableString Title => NotificationsStrings.HeaderTitle;
        public LocalisableString Description => NotificationsStrings.HeaderDescription;

        /// <summary>
        /// All current displayed notifications, whether in the toast tray or a section.
        /// </summary>
        public IEnumerable<Notification> AllNotifications => IsLoaded ? toastTray.Notifications.Concat(sections.SelectMany(s => s.Notifications)) : [];

        /// <summary>
        /// All ongoing operations (ie. any <see cref="ProgressNotification"/> not in a completed or cancelled state).
        /// </summary>
        public IEnumerable<ProgressNotification> OngoingOperations => AllNotifications.OfType<ProgressNotification>().Where(p => p.Ongoing);

        /// <summary>
        /// Whether there are any ongoing operations, such as imports or downloads.
        /// </summary>
        public bool HasOngoingOperations => OngoingOperations.Any();

        /// <summary>
        /// Current number of unread notifications.
        /// </summary>
        public IBindable<int> UnreadCount => unreadCount;

        public int ToastCount => toastTray.UnreadCount;

        public override bool IsPresent =>
            // Delegate presence as we need to consider the toast tray in addition to the main overlay.
            State.Value == Visibility.Visible || mainContent.IsPresent || toastTray.IsPresent || postScheduler.HasPendingTasks || criticalPostScheduler.HasPendingTasks;

        protected override double PopInOutSampleBalance => OsuGameBase.SFX_STEREO_STRENGTH;

        private FlowContainer<NotificationSection> sections = null!;
        private NotificationOverlayToastTray toastTray = null!;
        private Container mainContent = null!;
        private ScheduledDelegate? notificationsEnabler;
        private readonly BindableInt unreadCount = new BindableInt();
        private int runningDepth;
        private readonly Scheduler postScheduler = new Scheduler();
        private readonly Scheduler criticalPostScheduler = new Scheduler();
        private bool processingPosts = true;
        private double? lastSamplePlayback;

        [Resolved]
        private AudioManager audio { get; set; } = null!;

        [Resolved]
        private OsuGame? game { get; set; }

        [Cached]
        private OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);

        [BackgroundDependencyLoader]
        private void load()
        {
            X = WIDTH;
            Width = WIDTH;
            RelativeSizeAxes = Axes.Y;

            Children = new Drawable[]
            {
                toastTray = new NotificationOverlayToastTray
                {
                    ForwardNotificationToPermanentStore = addPermanently,
                    Origin = Anchor.TopRight,
                },
                mainContent = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Colour = Color4.Black.Opacity(0),
                        Type = EdgeEffectType.Shadow,
                        Radius = 10,
                        Hollow = true,
                    },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colourProvider.Background4,
                        },
                        new OsuScrollContainer
                        {
                            Masking = true,
                            RelativeSizeAxes = Axes.Both,
                            Children = new[]
                            {
                                sections = new FillFlowContainer<NotificationSection>
                                {
                                    Direction = FillDirection.Vertical,
                                    AutoSizeAxes = Axes.Y,
                                    RelativeSizeAxes = Axes.X,
                                    Children = new[]
                                    {
                                        // The main section adds as a catch-all for notifications which don't group into other sections.
                                        new NotificationSection(AccountsStrings.NotificationsTitle),
                                        new NotificationSection(NotificationsStrings.RunningTasks, new[] { typeof(ProgressNotification) }),
                                    }
                                }
                            }
                        }
                    }
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            State.BindValueChanged(_ => updateProcessingMode());
            OverlayActivationMode.BindValueChanged(_ => updateProcessingMode(), true);
        }

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos)
        {
            if (State.Value == Visibility.Visible)
                return base.ReceivePositionalInputAt(screenSpacePos);

            return toastTray.IsDisplayingToasts && toastTray.ReceivePositionalInputAt(screenSpacePos);
        }

        /// <summary>
        /// Post a new notification for display.
        /// </summary>
        /// <param name="notification">The notification to display.</param>
        public void Post(Notification notification) => (notification.IsCritical ? criticalPostScheduler : postScheduler).Add(() =>
        {
            ++runningDepth;

            Logger.Log($"⚠️ {notification.Text}");

            notification.Closed += () => notificationClosed(notification);

            if (notification is IHasCompletionTarget hasCompletionTarget)
                hasCompletionTarget.CompletionTarget ??= Post;

            playDebouncedSample(notification.PopInSampleName);

            if (notification.IsImportant)
            {
                game?.Window?.Flash();
                notification.Closed += () => game?.Window?.CancelFlash();
            }

            if (State.Value == Visibility.Hidden)
            {
                notification.IsInToastTray = true;
                toastTray.Post(notification);
            }
            else
                addPermanently(notification);

            updateCounts();
        });

        protected override void Update()
        {
            base.Update();

            criticalPostScheduler.Update();

            if (processingPosts)
                postScheduler.Update();
        }

        protected override void PopIn()
        {
            this.MoveToX(0, TRANSITION_LENGTH, Easing.OutQuint);
            mainContent.FadeTo(1, TRANSITION_LENGTH / 2, Easing.OutQuint);
            mainContent.FadeEdgeEffectTo(WaveContainer.SHADOW_OPACITY, WaveContainer.APPEAR_DURATION, Easing.Out);

            toastTray.FlushAllToasts();
        }

        protected override void PopOut()
        {
            markAllRead();

            this.MoveToX(WIDTH, TRANSITION_LENGTH, Easing.OutQuint);
            mainContent.FadeTo(0, TRANSITION_LENGTH / 2, Easing.OutQuint);
            mainContent.FadeEdgeEffectTo(0, WaveContainer.DISAPPEAR_DURATION, Easing.In);
        }

        private void addPermanently(Notification notification)
        {
            notification.IsInToastTray = false;

            var ourType = notification.GetType();
            int depth = notification.DisplayOnTop ? -runningDepth : runningDepth;

            var section = sections.Children.FirstOrDefault(s => s.AcceptedNotificationTypes?.Any(accept => accept.IsAssignableFrom(ourType)) == true)
                          ?? sections.First();

            section.Add(notification, depth);

            updateCounts();
        }

        private void notificationClosed(Notification notification) => Schedule(() =>
        {
            updateCounts();

            // this debounce is currently shared between popin/popout sounds, which means one could potentially not play when the user is expecting it.
            // popout is constant across all notification types, and should therefore be handled using playback concurrency instead, but seems broken at the moment.
            playDebouncedSample(notification.PopOutSampleName);
        });

        private void playDebouncedSample(string sampleName)
        {
            if (string.IsNullOrEmpty(sampleName))
                return;

            if (lastSamplePlayback is null || Time.Current - lastSamplePlayback > OsuGameBase.SAMPLE_DEBOUNCE_TIME)
            {
                audio.Samples.Get(sampleName)?.Play();
                lastSamplePlayback = Time.Current;
            }
        }

        private void markAllRead()
        {
            sections.Children.ForEach(s => s.MarkAllRead());
            toastTray.MarkAllRead();
            updateCounts();
        }

        private void updateCounts()
        {
            unreadCount.Value = sections.Select(c => c.UnreadCount).Sum() + toastTray.UnreadCount;
        }

        private void updateProcessingMode()
        {
            bool enabled = OverlayActivationMode.Value != OverlayActivation.Disabled || State.Value == Visibility.Visible;

            notificationsEnabler?.Cancel();

            if (enabled)
            {
                // we want a slight delay before toggling notifications on to avoid the user becoming overwhelmed.
                notificationsEnabler = Scheduler.AddDelayed(() => processingPosts = true, State.Value == Visibility.Visible ? 0 : 250);
            }
            else
            {
                processingPosts = false;
                toastTray.FlushAllToasts();
            }
        }
    }
}
