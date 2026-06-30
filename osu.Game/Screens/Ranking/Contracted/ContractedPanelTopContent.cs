// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;

namespace osu.Game.Screens.Ranking.Contracted
{
    public partial class ContractedPanelTopContent : CompositeDrawable
    {
        public readonly int? ScorePosition;

        public ContractedPanelTopContent(int? scorePosition)
        {
            ScorePosition = scorePosition;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            InternalChild = new OsuSpriteText
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = 6,
                Font = OsuFont.GetFont(size: 18, weight: FontWeight.Bold),
                Text = ScorePosition is not null ? $"#{ScorePosition}" : string.Empty,
            };
        }
    }
}
