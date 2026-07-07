// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Difficulty
{
    /// <summary>
    /// Describes the difficulty of a beatmap, as output by a <see cref="DifficultyCalculator"/>.
    /// </summary>
    public class DifficultyAttributes
    {
        /// <summary>
        /// The mods which were applied to the beatmap.
        /// </summary>
        public Mod[] Mods { get; init; } = [];

        /// <summary>
        /// The combined star rating of all skills.
        /// </summary>
        public double StarRating { get; init; }

        /// <summary>
        /// The maximum achievable combo.
        /// </summary>
        public int MaxCombo { get; init; }
    }
}
