// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Database;
using osu.Game.Medals;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
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

            medalEvaluator.ClearQueue();

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
            if (score.Mods.Length == 0 && score.Rank == ScoreRank.D && score.TotalScore >= 100000)
                medalEvaluator.AddToQueue(@"secret-all-consolation_prize");

            if (score.MaxCombo == score.GetMaximumAchievableCombo() && score.Accuracy <= 0.85d)
                medalEvaluator.AddToQueue(@"secret-all-stumbler");

            if (score.Mods.Any(m => m is ModNoFail) && score.MaxCombo == score.GetMaximumAchievableCombo())
                medalEvaluator.AddToQueue(@"secret-all-prepared");

            if (score.MaxCombo == score.GetMaximumAchievableCombo() - 1)
                medalEvaluator.AddToQueue(@"secret-all-the_sum_of_all_fears");

            if (score.Ruleset.ShortName == @"osu")
            {
                int greatCount = 0;
                int okCount = 0;
                int mehCount = 0;

                foreach (HitEvent hitEvent in score.HitEvents)
                {
                    switch (hitEvent.Result)
                    {
                        case HitResult.Great:
                            ++greatCount;
                            break;

                        case HitResult.Ok:
                            ++okCount;
                            break;

                        case HitResult.Meh:
                            ++mehCount;
                            break;
                    }
                }

                if (greatCount >= 15 && greatCount == okCount && greatCount == mehCount)
                    medalEvaluator.AddToQueue(@"secret-osu-equilibrium");
            }

            // [alexis] According to Osekai, a max combo equal to the user ID's last three digits is
            //          required. Since online accounts are not a thing, the new requirement is based on
            //          the default username "Player".
            if (score.MaxCombo == 288)
                medalEvaluator.AddToQueue(@"secret-all-value_your_identity");

            if (score.Mods.FirstOrDefault(m => m is ModAccuracyChallenge) is ModAccuracyChallenge ac && Precision.AlmostEquals(score.Accuracy, ac.MinimumAccuracy.Value))
                medalEvaluator.AddToQueue(@"secret-all-by_the_skin_of_the_teeth");

            if (score.Mods.Length >= 15)
                medalEvaluator.AddToQueue(@"secret-all-meticulous_mayhem");

            if (Beatmap.Value.BeatmapInfo.Length >= 420000.0d)
            {
                medalEvaluator.AddToQueue(@"secret-all-perseverance");
                if (score.MaxCombo == score.GetMaximumAchievableCombo())
                    medalEvaluator.AddToQueue(@"secret-all-feel_the_burn");
            }

            if (Beatmap.Value.Beatmap.Difficulty.ApproachRate >= 10.0f
                && Beatmap.Value.Beatmap.Difficulty.OverallDifficulty >= 10.0f
                && Beatmap.Value.Beatmap.Difficulty.DrainRate >= 10.0f
                && score.MaxCombo == score.GetMaximumAchievableCombo())
                medalEvaluator.AddToQueue(@"secret-all-up_for_the_challenge");

            if (score.Ruleset.ShortName == @"osu")
            {
                if (score.Mods.Any(m => m.Type == ModType.DifficultyIncrease) && score.Accuracy <= 0.65d)
                    medalEvaluator.AddToQueue(@"secret-osu-overconfident");

                if (score.Mods.Any(m => m is ModFlashlight) && score.Accuracy <= 0.65d)
                    medalEvaluator.AddToQueue(@"secret-osu-spooked");
            }

            foreach (Mod mod in score.Mods)
            {
                if (mod.MedalSlug is not null)
                    medalEvaluator.AddToQueue(mod.MedalSlug);

                switch (mod.Type)
                {
                    case ModType.Conversion:
                        medalEvaluator.AddToQueue(@"mods-all-any_conversion");
                        break;

                    case ModType.Fun:
                        medalEvaluator.AddToQueue(@"mods-all-any_fun");
                        break;
                }
            }

            queueGetComboMedalUnlock(score, 500);
            queueGetComboMedalUnlock(score, 750);
            queueGetComboMedalUnlock(score, 1000);
            queueGetComboMedalUnlock(score, 2000);

            switch (Beatmap.Value.BeatmapInfo.StarRating)
            {
                case < 2.0d:
                    queuePassStarMedalUnlock(score, 1);
                    break;

                case >= 2.0d and < 3.0d:
                    queuePassStarMedalUnlock(score, 2);
                    break;

                case >= 3.0d and < 4.0d:
                    queuePassStarMedalUnlock(score, 3);
                    break;

                case >= 4.0d and < 5.0d:
                    queuePassStarMedalUnlock(score, 4);
                    break;

                case >= 5.0d and < 6.0d:
                    queuePassStarMedalUnlock(score, 5);
                    break;

                case >= 6.0d and < 7.0d:
                    queuePassStarMedalUnlock(score, 6);
                    break;

                case >= 7.0d and < 8.0d:
                    queuePassStarMedalUnlock(score, 7);
                    break;

                case >= 8.0d and < 9.0d:
                    queuePassStarMedalUnlock(score, 8);
                    break;

                case >= 9.0d and < 10.0d:
                    queuePassStarMedalUnlock(score, 9);
                    break;

                case >= 10.0d:
                    queuePassStarMedalUnlock(score, 10);
                    break;
            }

            medalEvaluator.ProcessQueue();

            return new ResultsScreen(score)
            {
                AllowRetry = true,
            };
        }

        private void queueGetComboMedalUnlock(ScoreInfo score, int maxCombo)
        {
            if (score.MaxCombo >= maxCombo)
                medalEvaluator.AddToQueue($@"skill-all-reach_combo_{maxCombo}");
        }

        private void queuePassStarMedalUnlock(ScoreInfo score, int starRating)
        {
            if (score.Mods.Any(m => m.Type is ModType.DifficultyReduction or ModType.Automation))
                return;

            // [alexis] Other rulesets don't have medals for 9* and 10* beatmaps.
            if (score.Ruleset.ShortName != @"osu" && starRating > 8)
                starRating = 8;

            medalEvaluator.AddToQueue($@"skill-{score.Ruleset.ShortName}-pass_{starRating}_star");
            if (score.MaxCombo == score.GetMaximumAchievableCombo())
                medalEvaluator.AddToQueue($@"skill-{score.Ruleset.ShortName}-fc_{starRating}_star");
        }
    }
}
