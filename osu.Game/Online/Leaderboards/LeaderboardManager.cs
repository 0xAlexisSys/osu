// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Extensions;
using osu.Game.Online.API;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using Realms;

namespace osu.Game.Online.Leaderboards
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
        private IAPIProvider api { get; set; } = null!;

        [Resolved]
        private RealmAccess realm { get; set; } = null!;

        [Resolved]
        private RulesetStore rulesets { get; set; } = null!;

        /// <summary>
        /// Fetch leaderboard content with the new criteria specified in the background.
        /// On completion, <see cref="Scores"/> will be updated with the results from this call (unless a more recent call with a different criteria has completed).
        /// </summary>
        public void FetchWithCriteria(LeaderboardCriteria newCriteria, bool forceRefresh = false)
        {
            if (!ThreadSafety.IsUpdateThread)
                throw new InvalidOperationException(@$"{nameof(FetchWithCriteria)} must be called from the update thread.");

            if (!forceRefresh && CurrentCriteria?.Equals(newCriteria) == true && scores.Value?.FailState == null)
                return;

            CurrentCriteria = newCriteria;
            localScoreSubscription?.Dispose();
            scores.Value = null;

            if (newCriteria.Beatmap == null || newCriteria.Ruleset == null)
            {
                scores.Value = LeaderboardScores.Failure(LeaderboardFailState.NoneSelected);
                return;
            }

            localScoreSubscription = realm.RegisterForNotifications(r =>
                r.All<ScoreInfo>().Filter($"{nameof(ScoreInfo.BeatmapInfo)}.{nameof(BeatmapInfo.ID)} == $0"
                                          + $" AND {nameof(ScoreInfo.BeatmapInfo)}.{nameof(BeatmapInfo.Hash)} == {nameof(ScoreInfo.BeatmapHash)}"
                                          + $" AND {nameof(ScoreInfo.Ruleset)}.{nameof(RulesetInfo.ShortName)} == $1"
                                          + $" AND {nameof(ScoreInfo.DeletePending)} == false"
                    , newCriteria.Beatmap.ID, newCriteria.Ruleset.ShortName), localScoresChanged);
        }

        private void localScoresChanged(IRealmCollection<ScoreInfo> sender, ChangeSet? changes)
        {
            Debug.Assert(CurrentCriteria != null);

            // This subscription may fire from changes to linked beatmaps, which we don't care about.
            // It's currently not possible for a score to be modified after insertion, so we can safely ignore callbacks with only modifications.
            if (changes?.HasCollectionChanges() == false)
                return;

            var newScores = sender.AsEnumerable();

            if (CurrentCriteria.ExactMods != null)
            {
                if (!CurrentCriteria.ExactMods.Any())
                {
                    // we need to filter out all scores that have any mods to get all local nomod scores
                    newScores = newScores.Where(s => !s.Mods.Any());
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
            scores.Value = LeaderboardScores.Success(newScoresArray, scoresRequested: newScoresArray.Length, totalScores: newScoresArray.Length, null);
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
        public ICollection<ScoreInfo> TopScores { get; }

        /// <summary>
        /// The number of scores which was requested.
        /// Used to determine whether the returned leaderboard can be judged to be a partial or full leaderboard
        /// (i.e. whether <see cref="TopScores"/> contains all scores that it could ever contain).
        /// </summary>
        public int ScoresRequested { get; }

        /// <summary>
        /// The number of all scores that exist on the leaderboard.
        /// </summary>
        public int TotalScores { get; }

        public bool IsPartial => ScoresRequested < TotalScores;

        /// <summary>
        /// The local user's best score.
        /// </summary>
        public ScoreInfo? UserScore { get; }

        /// <summary>
        /// The failure state that occurred when attempting to retrieve the leaderboard.
        /// </summary>
        public LeaderboardFailState? FailState { get; }

        public IEnumerable<ScoreInfo> AllScores
        {
            get
            {
                foreach (var score in TopScores)
                    yield return score;

                if (UserScore != null && TopScores.All(topScore => !topScore.Equals(UserScore) && !topScore.MatchesOnlineID(UserScore)))
                    yield return UserScore;
            }
        }

        private LeaderboardScores(ICollection<ScoreInfo> topScores, int scoresRequested, int totalScores, ScoreInfo? userScore, LeaderboardFailState? failState)
        {
            TopScores = topScores;
            ScoresRequested = scoresRequested;
            TotalScores = totalScores;
            UserScore = userScore;
            FailState = failState;
        }

        public static LeaderboardScores Success(ICollection<ScoreInfo> topScores, int scoresRequested, int totalScores, ScoreInfo? userScore)
            => new LeaderboardScores(topScores, scoresRequested, totalScores, userScore, null);

        public static LeaderboardScores Failure(LeaderboardFailState failState)
            => new LeaderboardScores([], scoresRequested: 0, totalScores: 0, null, failState);
    }

    public enum LeaderboardFailState
    {
        NetworkFailure = -1,
        BeatmapUnavailable = -2,
        RulesetUnavailable = -3,
        NoneSelected = -4,
        NotLoggedIn = -5,
        NotSupporter = -6,
        NoTeam = -7
    }
}
