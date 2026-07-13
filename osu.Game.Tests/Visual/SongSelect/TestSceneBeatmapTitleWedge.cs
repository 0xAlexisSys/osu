// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Legacy;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Screens.Select;
using osu.Game.Skinning;

namespace osu.Game.Tests.Visual.SongSelect
{
    public partial class TestSceneBeatmapTitleWedge : SongSelectComponentsTestScene
    {
        private RulesetStore rulesets = null!;

        private BeatmapTitleWedge titleWedge = null!;
        private BeatmapTitleWedge.DifficultyDisplay difficultyDisplay => titleWedge.ChildrenOfType<BeatmapTitleWedge.DifficultyDisplay>().Single();

        [BackgroundDependencyLoader]
        private void load(RulesetStore rulesets)
        {
            this.rulesets = rulesets;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddRange(new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Shear = OsuGame.SHEAR,
                    Children = new Drawable[]
                    {
                        titleWedge = new BeatmapTitleWedge
                        {
                            State = { Value = Visibility.Visible },
                        },
                    },
                }
            });

            AddSliderStep("change star difficulty", 0, 11.9, 4.18, v =>
            {
                difficultyDisplay.ChildrenOfType<StarRatingDisplay>().Single().Current.Value = new StarDifficulty(v);
            });
        }

        [Test]
        public void TestRulesetChange()
        {
            selectBeatmap(Beatmap.Value.Beatmap);

            AddWaitStep("wait for select", 3);

            foreach (var rulesetInfo in rulesets.AvailableRulesets)
            {
                var testBeatmap = createTestBeatmapFromRuleset(rulesetInfo);

                setRuleset(rulesetInfo);
                selectBeatmap(testBeatmap);
            }
        }

        [Test]
        public void TestNullBeatmap()
        {
            selectBeatmap(null);
            AddAssert("check default title", () => titleWedge.DisplayedTitle == Beatmap.Default.BeatmapInfo.Metadata.Title);
            AddAssert("check default artist", () => titleWedge.DisplayedArtist == Beatmap.Default.BeatmapInfo.Metadata.Artist);
            AddAssert("statistics not visible",
                () => difficultyDisplay.ChildrenOfType<BeatmapTitleWedge.DifficultyStatisticsDisplay>()
                                       .All(d => d.Alpha == 0 || d.ChildrenOfType<BeatmapTitleWedge.StatisticDifficulty>().All(s => s.Alpha == 0)));
        }

        [Test]
        public void TestBPMUpdates()
        {
            const double bpm = 120;
            IBeatmap beatmap = createTestBeatmapFromRuleset(new OsuRuleset().RulesetInfo);
            beatmap.ControlPointInfo.Add(0, new TimingControlPoint { BeatLength = 60 * 1000 / bpm });

            OsuModDoubleTime doubleTime = null!;

            selectBeatmap(beatmap);
            checkDisplayedBPM($"{bpm}");

            AddStep("select DT", () => SelectedMods.Value = new[] { doubleTime = new OsuModDoubleTime() });
            checkDisplayedBPM($"{bpm * 1.5f}");

            AddStep("change DT rate", () => doubleTime.SpeedChange.Value = 2);
            checkDisplayedBPM($"{bpm * 2}");

            AddStep("select HT", () => SelectedMods.Value = new[] { new OsuModHalfTime() });
            checkDisplayedBPM($"{bpm * 0.75f}");
        }

        [Test]
        public void TestWedgeVisibility()
        {
            AddStep("hide", () => { titleWedge.Hide(); });
            AddWaitStep("wait for hide", 3);
            AddAssert("check visibility", () => titleWedge.Alpha == 0);
            AddStep("show", () => { titleWedge.Show(); });
            AddWaitStep("wait for show", 1);
            AddAssert("check visibility", () => titleWedge.Alpha > 0);
        }

        // TODO: [alexis] refactor this later

