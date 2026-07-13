// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Screens.Select;

namespace osu.Game.Tests.Visual.SongSelect
{
    public partial class TestSceneBeatmapMetadataWedge : SongSelectComponentsTestScene
    {
        private BeatmapMetadataWedge wedge = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Child = wedge = new BeatmapMetadataWedge
            {
                State = { Value = Visibility.Visible },
            };
        }

        [Test]
        public void TestShowHide()
        {
            AddStep("all metrics", () => Beatmap.Value = createTestBeatmap());

            AddStep("hide wedge", () => wedge.Hide());
            AddStep("show wedge", () => wedge.Show());
        }

        [Test]
        public void TestVariousMetrics()
        {
            AddStep("all metrics", () => Beatmap.Value = createTestBeatmap());

            AddStep("null beatmap", () => Beatmap.SetDefault());
            AddStep("no source", () =>
            {
                var working = createTestBeatmap();

                working.Metadata.Source = string.Empty;

                Beatmap.Value = working;
            });
            AddStep("local beatmap", () =>
            {
                var working = createTestBeatmap();

                Beatmap.Value = working;
            });
        }

        [Test]
        public void TestTruncation()
        {
            AddStep("long text", () =>
            {
                var working = createTestBeatmap();

                working.BeatmapInfo.Metadata.Author = "Verrrrryyyy llooonngggggg author";
                working.BeatmapInfo.Metadata.Source = "Verrrrryyyy llooonngggggg source";
                working.BeatmapInfo.Metadata.Tags = string.Join(' ', Enumerable.Repeat(working.BeatmapInfo.Metadata.Tags, 3));

                Beatmap.Value = working;
            });
        }

        private WorkingBeatmap createTestBeatmap() => CreateWorkingBeatmap(Ruleset.Value);
    }
}
