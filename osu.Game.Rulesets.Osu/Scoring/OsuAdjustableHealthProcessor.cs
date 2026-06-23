// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Scoring
{
    public partial class OsuAdjustableHealthProcessor : OsuHealthProcessor
    {
        public required double GreatValue { get; init; }
        public required double OkValue { get; init; }
        public required double MehValue { get; init; }
        public required double MissValue { get; init; }
        public required double LargeTickHitValue { get; init; }
        public required double LargeTickMissValue { get; init; }
        public required double SmallTickHitValue { get; init; }
        public required double SmallTickMissValue { get; init; }
        public required double LargeBonusValue { get; init; }
        public required double SmallBonusValue { get; init; }

        public OsuAdjustableHealthProcessor(double drainStartTime, double drainLenience = 0.0d)
            : base(drainStartTime, drainLenience) { }

        protected override double GetHealthIncreaseFor(JudgementResult result)
        {
            switch (result.Type)
            {
                case HitResult.Great:
                    return GreatValue;

                case HitResult.Ok:
                    return OkValue;

                case HitResult.Meh:
                    return MehValue;

                case HitResult.Miss:
                    return MissValue;

                case HitResult.SliderTailHit:
                case HitResult.LargeTickHit:
                    return LargeTickHitValue;

                case HitResult.SmallTickHit:
                    return SmallTickHitValue;

                case HitResult.LargeTickMiss:
                    return LargeTickMissValue;

                case HitResult.SmallTickMiss:
                    return SmallTickMissValue;

                case HitResult.SmallBonus:
                    return SmallBonusValue;

                case HitResult.LargeBonus:
                    return LargeBonusValue;

                default:
                    return base.GetHealthIncreaseFor(result);
            }
        }
    }
}
