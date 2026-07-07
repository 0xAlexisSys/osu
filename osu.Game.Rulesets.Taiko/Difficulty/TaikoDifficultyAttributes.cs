// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Difficulty;

namespace osu.Game.Rulesets.Taiko.Difficulty
{
    public class TaikoDifficultyAttributes : DifficultyAttributes
    {
        /// <summary>
        /// The difficulty corresponding to the mechanical skills in osu!taiko.
        /// This includes colour and stamina combined.
        /// </summary>
        public double MechanicalDifficulty { get; init; }

        /// <summary>
        /// The difficulty corresponding to the rhythm skill.
        /// </summary>
        public double RhythmDifficulty { get; init; }

        /// <summary>
        /// The difficulty corresponding to the reading skill.
        /// </summary>
        public double ReadingDifficulty { get; init; }

        /// <summary>
        /// The difficulty corresponding to the colour skill.
        /// </summary>
        public double ColourDifficulty { get; init; }

        /// <summary>
        /// The difficulty corresponding to the stamina skill.
        /// </summary>
        public double StaminaDifficulty { get; init; }

        /// <summary>
        /// The ratio of stamina difficulty from mono-color (single colour) streams to total stamina difficulty.
        /// </summary>
        public double MonoStaminaFactor { get; init; }

        /// <summary>
        /// The factor corresponding to the consistency of a map.
        /// </summary>
        public double ConsistencyFactor { get; init; }

        public double StaminaTopStrains { get; init; }
    }
}
