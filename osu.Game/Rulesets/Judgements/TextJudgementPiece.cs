// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Rulesets.Scoring;
using osu.Game.Screens.Play;
using PropertyPair = (osu.Framework.Graphics.Colour.ColourInfo colour, osu.Framework.Localisation.LocalisableString text);

namespace osu.Game.Rulesets.Judgements
{
    public abstract partial class TextJudgementPiece : CompositeDrawable
    {
        private static readonly Dictionary<HitResult, PropertyPair> cached_regular_property_pairs = [];

        private static readonly ImmutableArray<PropertyPair> cached_special_property_pairs =
        [
            (Color4Extensions.FromHex(@"b3d944"), HitResult.Good.GetDescription().ToUpperInvariant()), // Katu 100
            (Color4Extensions.FromHex(@"99eeff"), HitResult.Good.GetDescription().ToUpperInvariant()), // Katu 300
            (Color4Extensions.FromHex(@"ddffff"), HitResult.Perfect.GetDescription().ToUpperInvariant()), // Geki 300
        ];

        protected readonly HitResult Result;

        protected SpriteText Label { get; private set; } = null!;

        [Resolved(canBeNull: true)]
        private GameplayState? gameplayState { get; set; }

        [Resolved]
        private OsuColour colours { get; set; } = null!;

        protected TextJudgementPiece(HitResult result)
        {
            Result = result;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AddInternal(Label = CreateJudgementText().With(d =>
            {
                var regularPropertyPair = getCachedRegularPropertyPair();
                d.Colour = regularPropertyPair.colour;
                d.Text = regularPropertyPair.text;
            }));
        }

        protected abstract SpriteText CreateJudgementText();

        protected PropertyPair TryGetPropertyPairForSpecialJudgement()
        {
            if (gameplayState?.ScoreProcessor.Ruleset.ShortName != @"osu")
            {
                throw new InvalidOperationException(
                    $@"{nameof(TryGetPropertyPairForSpecialJudgement)} should not be called from ruleset '{gameplayState?.ScoreProcessor.Ruleset.ShortName ?? @"unknown"}'");
            }

            var specialJudgement = gameplayState?.GetSpecialJudgement() ?? SpecialJudgement.None;
            return specialJudgement switch
            {
                SpecialJudgement.None => getCachedRegularPropertyPair(),
                SpecialJudgement.Katu => Result switch
                {
                    HitResult.Ok => cached_special_property_pairs[0],
                    HitResult.Great => cached_special_property_pairs[1],
                    _ => throw new InvalidEnumArgumentException(nameof(Result), (int)Result, typeof(HitResult)),
                },
                SpecialJudgement.Geki => cached_special_property_pairs[2],
                _ => throw new InvalidEnumArgumentException(nameof(specialJudgement), (int)specialJudgement, typeof(SpecialJudgement)),
            };
        }

        private PropertyPair getCachedRegularPropertyPair()
        {
            if (!cached_regular_property_pairs.TryGetValue(Result, out var regularPropertyPair))
            {
                regularPropertyPair = (colours.ForHitResult(Result), Result.GetDescription().ToUpperInvariant());
                cached_regular_property_pairs.Add(Result, regularPropertyPair);
            }

            return regularPropertyPair;
        }
    }
}
