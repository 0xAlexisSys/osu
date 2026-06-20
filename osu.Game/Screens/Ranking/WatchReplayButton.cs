// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Scoring;
using osuTK;

namespace osu.Game.Screens.Ranking
{
    public partial class WatchReplayButton : GrayButton
    {
        public readonly ScoreInfo? Score;

        [Resolved]
        private OsuGame game { get; set; } = null!;

        public WatchReplayButton(ScoreInfo? score)
            : base(FontAwesome.Solid.Play)
        {
            Score = score;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Enabled.Value = Score is not null;
            Size = new Vector2(75.0f, 30.0f);
            TooltipText = @"watch replay";
            Action = () => game.PresentScore(Score!, ScorePresentType.Gameplay);
        }
    }
}
