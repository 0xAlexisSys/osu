// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Osu.Objects;

namespace osu.Game.Rulesets.Osu.Difficulty
{
    public class OsuDifficultyAttributes : DifficultyAttributes
    {
        /// <summary>
        /// The difficulty corresponding to the aim skill.
        /// </summary>
        public double AimDifficulty { get; init; }

        /// <summary>
        /// The number of <see cref="Slider"/>s weighted by difficulty.
        /// </summary>
        public double AimDifficultSliderCount { get; init; }

        /// <summary>
        /// The difficulty corresponding to the speed skill.
        /// </summary>
        public double SpeedDifficulty { get; init; }

        /// <summary>
        /// The number of clickable objects weighted by difficulty.
        /// Related to <see cref="SpeedDifficulty"/>
        /// </summary>
        public double SpeedNoteCount { get; init; }

        /// <summary>
        /// The difficulty corresponding to the flashlight skill.
        /// </summary>
        public double FlashlightDifficulty { get; init; }

        /// <summary>
        /// The difficulty corresponding to the reading skill.
        /// </summary>
        public double ReadingDifficulty { get; init; }

        /// <summary>
        /// Describes how much of <see cref="AimDifficulty"/> is contributed to by hitcircles or sliders.
        /// A value closer to 1.0 indicates most of <see cref="AimDifficulty"/> is contributed by hitcircles.
        /// A value closer to 0.0 indicates most of <see cref="AimDifficulty"/> is contributed by sliders.
        /// </summary>
        public double SliderFactor { get; init; }

        /// <summary>
        /// Describes how much of <see cref="AimDifficultStrainCount"/> is contributed to by hitcircles or sliders
        /// A value closer to 0.0 indicates most of <see cref="AimDifficultStrainCount"/> is contributed by hitcircles
        /// A value closer to Infinity indicates most of <see cref="AimDifficultStrainCount"/> is contributed by sliders
        /// </summary>
        public double AimTopWeightedSliderFactor { get; init; }

        /// <summary>
        /// Describes how much of <see cref="SpeedDifficultStrainCount"/> is contributed to by hitcircles or sliders
        /// A value closer to 0.0 indicates most of <see cref="SpeedDifficultStrainCount"/> is contributed by hitcircles
        /// A value closer to Infinity indicates most of <see cref="SpeedDifficultStrainCount"/> is contributed by sliders
        /// </summary>
        public double SpeedTopWeightedSliderFactor { get; init; }

        public double AimDifficultStrainCount { get; init; }

        public double SpeedDifficultStrainCount { get; init; }

        public double ReadingDifficultNoteCount { get; init; }

        public double NestedScorePerObject { get; init; }

        public double LegacyScoreBaseMultiplier { get; init; }

        public double MaximumLegacyComboScore { get; init; }

        /// <summary>
        /// The number of hitcircles in the beatmap.
        /// </summary>
        public int HitCircleCount { get; init; }

        /// <summary>
        /// The number of sliders in the beatmap.
        /// </summary>
        public int SliderCount { get; init; }

        /// <summary>
        /// The number of spinners in the beatmap.
        /// </summary>
        public int SpinnerCount { get; init; }
    }
}
