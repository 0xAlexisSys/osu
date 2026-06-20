// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Scoring;
using osuTK;

namespace osu.Game.Screens.Ranking
{
    public partial class ExportReplayButton : GrayButton
    {
        public readonly ScoreInfo? Score;

        [Resolved]
        private ScoreManager scoreManager { get; set; } = null!;

        public ExportReplayButton(ScoreInfo? score)
            : base(FontAwesome.Solid.FileExport)
        {
            Score = score;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Enabled.Value = Score is not null;
            Size = new Vector2(75.0f, 30.0f);
            TooltipText = @"export replay";
            Action = () => scoreManager.Export(Score!);
        }
    }
}
