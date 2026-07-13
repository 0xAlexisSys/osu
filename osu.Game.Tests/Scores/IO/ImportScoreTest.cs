// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.IO.Archives;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using osu.Game.Tests.Beatmaps.IO;
using osu.Game.Tests.Resources;
using osu.Game.Users;

namespace osu.Game.Tests.Scores.IO
{
    public class ImportScoreTest : ImportTest
    {
        [Test]
        public void TestBasicImport()
        {
            using (HeadlessGameHost host = new CleanRunHeadlessGameHost())
            {
                try
                {
                    var osu = LoadOsuIntoHost(host, true);

                    var beatmap = BeatmapImportHelper.LoadOszIntoOsu(osu, TestResources.GetQuickTestBeatmapForImport()).GetResultSafely();

                    var toImport = new ScoreInfo
                    {
                        Rank = ScoreRank.B,
                        TotalScore = 987654,
                        Accuracy = 0.8,
                        MaxCombo = 500,
                        Combo = 250,
                        User = new User { Name = "Test user" },
                        Date = DateTimeOffset.Now,
                        Ruleset = new OsuRuleset().RulesetInfo,
                        BeatmapInfo = beatmap.Beatmaps.First()
                    };

                    var imported = LoadScoreIntoOsu(osu, toImport);

                    ClassicAssert.AreEqual(toImport.Rank, imported.Rank);
                    ClassicAssert.AreEqual(toImport.TotalScore, imported.TotalScore);
                    ClassicAssert.AreEqual(toImport.Accuracy, imported.Accuracy);
                    ClassicAssert.AreEqual(toImport.MaxCombo, imported.MaxCombo);
                    ClassicAssert.AreEqual(toImport.User.Name, imported.User.Name);
                    ClassicAssert.AreEqual(toImport.Date, imported.Date);
                }
                finally
                {
                    host.Exit();
                }
            }
        }

        [Test]
        public void TestLastPlayedNotUpdatedDueToNewerPlays()
        {
            using (HeadlessGameHost host = new CleanRunHeadlessGameHost())
            {
                try
                {
                    var osu = LoadOsuIntoHost(host, true);

                    var beatmap = BeatmapImportHelper.LoadOszIntoOsu(osu, TestResources.GetQuickTestBeatmapForImport()).GetResultSafely();
                    var beatmapInfo = beatmap.Beatmaps.First();

                    var realmAccess = osu.Dependencies.Get<RealmAccess>();
                    realmAccess.Write(r => r.Find<BeatmapInfo>(beatmapInfo.ID)!.LastPlayed = new DateTimeOffset(2023, 10, 30, 0, 0, 0, TimeSpan.Zero));

                    var toImport = new ScoreInfo
                    {
                        Rank = ScoreRank.B,
                        TotalScore = 987654,
                        Accuracy = 0.8,
                        MaxCombo = 500,
                        Combo = 250,
                        User = new User { Name = "Test user" },
                        Date = new DateTimeOffset(2023, 10, 27, 0, 0, 0, TimeSpan.Zero),
                        Ruleset = new OsuRuleset().RulesetInfo,
                        BeatmapInfo = beatmapInfo
                    };

                    var imported = LoadScoreIntoOsu(osu, toImport);

                    ClassicAssert.AreEqual(toImport.Rank, imported.Rank);
                    ClassicAssert.AreEqual(toImport.TotalScore, imported.TotalScore);
                    ClassicAssert.AreEqual(toImport.Accuracy, imported.Accuracy);
                    ClassicAssert.AreEqual(toImport.MaxCombo, imported.MaxCombo);
                    ClassicAssert.AreEqual(toImport.User.Name, imported.User.Name);
                    ClassicAssert.AreEqual(toImport.Date, imported.Date);

                    Assert.That(imported.BeatmapInfo!.LastPlayed, Is.EqualTo(new DateTimeOffset(2023, 10, 30, 0, 0, 0, TimeSpan.Zero)));
                }
                finally
                {
                    host.Exit();
                }
            }
        }

