// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Scoring;

namespace osu.Game.Medals
{
    public class MedalEvaluationContext
    {
        /// <summary>
        /// The score that was just set (null during retroactive startup scan).
        /// </summary>
        public ScoreInfo? LatestScore { get; init; }

        /// <summary>
        /// Total play count across all rulesets.
        /// </summary>
        public int TotalPlayCount { get; init; }

        /// <summary>
        /// Total pass count across all rulesets.
        /// </summary>
        public int TotalPassCount { get; init; }

        /// <summary>
        /// Best combo reached across all rulesets.
        /// </summary>
        public int BestCombo { get; init; }

        /// <summary>
        /// Best accuracy reached across all rulesets.
        /// </summary>
        public double BestAccuracy { get; init; }

        /// <summary>
        /// Best rank reached across all rulesets.
        /// </summary>
        public ScoreRank BestRank { get; init; }

        /// <summary>
        /// Aggregated stats per ruleset (keyed by ruleset ShortName, e.g., "osu", "taiko").
        /// </summary>
        public Dictionary<string, RulesetStats> ByRuleset { get; init; } = new Dictionary<string, RulesetStats>();

        public record RulesetStats(int PlayCount, int PassCount, int BestCombo, double BestAccuracy, ScoreRank BestRank);
    }
}
