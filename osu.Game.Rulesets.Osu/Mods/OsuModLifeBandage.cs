// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModLifeBandage : Mod, IApplicableToHealthProcessor
    {
        public override string Name => "Life Bandage";
        public override string Acronym => "LB";
        public override IconUsage? Icon => OsuIcon.Heart;
        public override ModType Type => ModType.DifficultyReduction;
        public override LocalisableString Description => "Weakens passive HP drain.";

        [SettingSource("Lenience", "0.5 halves the passive HP drain rate; 1 completely removes passive HP drain.")]
        public BindableDouble DrainLenience { get; } = new BindableDouble(1.0d)
        {
            Precision = 0.005d,
            MinValue = 0.005d,
            MaxValue = 1.0d,
        };

        public void ApplyToHealthProcessor(HealthProcessor healthProcessor)
        {
            if (healthProcessor is DrainingHealthProcessor drainingHealthProcessor) drainingHealthProcessor.DrainLenience = DrainLenience.Value;
        }
    }
}