        [Test]
        public void TestImportMods()
        {
            using (HeadlessGameHost host = new CleanRunHeadlessGameHost())
            {
                try
                {
                    var osu = LoadOsuIntoHost(host, true);

                    var beatmap = BeatmapImportHelper.LoadOszIntoOsu(osu, TestResources.GetQuickTestBeatmapForImport()).GetResultSafely();

                    var toImport = new ScoreInfo
                    {
                        User = new User { Name = "Test user" },
                        BeatmapInfo = beatmap.Beatmaps.First(),
                        Ruleset = new OsuRuleset().RulesetInfo,
                        ClientVersion = "12345",
                        Mods = new Mod[] { new OsuModHardRock(), new OsuModDoubleTime() },
                    };

                    var imported = LoadScoreIntoOsu(osu, toImport);

                    ClassicAssert.True(imported.Mods.Any(m => m is OsuModHardRock));
                    ClassicAssert.True(imported.Mods.Any(m => m is OsuModDoubleTime));
                    Assert.That(imported.ClientVersion, Is.EqualTo(toImport.ClientVersion));
                }
                finally
                {
                    host.Exit();
                }
            }
        }

        [Test]
        public void TestScoreWithInvalidModCombinationsWillNotImport()
        {
            using (HeadlessGameHost host = new CleanRunHeadlessGameHost())
            {
                try
                {
                    var osu = LoadOsuIntoHost(host, true);

                    var beatmap = BeatmapImportHelper.LoadOszIntoOsu(osu, TestResources.GetQuickTestBeatmapForImport()).GetResultSafely();

                    var toImport = new ScoreInfo
                    {
                        User = new User { Name = "Test user" },
                        BeatmapInfo = beatmap.Beatmaps.First(),
                        Ruleset = new OsuRuleset().RulesetInfo,
                        ClientVersion = "12345",
                        Mods = new Mod[] { new OsuModHalfTime(), new OsuModDoubleTime() },
                    };

                    Assert.Throws<InvalidOperationException>(() => LoadScoreIntoOsu(osu, toImport));
                }
                finally
                {
                    host.Exit();
                }
            }
        }

        [Test]
        public void TestImportStatistics()
        {
            using (HeadlessGameHost host = new CleanRunHeadlessGameHost())
            {
                try
                {
                    var osu = LoadOsuIntoHost(host, true);

                    var beatmap = BeatmapImportHelper.LoadOszIntoOsu(osu, TestResources.GetQuickTestBeatmapForImport()).GetResultSafely();

                    var toImport = new ScoreInfo
                    {
                        User = new User { Name = "Test user" },
                        BeatmapInfo = beatmap.Beatmaps.First(),
                        Ruleset = new OsuRuleset().RulesetInfo,
                        Statistics = new Dictionary<HitResult, int>
                        {
                            { HitResult.Perfect, 100 },
                            { HitResult.Miss, 50 }
                        }
                    };

                    var imported = LoadScoreIntoOsu(osu, toImport);

                    ClassicAssert.AreEqual(toImport.Statistics[HitResult.Perfect], imported.Statistics[HitResult.Perfect]);
                    ClassicAssert.AreEqual(toImport.Statistics[HitResult.Miss], imported.Statistics[HitResult.Miss]);
                }
                finally
                {
                    host.Exit();
                }
            }
        }

        public static ScoreInfo LoadScoreIntoOsu(OsuGameBase osu, ScoreInfo score, ArchiveReader archive = null)
        {
            // clone to avoid attaching the input score to realm.
            score = score.DeepClone();

            var scoreManager = osu.Dependencies.Get<ScoreManager>();

            scoreManager.Import(score, archive);

            return scoreManager.Query(_ => true);
        }

        internal class TestArchiveReader : ArchiveReader
        {
            public TestArchiveReader()
                : base("test_archive")
            {
            }

            public override Stream GetStream(string name) => new MemoryStream();

            public override void Dispose()
            {
            }

            public override IEnumerable<string> Filenames => new[] { "test_file.osr" };
        }
    }
}
