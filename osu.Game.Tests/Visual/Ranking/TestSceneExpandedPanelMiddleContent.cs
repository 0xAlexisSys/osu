// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Scoring;
using osu.Game.Screens.Ranking;
using osu.Game.Screens.Ranking.Expanded;
using osu.Game.Tests.Beatmaps;
using osu.Game.Tests.Resources;
using osuTK;

namespace osu.Game.Tests.Visual.Ranking
{
    public partial class TestSceneExpandedPanelMiddleContent : OsuTestScene
    {
        [Resolved]
        private RulesetStore rulesetStore { get; set; }

        [Test]
        public void TestMapWithKnownMapper()
        {
            AddStep("show example score", () => showPanel(TestResources.CreateTestScoreInfo(createTestBeatmap("mapper_name"))));
        }

        [Test]
        public void TestExcessMods()
        {
            AddStep("show excess mods score", () =>
            {
                var score = TestResources.CreateTestScoreInfo(createTestBeatmap("mapper_name"));
                score.Mods = score.BeatmapInfo!.Ruleset.CreateInstance().CreateAllMods().ToArray();

                showPanel(score);
            });

            AddAssert("mapper name present", () => this.ChildrenOfType<OsuSpriteText>().Any(spriteText => spriteText.Current.Value == "mapper_name"));
        }

        [Test]
        public void TestMapWithUnknownMapper()
        {
            AddStep("show example score", () => showPanel(TestResources.CreateTestScoreInfo(createTestBeatmap(string.Empty))));

            AddAssert("mapped by text not present", () =>
                this.ChildrenOfType<OsuSpriteText>().All(spriteText => !containsAny(spriteText.Text.ToString(), "mapped", "by")));

            AddAssert("play time displayed", () => this.ChildrenOfType<PlayedOnText>().Any());
        }

        [Test]
        public void TestWithDefaultDate()
        {
            AddStep("show autoplay score", () =>
            {
                var ruleset = new OsuRuleset();

                var mods = new Mod[] { ruleset.GetAutoplayMod() };
                var beatmap = createTestBeatmap(string.Empty);

                var score = TestResources.CreateTestScoreInfo(beatmap);

                score.Mods = mods;
                score.Date = default;

                showPanel(score);
            });

            AddAssert("play time not displayed", () => !this.ChildrenOfType<PlayedOnText>().Any());
        }

        [Test]
        public void TestFailedSDisplay([Values] bool withFlair)
        {
            AddStep("show failed S score", () =>
            {
                var score = TestResources.CreateTestScoreInfo(createTestBeatmap(string.Empty));
                score.Rank = ScoreRank.A;
                score.Accuracy = 0.975;
                showPanel(score, withFlair);
            });
        }

        private void showPanel(ScoreInfo score, bool withFlair = false) =>
            Child = new ExpandedPanelMiddleContentContainer(score, withFlair);

        private BeatmapInfo createTestBeatmap(string author)
        {
            var beatmap = new TestBeatmap(rulesetStore.GetRuleset(0)!).BeatmapInfo;

            beatmap.Metadata.Author = author;
            beatmap.Metadata.Title = "Verrrrrrrrrrrrrrrrrrry looooooooooooooooooooooooong beatmap title";
            beatmap.Metadata.Artist = "Verrrrrrrrrrrrrrrrrrry looooooooooooooooooooooooong beatmap artist";
            beatmap.DifficultyName = "Verrrrrrrrrrrrrrrrrrry looooooooooooooooooooooooong difficulty name";

            return beatmap;
        }

        private bool containsAny(string text, params string[] stringsToMatch) => stringsToMatch.Any(text.Contains);

        private partial class ExpandedPanelMiddleContentContainer : Container
        {
            public ExpandedPanelMiddleContentContainer(ScoreInfo score, bool withFlair)
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
                Size = new Vector2(ScorePanel.EXPANDED_WIDTH, 700);
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4Extensions.FromHex("#444"),
                    },
                    new ExpandedPanelMiddleContent(score, withFlair)
                };
            }
        }
    }
}
