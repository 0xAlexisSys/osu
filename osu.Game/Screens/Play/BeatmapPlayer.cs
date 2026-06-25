// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Database;
using osu.Game.Medals;
using osu.Game.Scoring;
using osu.Game.Screens.Play.Leaderboards;
using osu.Game.Screens.Ranking;

namespace osu.Game.Screens.Play
{
    /// <summary>
    /// A player instance which supports submitting scores to an online store.
    /// </summary>
    public partial class BeatmapPlayer : Player
    {
        [Cached(typeof(IGameplayLeaderboardProvider))]
        private readonly SoloGameplayLeaderboardProvider leaderboardProvider = new SoloGameplayLeaderboardProvider();

        [Resolved]
        private SessionStatics statics { get; set; }

        [Resolved]
        private MedalEvaluator medalEvaluator { get; set; }

        public BeatmapPlayer(PlayerConfiguration configuration = null)
            : base(configuration)
        {
            Configuration.ShowLeaderboard = true;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (DrawableRuleset == null)
            {
                // base load must have failed (e.g. due to an unknown mod); bail.
                return;
            }

            AddInternal(new PlayerTouchInputDetector());

            // We probably want to move this display to something more global.
            // Probably using the OSD somehow.
            AddInternal(new GameplayOffsetControl
            {
                Margin = new MarginPadding(20),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
            });

            AddInternal(leaderboardProvider);
        }

        protected override GameplayClockContainer CreateGameplayClockContainer(WorkingBeatmap beatmap, double gameplayStart) => new MasterGameplayClockContainer(beatmap, gameplayStart)
        {
            ShouldValidatePlaybackRate = true,
        };

        public override bool AllowCriticalSettingsAdjustment
        {
            get
            {
                // General limitations to ensure players don't do anything too weird.
                // These match stable for now.

                // TODO: the blocking conditions should probably display a message.
                if (!IsBreakTime.Value && GameplayClockContainer.CurrentTime - GameplayClockContainer.GameplayStartTime > 10000)
                    return false;

                if (GameplayClockContainer.IsPaused.Value)
                    return false;

                return base.AllowCriticalSettingsAdjustment;
            }
        }

        protected override async Task PrepareScoreForResultsAsync(Score score)
        {
            // TODO: [alexis] Show a warning in the UI if there's an audio playback rate discrepancy or
            //                mods not normally usable by the player are detected?
            await base.PrepareScoreForResultsAsync(score).ConfigureAwait(false);
            score.ScoreInfo.Date = DateTimeOffset.Now;
        }

        [Resolved]
        private RealmAccess realm { get; set; }

        protected override void StartGameplay()
        {
            base.StartGameplay();

            // User expectation is that last played should be updated when entering the gameplay loop.
            realm.WriteAsync(r =>
            {
                var realmBeatmap = r.Find<BeatmapInfo>(Beatmap.Value.BeatmapInfo.ID);
                if (realmBeatmap != null)
                    realmBeatmap.LastPlayed = DateTimeOffset.Now;
            });
        }

        public override bool Pause()
        {
            bool wasPaused = GameplayClockContainer.IsPaused.Value;

            bool paused = base.Pause();

            if (!wasPaused && paused)
                Score.ScoreInfo.Pauses.Add((int)Math.Round(GameplayClockContainer.CurrentTime));

            return paused;
        }

        public override bool OnExiting(ScreenExitEvent e)
        {
            bool exiting = base.OnExiting(e);
            statics.SetValue(Static.LastLocalUserScore, Score?.ScoreInfo.DeepClone());
            return exiting;
        }

        protected override ResultsScreen CreateResults(ScoreInfo score)
        {
            medalEvaluator.EvaluateScore(score);

            return new ResultsScreen(score)
            {
                AllowRetry = true,
            };
        }
    }
}
