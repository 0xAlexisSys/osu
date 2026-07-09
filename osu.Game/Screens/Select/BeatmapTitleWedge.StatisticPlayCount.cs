// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Graphics;
using osu.Game.Users;

namespace osu.Game.Screens.Select
{
    public partial class BeatmapTitleWedge
    {
        public partial class StatisticPlayCount : Statistic
        {
            [Resolved]
            private RealmAccess realm { get; set; } = null!;

            private IDisposable? realmSubscription;

            public StatisticPlayCount(bool background = false, float leftPadding = 10.0f, float? minSize = null)
                : base(OsuIcon.Play, background, leftPadding, minSize) { }

            public void UpdateText(BeatmapInfo beatmap, string rulesetName)
            {
                realmSubscription?.Dispose();
                realmSubscription = realm.RegisterForNotifications(r => r.All<Statistics>().Where(s => s.RulesetName == rulesetName), (sender, _) =>
                {
                    if (sender.Count != 0)
                    {
                        sender[0].BeatmapPlayCounts.TryGetValue(beatmap.Hash, out int playCount);
                        Text = playCount.ToString();
                    }
                });
            }
        }
    }
}
