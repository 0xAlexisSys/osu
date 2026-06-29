// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Game.Audio;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Input.Bindings;
using osu.Game.Localisation;
using osu.Game.Online.API;
using osu.Game.Online.Leaderboards;
using osu.Game.Online.Placeholders;
using osu.Game.Overlays;
using osu.Game.Overlays.Volume;
using osu.Game.Scoring;
using osu.Game.Screens.Play;
using osu.Game.Screens.Ranking.Expanded.Accuracy;
using osu.Game.Screens.Ranking.Statistics;
using osu.Game.Skinning;
using osu.Game.Users;
using osuTK;

namespace osu.Game.Screens.Ranking
{
    [Cached]
    public partial class ResultsScreen : ScreenWithBeatmapBackground, IKeyBindingHandler<GlobalAction>
    {
        protected const float BACKGROUND_BLUR = 10;

        private static readonly float screen_height = 768 - TwoLayerButton.SIZE_EXTENDED.Y;

        public override bool DisallowExternalBeatmapRulesetChanges => true;
        public override bool? AllowGlobalTrackControl => true;

        protected override OverlayActivation InitialOverlayActivationMode => OverlayActivation.UserTriggered;

        /// <summary>
        /// Whether the user can retry the beatmap from the results screen.
        /// </summary>
        public bool AllowRetry { get; init; }

        /// <summary>
        /// Whether the user can watch the replay of the completed play from the results screen.
        /// </summary>
        public bool AllowWatchingReplay { get; init; } = true;

        public readonly Bindable<ScoreInfo?> SelectedScore = new Bindable<ScoreInfo?>();
        public readonly ScoreInfo Score;

        protected ScorePanelList ScorePanelList { get; private set; } = null!;
        protected VerticalScrollContainer VerticalScrollContent { get; private set; } = null!;
        protected StatisticsPanel StatisticsPanel { get; private set; } = null!;

        private bool skipExitTransition;
        private Drawable bottomPanel = null!;
        private Container<ScorePanel> detachedPanelContainer = null!;
        private Sample? popInSample;
        private readonly IBindable<LeaderboardScores?> globalScores = new Bindable<LeaderboardScores?>();
        private Task lastFetchTask = Task.CompletedTask;
        private TaskCompletionSource<LeaderboardScores>? requestTaskSource;

        [Resolved]
        private DummyAPIAccess api { get; set; } = null!;

        [Resolved]
        private Player? player { get; set; }

        [Resolved]
        private LeaderboardManager leaderboardManager { get; set; } = null!;

        [Cached]
        private OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Aquamarine);

