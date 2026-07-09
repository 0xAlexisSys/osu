// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Database;
using osu.Game.Rulesets;
using Realms;

namespace osu.Game.Users
{
    public partial class StatisticsManager : Component
    {
        [Resolved]
        private RealmAccess realm { get; set; } = null!;

        public static Statistics GetForRuleset(Realm r, RulesetInfo ruleset) =>
            GetForRuleset(r, ruleset.ShortName);

        public static Statistics GetForRuleset(Realm r, string rulesetName)
        {
            var statistics = r.Find<Statistics>(rulesetName);

            if (statistics is null)
            {
                statistics = new Statistics { RulesetName = rulesetName };
                r.Add(statistics);
            }

            return statistics;
        }

        public void WriteToRuleset(RulesetInfo ruleset, StatisticsChanges changes) =>
            WriteToRuleset(ruleset.ShortName, changes);

        public void WriteToRuleset(string rulesetName, StatisticsChanges changes)
        {
            if (changes.AddedHitCount is null
                && changes.AddedMissCount is null
                && changes.Accuracy is null
                && changes.RankString is null)
                return;

            realm.Write(r =>
            {
                var statistics = GetForRuleset(r, rulesetName);

                if (changes.AddedHitCount is not null)
                    statistics.HitCount += (int)changes.AddedHitCount;

                if (changes.AddedMissCount is not null)
                    statistics.MissCount += (int)changes.AddedMissCount;

                if (changes.Accuracy is not null)
                    statistics.AccuracySamples.Add((double)changes.Accuracy);

                if (changes.RankString is not null)
                {
                    statistics.ScoreRankCounts.TryAdd(changes.RankString, 0);
                    ++statistics.ScoreRankCounts[changes.RankString];
                }
            });
        }
    }
}
