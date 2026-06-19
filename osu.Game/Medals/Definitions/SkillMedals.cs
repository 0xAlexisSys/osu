// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Scoring;
using StarRatingRange = (double min, double max);

namespace osu.Game.Medals.Definitions
{
    public class ComboClearMedal : Medal
    {
        public override string DisplayName { get; }
        public override string InternalName { get; }
        public override string Description { get; }
        public override string HoverDescription { get; }

        private readonly int requiredMaxCombo;

        public ComboClearMedal(string internalName, string name, string description, string hoverDescription, int requiredMaxCombo)
        {
            DisplayName = name;
            InternalName = internalName;
            Description = description;
            HoverDescription = hoverDescription;
            this.requiredMaxCombo = requiredMaxCombo;
        }

        public override bool CanUnlock(MedalEvaluationContext context)
        {
            var score = context.LatestScore;

            return score is { Rank: > ScoreRank.F }
                   && score.BeatmapInfo is { Length: >= 0.0d } //90000.0d }
                   && score.MaxCombo >= requiredMaxCombo;
        }
    }

    public class StarRatingPassMedal : Medal
    {
        public override string DisplayName { get; }
        public override string InternalName { get; }
        public override string Description { get; }
        public override string HoverDescription { get; }

        private readonly StarRatingRange starRatingRange;
        private readonly string requiredRulesetShortName;

        public StarRatingPassMedal(string internalName, string name, string description, string hoverDescription, StarRatingRange starRatingRange, string requiredRulesetShortName)
        {
            DisplayName = name;
            InternalName = internalName;
            Description = description;
            HoverDescription = hoverDescription;
            this.starRatingRange = starRatingRange;
            this.requiredRulesetShortName = requiredRulesetShortName;
        }

        public override bool CanUnlock(MedalEvaluationContext context)
        {
            var score = context.LatestScore;

            return score is { Rank: > ScoreRank.F }
                   && score.Ruleset.ShortName == requiredRulesetShortName
                   && score.BeatmapInfo is { Length: >= 0.0d } //90000.0d }
                   && score.BeatmapInfo.StarRating >= starRatingRange.min
                   && score.BeatmapInfo.StarRating < starRatingRange.max;
        }
    }

    public class StarRatingFullComboMedal : Medal
    {
        public override string DisplayName { get; }
        public override string InternalName { get; }
        public override string Description { get; }
        public override string HoverDescription { get; }

        private readonly StarRatingRange starRatingRange;
        private readonly string requiredRulesetShortName;

        public StarRatingFullComboMedal(string internalName, string name, string description, string hoverDescription, StarRatingRange starRatingRange, string requiredRulesetShortName)
        {
            DisplayName = name;
            InternalName = internalName;
            Description = description;
            HoverDescription = hoverDescription;
            this.starRatingRange = starRatingRange;
            this.requiredRulesetShortName = requiredRulesetShortName;
        }

        public override bool CanUnlock(MedalEvaluationContext context)
        {
            var score = context.LatestScore;

            return score is { Rank: > ScoreRank.F }
                   && score.Ruleset.ShortName == requiredRulesetShortName
                   && score.BeatmapInfo is { Length: >= 0.0d } //90000.0d }
                   && score.BeatmapInfo.StarRating >= starRatingRange.min
                   && score.BeatmapInfo.StarRating < starRatingRange.max
                   && score.MaxCombo >= score.GetMaximumAchievableCombo();
        }
    }
}
