// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModBrittleSpirit : Mod, IApplicableToHealthProcessor
    {
        public override string Name { get; } = "Brittle Spirit";
        public override string Acronym { get; } = "BS";
        public override IconUsage? Icon { get; } = OsuIcon.Maintenance;
        public override ModType Type { get; } = ModType.DifficultyIncrease;
        public override LocalisableString Description { get; } = "Passive HP drain can cause you to fail.";

        public override Type[] IncompatibleMods { get; } =
        [
            typeof(OsuModLifeBandage),
        ];

        public void ApplyToHealthProcessor(HealthProcessor healthProcessor)
        {
            if (healthProcessor is DrainingHealthProcessor drainingHealthProcessor) drainingHealthProcessor.PassiveFailAllowed = true;
        }
    }
}
