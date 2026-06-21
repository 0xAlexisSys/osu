// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osu.Game.Database;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Beatmaps
{
    public class BeatmapUpdater : IDisposable
    {
        private readonly IWorkingBeatmapCache workingBeatmapCache;

        private readonly BeatmapDifficultyCache difficultyCache;

        private readonly BeatmapUpdaterMetadataLookup metadataLookup;

        private const int update_queue_request_concurrency = 4;

        private readonly ThreadedTaskScheduler updateScheduler = new ThreadedTaskScheduler(update_queue_request_concurrency, nameof(BeatmapUpdaterMetadataLookup));

        public BeatmapUpdater(IWorkingBeatmapCache workingBeatmapCache, BeatmapDifficultyCache difficultyCache, Storage storage)
        {
            this.workingBeatmapCache = workingBeatmapCache;
            this.difficultyCache = difficultyCache;

            metadataLookup = new BeatmapUpdaterMetadataLookup(storage);
        }

        /// <summary>
        /// Run all processing on a beatmap immediately.
        /// </summary>
        /// <param name="beatmapSet">The managed beatmap set to update. A transaction will be opened to apply changes.</param>
        /// <param name="queueUpdate">If <see langword="true"/>, the beatmap set is updated.</param>
        public void Process(BeatmapSetInfo beatmapSet, bool queueUpdate)
        {
            beatmapSet.Realm!.Write(_ =>
            {
                // Before we use below, we want to invalidate.
                workingBeatmapCache.Invalidate(beatmapSet);

                if (queueUpdate)
                    metadataLookup.Update(beatmapSet);

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

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (metadataLookup.IsNotNull())
                metadataLookup.Dispose();

            if (updateScheduler.IsNotNull())
                updateScheduler.Dispose();
        }

        #endregion
    }
}
