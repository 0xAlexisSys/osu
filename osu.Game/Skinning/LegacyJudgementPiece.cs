// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Screens.Play;

namespace osu.Game.Skinning
{
    public partial class LegacyJudgementPiece : CompositeDrawable, IAnimatableJudgement
    {
        private static readonly ImmutableArray<string> cached_special_texture_paths =
        [
            @"hit100k", // Katu 100
            @"hit300k", // Katu 300
            @"hit300g", // Geki 300
        ];

        private readonly HitResult result;
        private readonly IAnimatableJudgement regularPiece;
        private readonly Dictionary<string, IAnimatableJudgement> alternativePieces = [];
        private IAnimatableJudgement activePiece;

        [Resolved(canBeNull: true)]
        private GameplayState? gameplayState { get; set; }

        public LegacyJudgementPiece(HitResult result, IAnimatableJudgement regularPiece, Func<string, Drawable?> createAnimationByName, Texture? particle)
        {
            this.result = result;
            this.regularPiece = regularPiece;
            activePiece = regularPiece;

            AutoSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            AddInternal(((Drawable)regularPiece).With(d =>
            {
                d.Anchor = Anchor;
                d.Origin = Origin;
            }));

            foreach (string alternativeAnimationName in getAlternativeAnimationNamesForResult())
            {
                if (createAnimationByName(alternativeAnimationName) is null) continue;

                Func<Drawable> createAlternativeAnimation = () => createAnimationByName(alternativeAnimationName)!;

                IAnimatableJudgement alternativePiece = particle is not null
                    ? new LegacyJudgementPieceNew(result, createAlternativeAnimation, particle)
                    : new LegacyJudgementPieceOld(result, createAlternativeAnimation);

                AddInternal(((Drawable)alternativePiece).With(d =>
                {
                    d.Anchor = Anchor;
                    d.Origin = Origin;
                    d.Alpha = 0.0f;
                }));
                alternativePieces[alternativeAnimationName] = alternativePiece;
            }
        }

        public void PlayAnimation()
        {
            // [alexis] Reset to the regular piece, hiding all alternative pieces.
            ((Drawable)regularPiece).Alpha = 1.0f;

            foreach (var alternativePiece in alternativePieces.Values) ((Drawable)alternativePiece).Alpha = 0.0f;

            activePiece = regularPiece;

            string? alternativeAnimationName = getAlternativeAnimationName();

            if (alternativeAnimationName is not null && alternativePieces.TryGetValue(alternativeAnimationName, out var newActivePiece))
            {
                ((Drawable)regularPiece).Alpha = 0.0f;
                ((Drawable)newActivePiece).Alpha = 1.0f;
                activePiece = newActivePiece;
            }

            activePiece.PlayAnimation();
        }

        public Drawable? GetAboveHitObjectsProxiedContent() => CreateProxy();

        private IEnumerable<string> getAlternativeAnimationNamesForResult()
        {
            switch (result)
            {
                case HitResult.Ok:
                    yield return cached_special_texture_paths[0];

                    break;

                case HitResult.Great:
                    yield return cached_special_texture_paths[1];
                    yield return cached_special_texture_paths[2];

                    break;
            }
        }

        private string? getAlternativeAnimationName()
        {
            if (gameplayState?.ScoreProcessor.Ruleset.ShortName != @"osu")
                throw new InvalidOperationException($@"{nameof(getAlternativeAnimationName)} should not be called from ruleset '{gameplayState?.ScoreProcessor.Ruleset.ShortName ?? @"unknown"}'");

            var specialJudgement = gameplayState?.GetSpecialJudgement() ?? SpecialJudgement.None;
            return specialJudgement switch
            {
                SpecialJudgement.None => null,
                SpecialJudgement.Katu => result switch
                {
                    HitResult.Ok => cached_special_texture_paths[0],
                    HitResult.Great => cached_special_texture_paths[1],
                    _ => throw new InvalidEnumArgumentException(nameof(result), (int)result, typeof(HitResult)),
                },
                SpecialJudgement.Geki => cached_special_texture_paths[2],
                _ => throw new InvalidEnumArgumentException(nameof(specialJudgement), (int)specialJudgement, typeof(SpecialJudgement)),
            };
        }
    }
}