#if NOTHING
        [Test]
        public void TestFavouriting()
        {
            var resetEvent = new ManualResetEventSlim(false);

            AddStep("set up request handler", () =>
            {
                ((DummyAPIAccess)API).HandleRequest = request =>
                {
                    switch (request)
                    {
                        case PostBeatmapFavouriteRequest favourite:
                            Task.Run(() =>
                            {
                                resetEvent.Wait(10000);
                                favourite.TriggerSuccess();
                            });
                            return true;

                        default:
                            return false;
                    }
                };
            });

            AddStep("online beatmapset", () => (Beatmap.Value, onlineLookupResult.Value) = createTestBeatmap());

            AddUntilStep("play count is 10000", () => this.ChildrenOfType<BeatmapTitleWedge.Statistic>().ElementAt(0).Text.ToString(), () => Is.EqualTo("10,000"));
            AddUntilStep("favourites count is 2345", () => this.ChildrenOfType<BeatmapTitleWedge.FavouriteButton>().Single().Text.ToString(), () => Is.EqualTo("2,345"));

            AddStep("click favourite button", () => this.ChildrenOfType<BeatmapTitleWedge.FavouriteButton>().Single().TriggerClick());
            AddStep("allow request to complete", resetEvent.Set);
            AddUntilStep("favourites count is 2346", () => this.ChildrenOfType<BeatmapTitleWedge.FavouriteButton>().Single().Text.ToString(), () => Is.EqualTo("2,346"));

            AddStep("reset event", resetEvent.Reset);
            AddStep("click favourite button", () => this.ChildrenOfType<BeatmapTitleWedge.FavouriteButton>().Single().TriggerClick());
            AddStep("allow request to complete", resetEvent.Set);
            AddUntilStep("favourites count is 2345", () => this.ChildrenOfType<BeatmapTitleWedge.FavouriteButton>().Single().Text.ToString(), () => Is.EqualTo("2,345"));

            AddStep("reset event", resetEvent.Reset);
            AddStep("click favourite button", () => this.ChildrenOfType<BeatmapTitleWedge.FavouriteButton>().Single().TriggerClick());
            AddStep("change to another beatmap", () =>
            {
                var (working, online) = createTestBeatmap();
                online.Result!.FavouriteCount = 9999;
                online.Result!.HasFavourited = true;
                working.BeatmapSetInfo.OnlineID = online.Result!.OnlineID = 99999;

                Beatmap.Value = working;
                onlineLookupResult.Value = online;
            });
            AddStep("allow request to complete", resetEvent.Set);
            AddUntilStep("favourites count is 9999", () => this.ChildrenOfType<BeatmapTitleWedge.FavouriteButton>().Single().Text.ToString(), () => Is.EqualTo("9,999"));

            AddStep("set up request handler to fail", () =>
            {
                ((DummyAPIAccess)API).HandleRequest = request =>
                {
                    switch (request)
                    {
                        case PostBeatmapFavouriteRequest favourite:
                            Task.Run(() =>
                            {
                                resetEvent.Wait(10000);
                                favourite.TriggerFailure(new APIException("You have too many favourited beatmaps! Please unfavourite some before trying again.", null));
                            });
                            return true;

                        default:
                            return false;
                    }
                };
            });
            AddStep("reset event", resetEvent.Reset);
            AddStep("click favourite button", () => this.ChildrenOfType<BeatmapTitleWedge.FavouriteButton>().Single().TriggerClick());
            AddUntilStep("spinner visible", () => this.ChildrenOfType<BeatmapTitleWedge.FavouriteButton>().Single()
                                                      .ChildrenOfType<LoadingSpinner>().Single().State.Value, () => Is.EqualTo(Visibility.Visible));
            AddStep("allow request to complete", resetEvent.Set);
            AddUntilStep("spinner hidden", () => this.ChildrenOfType<BeatmapTitleWedge.FavouriteButton>().Single()
                                                     .ChildrenOfType<LoadingSpinner>().Single().State.Value, () => Is.EqualTo(Visibility.Hidden));
        }
