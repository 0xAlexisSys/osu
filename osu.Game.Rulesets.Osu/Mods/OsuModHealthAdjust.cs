// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Scoring;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModHealthAdjust : Mod, IApplicableMod, IApplicableHealthProcessor
    {
        public override string Name => "Health Adjust";
        public override string Acronym => "HA";
        public override IconUsage? Icon => OsuIcon.Star;
        public override ModType Type => ModType.Conversion;
        public override LocalisableString Description => "Adjust the amount of health given on hit judgement.";

        public override Type[] IncompatibleMods => new[]
        {
            typeof(OsuModClassic),
        };

        [SettingSource("Health on Great")]
        public BindableDouble Great { get; } = createBindableHealthValue(0.05d);

        [SettingSource("Health on Ok")]
        public BindableDouble Ok { get; } = createBindableHealthValue(0.025d);

        [SettingSource("Health on Meh")]
        public BindableDouble Meh { get; } = createBindableHealthValue(0.0025d);

        [SettingSource("Health on Miss")]
        public BindableDouble Miss { get; } = createBindableHealthValue(-0.1d);

        [SettingSource("Health on LargeTickHit (sliders)")]
        public BindableDouble LargeTickHit { get; } = createBindableHealthValue(0.05d);

        [SettingSource("Health on LargeTickMiss (sliders)")]
        public BindableDouble LargeTickMiss { get; } = createBindableHealthValue(-0.05d);

        [SettingSource("Health on SmallTickHit (sliders)")]
        public BindableDouble SmallTickHit { get; } = createBindableHealthValue(0.025d);

        [SettingSource("Health on SmallTickMiss (sliders)")]
        public BindableDouble SmallTickMiss { get; } = createBindableHealthValue(-0.025d);

        [SettingSource("Health on LargeBonus (spinners)")]
        public BindableDouble LargeBonus { get; } = createBindableHealthValue(0.05d);

        [SettingSource("Health on SmallBonus (spinners)")]
        public BindableDouble SmallBonus { get; } = createBindableHealthValue(0.025d);

        public HealthProcessor CreateHealthProcessor(double drainStartTime) => new OsuAdjustableHealthProcessor(drainStartTime)
        {
            GreatValue = Great.Value,
            OkValue = Ok.Value,
            MehValue = Meh.Value,
            MissValue = Miss.Value,
            LargeTickHitValue = LargeTickHit.Value,
            LargeTickMissValue = LargeTickMiss.Value,
            SmallTickHitValue = SmallTickHit.Value,
            SmallTickMissValue = SmallTickMiss.Value,
            LargeBonusValue = LargeBonus.Value,
            SmallBonusValue = SmallBonus.Value,
        };

        private static BindableDouble createBindableHealthValue(double defaultValue) => new BindableDouble(defaultValue)
        {
            Precision = 0.0025d,
            MinValue = -0.25d,
            MaxValue = 0.25d,
        };
    }
}
