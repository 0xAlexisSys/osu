// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Medals.Definitions
{
    public class PlayCountMedal : Medal
    {
        public override string DisplayName { get; }
        public override string InternalName { get; }
        public override string Description { get; }
        public override string? HoverDescription => null;

        private readonly int requirement;

        public PlayCountMedal(string internalName, string name, string description, int requirement)
        {
            InternalName = internalName;
            DisplayName = name;
            Description = description;
            this.requirement = requirement;
        }

        public override bool CanUnlock(MedalEvaluationContext context)
        {
            return context.TotalPlayCount >= requirement;
        }
    }
}