#endif

        [TestCase(120, 125, null, "120-125 (mostly 120)")]
        [TestCase(120, 120.6, null, "120-121 (mostly 120)")]
        [TestCase(120, 120.4, null, "120")]
        [TestCase(120, 120.6, "DT", "180-181 (mostly 180)")]
        [TestCase(120, 120.4, "DT", "180-181 (mostly 180)")]
        public void TestVaryingBPM(double commonBpm, double otherBpm, string? mod, string expectedDisplay)
        {
            IBeatmap beatmap = createTestBeatmapFromRuleset(new OsuRuleset().RulesetInfo);
            beatmap.ControlPointInfo.Add(0, new TimingControlPoint { BeatLength = 60 * 1000 / commonBpm });
            beatmap.ControlPointInfo.Add(100, new TimingControlPoint { BeatLength = 60 * 1000 / otherBpm });
            beatmap.ControlPointInfo.Add(200, new TimingControlPoint { BeatLength = 60 * 1000 / commonBpm });

            if (mod is not null)
                AddStep($"select {mod}", () => SelectedMods.Value = new[] { Ruleset.Value.CreateInstance().CreateModFromAcronym(mod) });

            selectBeatmap(beatmap);
            checkDisplayedBPM(expectedDisplay);
        }

        [Test]
        [Explicit]
        public void TestPerformanceWithLongBeatmap()
        {
            AddStep("select heavy beatmap", () => Beatmap.Value = new HeavyWorkingBeatmap(Audio));

            foreach (var rulesetInfo in rulesets.AvailableRulesets)
                setRuleset(rulesetInfo);
        }

        private void setRuleset(RulesetInfo rulesetInfo)
        {
            AddStep("set ruleset", () => Ruleset.Value = rulesetInfo);
        }

        private void selectBeatmap(IBeatmap? b)
        {
            AddStep($"select {b?.Metadata.Title ?? "null"} beatmap", () =>
            {
                Beatmap.Value = b is null ? Beatmap.Default : CreateWorkingBeatmap(b);
            });
        }

        private void checkDisplayedBPM(string target)
        {
            AddUntilStep($"displayed bpm is {target}", () =>
            {
                var label = titleWedge.ChildrenOfType<BeatmapTitleWedge.Statistic>().Single(l => l.TooltipText == BeatmapsetsStrings.ShowStatsBpm);
                return label.Text.ToString() == target;
            });
        }

        private WorkingBeatmap createTestBeatmap() => CreateWorkingBeatmap(Ruleset.Value);

        private static IBeatmap createTestBeatmapFromRuleset(RulesetInfo ruleset)
        {
            List<HitObject> objects = new List<HitObject>();
            for (double i = 0; i < 50000; i += 1000)
                objects.Add(new TestHitObject { StartTime = i });

            return new Beatmap
            {
                BeatmapInfo = new BeatmapInfo
                {
                    Metadata = new BeatmapMetadata
                    {
                        Author = $"{ruleset.ShortName}Author",
                        Artist = $"{ruleset.ShortName}Artist",
                        Source = $"{ruleset.ShortName}Source",
                        Title = $"{ruleset.ShortName}Title"
                    },
                    Ruleset = ruleset,
                    StarRating = 6,
                    DifficultyName = $"{ruleset.ShortName}Version",
                    Difficulty = new BeatmapDifficulty()
                },
                HitObjects = objects
            };
        }

        private class TestHitObject : ConvertHitObject;

        private class HeavyWorkingBeatmap : WorkingBeatmap
        {
            private static readonly BeatmapInfo beatmap_info = new BeatmapInfo
            {
                Metadata = new BeatmapMetadata
                {
                    Author = "osuAuthor",
                    Artist = "osuArtist",
                    Source = "osuSource",
                    Title = "osuTitle"
                },
                Ruleset = new OsuRuleset().RulesetInfo,
                StarRating = 6,
                DifficultyName = "osuVersion",
                Difficulty = new BeatmapDifficulty()
            };

            public HeavyWorkingBeatmap(AudioManager audioManager)
                : base(beatmap_info, audioManager)
            {
            }

            protected override IBeatmap GetBeatmap()
            {
                List<HitObject> objects = new List<HitObject>();

                for (int i = 0; i < 200_000; i++)
                    objects.Add(new TestHitObject { StartTime = i * 1000 });

                return new Beatmap
                {
                    BeatmapInfo = beatmap_info,
                    HitObjects = objects
                };
            }

            public override Texture? GetBackground() => null;
            public override Stream? GetStream(string storagePath) => null;
            protected override Track? GetBeatmapTrack() => null;
            protected internal override ISkin? GetSkin() => null;
        }
    }
}
