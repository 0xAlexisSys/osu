// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Mania.Mods
{
    public class ManiaModDifficultyAdjust : ModDifficultyAdjust
    {
        public override DifficultyBindable OverallDifficulty { get; } = new DifficultyBindable
        {
            Precision = 0.05f,
            MinValue = 0.0f,
            MaxValue = 10.0f,
            // Use larger extended limits for mania to include OD values that occur with EZ or HR enabled
            ExtendedMaxValue = 20.0f,
            ExtendedMinValue = -20.0f,
            ReadCurrentFromDifficulty = diff => diff.OverallDifficulty,
        };
    }
}