        public ResultsScreen(ScoreInfo score)
        {
            Score = score;
            SelectedScore.Value = score;
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            FillFlowContainer buttons;

            popInSample = audio.Samples.Get(@"UI/overlay-pop-in");

            InternalChild = new PopoverContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            VerticalScrollContent = new VerticalScrollContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ScrollbarVisible = false,
                                Child = new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Children = new Drawable[]
                                    {
                                        new GlobalScrollAdjustsVolume(),
                                        StatisticsPanel = new StatisticsPanel
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Score = { BindTarget = SelectedScore },
                                        },
                                        ScorePanelList = new ScorePanelList
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            SelectedScore = { BindTarget = SelectedScore },
                                            PostExpandAction = () => StatisticsPanel.ToggleVisibility()
                                        },
                                        detachedPanelContainer = new Container<ScorePanel>
                                        {
                                            RelativeSizeAxes = Axes.Both
                                        },
                                    }
                                }
                            },
                        },
                        new[]
                        {
                            bottomPanel = new Container
                            {
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft,
                                RelativeSizeAxes = Axes.X,
                                Height = TwoLayerButton.SIZE_EXTENDED.Y,
                                Alpha = 0,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = Color4Extensions.FromHex("#333")
                                    },
                                    buttons = new FillFlowContainer
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        AutoSizeAxes = Axes.Both,
                                        Spacing = new Vector2(5),
                                        Direction = FillDirection.Horizontal
                                    },
                                }
                            }
                        }
                    },
                    RowDimensions = new[]
                    {
                        new Dimension(),
                        new Dimension(GridSizeMode.AutoSize)
                    }
                }
            };

            // only show flair / animation when arriving after watching a play that isn't autoplay.
            bool shouldFlair = player != null && Score.User.ID != User.BOT_USER_ID;

            ScorePanelList.AddScore(Score, shouldFlair);

            // this is mostly for medal display.
            // we don't want the medal animation to trample on the results screen animation, so we (ab)use `OverlayActivationMode`
            // to give the results screen enough time to play the animation out before the medals can be shown.
            Scheduler.AddDelayed(() => OverlayActivationMode.Value = OverlayActivation.All, shouldFlair ? AccuracyCircle.TOTAL_DURATION + 1000 : 0);

            bool allowHotkeyRetry = false;

            if (AllowWatchingReplay)
            {
                buttons.Add(new WatchReplayButton(SelectedScore.Value)
                {
                    Width = 300,
                });

                buttons.Add(new ExportReplayButton(SelectedScore.Value)
                {
                    Width = 300,
                });

                // for simplicity, only allow this when coming from a replay player where we know the replay is ready to be played.
                //
                // if we show it in all cases, consider the case where a user comes from song select and potentially has to download
                // the replay before it can be played back. it wouldn't flow well with the quick retry in such a case.
                allowHotkeyRetry = player is ReplayPlayer;
            }

            if (player != null && AllowRetry)
            {
                buttons.Add(new RetryButton { Width = 300 });
                allowHotkeyRetry = true;
            }

            if (allowHotkeyRetry)
            {
                AddInternal(new HotkeyRetryOverlay
                {
                    Action = () =>
                    {
                        if (!this.IsCurrentScreen()) return;

                        skipExitTransition = true;
                        player?.Restart(true);
                    },
                });
            }

            if (Score.BeatmapInfo != null)
                buttons.Add(new CollectionButton(Score.BeatmapInfo));

            if (Score.BeatmapInfo?.BeatmapSet != null)
                buttons.Add(new FavouriteButton(Score.BeatmapInfo.BeatmapSet));
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            StatisticsPanel.State.BindValueChanged(onStatisticsStateChanged, true);
            fetchScores();
            globalScores.BindTo(leaderboardManager.Scores);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (requestTaskSource?.Task.IsCompleted == false)
                requestTaskSource.SetCanceled();
        }

        #region Applause

        private PoolableSkinnableSample? rankApplauseSound;

        public void PlayApplause(ScoreRank rank)
        {
            const double applause_volume = 0.8f;

            if (!this.IsCurrentScreen())
                return;

            rankApplauseSound?.Dispose();

            var applauseSamples = new List<string>();

            if (rank >= ScoreRank.B)
                // when rank is B or higher, play legacy applause sample on legacy skins.
                applauseSamples.Add(@"applause");

            switch (rank)
            {
                default:
                case ScoreRank.D:
                    applauseSamples.Add(@"Results/applause-d");
                    break;

                case ScoreRank.C:
                    applauseSamples.Add(@"Results/applause-c");
                    break;

                case ScoreRank.B:
                    applauseSamples.Add(@"Results/applause-b");
                    break;

                case ScoreRank.A:
                    applauseSamples.Add(@"Results/applause-a");
                    break;

                case ScoreRank.S:
                case ScoreRank.SH:
                case ScoreRank.X:
                case ScoreRank.XH:
                    applauseSamples.Add(@"Results/applause-s");
                    break;
            }

            LoadComponentAsync(rankApplauseSound = new PoolableSkinnableSample(new SampleInfo(applauseSamples.ToArray())), s =>
            {
                if (!this.IsCurrentScreen() || s != rankApplauseSound)
                    return;

                AddInternal(rankApplauseSound);

                rankApplauseSound.VolumeTo(applause_volume);
                rankApplauseSound.Play();
            });
        }

        #endregion

        private void fetchScores()
        {
            if (!lastFetchTask.IsCompleted)
                return;

            lastFetchTask = Task.Run(async () =>
            {
                ScoreInfo[] scores = await FetchScores().ConfigureAwait(false);
                await addScores(scores).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Performs a fetch/refresh of scores to be displayed.
        /// </summary>
        protected async Task<ScoreInfo[]> FetchScores()
        {
            Debug.Assert(Score != null);

            // sort mode intentionally omitted to default to score - results screen only supports sorting by score, so don't pass any other to avoid confusion
            var criteria = new LeaderboardCriteria(
                Score.BeatmapInfo!,
                Score.Ruleset,
                leaderboardManager.CurrentCriteria?.ExactMods
            );

            Debug.Assert(requestTaskSource == null || requestTaskSource.Task.IsCompleted);

            requestTaskSource = new TaskCompletionSource<LeaderboardScores>();

            globalScores.BindValueChanged(_ =>
            {
                if (globalScores.Value != null && leaderboardManager.CurrentCriteria?.Equals(criteria) == true)
                    requestTaskSource.TrySetResult(globalScores.Value);
            });

            Schedule(() => leaderboardManager.FetchWithCriteria(criteria, forceRefresh: true));

            var result = await requestTaskSource.Task.ConfigureAwait(false);

            if (result.FailState != null)
            {
                Logger.Log($"Failed to fetch scores (beatmap: {Score.BeatmapInfo}, ruleset: {Score.Ruleset}): {result.FailState}");
                return [];
            }

            var clonedScores = result.AllScores.Select(s => s.DeepClone()).ToArray();

            List<ScoreInfo> sortedScores = [];

            foreach (var clonedScore in clonedScores)
            {
                // ensure that we do not double up on the score being presented here.
                // additionally, ensure that the reference that ends up in `sortedScores` is the `Score` reference specifically.
                // this simplifies handling later.
                if (clonedScore.Equals(Score))
                {
                    // this is a precautionary guard that prevents `Score` from appearing multiple times in the list.
                    // that can occur in rare cases wherein two local scores have the same online ID but different replay contents
                    // (this is possible e.g. in cases of client-side vs server-side recorded replays, see https://github.com/ppy/osu-server-spectator/issues/193)
                    if (sortedScores.Contains(Score))
                        continue;

                    Score.Position = clonedScore.Position;
                    sortedScores.Add(Score);
                }
                else
                {
                    bool presentingUserScore = Score.User.ID == api.User.ID;
                    bool presentedUserScoreIsBetter = presentingUserScore && clonedScore.User.ID == api.User.ID && clonedScore.TotalScore < Score.TotalScore;

                    if (presentedUserScoreIsBetter)
                        continue;

                    sortedScores.Add(clonedScore);
                }
            }

            // if we haven't encountered a match for the presented score, we still need to attach it.
            // note that the above block ensuring that the `Score` reference makes it in here makes this valid to write in this way.
            if (!sortedScores.Contains(Score))
                sortedScores.Add(Score);

            sortedScores = sortedScores.OrderByTotalScore().ToList();

            int delta = 0;
            bool isPartialLeaderboard = result.IsPartial;

            for (int i = 0; i < sortedScores.Count; i++)
            {
                var sortedScore = sortedScores[i];

                // see `SoloGameplayLeaderboardProvider.sort()` for another place that does the same thing with slight deviations
                // if this code is changed, that code should probably be changed as well

                if (!isPartialLeaderboard)
                {
                    sortedScore.Position = i + 1;
                }
                else
                {
                    if (ReferenceEquals(sortedScore, Score) && sortedScore.Position == null)
                    {
                        int? previousScorePosition = i > 0 ? sortedScores[i - 1].Position : 0;
                        int? nextScorePosition = i < result.TopScores.Count - 1 ? sortedScores[i + 1].Position : null;

                        if (previousScorePosition != null && nextScorePosition != null && previousScorePosition + 1 == nextScorePosition)
                        {
                            sortedScore.Position = previousScorePosition + 1;
                            delta += 1;
                        }
                        else
                        {
                            sortedScore.Position = null;
                        }
                    }
                    else
                    {
                        sortedScore.Position += delta;
                    }
                }
            }

            // there's a non-zero chance that the `Score.Position` was mutated above,
            // but that is not actually coupled to `ScorePosition` of the relevant score panel in any way,
            // so ensure that the drawable panel also receives the updated position.
            // note that this is valid to do precisely because we ensured `Score` was in `sortedScores` earlier.
            ScorePanelList.GetPanelForScore(Score).ScorePosition.Value = Score.Position;

            sortedScores.Remove(Score);
            return sortedScores.ToArray();
        }

        private Task addScores(ScoreInfo[] scores)
        {
            var tcs = new TaskCompletionSource();

            Schedule(() =>
            {
                foreach (var s in scores)
                {
                    var panel = ScorePanelList.AddScore(s);
                    if (detachedPanel != null)
                        panel.Alpha = 0;
                }

                // allow a frame for scroll container to adjust its dimensions with the added scores before fetching again.
                Schedule(tcs.SetResult);

                if (ScorePanelList.IsEmpty)
                {
                    // This can happen if for example a beatmap that is part of a playlist hasn't been played yet.
                    VerticalScrollContent.Add(new MessagePlaceholder(LeaderboardStrings.NoRecordsYet));
                }
            });

            return tcs.Task;
        }

        public override void OnEntering(ScreenTransitionEvent e)
        {
            base.OnEntering(e);

            ApplyToBackground(b =>
            {
                b.BlurAmount.Value = BACKGROUND_BLUR;
                b.FadeColour(OsuColour.Gray(0.5f), 250);
            });

            bottomPanel.FadeTo(1, 250);

            popInSample?.Play();
        }

        public override bool OnExiting(ScreenExitEvent e)
        {
            if (base.OnExiting(e))
                return true;

            // This is a stop-gap safety against components holding references to gameplay after exiting the gameplay flow.
            // Right now, HitEvents are only used up to the results screen. If this changes in the future we need to remove
            // HitObject references from HitEvent.
            Score.HitEvents.Clear();

            if (!skipExitTransition)
                this.FadeOut(100);

            rankApplauseSound?.Stop();
            return false;
        }

        public override bool OnBackButton()
        {
            if (StatisticsPanel.State.Value == Visibility.Visible)
            {
                StatisticsPanel.Hide();
                return true;
            }

            return false;
        }

        private ScorePanel? detachedPanel;

        private void onStatisticsStateChanged(ValueChangedEvent<Visibility> state)
        {
            if (state.NewValue == Visibility.Visible)
            {
                Debug.Assert(SelectedScore.Value != null);
                // Detach the panel in its original location, and move into the desired location in the local container.
                var expandedPanel = ScorePanelList.GetPanelForScore(SelectedScore.Value);
                var screenSpacePos = expandedPanel.ScreenSpaceDrawQuad.TopLeft;

                // Detach and move into the local container.
                ScorePanelList.Detach(expandedPanel);
                detachedPanelContainer.Add(expandedPanel);

                // Move into its original location in the local container first, then to the final location.
                float origLocation = detachedPanelContainer.ToLocalSpace(screenSpacePos).X;
                expandedPanel.MoveToX(origLocation)
                             .Then()
                             .MoveToX(StatisticsPanel.SIDE_PADDING, 400, Easing.OutElasticQuarter);

                // Hide contracted panels.
                foreach (var contracted in ScorePanelList.GetScorePanels().Where(p => p.State == PanelState.Contracted))
                    contracted.FadeOut(150, Easing.OutQuint);
                ScorePanelList.HandleInput = false;

                // Dim background.
                ApplyToBackground(b => b.FadeColour(OsuColour.Gray(0.4f), 400, Easing.OutQuint));

                detachedPanel = expandedPanel;
            }
            else if (detachedPanel != null)
            {
                var screenSpacePos = detachedPanel.ScreenSpaceDrawQuad.TopLeft;

                // Remove from the local container and re-attach.
                detachedPanelContainer.Remove(detachedPanel, false);
                ScorePanelList.Attach(detachedPanel);

                // Move into its original location in the attached container first, then to the final location.
                float origLocation = detachedPanel.Parent!.ToLocalSpace(screenSpacePos).X;
                detachedPanel.MoveToX(origLocation)
                             .Then()
                             .MoveToX(0, 250, Easing.OutElasticQuarter);

                // Show contracted panels.
                foreach (var contracted in ScorePanelList.GetScorePanels().Where(p => p.State == PanelState.Contracted))
                    contracted.FadeIn(150, Easing.OutQuint);
                ScorePanelList.HandleInput = true;

                // Un-dim background.
                ApplyToBackground(b => b.FadeColour(OsuColour.Gray(0.5f), 250, Easing.OutQuint));

                detachedPanel = null;
            }
        }

        public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
        {
            if (e.Repeat)
                return false;

            switch (e.Action)
            {
                case GlobalAction.QuickExit:
                    if (this.IsCurrentScreen())
                    {
                        this.Exit();
                        return true;
                    }

                    break;

                case GlobalAction.Select:
                    if (SelectedScore.Value != null)
                        StatisticsPanel.ToggleVisibility();
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
        {
        }

        protected override bool OnScroll(ScrollEvent e)
        {
            // Match stable behaviour of only alt-scroll adjusting volume.
            // This is the same behaviour as the song selection screen.
            if (!e.CurrentState.Keyboard.AltPressed)
                return true;

            return base.OnScroll(e);
        }

        protected partial class VerticalScrollContainer : OsuScrollContainer
        {
            protected override Container<Drawable> Content => content;

            private readonly Container content;

            protected override bool OnScroll(ScrollEvent e) => !e.ControlPressed && !e.AltPressed && !e.ShiftPressed && !e.SuperPressed;

            public VerticalScrollContainer()
            {
                Masking = false;

                base.Content.Add(content = new Container { RelativeSizeAxes = Axes.X });
            }

            protected override void Update()
            {
                base.Update();
                content.Height = Math.Max(screen_height, DrawHeight);
            }
        }
    }
}
