// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Medals
{
    public abstract class Medal
    {
        public abstract string InternalName { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public abstract string? HoverDescription { get; }

        public string Icon => $@"Medals/{InternalName}";

        public abstract bool CanUnlock(MedalEvaluationContext context);
    }
}
