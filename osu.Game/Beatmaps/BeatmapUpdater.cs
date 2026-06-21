// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Game.Database;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Beatmaps
{
    public class BeatmapUpdater
    {
        private readonly IWorkingBeatmapCache workingBeatmapCache;
        private readonly BeatmapDifficultyCache difficultyCache;

        public BeatmapUpdater(IWorkingBeatmapCache workingBeatmapCache, BeatmapDifficultyCache difficultyCache)
        {
            this.workingBeatmapCache = workingBeatmapCache;
            this.difficultyCache = difficultyCache;
        }

        /// <summary>
        /// Run all processing on a beatmap immediately.
        /// </summary>
        /// <param name="beatmapSet">The managed beatmap set to update. A transaction will be opened to apply changes.</param>
        public void Process(BeatmapSetInfo beatmapSet)
        {
            beatmapSet.Realm!.Write(_ =>
            {
                // Before we use below, we want to invalidate.
                workingBeatmapCache.Invalidate(beatmapSet);

                foreach (BeatmapInfo beatmap in beatmapSet.Beatmaps)
                {
                    var working = workingBeatmapCache.GetWorkingBeatmap(beatmap);

                    difficultyCache.Invalidate(beatmap, working.BeatmapInfo);

                    var ruleset = working.BeatmapInfo.Ruleset.CreateInstance();
                    var calculator = ruleset.CreateDifficultyCalculator(working);

                    beatmap.StarRating = calculator.Calculate().StarRating;
                    beatmap.UpdateStatisticsFromBeatmap(working.Beatmap);
                }

                // And invalidate again afterwards as re-fetching the most up-to-date database metadata will be required.
                workingBeatmapCache.Invalidate(beatmapSet);
            });
        }

        /// <summary>
        /// Runs a subset of processing focused on updating any cached beatmap object counts.
        /// </summary>
        /// <param name="beatmapInfo">The managed beatmap to update. A transaction will be opened to apply changes.</param>
        public void ProcessObjectCounts(BeatmapInfo beatmapInfo)
        {
            beatmapInfo.Realm!.Write(_ =>
            {
                // Before we use below, we want to invalidate.
                workingBeatmapCache.Invalidate(beatmapInfo);

                var working = workingBeatmapCache.GetWorkingBeatmap(beatmapInfo);
                var beatmap = working.Beatmap;

                beatmapInfo.EndTimeObjectCount = beatmap.HitObjects.Count(h => h is IHasDuration);
                beatmapInfo.TotalObjectCount = beatmap.HitObjects.Count;

                // And invalidate again afterwards as re-fetching the most up-to-date database metadata will be required.
                workingBeatmapCache.Invalidate(beatmapInfo);
            });
        }
    }
}
