// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Objects;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModStrictSpinning : Mod, IApplicableToHitObject
    {
        public override string Name { get; } = "Strict Spinning";
        public override string Acronym { get; } = "SS";
        public override IconUsage? Icon { get; } = OsuIcon.EditorSpinner;
        public override ModType Type { get; } = ModType.DifficultyIncrease;
        public override LocalisableString Description { get; } = "Spinners must be fully cleared.";

        public override Type[] IncompatibleMods { get; } =
        [
            typeof(OsuModLifeBandage),
            typeof(OsuModTargetPractice),
        ];

        [SettingSource("Spin Difficulty", "How hard it is to clear a spinner.")]
        public BindableFloat SpinDifficulty { get; } = new BindableFloat(1.0f)
        {
            Precision = 0.01f,
            MinValue = 1.0f,
            MaxValue = 1.5f,
        };

        public void ApplyToHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner spinner)
            {
                spinner.FullClearRequired = true;
                spinner.SpinDifficulty = SpinDifficulty.Value;
            }
        }
    }
}
