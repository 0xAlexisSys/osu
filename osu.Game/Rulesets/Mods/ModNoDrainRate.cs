// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Mods
{
    public abstract class ModNoDrainRate : Mod, IApplicableToHealthProcessor
    {
        public override string Name => "No HP Drain";
        public override string Acronym => "ND";
        public override IconUsage? Icon => OsuIcon.Heart;
        public override ModType Type => ModType.DifficultyReduction;
        public override LocalisableString Description => "Passive HP drain is disabled.";

        public void ApplyToHealthProcessor(HealthProcessor healthProcessor)
        {
            if (healthProcessor is DrainingHealthProcessor drainingHealthProcessor) drainingHealthProcessor.DrainLenience = 1.0d;
        }
    }
}
