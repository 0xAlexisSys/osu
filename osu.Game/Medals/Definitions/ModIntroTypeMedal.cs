// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;

namespace osu.Game.Medals.Definitions
{
    public class ModIntroTypeMedal : Medal
    {
        public override string InternalName { get; }
        public override string DisplayName { get; }
        public override string Description { get; }
        public override string HoverDescription { get; }

        private readonly ModType requirement;

        public ModIntroTypeMedal(string internalName, string name, string description, string hoverDescription, ModType requirement)
        {
            InternalName = internalName;
            DisplayName = name;
            Description = description;
            HoverDescription = hoverDescription;
            this.requirement = requirement;
        }

        public override bool CanUnlock(MedalEvaluationContext context)
        {
            var score = context.LatestScore;

            return score is { Rank: > ScoreRank.F }
                   && score.Mods.Any(m => m.Type == requirement);
        }
    }
}
