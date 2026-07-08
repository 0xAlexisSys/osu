// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using osu.Game.Users;
using Realms;

namespace osu.Game.Leaderboards
{
    public partial class LeaderboardManager : Component
    {
        /// <summary>
        /// The latest leaderboard scores fetched by the criteria in <see cref="CurrentCriteria"/>.
        /// </summary>
        public IBindable<LeaderboardScores?> Scores => scores;

        private readonly Bindable<LeaderboardScores?> scores = new Bindable<LeaderboardScores?>();

        public LeaderboardCriteria? CurrentCriteria { get; private set; }

        private IDisposable? localScoreSubscription;

        [Resolved]
        private RealmAccess realm { get; set; } = null!;

        [Resolved]
        private Session session { get; set; } = null!;

        /// <summary>
        /// Fetch leaderboard content with the new criteria specified in the background.
        /// On completion, <see cref="Scores"/> will be updated with the results from this call (unless a more recent call with a different criteria has completed).
        /// </summary>
        public void FetchWithCriteria(LeaderboardCriteria newCriteria, bool forceRefresh = false)
        {
            if (!ThreadSafety.IsUpdateThread)
                throw new InvalidOperationException($@"{nameof(FetchWithCriteria)} must be called from the update thread.");

            if (!forceRefresh && CurrentCriteria?.Equals(newCriteria) == true && scores.Value?.FailState is null)
                return;

            CurrentCriteria = newCriteria;
            localScoreSubscription?.Dispose();
            scores.Value = null;

            if (newCriteria.Beatmap is null || newCriteria.Ruleset is null)
            {
                scores.Value = LeaderboardScores.Failure(LeaderboardFailState.NoneSelected);
                return;
            }

            localScoreSubscription = realm.RegisterForNotifications(r =>
                r.All<ScoreInfo>().Filter($@"{nameof(ScoreInfo.BeatmapInfo)}.{nameof(BeatmapInfo.ID)} == $0"
                                          + $@" AND {nameof(ScoreInfo.BeatmapInfo)}.{nameof(BeatmapInfo.Hash)} == {nameof(ScoreInfo.BeatmapHash)}"
                                          + $@" AND {nameof(ScoreInfo.Ruleset)}.{nameof(RulesetInfo.ShortName)} == $1"
                                          + $@" AND {nameof(ScoreInfo.DeletePending)} == false"
                    , newCriteria.Beatmap.ID, newCriteria.Ruleset.ShortName), localScoresChanged);
        }

        private void localScoresChanged(IRealmCollection<ScoreInfo> sender, ChangeSet? changes)
        {
            Debug.Assert(CurrentCriteria is not null);

            // This subscription may fire from changes to linked beatmaps, which we don't care about.
            // It's currently not possible for a score to be modified after insertion, so we can safely ignore callbacks with only modifications.
            if (changes?.HasCollectionChanges() == false)
                return;

            var newScores = sender.AsEnumerable();

            if (CurrentCriteria.ExactMods is not null)
            {
                if (CurrentCriteria.ExactMods.Length == 0)
                {
                    // we need to filter out all scores that have any mods to get all local nomod scores
                    newScores = newScores.Where(s => s.Mods.Length == 0);
                }
                else
                {
                    // otherwise find all the scores that have all of the currently selected mods (similar to how web applies mod filters)
                    // we're creating and using a string HashSet representation of selected mods so that it can be translated into the DB query itself
                    var selectedMods = CurrentCriteria.ExactMods.Select(m => m.Acronym).ToHashSet();

                    newScores = newScores.Where(s => selectedMods.SetEquals(s.Mods.Select(m => m.Acronym)));
                }
            }

            newScores = newScores.Detach().OrderByCriteria(CurrentCriteria.Sorting);

            var newScoresArray = newScores.ToArray();
            scores.Value = LeaderboardScores.Success(newScoresArray, newScoresArray.Where(s => s.User.ID == session.User.ID && s.Rank != ScoreRank.F)
                                                                                   .OrderByTotalScore()
                                                                                   .FirstOrDefault());
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            localScoreSubscription?.Dispose();
        }
    }

    public record LeaderboardCriteria(
        BeatmapInfo? Beatmap,
        RulesetInfo? Ruleset,
        Mod[]? ExactMods,
        LeaderboardSortMode Sorting = LeaderboardSortMode.Score
    );

    public record LeaderboardScores
    {
        /// <summary>
        /// The collection of all scores received through the leaderboard lookup.
        /// </summary>
        public ScoreInfo[] AllScores { get; }

        /// <summary>
        /// The number of all scores that exist on the leaderboard.
        /// </summary>
        public int ScoreCount { get; }

        /// <summary>
        /// The local user's best score.
        /// </summary>
        public ScoreInfo? PersonalBestScore { get; }

        /// <summary>
        /// The failure state that occurred when attempting to retrieve the leaderboard.
        /// </summary>
        public LeaderboardFailState? FailState { get; }

        private LeaderboardScores(ScoreInfo[] allScores, ScoreInfo? personalBestScore, LeaderboardFailState? failState)
        {
            AllScores = allScores;
            ScoreCount = allScores.Length;
            PersonalBestScore = personalBestScore;
            FailState = failState;
        }

        public static LeaderboardScores Success(ScoreInfo[] allScores, ScoreInfo? userScore)
            => new LeaderboardScores(allScores, userScore, null);

        public static LeaderboardScores Failure(LeaderboardFailState failState)
            => new LeaderboardScores([], null, failState);
    }

    public enum LeaderboardFailState
    {
        NoneSelected = -1,
    }
}
