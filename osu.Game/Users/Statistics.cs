// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Realms;

namespace osu.Game.Users
{
    public class Statistics : RealmObject
    {
        [PrimaryKey]
        public string RulesetName { get; init; } = string.Empty;

        [UsedImplicitly]
        public IDictionary<string, int> BeatmapPlayCounts { get; } = null!;

        [Ignored]
        public int PlayCount => BeatmapPlayCounts.Values.Sum();

        public int HitCount { get; set; }

        public int MissCount { get; set; }

        [UsedImplicitly]
        public IList<double> AccuracySamples { get; } = null!;

        [Ignored]
        public double AverageAccuracy => AccuracySamples.Count != 0 ? AccuracySamples.Average() : 0.0d;

        [UsedImplicitly]
        public IDictionary<string, int> ScoreRankCounts { get; } = null!;
    }

    public readonly record struct StatisticsChanges(
        int? AddedHitCount = null,
        int? AddedMissCount = null,
        double? Accuracy = null,
        string? RankString = null);
}
